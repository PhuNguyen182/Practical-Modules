using System.Collections.Generic;

namespace PracticalModules.Probabilities.ProbabilityHandleByWeights
{
    /// <summary>
    /// Static utility methods for quick weighted random operations
    /// </summary>
    public static class WeightedRandomUtils
    {
        /// <summary>
        /// Quick method to get a random index from a weights array with optimized performance
        /// </summary>
        public static int GetRandomIndex(float[] weights)
        {
            var selector = new WeightedRandomSelector(weights);
            return selector.GetRandomIndex();
        }
        
        /// <summary>
        /// Quick method to get a random index from a weights list
        /// </summary>
        public static int GetRandomIndex(List<float> weights)
        {
            return WeightedRandomUtils.GetRandomIndex(weights.ToArray());
        }
        
        /// <summary>
        /// Quick method to get a random index from integer weights
        /// </summary>
        public static int GetRandomIndex(int[] weights)
        {
            var selector = new WeightedRandomSelector(weights);
            return selector.GetRandomIndex();
        }
        
        /// <summary>
        /// Get multiple random indices quickly
        /// </summary>
        public static int[] GetRandomIndices(float[] weights, int count)
        {
            var selector = new WeightedRandomSelector(weights);
            return selector.GetRandomIndices(count);
        }
        
        /// <summary>
        /// Get multiple unique random indices quickly
        /// </summary>
        public static int[] GetUniqueRandomIndices(float[] weights, int count)
        {
            var selector = new WeightedRandomSelector(weights);
            return selector.GetUniqueRandomIndices(count);
        }
    }
}