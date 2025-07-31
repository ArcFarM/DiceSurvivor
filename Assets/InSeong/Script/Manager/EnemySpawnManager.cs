using UnityEngine;

public class EnemySpawnManger : MonoBehaviour
{
    //정해진 순서에 따라 적을 생성
    #region Variables
    //생성할 적 정보가 담긴 배열
    [SerializeField] private EnemyDataArray enemies;
    #endregion

    #region Properties
    #endregion

    #region Unity Event Methods
    #endregion

    #region Custom Methods
    void foo() {
        EnemyData[] tmp = enemies.enemyDataArray;
        tmp[1].enemyName = "Test"; //적 이름을 Test로 변경
    }
    #endregion
}
