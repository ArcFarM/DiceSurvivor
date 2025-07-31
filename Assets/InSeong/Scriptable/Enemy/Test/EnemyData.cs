using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject {

    public enum EnemyType {
        Normal = 0,
        Elite = 1,
        Boss = 2,
        Summoned = 3, //Summoned = 소환수 형태 적으로, 처치 시 보상 없음
    }
    [Header("적 고유 정보")]
    public string enemyName;
    public GameObject body; //적이 사용할 육체
    [Header("적 능력치")]
    public EnemyType type;
    public float health;
    public float damage;
    public float speed;
    public float armor;
    [Header("처치 시 보상")]
    public float exp; //처치 시 드랍할 경험치
    public float gold; //처치 시 드랍할 골드 
    public float expDropRate; //경험치 드랍 확률 (확률의 모수 -> 1/expDropRate)
    public float goldDropRate; //골드 드랍 확률 (확률의 모수 -> 1/goldDropRate)
    public bool boxDropSwitch; //처치 시 보상 상자 출현 여부
}
