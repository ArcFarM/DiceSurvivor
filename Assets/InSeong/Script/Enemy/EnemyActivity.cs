using DiceSurvivor.Manager;
using UnityEngine;

namespace DiceSurvivor.Enemy {
    public class EnemyActivity : MonoBehaviour {
        #region Variables
        EnemyData enemyData; 
        public Transform playerPos;
        #endregion

        #region Properties
        public EnemyData EnemyData {
            get { return enemyData; }
            set { enemyData = value; }
        }
        #endregion

        #region Unity Event Methods
        private void Start() {
            // 플레이어의 위치를 찾기
            playerPos = EnemySpawnManager.Instance.playerPos;
            if (playerPos == null) {
                Debug.LogError("Player position not found. Please ensure the player is tagged correctly.");
            }
        }
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
