using System;
using System.Collections.Generic;
using UnityEngine;

namespace PracticalModules.Probabilities.ProbabilityHandleByWeights
{
    /// <summary>
    /// A weighted random selector that returns indices based on weight probabilities using UnityEngine.Random
    /// </summary>
    public class WeightedRandomSelector
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
            SetWeights(floatWeights);
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
            
            // Validate weights (must be non-negative)
            for (int i = 0; i < newWeights.Length; i++)
            {
                if (newWeights[i] < 0f)
                {
                    Debug.LogWarning($"Negative weight at index {i}: {newWeights[i]}. Setting to 0.");
                    newWeights[i] = 0f;
                }
            }
            
            _weights = new float[newWeights.Length];
            Array.Copy(newWeights, _weights, newWeights.Length);
            
            _cumulativeWeights = new float[_weights.Length];
            _isDirty = true;
        }
        
        /// <summary>
        /// Update weight at a specific index
        /// </summary>
        public void UpdateWeight(int index, float newWeight)
        {
            if (index < 0 || index >= _weights.Length)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range for weights array of length {_weights.Length}");
            }
            
            if (newWeight < 0f)
            {
                Debug.LogWarning($"Negative weight at index {index}: {newWeight}. Setting to 0.");
                newWeight = 0f;
            }
            
            _weights[index] = newWeight;
            _isDirty = true;
        }
        
        /// <summary>
        /// Get a random index based on weights
        /// </summary>
        public int GetRandomIndex()
        {
            if (_isDirty)
            {
                RecalculateCumulativeWeights();
            }
            
            if (_totalWeight <= 0f)
            {
                Debug.LogWarning("Total weight is 0 or negative. Returning random index.");
                return UnityEngine.Random.Range(0, _weights.Length);
            }
            
            float randomValue = UnityEngine.Random.Range(0f, _totalWeight);
            
            // Binary search for efficiency with large arrays
            return BinarySearchIndex(randomValue);
        }
        
        /// <summary>
        /// Get multiple random indices (with replacement)
        /// </summary>
        public int[] GetRandomIndices(int count)
        {
            int[] indices = new int[count];
            for (int i = 0; i < count; i++)
            {
                indices[i] = GetRandomIndex();
            }
            return indices;
        }
        
        /// <summary>
        /// Get multiple unique random indices (without replacement)
        /// </summary>
        public int[] GetUniqueRandomIndices(int count)
        {
            if (count > _weights.Length)
            {
                throw new ArgumentException($"Cannot select {count} unique indices from array of length {_weights.Length}");
            }
            
            var availableIndices = new List<int>();
            var tempWeights = new List<float>();
            
            // Build available indices with non-zero weights
            for (int i = 0; i < _weights.Length; i++)
            {
                if (_weights[i] > 0f)
                {
                    availableIndices.Add(i);
                    tempWeights.Add(_weights[i]);
                }
            }
            
            if (count > availableIndices.Count)
            {
                throw new ArgumentException($"Cannot select {count} unique indices from {availableIndices.Count} available non-zero weighted items");
            }
            
            var selectedIndices = new List<int>();
            var tempSelector = new WeightedRandomSelector(tempWeights);
            
            for (int i = 0; i < count; i++)
            {
                int tempIndex = tempSelector.GetRandomIndex();
                int actualIndex = availableIndices[tempIndex];
                
                selectedIndices.Add(actualIndex);
                
                // Remove selected item from temporary lists
                availableIndices.RemoveAt(tempIndex);
                tempWeights.RemoveAt(tempIndex);
                
                if (tempWeights.Count > 0)
                {
                    tempSelector.SetWeights(tempWeights.ToArray());
                }
            }
            
            return selectedIndices.ToArray();
        }
        
        /// <summary>
        /// Recalculate cumulative weights for efficient selection
        /// </summary>
        private void RecalculateCumulativeWeights()
        {
            _totalWeight = 0f;
            
            for (int i = 0; i < _weights.Length; i++)
            {
                _totalWeight += _weights[i];
                _cumulativeWeights[i] = _totalWeight;
            }
            
            _isDirty = false;
        }
        
        /// <summary>
        /// Binary search to find the correct index based on cumulative weights
        /// </summary>
        private int BinarySearchIndex(float randomValue)
        {
            int left = 0;
            int right = _cumulativeWeights.Length - 1;
            
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                
                if (_cumulativeWeights[mid] < randomValue)
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
            if (index < 0 || index >= _weights.Length)
            {
                return 0f;
            }
            
            if (_isDirty)
            {
                RecalculateCumulativeWeights();
            }
            
            return _totalWeight > 0f ? _weights[index] / _totalWeight : 0f;
        }
        
        /// <summary>
        /// Get all probabilities as percentages
        /// </summary>
        public float[] GetAllProbabilities()
        {
            if (_isDirty)
            {
                RecalculateCumulativeWeights();
            }
            
            float[] probabilities = new float[_weights.Length];
            
            for (int i = 0; i < _weights.Length; i++)
            {
                probabilities[i] = _totalWeight > 0f ? _weights[i] / _totalWeight : 0f;
            }
            
            return probabilities;
        }
        
        /// <summary>
        /// Get weight at a specific index
        /// </summary>
        public float GetWeight(int index)
        {
            if (index < 0 || index >= _weights.Length)
            {
                return 0f;
            }
            
            return _weights[index];
        }
        
        /// <summary>
        /// Get total weight sum
        /// </summary>
        public float GetTotalWeight()
        {
            if (_isDirty)
            {
                RecalculateCumulativeWeights();
            }
            
            return _totalWeight;
        }
        
        /// <summary>
        /// Get a copy of a current weights array
        /// </summary>
        public float[] GetWeights()
        {
            float[] copy = new float[_weights.Length];
            Array.Copy(_weights, copy, _weights.Length);
            return copy;
        }
        
        /// <summary>
        /// Normalize all weights so they sum to 1.0
        /// </summary>
        public void NormalizeWeights()
        {
            if (_isDirty)
            {
                RecalculateCumulativeWeights();
            }
            
            if (_totalWeight <= 0f)
            {
                Debug.LogWarning("Cannot normalize weights: total weight is 0 or negative");
                return;
            }
            
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] /= _totalWeight;
            }
            
            _isDirty = true;
        }
        
        /// <summary>
        /// Scale all weights by a multiplier
        /// </summary>
        public void ScaleWeights(float multiplier)
        {
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] *= multiplier;
                if (_weights[i] < 0f) _weights[i] = 0f;
            }
            
            _isDirty = true;
        }
        
        /// <summary>
        /// Simulate multiple selections and return frequency distribution
        /// </summary>
        public Dictionary<int, int> Simulate(int selectionCount)
        {
            var results = new Dictionary<int, int>();
            
            // Initialize results
            for (int i = 0; i < _weights.Length; i++)
            {
                results[i] = 0;
            }
            
            // Perform simulation
            for (int i = 0; i < selectionCount; i++)
            {
                int selectedIndex = GetRandomIndex();
                results[selectedIndex]++;
            }
            
            return results;
        }
        
        /// <summary>
        /// Get the number of available indices (weights.Length)
        /// </summary>
        public int Count => _weights.Length;
        
        /// <summary>
        /// Check if the selector has valid weights
        /// </summary>
        public bool IsValid()
        {
            if (_weights == null || _weights.Length == 0)
                return false;
                
            if (_isDirty)
            {
                RecalculateCumulativeWeights();
            }
            
            return _totalWeight > 0f;
        }
    }
}