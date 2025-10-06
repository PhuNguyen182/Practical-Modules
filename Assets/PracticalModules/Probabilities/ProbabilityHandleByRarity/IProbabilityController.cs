using System.Collections.Generic;

namespace PracticalModules.Probabilities.ProbabilityHandleByRarity
{
    /// <summary>
    /// Interface for managing probability-based random selection with rarity system
    /// </summary>
    public interface IProbabilityController
    {
        /// <summary>
        /// Initialize the probability controller with optional seed
        /// </summary>
        /// <param name="seed">Optional seed for reproducible random results</param>
        void Initialize(int? seed = null);
        
        /// <summary>
        /// Get probability for a specific rarity
        /// </summary>
        /// <param name="rarity">The rarity to get probability for</param>
        /// <returns>Probability value between 0 and 1</returns>
        float GetProbability(Rarity rarity);
        
        /// <summary>
        /// Roll for a specific rarity and return success
        /// </summary>
        /// <param name="rarity">The rarity to roll for</param>
        /// <returns>True if roll was successful</returns>
        bool RollForRarity(Rarity rarity);
        
        /// <summary>
        /// Get a random rarity based on weighted probabilities
        /// </summary>
        /// <returns>Selected rarity based on current probabilities</returns>
        Rarity SelectRandomRarity();
        
        /// <summary>
        /// Get all current probabilities as a dictionary
        /// </summary>
        /// <returns>Dictionary mapping rarities to their probabilities</returns>
        Dictionary<Rarity, float> GetAllProbabilities();
        
        /// <summary>
        /// Simulate multiple rolls and return statistics
        /// </summary>
        /// <param name="rollCount">Number of rolls to simulate</param>
        /// <returns>Dictionary with rarity distribution percentages</returns>
        Dictionary<Rarity, float> SimulateRolls(int rollCount);
        
        /// <summary>
        /// Modify probability for a specific rarity at runtime
        /// </summary>
        /// <param name="rarity">The rarity to modify</param>
        /// <param name="newBaseProbability">New base probability value</param>
        void ModifyProbability(Rarity rarity, float newBaseProbability);
        
        /// <summary>
        /// Apply temporary multiplier to a specific rarity
        /// </summary>
        /// <param name="rarity">The rarity to apply multiplier to</param>
        /// <param name="multiplier">Multiplier value</param>
        void ApplyTemporaryMultiplier(Rarity rarity, float multiplier);
        
        /// <summary>
        /// Reset all tracking data for all rarities
        /// </summary>
        void ResetAllTracking();
        
        /// <summary>
        /// Get statistics for a specific rarity
        /// </summary>
        /// <param name="rarity">The rarity to get stats for</param>
        /// <returns>Tuple containing attempts, successes, rate, and streak</returns>
        (long attempts, long successes, float rate, int streak) GetRarityStats(Rarity rarity);
        
        /// <summary>
        /// Validate probability configuration
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        bool ValidateConfiguration();
    }
}
