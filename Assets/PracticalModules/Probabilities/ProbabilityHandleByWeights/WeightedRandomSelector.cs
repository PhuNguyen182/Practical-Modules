using System;
using System.Collections.Generic;
using UnityEngine;

namespace PracticalModules.Probabilities.ProbabilityHandleByWeights
{
    /// <summary>
    /// A weighted random selector that returns indices based on weight probabilities using UnityEngine.Random
    /// </summary>
    public class WeightedRandomSelector : IWeightedRandomSelector
    {
        private float[] _weights;
        private float[] _cumulativeWeights;
        private float _totalWeight;
        private bool _isDirty = true;
        
        /// <summary>
        /// Initialize with an array of weights
        /// </summary>
        public WeightedRandomSelector(float[] weights)
        {
            SetWeights(weights);
        }
        
        /// <summary>
        /// Initialize with a list of weights
        /// </summary>
        public WeightedRandomSelector(List<float> weights)
        {
            SetWeights(weights.ToArray());
        }
        
        /// <summary>
        /// Initialize with integer weights (automatically converted to float)
        /// </summary>
        public WeightedRandomSelector(int[] weights)
        {
            float[] floatWeights = new float[weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                floatWeights[i] = weights[i];
            }
            this.SetWeights(floatWeights);
        }
        
        /// <summary>
        /// Initialize with partial array (for internal use with count parameter)
        /// </summary>
        private WeightedRandomSelector(float[] weights, int count)
        {
            this.SetWeights(weights, count);
        }
        
        /// <summary>
        /// Set new weights and mark for recalculation
        /// </summary>
        public void SetWeights(float[] newWeights)
        {
            if (newWeights == null || newWeights.Length == 0)
            {
                throw new ArgumentException("Weights array cannot be null or empty");
            }
            
            this.SetWeights(newWeights, newWeights.Length);
        }
        
        /// <summary>
        /// Set new weights with count parameter for partial arrays
        /// </summary>
        private void SetWeights(float[] newWeights, int count)
        {
            if (newWeights == null || count <= 0)
            {
                throw new ArgumentException("Weights array cannot be null or empty");
            }
            
            // Validate weights (must be non-negative)
            for (int i = 0; i < count; i++)
            {
                if (newWeights[i] < 0f)
                {
                    Debug.LogWarning($"Negative weight at index {i}: {newWeights[i]}. Setting to 0.");
                    newWeights[i] = 0f;
                }
            }
            
            this._weights = new float[count];
            Array.Copy(newWeights, this._weights, count);
            
            this._cumulativeWeights = new float[this._weights.Length];
            this._isDirty = true;
        }
        
        /// <summary>
        /// Update weight at a specific index
        /// </summary>
        public void UpdateWeight(int index, float newWeight)
        {
            if (index < 0 || index >= this._weights.Length)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range for weights array of length {this._weights.Length}");
            }
            
            if (newWeight < 0f)
            {
                Debug.LogWarning($"Negative weight at index {index}: {newWeight}. Setting to 0.");
                newWeight = 0f;
            }
            
            this._weights[index] = newWeight;
            this._isDirty = true;
        }
        
        /// <summary>
        /// Get a random index based on weights with optimized performance
        /// </summary>
        public int GetRandomIndex()
        {
            if (this._isDirty)
            {
                this.RecalculateCumulativeWeights();
            }
            
            if (this._totalWeight <= 0f)
            {
                Debug.LogWarning("Total weight is 0 or negative. Returning random index.");
                return UnityEngine.Random.Range(0, this._weights.Length);
            }
            
            float randomValue = UnityEngine.Random.Range(0f, this._totalWeight);
            
            // Binary search for efficiency with large arrays
            return this.BinarySearchIndex(randomValue);
        }
        
        /// <summary>
        /// Get multiple random indices (with replacement)
        /// </summary>
        public int[] GetRandomIndices(int count)
        {
            int[] indices = new int[count];
            for (int i = 0; i < count; i++)
            {
                indices[i] = this.GetRandomIndex();
            }
            return indices;
        }
        
        /// <summary>
        /// Get multiple unique random indices (without replacement) with optimized performance
        /// </summary>
        public int[] GetUniqueRandomIndices(int count)
        {
            if (count > this._weights.Length)
            {
                throw new ArgumentException($"Cannot select {count} unique indices from array of length {this._weights.Length}");
            }
            
            // Use arrays instead of Lists for better performance
            var availableIndices = new int[this._weights.Length];
            var tempWeights = new float[this._weights.Length];
            int availableCount = 0;
            
            // Build available indices with non-zero weights
            for (int i = 0; i < this._weights.Length; i++)
            {
                if (this._weights[i] > 0f)
                {
                    availableIndices[availableCount] = i;
                    tempWeights[availableCount] = this._weights[i];
                    availableCount++;
                }
            }
            
            if (count > availableCount)
            {
                throw new ArgumentException($"Cannot select {count} unique indices from {availableCount} available non-zero weighted items");
            }
            
            var selectedIndices = new int[count];
            var tempSelector = new WeightedRandomSelector(tempWeights, availableCount);
            
            for (int i = 0; i < count; i++)
            {
                int tempIndex = tempSelector.GetRandomIndex();
                int actualIndex = availableIndices[tempIndex];
                
                selectedIndices[i] = actualIndex;
                
                // Remove selected item by swapping with last element
                availableIndices[tempIndex] = availableIndices[availableCount - 1];
                tempWeights[tempIndex] = tempWeights[availableCount - 1];
                availableCount--;
                
                if (availableCount > 0)
                {
                    tempSelector.SetWeights(tempWeights, availableCount);
                }
            }
            
            return selectedIndices;
        }
        
