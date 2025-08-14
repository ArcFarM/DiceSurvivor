using System.Collections.Generic;
using UnityEngine;

namespace DiceSurvivor.Utility
{
    public class WayPointsGenerator : MonoBehaviour
    {
        #region Variables
        public Transform center;
        public GameObject wayPointPrefab;
        public int count = 10;
        public float radius = 2f;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void OnDrawGizmos()
        {
            float angleStep = 180f / (count - 1);
            Vector3 center = transform.position;

            for (int i = 0; i < count; i++)
            {
                float angleA = -90f + angleStep * i;
                float angleB = -90f + angleStep * (i + 1);

                Vector3 pointA = center + new Vector3(Mathf.Cos(angleA * Mathf.Deg2Rad), 0, Mathf.Sin(angleA * Mathf.Deg2Rad)) * radius;
                Vector3 pointB = center + new Vector3(Mathf.Cos(angleB * Mathf.Deg2Rad), 0, Mathf.Sin(angleB * Mathf.Deg2Rad)) * radius;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(pointA, pointB);
                Gizmos.DrawSphere(pointA, 0.1f);
            }
        }
        #endregion

        #region Custom Methods
        [ContextMenu("Generate WayPoints")]
        public List<Transform> GenerateWayPoints()
        {
            List<Transform> wayPoints = new List<Transform>();

            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.Lerp(-90f, 90f, i / (float)(count - 1));
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 pos = center.position + dir * radius;

                GameObject wp = Instantiate(wayPointPrefab, pos, Quaternion.LookRotation(dir), center);
                wp.name = $"WayPoint_{i:D2}";
                wayPoints.Add(wp.transform);
            }

            return wayPoints;
        }
        #endregion
    }

}
