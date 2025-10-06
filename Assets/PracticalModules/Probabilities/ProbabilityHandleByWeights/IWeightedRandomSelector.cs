using System.Collections.Generic;

namespace PracticalModules.Probabilities.ProbabilityHandleByWeights
{
    /// <summary>
    /// Interface for weighted random selection operations
    /// </summary>
    public interface IWeightedRandomSelector
    {
        /// <summary>
        /// Get a random index based on weights
        /// </summary>
        /// <returns>Random index based on weight distribution</returns>
        int GetRandomIndex();
        
        /// <summary>
        /// Get multiple random indices (with replacement)
        /// </summary>
        /// <param name="count">Number of indices to select</param>
        /// <returns>Array of random indices</returns>
        int[] GetRandomIndices(int count);
        
        /// <summary>
        /// Get multiple unique random indices (without replacement)
        /// </summary>
        /// <param name="count">Number of unique indices to select</param>
        /// <returns>Array of unique random indices</returns>
        int[] GetUniqueRandomIndices(int count);
        
        /// <summary>
        /// Get the probability of selecting a specific index
        /// </summary>
        /// <param name="index">Index to get probability for</param>
        /// <returns>Probability value between 0 and 1</returns>
        float GetProbability(int index);
        
        /// <summary>
        /// Get all probabilities as percentages
        /// </summary>
        /// <returns>Array of probability values</returns>
        float[] GetAllProbabilities();
        
        /// <summary>
        /// Get weight at a specific index
        /// </summary>
        /// <param name="index">Index to get weight for</param>
        /// <returns>Weight value at the specified index</returns>
        float GetWeight(int index);
        
        /// <summary>
        /// Get total weight sum
        /// </summary>
        /// <returns>Sum of all weights</returns>
        float GetTotalWeight();
        
        /// <summary>
        /// Get a copy of current weights array
        /// </summary>
        /// <returns>Copy of weights array</returns>
        float[] GetWeights();
        
        /// <summary>
        /// Update weight at a specific index
        /// </summary>
        /// <param name="index">Index to update</param>
        /// <param name="newWeight">New weight value</param>
        void UpdateWeight(int index, float newWeight);
        
        /// <summary>
        /// Set new weights array
        /// </summary>
        /// <param name="newWeights">New weights array</param>
        void SetWeights(float[] newWeights);
        
        /// <summary>
        /// Normalize all weights so they sum to 1.0
        /// </summary>
        void NormalizeWeights();
        
        /// <summary>
        /// Scale all weights by a multiplier
        /// </summary>
        /// <param name="multiplier">Multiplier value</param>
        void ScaleWeights(float multiplier);
        
        /// <summary>
        /// Simulate multiple selections and return frequency distribution
        /// </summary>
        /// <param name="selectionCount">Number of selections to simulate</param>
        /// <returns>Dictionary mapping indices to selection counts</returns>
        Dictionary<int, int> Simulate(int selectionCount);
        
        /// <summary>
        /// Get the number of available indices
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Check if the selector has valid weights
        /// </summary>
        /// <returns>True if weights are valid</returns>
        bool IsValid();
    }
}
