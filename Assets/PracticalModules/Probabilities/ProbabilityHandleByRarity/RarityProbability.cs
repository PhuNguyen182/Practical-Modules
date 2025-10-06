using System;
using UnityEngine;

namespace PracticalModules.Probabilities.ProbabilityHandleByRarity
{
    [Serializable]
    public class RarityProbability
    {
        [Header("Basic Probability Settings")]
        public Rarity rarity;
        [Range(0f, 1f)]
        public float baseProbability;
        [Range(0f, 10f)]
        public float weightMultiplier = 1f;
        
        [Header("Dynamic Probability Modifiers")]
        public bool enableDynamicScaling;
        
        [Range(0f, 2f)]
        public float dynamicScalingFactor = 1f;
        
        [Header("Streak Protection")]
        public bool enableStreakProtection;
        public int maxConsecutiveFailures = 10;
        
        [Range(1f, 5f)]
        public float streakProtectionMultiplier = 1.5f;
        
        [Header("Time-based Modifiers")]
        public bool enableTimeBonus;
        public float timeBonusMultiplier = 1.2f;
        public float timeBonusDurationHours = 24f;
        
        // Runtime tracking data
        [NonSerialized]
        public int ConsecutiveFailures;
        
        [NonSerialized]
        public DateTime LastSuccessTime = DateTime.MinValue;
        
        [NonSerialized]
        public long TotalAttempts;
        
        [NonSerialized]
        public long TotalSuccesses;
        
        // Calculated probability cache
        [NonSerialized]
        private float _cachedFinalProbability = -1f;
        [NonSerialized]
        private bool _isDirty = true;
        
        public RarityProbability(Rarity rarity, float baseProbability)
        {
            this.rarity = rarity;
            this.baseProbability = baseProbability;
        }
        
        /// <summary>
        /// Gets the current effective probability with all modifiers applied
        /// </summary>
        public float GetCurrentProbability()
        {
            if (_isDirty || _cachedFinalProbability < 0)
            {
                _cachedFinalProbability = CalculateFinalProbability();
                _isDirty = false;
            }
            return _cachedFinalProbability;
        }
        
        /// <summary>
        /// Calculates the final probability with all active modifiers
        /// </summary>
        private float CalculateFinalProbability()
        {
            float finalProb = baseProbability * weightMultiplier;
            
            // Apply dynamic scaling based on success-rate
            if (enableDynamicScaling)
            {
                finalProb *= CalculateDynamicScaling();
            }
            
            // Apply streak protection
            if (enableStreakProtection && ConsecutiveFailures >= maxConsecutiveFailures)
            {
                float streakBonus = 1f + (ConsecutiveFailures - maxConsecutiveFailures + 1) * 0.1f;
                finalProb *= streakProtectionMultiplier * streakBonus;
            }
            
            // Apply time bonus
            if (enableTimeBonus)
            {
                finalProb *= CalculateTimeBonus();
            }
            
            return Mathf.Clamp01(finalProb);
        }
        
        /// <summary>
        /// Calculates dynamic scaling based on historical success rate
        /// </summary>
        private float CalculateDynamicScaling()
        {
            if (TotalAttempts == 0) return 1f;
            
            float actualRate = (float)TotalSuccesses / TotalAttempts;
            float expectedRate = baseProbability;
            
            if (actualRate < expectedRate * 0.8f) // If significantly below expected
            {
                return 1f + (expectedRate - actualRate) * dynamicScalingFactor;
            }
            else if (actualRate > expectedRate * 1.2f) // If significantly above expected
            {
                return 1f - (actualRate - expectedRate) * dynamicScalingFactor * 0.5f;
            }
            
            return 1f;
        }
        
        /// <summary>
        /// Calculates time-based bonus multiplier with optimized DateTime operations
        /// </summary>
        private float CalculateTimeBonus()
        {
            if (this.LastSuccessTime == DateTime.MinValue) 
            {
                return this.timeBonusMultiplier;
            }
            
            // Cache current time to avoid multiple DateTime.Now calls
            var currentTime = DateTime.Now;
            double hoursSinceLastSuccess = (currentTime - this.LastSuccessTime).TotalHours;
            
            if (hoursSinceLastSuccess >= this.timeBonusDurationHours)
            {
                return this.timeBonusMultiplier;
            }
            
            return 1f + (float)(hoursSinceLastSuccess / this.timeBonusDurationHours) * (this.timeBonusMultiplier - 1f);
        }
        
        /// <summary>
        /// Records an attempt and its result with optimized DateTime caching
        /// </summary>
        public void RecordAttempt(bool wasSuccess)
        {
            this.TotalAttempts++;
            
            if (wasSuccess)
            {
                this.TotalSuccesses++;
                this.ConsecutiveFailures = 0;
                this.LastSuccessTime = DateTime.Now;
            }
            else
            {
                this.ConsecutiveFailures++;
            }
            
            this._isDirty = true;
        }
        
        /// <summary>
        /// Gets the current success rate percentage with optimized calculation
        /// </summary>
        public float GetSuccessRate()
        {
            return this.TotalAttempts > 0 ? (float)this.TotalSuccesses / this.TotalAttempts : 0f;
        }
        
        /// <summary>
        /// Resets all tracking data
        /// </summary>
        public void ResetTracking()
        {
            this.ConsecutiveFailures = 0;
            this.TotalAttempts = 0;
            this.TotalSuccesses = 0;
            this.LastSuccessTime = DateTime.MinValue;
            this._isDirty = true;
        }
        
        /// <summary>
        /// Force recalculation on next probability request
        /// </summary>
        public void MarkDirty()
        {
            this._isDirty = true;
        }
    }
}