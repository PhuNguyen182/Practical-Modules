using System;
using System.Collections.Generic;
using System.Linq;
using PracticalModules.Probabilities.ProbabilityHandleByRarity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PracticalModules.Probabilities.ProbabilityHandle
{
    /// <summary>
    /// Main controller for managing individual rarity probabilities
    /// </summary>
    [CreateAssetMenu(fileName = "ProbabilityController", menuName = "Randomness/Probability Controller")]
    public class ProbabilityController : ScriptableObject
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
        
        // Runtime data
        //private System.Random _randomNumberGenerator;
        private Dictionary<Rarity, RarityProbability> _probabilityLookup;
        
        private float _totalProbabilityWeight;
        private bool _isInitialized;
        
        /// <summary>
        /// Initialize the probability controller
        /// </summary>
        public void Initialize(int? seed = null)
        {
            //_randomNumberGenerator = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
            if (seed.HasValue)
                Random.InitState(seed.Value);
            
            BuildLookupDictionary();
            CalculateTotalWeight();
            _isInitialized = true;
        }
        
        /// <summary>
        /// Build the lookup dictionary for faster access
        /// </summary>
        private void BuildLookupDictionary()
        {
            _probabilityLookup = new Dictionary<Rarity, RarityProbability>();
            
            foreach (var rarityProb in rarityProbabilities)
            {
                _probabilityLookup[rarityProb.rarity] = rarityProb;
            }
            
            // Ensure all rarities have entries
            foreach (var rarity in Enum.GetValues(typeof(Rarity)).Cast<Rarity>())
            {
                if (!_probabilityLookup.ContainsKey(rarity))
                {
                    var defaultProb = new RarityProbability(rarity, GetDefaultProbability(rarity));
                    rarityProbabilities.Add(defaultProb);
                    _probabilityLookup[rarity] = defaultProb;
                }
            }
        }
        
        /// <summary>
        /// Calculate total probability weight for normalization
        /// </summary>
        private void CalculateTotalWeight()
        {
            _totalProbabilityWeight = 0f;
            
            foreach (var rarityProb in rarityProbabilities)
            {
                _totalProbabilityWeight += rarityProb.GetCurrentProbability();
            }
            
            if (autoNormalizeProbabilities && _totalProbabilityWeight > 1f)
            {
                NormalizeProbabilities();
            }
        }
        
        /// <summary>
        /// Normalize probabilities to sum to 1.0 while maintaining ratios
        /// </summary>
        private void NormalizeProbabilities()
        {
            if (_totalProbabilityWeight <= 0) return;
            
            if (maintainRelativeRatios)
            {
                float normalizationFactor = 1f / _totalProbabilityWeight;
                foreach (var rarityProb in rarityProbabilities)
                {
                    rarityProb.baseProbability *= normalizationFactor;
                    rarityProb.MarkDirty();
                }
            }
            
            _totalProbabilityWeight = 1f;
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
        /// Get a random rarity based on weighted probabilities
        /// </summary>
        public Rarity SelectRandomRarity()
        {
            if (!_isInitialized) Initialize();
            
            // Recalculate total weight to account for dynamic changes
            CalculateTotalWeight();
            
            float randomValue = Random.value * _totalProbabilityWeight;
            float currentWeight = 0f;
            
            // Sort by rarity (highest first) for better distribution
            var sortedRarities = rarityProbabilities.OrderByDescending(x => (int)x.rarity);
            
            foreach (var rarityProb in sortedRarities)
            {
                currentWeight += rarityProb.GetCurrentProbability() * globalProbabilityMultiplier;
                
                if (enableGlobalLuckFactor)
                {
                    currentWeight *= luckFactor;
                }
                
                if (randomValue <= currentWeight)
                {
                    rarityProb.RecordAttempt(true);
                    
                    // Record failures for other rarities
                    foreach (var otherRarity in rarityProbabilities.Where(x => x.rarity != rarityProb.rarity))
                    {
                        otherRarity.RecordAttempt(false);
                    }
                    
                    return rarityProb.rarity;
                }
            }
            
            // Fallback to common
            return Rarity.Common;
        }
        
        /// <summary>
        /// Get all current probabilities as a dictionary
        /// </summary>
        public Dictionary<Rarity, float> GetAllProbabilities()
        {
            var result = new Dictionary<Rarity, float>();
            
            foreach (var rarity in Enum.GetValues(typeof(Rarity)).Cast<Rarity>())
            {
                result[rarity] = GetProbability(rarity);
            }
            
            return result;
        }
        
        /// <summary>
        /// Simulate multiple rolls and return statistics
        /// </summary>
        public Dictionary<Rarity, float> SimulateRolls(int rollCount)
        {
            var results = new Dictionary<Rarity, int>();
            var originalStates = SaveCurrentStates();
            
            // Initialize results
            foreach (var rarity in Enum.GetValues(typeof(Rarity)).Cast<Rarity>())
            {
                results[rarity] = 0;
            }
            
            // Perform simulation
            for (int i = 0; i < rollCount; i++)
            {
                var selectedRarity = SelectRandomRarity();
                results[selectedRarity]++;
            }
            
            // Restore original states
            RestoreStates(originalStates);
            
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
            if (_probabilityLookup.ContainsKey(rarity))
            {
                _probabilityLookup[rarity].baseProbability = Mathf.Clamp01(newBaseProbability);
                _probabilityLookup[rarity].MarkDirty();
                CalculateTotalWeight();
            }
        }
        
        /// <summary>
        /// Apply temporary multiplier to a specific rarity
        /// </summary>
        public void ApplyTemporaryMultiplier(Rarity rarity, float multiplier)
        {
            if (_probabilityLookup.ContainsKey(rarity))
            {
                _probabilityLookup[rarity].weightMultiplier = multiplier;
                _probabilityLookup[rarity].MarkDirty();
            }
        }
        
        /// <summary>
        /// Reset all tracking data for all rarities
        /// </summary>
        public void ResetAllTracking()
        {
            foreach (var rarityProb in rarityProbabilities)
            {
                rarityProb.ResetTracking();
            }
        }
        
        /// <summary>
        /// Get statistics for a specific rarity
        /// </summary>
        public (long attempts, long successes, float rate, int streak) GetRarityStats(Rarity rarity)
        {
            if (_probabilityLookup.TryGetValue(rarity, out var prob))
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
            
            foreach (var kvp in _probabilityLookup)
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
                if (_probabilityLookup.ContainsKey(kvp.Key))
                {
                    var prob = _probabilityLookup[kvp.Key];
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
            
            foreach (var rarityProb in rarityProbabilities)
            {
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