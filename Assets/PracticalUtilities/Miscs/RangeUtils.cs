using System;
using System.Collections.Generic;

namespace PracticalUtilities.Miscs
{
    [Serializable]
    public struct Range<TValue> where TValue : IComparable<TValue>
    {
        public TValue minValue;
        public TValue maxValue;

        public bool IsValueInRange(TValue value)
        {
            int minCompare = Comparer<TValue>.Default.Compare(value, minValue);
            int maxCompare = Comparer<TValue>.Default.Compare(value, maxValue);
            return minCompare >= 0 && maxCompare <= 0;
        }
    }
}