        /// <summary>
        /// Recalculate cumulative weights for efficient selection
        /// </summary>
        private void RecalculateCumulativeWeights()
        {
            this._totalWeight = 0f;
            
            for (int i = 0; i < this._weights.Length; i++)
            {
                this._totalWeight += this._weights[i];
                this._cumulativeWeights[i] = this._totalWeight;
            }
            
            this._isDirty = false;
        }
        
        /// <summary>
        /// Binary search to find the correct index based on cumulative weights
        /// </summary>
        private int BinarySearchIndex(float randomValue)
        {
            int left = 0;
            int right = this._cumulativeWeights.Length - 1;
            
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                
                if (this._cumulativeWeights[mid] < randomValue)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid;
                }
            }
            
            return left;
        }
        
        /// <summary>
        /// Get the probability of selecting a specific index
        /// </summary>
        public float GetProbability(int index)
        {
            if (index < 0 || index >= this._weights.Length)
            {
                return 0f;
            }
            
            if (this._isDirty)
            {
                this.RecalculateCumulativeWeights();
            }
            
            return this._totalWeight > 0f ? this._weights[index] / this._totalWeight : 0f;
        }
        
        /// <summary>
        /// Get all probabilities as percentages
        /// </summary>
        public float[] GetAllProbabilities()
        {
            if (this._isDirty)
            {
                this.RecalculateCumulativeWeights();
            }
            
            float[] probabilities = new float[this._weights.Length];
            
            for (int i = 0; i < this._weights.Length; i++)
            {
                probabilities[i] = this._totalWeight > 0f ? this._weights[i] / this._totalWeight : 0f;
            }
            
            return probabilities;
        }
        
        /// <summary>
        /// Get weight at a specific index
        /// </summary>
        public float GetWeight(int index)
        {
            if (index < 0 || index >= this._weights.Length)
            {
                return 0f;
            }
            
            return this._weights[index];
        }
        
        /// <summary>
        /// Get total weight sum
        /// </summary>
        public float GetTotalWeight()
        {
            if (this._isDirty)
            {
                this.RecalculateCumulativeWeights();
            }
            
            return this._totalWeight;
        }
        
        /// <summary>
        /// Get a copy of a current weights array
        /// </summary>
        public float[] GetWeights()
        {
            float[] copy = new float[this._weights.Length];
            Array.Copy(this._weights, copy, this._weights.Length);
            return copy;
        }
        
        /// <summary>
        /// Normalize all weights so they sum to 1.0
        /// </summary>
        public void NormalizeWeights()
        {
            if (this._isDirty)
            {
                this.RecalculateCumulativeWeights();
            }
            
            if (this._totalWeight <= 0f)
            {
                Debug.LogWarning("Cannot normalize weights: total weight is 0 or negative");
                return;
            }
            
            for (int i = 0; i < this._weights.Length; i++)
            {
                this._weights[i] /= this._totalWeight;
            }
            
            this._isDirty = true;
        }
        
        /// <summary>
        /// Scale all weights by a multiplier
        /// </summary>
        public void ScaleWeights(float multiplier)
        {
            for (int i = 0; i < this._weights.Length; i++)
            {
                this._weights[i] *= multiplier;
                if (this._weights[i] < 0f) this._weights[i] = 0f;
            }
            
            this._isDirty = true;
        }
        
        /// <summary>
        /// Simulate multiple selections and return frequency distribution
        /// </summary>
        public Dictionary<int, int> Simulate(int selectionCount)
        {
            var results = new Dictionary<int, int>();
            
            // Initialize results
            for (int i = 0; i < this._weights.Length; i++)
            {
                results[i] = 0;
            }
            
            // Perform simulation
            for (int i = 0; i < selectionCount; i++)
            {
                int selectedIndex = this.GetRandomIndex();
                results[selectedIndex]++;
            }
            
            return results;
        }
        
        /// <summary>
        /// Get the number of available indices (weights.Length)
        /// </summary>
        public int Count => this._weights.Length;
        
        /// <summary>
        /// Check if the selector has valid weights
        /// </summary>
        public bool IsValid()
        {
            if (this._weights == null || this._weights.Length == 0)
                return false;
                
            if (this._isDirty)
            {
                this.RecalculateCumulativeWeights();
            }
            
            return this._totalWeight > 0f;
        }
    }
}