using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PracticalModules.Probabilities.ProbabilityHandleByRarity
{
    /// <summary>
    /// Main controller for managing individual rarity probabilities with optimized performance
    /// </summary>
    [CreateAssetMenu(fileName = "ProbabilityController", menuName = "Randomness/Probability Controller")]
    public class ProbabilityController : ScriptableObject, IProbabilityController
    {
        [Header("Rarity Probability Configuration")]
        public List<RarityProbability> rarityProbabilities = new();
        
        [Header("Global Modifiers")]
        [Range(0.1f, 3f)]
        public float globalProbabilityMultiplier = 1f;
        public bool enableGlobalLuckFactor;
        
        [Range(0.8f, 1.2f)]
        public float luckFactor = 1f;
        
        [Header("Normalization Settings")]
        public bool autoNormalizeProbabilities = true;
        public bool maintainRelativeRatios = true;
        
        // Runtime data - optimized for performance
        private Dictionary<Rarity, RarityProbability> _probabilityLookup;
        private RarityProbability[] _sortedRarities; // Cached sorted array for better performance
        private float _totalProbabilityWeight;
        private bool _isInitialized;
        private bool _needsRecalculation = true;
        
        /// <summary>
        /// Initialize the probability controller with optimized setup
        /// </summary>
        public void Initialize(int? seed = null)
        {
            if (seed.HasValue)
            {
                Random.InitState(seed.Value);
            }
            
            this.BuildLookupDictionary();
            this.CalculateTotalWeight();
            this._isInitialized = true;
        }
        
        /// <summary>
        /// Build the lookup dictionary and sorted array for optimized access
        /// </summary>
        private void BuildLookupDictionary()
        {
            this._probabilityLookup = new Dictionary<Rarity, RarityProbability>();
            
            // Add existing probabilities
            for (int i = 0; i < this.rarityProbabilities.Count; i++)
            {
                var rarityProb = this.rarityProbabilities[i];
                this._probabilityLookup[rarityProb.rarity] = rarityProb;
            }
            
            // Ensure all rarities have entries - optimized without LINQ
            var allRarities = (Rarity[])Enum.GetValues(typeof(Rarity));
            for (int i = 0; i < allRarities.Length; i++)
            {
                var rarity = allRarities[i];
                if (!this._probabilityLookup.ContainsKey(rarity))
                {
                    var defaultProb = new RarityProbability(rarity, this.GetDefaultProbability(rarity));
                    this.rarityProbabilities.Add(defaultProb);
                    this._probabilityLookup[rarity] = defaultProb;
                }
            }
            
            // Cache sorted rarities for better performance
            this._sortedRarities = new RarityProbability[this.rarityProbabilities.Count];
            for (int i = 0; i < this.rarityProbabilities.Count; i++)
            {
                this._sortedRarities[i] = this.rarityProbabilities[i];
            }
            
            // Sort by rarity value (highest first) for better distribution
            this.QuickSortRarities(0, this._sortedRarities.Length - 1);
        }
        
        /// <summary>
        /// Optimized quicksort for rarity probabilities
        /// </summary>
        private void QuickSortRarities(int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = this.PartitionRarities(low, high);
                this.QuickSortRarities(low, pivotIndex - 1);
                this.QuickSortRarities(pivotIndex + 1, high);
            }
        }
        
        /// <summary>
        /// Partition method for quicksort
        /// </summary>
        private int PartitionRarities(int low, int high)
        {
            var pivot = this._sortedRarities[high];
            int i = low - 1;
            
            for (int j = low; j < high; j++)
            {
                if ((int)this._sortedRarities[j].rarity >= (int)pivot.rarity)
                {
                    i++;
                    this.SwapRarities(i, j);
                }
            }
            
            this.SwapRarities(i + 1, high);
            return i + 1;
        }
        
        /// <summary>
        /// Swap two rarity probabilities in the sorted array
        /// </summary>
        private void SwapRarities(int i, int j)
        {
            (this._sortedRarities[i], this._sortedRarities[j]) = (this._sortedRarities[j], this._sortedRarities[i]);
        }
        
        /// <summary>
        /// Calculate total probability weight for normalization with caching
        /// </summary>
        private void CalculateTotalWeight()
        {
            if (!this._needsRecalculation)
            {
                return;
            }
            
            this._totalProbabilityWeight = 0f;
            
            for (int i = 0; i < this.rarityProbabilities.Count; i++)
            {
                this._totalProbabilityWeight += this.rarityProbabilities[i].GetCurrentProbability();
            }
            
            if (this.autoNormalizeProbabilities && this._totalProbabilityWeight > 1f)
            {
                this.NormalizeProbabilities();
            }
            
            this._needsRecalculation = false;
        }
        
        /// <summary>
        /// Normalize probabilities to sum to 1.0 while maintaining ratios
        /// </summary>
        private void NormalizeProbabilities()
        {
            if (this._totalProbabilityWeight <= 0) 
            {
                return;
            }
            
            if (this.maintainRelativeRatios)
            {
                float normalizationFactor = 1f / this._totalProbabilityWeight;
                for (int i = 0; i < this.rarityProbabilities.Count; i++)
                {
                    var rarityProb = this.rarityProbabilities[i];
                    rarityProb.baseProbability *= normalizationFactor;
                    rarityProb.MarkDirty();
                }
            }
            
            this._totalProbabilityWeight = 1f;
        }
        
        /// <summary>
        /// Get probability for a specific rarity
        /// </summary>
        public float GetProbability(Rarity rarity)
        {
            if (!_isInitialized) Initialize();
            
            if (_probabilityLookup.ContainsKey(rarity))
            {
                float prob = _probabilityLookup[rarity].GetCurrentProbability();
                prob *= globalProbabilityMultiplier;
                
                if (enableGlobalLuckFactor)
                {
                    prob *= luckFactor;
                }
                
                return Mathf.Clamp01(prob);
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Roll for a specific rarity and return success
        /// </summary>
        public bool RollForRarity(Rarity rarity)
        {
            if (!_isInitialized) Initialize();
            
            float probability = GetProbability(rarity);
            float roll = Random.value;
            bool success = roll <= probability;
            
            // Record the attempt
            if (_probabilityLookup.ContainsKey(rarity))
            {
                _probabilityLookup[rarity].RecordAttempt(success);
            }
            
            return success;
        }
        
        /// <summary>
        /// Get a random rarity based on weighted probabilities with optimized performance
        /// </summary>
        public Rarity SelectRandomRarity()
        {
            if (!this._isInitialized) 
            {
                this.Initialize();
            }
            
            // Recalculate total weight to account for dynamic changes
            this.CalculateTotalWeight();
            
            float randomValue = Random.value * this._totalProbabilityWeight;
            float currentWeight = 0f;
            
            // Use cached sorted array for better performance
            for (int i = 0; i < this._sortedRarities.Length; i++)
            {
                var rarityProb = this._sortedRarities[i];
                float probability = rarityProb.GetCurrentProbability() * this.globalProbabilityMultiplier;
                
                if (this.enableGlobalLuckFactor)
                {
                    probability *= this.luckFactor;
                }
                
                currentWeight += probability;
                
                if (randomValue <= currentWeight)
                {
                    rarityProb.RecordAttempt(true);
                    
                    // Record failures for other rarities - optimized without LINQ
                    for (int j = 0; j < this._sortedRarities.Length; j++)
                    {
                        if (j != i)
                        {
                            this._sortedRarities[j].RecordAttempt(false);
                        }
                    }
                    
                    return rarityProb.rarity;
                }
            }
            
            // Fallback to common
            return Rarity.Common;
        }
        
        /// <summary>
        /// Get all current probabilities as a dictionary with optimized performance
        /// </summary>
        public Dictionary<Rarity, float> GetAllProbabilities()
        {
            var result = new Dictionary<Rarity, float>();
            var allRarities = (Rarity[])Enum.GetValues(typeof(Rarity));
            
            for (int i = 0; i < allRarities.Length; i++)
            {
                var rarity = allRarities[i];
                result[rarity] = this.GetProbability(rarity);
            }
            
            return result;
        }
        
        /// <summary>
        /// Simulate multiple rolls and return statistics with optimized performance
        /// </summary>
        public Dictionary<Rarity, float> SimulateRolls(int rollCount)
        {
            var results = new Dictionary<Rarity, int>();
            var originalStates = this.SaveCurrentStates();
            var allRarities = (Rarity[])Enum.GetValues(typeof(Rarity));
            
            // Initialize results - optimized without LINQ
            for (int i = 0; i < allRarities.Length; i++)
            {
                results[allRarities[i]] = 0;
            }
            
            // Perform simulation
            for (int i = 0; i < rollCount; i++)
            {
                var selectedRarity = this.SelectRandomRarity();
                results[selectedRarity]++;
            }
            
            // Restore original states
            this.RestoreStates(originalStates);
            
            // Convert to percentages
            var percentages = new Dictionary<Rarity, float>();
            foreach (var kvp in results)
            {
                percentages[kvp.Key] = (float)kvp.Value / rollCount;
            }
            
            return percentages;
        }
        
        /// <summary>
        /// Modify probability for a specific rarity at runtime
        /// </summary>
        public void ModifyProbability(Rarity rarity, float newBaseProbability)
        {
            if (this._probabilityLookup.ContainsKey(rarity))
            {
                this._probabilityLookup[rarity].baseProbability = Mathf.Clamp01(newBaseProbability);
                this._probabilityLookup[rarity].MarkDirty();
                this.MarkDirty();
            }
        }
        
        /// <summary>
        /// Apply temporary multiplier to a specific rarity
        /// </summary>
        public void ApplyTemporaryMultiplier(Rarity rarity, float multiplier)
        {
            if (this._probabilityLookup.ContainsKey(rarity))
            {
                this._probabilityLookup[rarity].weightMultiplier = multiplier;
                this._probabilityLookup[rarity].MarkDirty();
                this.MarkDirty();
            }
        }
        
        /// <summary>
        /// Mark the controller as needing recalculation
        /// </summary>
        private void MarkDirty()
        {
            this._needsRecalculation = true;
        }
        
        /// <summary>
        /// Reset all tracking data for all rarities
        /// </summary>
        public void ResetAllTracking()
        {
            for (int i = 0; i < this.rarityProbabilities.Count; i++)
            {
                this.rarityProbabilities[i].ResetTracking();
            }
        }
        
        /// <summary>
        /// Get statistics for a specific rarity
        /// </summary>
        public (long attempts, long successes, float rate, int streak) GetRarityStats(Rarity rarity)
        {
            if (this._probabilityLookup.TryGetValue(rarity, out var prob))
            {
                return (prob.TotalAttempts, prob.TotalSuccesses, prob.GetSuccessRate(), prob.ConsecutiveFailures);
            }
            
            return (0, 0, 0f, 0);
        }
        
        /// <summary>
        /// Get default probability for a rarity tier
        /// </summary>
        private float GetDefaultProbability(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Common => 0.50f,
                Rarity.Uncommon => 0.30f,
                Rarity.Rare => 0.15f,
                Rarity.Epic => 0.04f,
                Rarity.Legendary => 0.008f,
                Rarity.Mythic => 0.002f,
                _ => 0.01f
            };
        }
        
        /// <summary>
        /// Save current states for simulation restoration
        /// </summary>
        private Dictionary<Rarity, (int failures, long attempts, long successes, DateTime lastSuccess)> SaveCurrentStates()
        {
            var states = new Dictionary<Rarity, (int, long, long, DateTime)>();
            
            foreach (var kvp in this._probabilityLookup)
            {
                var prob = kvp.Value;
                states[kvp.Key] = (prob.ConsecutiveFailures, prob.TotalAttempts, prob.TotalSuccesses, prob.LastSuccessTime);
            }
            
            return states;
        }
        
        /// <summary>
        /// Restore saved states after simulation
        /// </summary>
        private void RestoreStates(Dictionary<Rarity, (int failures, long attempts, long successes, DateTime lastSuccess)> states)
        {
            foreach (var kvp in states)
            {
                if (this._probabilityLookup.ContainsKey(kvp.Key))
                {
                    var prob = this._probabilityLookup[kvp.Key];
                    prob.ConsecutiveFailures = kvp.Value.failures;
                    prob.TotalAttempts = kvp.Value.attempts;
                    prob.TotalSuccesses = kvp.Value.successes;
                    prob.LastSuccessTime = kvp.Value.lastSuccess;
                    prob.MarkDirty();
                }
            }
        }
        
        /// <summary>
        /// Validate probability configuration
        /// </summary>
        public bool ValidateConfiguration()
        {
            float totalProb = 0f;
            
            for (int i = 0; i < this.rarityProbabilities.Count; i++)
            {
                var rarityProb = this.rarityProbabilities[i];
                if (rarityProb.baseProbability < 0f || rarityProb.baseProbability > 1f)
                {
                    Debug.LogError($"Invalid probability for {rarityProb.rarity}: {rarityProb.baseProbability}");
                    return false;
                }
                
                totalProb += rarityProb.baseProbability;
            }
            
            if (totalProb > 1.01f) // Allow small floating point errors
            {
                Debug.LogWarning($"Total probability exceeds 1.0: {totalProb}. Consider enabling auto-normalization.");
            }
            
            return true;
        }
    }
}