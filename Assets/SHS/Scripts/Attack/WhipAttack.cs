
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DiceSurvivor.Attack
{
    public class WhipAttack : MonoBehaviour
    {
        #region Variables
        public List<Transform> wayPoints = new List<Transform>();
        public float rayLength = 2f;
        public float damage = 10f;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        #endregion

        #region Custom Methods
        public void SetWayPoints(List<Transform> points)
        {
            wayPoints = points;

            if (wayPoints != null && wayPoints.Count > 0)
            {
                foreach (Transform wp in wayPoints)
                {
                    Vector3 direction = wp.forward;
                    Ray ray = new Ray(wp.position, direction);

                    Debug.DrawRay(wp.position, direction * rayLength, Color.red, 1f);

                    if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
                    {
                        if (hit.transform.CompareTag("Enemy"))
                        {
                            Debug.Log("Hit Enemy");
                        }
                    }
                }
            }
        }
        #endregion
    }

}
