using System; // System 네임스페이스 사용 (Serializable 어트리뷰트를 위해)
using UnityEngine; // Unity 엔진 기능 사용

namespace DiceSurvivor.WeaponDataSystem
{
    [Serializable] // 이 클래스를 직렬화 가능하게 만듦 (JSON 변환을 위해)
    public class WeaponData
    {
        [Header("기본 정보")]                        // Inspector에서 헤더로 표시
        public string weaponName;                   // 무기 이름
        public string description;                  // 무기 설명
        public WeaponType weaponType;               // 무기 타입 (투사체, 근접, 공전 등)

        [Header("레벨별 속성")]                      // Inspector에서 헤더로 표시
        public float damage;                        // 데미지 값
        public float radius;                        // 무기 반경 (범위)
        public float cooldown;                      // 쿨다운 시간 (초)
        public float projectileSpeed;               // 투사체 속도
        public int projectileCount;                 // 투사체 개수
        public float duration;                      // 지속 시간

        [Header("특수 속성 (JSON에서 사용)")]         // Inspector에서 헤더로 표시
        public float explosionRadius;               // 폭발 반경 (Icicle, LightningStaff용)
        public float explosionDamage;               // 폭발 데미지 (Icicle, LightningStaff용)
        public float dotDamage;                     // 지속 데미지 (KillingAura, Icicle용)

        [Header("특수 속성 (선택사항)")]              // Inspector에서 헤더로 표시
        public float penetration = 1f;              // 관통력 (몇 개의 적을 관통할 수 있는지)
        public float knockback = 2f;                // 넉백 힘
        public bool hasAreaDamage = false;          // 범위 데미지 여부
        public float areaDamageRadius = 1f;         // 범위 데미지 반경
        public float orbitRadius = 2f;              // 공전 무기의 공전 반경
        public float orbitSpeed = 180f;             // 공전 무기의 공전 속도 (도/초)

        // 공격 속도 계산 (쿨다운의 역수)
        public float GetAttackSpeed() => 1f / cooldown; // 쿨다운을 공격속도로 변환하는 함수
    }

    public enum WeaponType // 무기 타입을 정의하는 열거형
    {
        Projectile,  // 투사체 타입
        Melee,       // 근접 타입
        Orbit,       // 공전 타입
        DonutArea    // 도넛형 범위 타입
    }
}