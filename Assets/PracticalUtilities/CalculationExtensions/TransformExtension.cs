using System.Collections.Generic;
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

        public static Transform GetClosestTransform(this Transform transform, Transform[]? transforms)
        {
            if (transforms == null || transforms.Length <= 0)
                return null;
            
            float minDistance = Mathf.Infinity;
            Transform closestTransform = transforms[0];
            
            for (int i = 0; i < transforms.Length; i++)
            {
                float sqrDistance = Vector3.SqrMagnitude(transforms[i].position - transform.position);
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    closestTransform = transforms[i];
                }
            }
            
            return closestTransform;
        }
        
        public static Vector3 GetClosestTransform(this Transform transform, List<Vector3>? positions)
        {
            if (positions == null || positions.Count <= 0)
                return transform.position;
            
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = positions[0];
            
            for (int i = 0; i < positions.Count; i++)
            {
                float sqrDistance = Vector3.SqrMagnitude(positions[i] - transform.position);
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    closestPosition = positions[i];
                }
            }
            
            return closestPosition;
        }
        
        public static Vector3 GetClosestPosition(this Transform transform, Vector3[]? positions)
        {
            if (positions == null || positions.Length <= 0)
                return transform.position;
            
            float minDistance = Mathf.Infinity;
            Vector3 closestPosition = positions[0];
            
            for (int i = 0; i < positions.Length; i++)
            {
                float sqrDistance = Vector3.SqrMagnitude(positions[i] - transform.position);
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    closestPosition = positions[i];
                }
            }
            
            return closestPosition;
        }
        
        public static Transform GetClosestTransform(this Transform transform, List<Transform>? transforms)
        {
            if (transforms == null || transforms.Count <= 0)
                return null;
            
            float minDistance = Mathf.Infinity;
            Transform closestTransform = transforms[0];
            
            for (int i = 0; i < transforms.Count; i++)
            {
                float sqrDistance = Vector3.SqrMagnitude(transforms[i].position - transform.position);
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    closestTransform = transforms[i];
                }
            }
            
            return closestTransform;
        }

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

        public static bool TryGetChildComponent<T>(this Transform transform, out T component,
            params int[] hierarchyIndexes) where T : Component
        {
            component = null;
            if (transform.childCount == 0)
                return false;

            Transform checkinTransform = transform;
            for (int i = 0; i < hierarchyIndexes.Length; i++)
            {
                int childIndex = hierarchyIndexes[i];
                if (checkinTransform.childCount == 0 || childIndex >= checkinTransform.childCount)
                    return false;

                checkinTransform = checkinTransform.GetChild(childIndex);
            }

            return checkinTransform.TryGetComponent(out component);
        }
    }
}
