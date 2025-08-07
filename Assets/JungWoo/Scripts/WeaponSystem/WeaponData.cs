using System; // System 네임스페이스 사용 (Serializable 어트리뷰트를 위해)
using UnityEngine; // Unity 엔진 기능 사용

namespace DiceSurvivor.WeaponDataSystem
{
    [Serializable] // 이 클래스를 직렬화 가능하게 만듦 (JSON 변환을 위해)
    public class WeaponData
    {
        [Header("기본 정보")]
        public string weaponName;                   // 무기 이름
        public string description;                  // 무기 설명
        public WeaponType weaponType;               // 무기 타입 (투사체, 근접, 공전 등)

        [Header("레벨별 속성")]
        public float damage = 0f;                   // 데미지 값
        public float range = 0f;                    // 사거리
        public float cooldown = 1f;                 // 쿨다운 시간 (초)

        [Header("투사체 속성")]
        public float attackSpeed;                   // 공격 속도
        public float projectileSpeed = 0f;          // 투사체 속도
        public int projectileCount = 1;             // 투사체 개수
        public float projectileSize = 1f;           // 투사체 크기
        public bool isPiercing = false;             // 관통 여부
        public bool canReturn = false;              // 투사체 다시 돌아오기

        [Header("폭발/범위 속성")]
        public float explosionRadius = 0f;          // 폭발 반경 (Icicle, LightningStaff용)
        public float explosionDamage = 0f;          // 폭발 데미지 (Icicle, LightningStaff용)
        public float radius = 1f;                   // 범위 데미지 반경

        [Header("지속 효과")]
        public float dotDamage;                     // 지속 데미지 (KillingAura, Icicle용)
        public float duration;                      // 지속 시간


        [Header("게임 플레이 속성")]
        public float penetration = 1f;              // 관통력 (몇 개의 적을 관통할 수 있는지)
        public float knockback = 2f;                // 넉백 힘
        public float orbitRadius = 2f;              // 공전 무기의 공전 반경
        public float orbitSpeed = 180f;             // 공전 무기의 공전 속도 (도/초)

        // 공격 속도 계산 (쿨다운의 역수)
        public float GetAttackSpeed() => 1f / cooldown; // 쿨다운을 공격속도로 변환하는 함수
     

    // 무기 초기화 메서드 - 각 무기 스크립트에서 호출
        public void InitializeWeapon(MonoBehaviour weaponScript)
        {
            // 무기 타입에 따라 적절한 초기화 수행
            switch (weaponType)
            {
                case WeaponType.Projectile:
                    InitializeProjectileWeapon(weaponScript);
                    break;
                case WeaponType.Melee:
                    InitializeMeleeWeapon(weaponScript);
                    break;
                case WeaponType.Orbit:
                    InitializeOrbitWeapon(weaponScript);
                    break;
                case WeaponType.DonutArea:
                    InitializeAreaWeapon(weaponScript);
                    break;
            }
        }

        private void InitializeProjectileWeapon(MonoBehaviour weaponScript)
        {
            // 투사체 무기 초기화 로직
            Debug.Log($"투사체 무기 '{weaponName}' 초기화 - 데미지: {damage}, 속도: {projectileSpeed}, 개수: {projectileCount}");
        }

        private void InitializeMeleeWeapon(MonoBehaviour weaponScript)
        {
            // 근접 무기 초기화 로직
            Debug.Log($"근접 무기 '{weaponName}' 초기화 - 데미지: {damage}, 사거리: {range}, 쿨다운: {cooldown}");
        }

        private void InitializeOrbitWeapon(MonoBehaviour weaponScript)
        {
            // 공전 무기 초기화 로직
            Debug.Log($"공전 무기 '{weaponName}' 초기화 - 데미지: {damage}, 공전반경: {orbitRadius}, 속도: {orbitSpeed}");
        }

        private void InitializeAreaWeapon(MonoBehaviour weaponScript)
        {
            // 범위 무기 초기화 로직
            Debug.Log($"범위 무기 '{weaponName}' 초기화 - 반경: {radius}, DoT데미지: {dotDamage}, 지속시간: {duration}");
        }

        // 속성이 유효한지 체크하는 메서드들
        public bool HasExplosion() => explosionRadius > 0f && explosionDamage > 0f;
        public bool HasDotDamage() => dotDamage > 0f && duration > 0f;
        public bool HasProjectiles() => projectileCount > 0 && projectileSpeed > 0f;
        public bool HasAreaEffect() => radius > 0f || explosionRadius > 0f;
    }

    public enum WeaponType // 무기 타입을 정의하는 열거형
    {
        Projectile,  // 투사체 타입
        Melee,       // 근접 타입
        Orbit,       // 공전 타입
        DonutArea    // 도넛형 범위 타입
    }
}