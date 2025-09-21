using UnityEngine;

namespace PracticalUtilities.CalculationExtensions
{
    public static class TransformExtension
    {
        public static bool IsCloseTo(this Transform transform, Transform targetPoint, float minDistance) =>
            Vector3.SqrMagnitude(targetPoint.position - transform.position) <= minDistance * minDistance;

        public static bool IsCloseTo(this Transform transform, Vector3 targetPosition, float minDistance) =>
            Vector3.SqrMagnitude(targetPosition - transform.position) <= minDistance * minDistance;

        public static float GetDistance(this Transform transform, Vector3 targetPosition) =>
            Vector3.Magnitude(targetPosition - transform.position);

        public static float GetSquaredDistance(this Transform transform, Vector3 targetPosition) =>
            Vector3.SqrMagnitude(targetPosition - transform.position);

        public static void SetTRS(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = scale;
        }

        public static void SetTRS(this Transform transform, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Quaternion quaternion = Quaternion.Euler(rotation);
            transform.SetTRS(position, quaternion, scale);
        }

        public static void SetTRSP(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale,
            Transform parent = null)
        {
            transform.SetTRS(position, rotation, scale);
            transform.SetParent(parent);
        }

        public static void SetTRSP(this Transform transform, Vector3 position, Vector3 rotation, Vector3 scale,
            Transform parent = null)
        {
            transform.SetTRS(position, rotation, scale);
            transform.SetParent(parent);
        }

        public static bool TryGetChildComponent<T>(this Transform transform, out T component, int childIndex)
            where T : Component
        {
            component = null;
            if (transform.childCount == 0 || childIndex >= transform.childCount)
                return false;

            return transform.GetChild(childIndex).TryGetComponent(out component);
        }

        public static bool TryGetChildComponent<T>(this Transform transform, out T component, params int[] childIndexes)
            where T : Component
        {
            component = null;
            if (transform.childCount == 0)
                return false;

            Transform checkinParent = transform;
            for (int i = 0; i < childIndexes.Length; i++)
            {
                int childIndex = childIndexes[i];
                if (checkinParent.childCount == 0 || childIndex >= checkinParent.childCount)
                    return false;
                
                checkinParent = checkinParent.GetChild(childIndex);
            }
            
            return checkinParent.TryGetComponent(out component);
        }
    }
}
