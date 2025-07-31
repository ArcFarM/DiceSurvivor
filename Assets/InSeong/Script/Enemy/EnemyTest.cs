using UnityEngine;

namespace DiceSurvivor.Enemy {
    public class EnemyTest : MonoBehaviour {
        #region Variables
        public Transform playerPos;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void Update() {
            //playerPos를 향해 이동
            if (playerPos != null) {
                Vector3 direction = (playerPos.position - transform.position).normalized;
                transform.position += direction * Time.deltaTime;
            }
        }

        
        #endregion

        #region Custom Methods
        #endregion
    }

}
