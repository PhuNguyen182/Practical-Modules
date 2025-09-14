using UnityEngine;

namespace PracticalUtilities.CalculationExtensions
{
    public static class VectorExtension
    {
        public static Vector3 GetFlatVector(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }
        
        public static Vector3 Quantize(this Vector3Int vector, Vector3 quantization)
        {
            Vector3 scaleVector = new()
            {
                x = Mathf.Floor(vector.x / quantization.x),
                y = Mathf.Floor(vector.y / quantization.y),
                z = Mathf.Floor(vector.z / quantization.z)
            };
            
            Vector3 quantizedVector = Vector3.Scale(quantization, scaleVector);
            return quantizedVector;
        }
    }
}
