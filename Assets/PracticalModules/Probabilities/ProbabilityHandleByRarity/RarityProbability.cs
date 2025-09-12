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
        /// Calculates time-based bonus multiplier
        /// </summary>
        private float CalculateTimeBonus()
        {
            if (LastSuccessTime == DateTime.MinValue) return timeBonusMultiplier;
            
            double hoursSinceLastSuccess = (DateTime.Now - LastSuccessTime).TotalHours;
            
            if (hoursSinceLastSuccess >= timeBonusDurationHours)
            {
                return timeBonusMultiplier;
            }
            
            return 1f + (float)(hoursSinceLastSuccess / timeBonusDurationHours) * (timeBonusMultiplier - 1f);
        }
        
        /// <summary>
        /// Records an attempt and its result
        /// </summary>
        public void RecordAttempt(bool wasSuccess)
        {
            TotalAttempts++;
            
            if (wasSuccess)
            {
                TotalSuccesses++;
                ConsecutiveFailures = 0;
                LastSuccessTime = DateTime.Now;
            }
            else
            {
                ConsecutiveFailures++;
            }
            
            _isDirty = true;
        }
        
        /// <summary>
        /// Gets the current success rate percentage
        /// </summary>
        public float GetSuccessRate()
        {
            return TotalAttempts > 0 ? (float)TotalSuccesses / TotalAttempts : 0f;
        }
        
        /// <summary>
        /// Resets all tracking data
        /// </summary>
        public void ResetTracking()
        {
            ConsecutiveFailures = 0;
            TotalAttempts = 0;
            TotalSuccesses = 0;
            LastSuccessTime = DateTime.MinValue;
            _isDirty = true;
        }
        
        /// <summary>
        /// Force recalculation on next probability request
        /// </summary>
        public void MarkDirty()
        {
            _isDirty = true;
        }
    }
}