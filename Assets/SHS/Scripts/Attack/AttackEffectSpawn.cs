using DiceSurvivor.Attack;
using DiceSurvivor.Weapon;
using UnityEngine;

namespace DiceSurvivor.SHS
{
    public class AttackEffectSpawn : MonoBehaviour
    {
        // 캐릭터 애니메이션을 제어할 Animator
        private Animator animator;

        // 현재 생성된 공격 이펙트의 인스턴스
        private ParticleSystem effect;

        // 무기 위치를 나타내는 소켓 (스피어/스태프 등은 이 위치를 따라감)
        public GameObject weaponSocket;

        // 생성할 공격 이펙트의 프리팹
        public ParticleSystem attackEffect;

        // 이펙트를 생성할 위치
        public Transform effectSpawnTransform;

        // 기본 이펙트 삭제 시간 (무기에 따라 변경 가능)
        [SerializeField] private float defaultDestroyTime = 5f;

        // 현재 장착 중인 무기 타입
        [SerializeField] private WeaponType currentWeapon;

        // 초기화 (Animator 참조 가져오기)
        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        // 매 프레임마다 입력 감지
        private void Update()
        {
            // 마우스 왼쪽 클릭 시 공격 애니메이션 트리거 실행
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("IsAttack");
            }
        }

        // LateUpdate: 모든 Update가 끝난 뒤 호출 (이펙트 위치를 무기 소켓에 맞춤)
        private void LateUpdate()
        {
            if (effect == null) return;

            switch (currentWeapon)
            {
                case WeaponType.Hammer:
                    // 해머는 위치 고정 (아무 것도 안 함)
                    break;

                case WeaponType.GreatSword:
                    // 대검 전용 위치 보정 (필요 시)
                    break;

                case WeaponType.Scythe:
                    // 낫 전용 위치 보정 (필요 시)
                    break;

                case WeaponType.Whip:
                    // 채찍 전용 위치 보정 (필요 시)
                    break;

                case WeaponType.Staff:
                case WeaponType.Spear:
                    // 스태프/스피어는 무기 소켓을 따라감
                    effect.transform.position = effectSpawnTransform.transform.position;
                    break;
            }
        }

        // 공격 이펙트를 생성하는 메서드 (Animation Event로 호출 가능)
        public void SpawnAttackEffect()
        {
            effect = Instantiate(attackEffect, effectSpawnTransform.position, effectSpawnTransform.rotation);

            float destroyTime = (currentWeapon == WeaponType.Staff || currentWeapon == WeaponType.Spear)
                ? 0.5f
                : defaultDestroyTime;

            switch (currentWeapon)
            {
                case WeaponType.Hammer:
                    WeaponController weaponController = this.GetComponent<WeaponController>();

                    // 해머 전용 이펙트 설정
                    WeaponSplashAttack aoe = effect.GetComponent<WeaponSplashAttack>();
                    aoe.weaponData = weaponController;
                    break;

                case WeaponType.Staff:
                case WeaponType.Spear:
                    effect.GetComponentInChildren<rotation>().topEnd = effectSpawnTransform;
                    break;

                case WeaponType.Whip:
                    effect.GetComponentInChildren<rotation>().topEnd = effectSpawnTransform;
                    break;
            }

            Destroy(effect.gameObject, destroyTime);
        }
    }
}
