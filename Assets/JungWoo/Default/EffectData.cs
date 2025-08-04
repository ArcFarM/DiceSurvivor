using UnityEngine;

namespace DiceSurvivor.WeaponSystem
{
    /// <summary>
    /// 투사체 데이터 관리하는 클래스 
    /// 속성: 투사체 대미지, 갯수, 방향, 지속시간
    /// </summary>

    [System.Serializable]
    public class EffectData
    {
        public GameObject visualEffectPrefab;       // 시각 효과 프리팹

        [Header("광역 효과")]
        public bool isLineAttack;                   // 직선 공격 여부 (true면 화살처럼 직선)
        public float explosionRadius;               // 광역 범위 반경
        public float explosionDamage;               // 폭발 데미지
        public float dotDamage;                     // 지속 피해 (불꽃, 독 등)
        public float duration;                      // 지속 시간
        public bool isPiercing;                     // 관통 여부
        public bool canReturn;                      // 투사체 복귀 여부


        [Header("투사체 관련")]
        public int projectileCount;                 // 투사체 수
        public int throwCount;                      // 투사체 반복 던지는 횟수
        public float projectileSize;                //투사체 사이즈
        public float angleSpread = 0f;              // 투사체 퍼짐 각도
        public bool rotateAroundPlayer = false;     // 360도 회전 여부
        public bool shootTowardNearestEnemy = false;// 자동 조준 여부
    }

}