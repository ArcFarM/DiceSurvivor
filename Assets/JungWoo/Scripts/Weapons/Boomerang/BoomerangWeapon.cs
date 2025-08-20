using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Boomerang 무기 - 가장 가까운 적에게 투척 후 돌아오는 무기
    /// </summary>
    public class BoomerangWeapon : RangedWeapon
    {
        [Header("Boomerang Specific")]
        [SerializeField] private GameObject boomerangPrefab;           // 부메랑 프리팹
        [SerializeField] private float rotationSpeed = 720f;           // 회전 속도 (도/초)

        [Header("Runtime")]
        private List<BoomerangProjectile> activeBoomerangs;            // 활성 부메랑 목록
        private float attackTimer = 0f;                                // 공격 타이머

        protected override void Start()
        {
            weaponName = "Boomerang";
            base.Start();

            // 리스트 초기화
            activeBoomerangs = new List<BoomerangProjectile>();
        }

        protected override void Update()
        {
            if (player == null) return;

            // 쿨다운 체크
            attackTimer += Time.deltaTime;

            if (attackTimer >= cooldown)
            {
                PerformAttack();
                attackTimer = 0f;
            }

            // 비활성 부메랑 정리
            CleanupInactiveBoomerangs();
        }

        /// <summary>
        /// 무기 초기화
        /// </summary>
        protected override void InitializeWeapon()
        {
            LoadWeaponData();
        }

        /// <summary>
        /// 무기 데이터 로드
        /// </summary>
        protected override void LoadWeaponData()
        {
            var dataManager = DataTableManager.Instance;
            if (dataManager == null)
            {
                Debug.LogError("[Boomerang] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetRangedWeapon("Boomerang", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                Debug.Log($"[Boomerang] Lv.{currentLevel} 로드 - Damage: {damage}, Range: {range}, Count: {projectileCount}");
            }
            else
            {
                Debug.LogError($"[Boomerang] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행
        /// </summary>
        protected override void PerformAttack()
        {
            // 가장 가까운 적 찾기
            GameObject closestEnemy = FindClosestEnemy();

            if (closestEnemy == null)
            {
                Debug.Log("[Boomerang] 타겟이 없습니다.");
                return;
            }

            // projectileCount 만큼 부메랑 발사
            for (int i = 0; i < projectileCount; i++)
            {
                LaunchBoomerang(closestEnemy, i * 0.1f); // 약간의 딜레이를 두고 발사
            }
        }

        /// <summary>
        /// 부메랑 발사
        /// </summary>
        private void LaunchBoomerang(GameObject target, float delay)
        {
            StartCoroutine(LaunchBoomerangWithDelay(target, delay));
        }

        /// <summary>
        /// 딜레이 후 부메랑 발사
        /// </summary>
        private IEnumerator LaunchBoomerangWithDelay(GameObject target, float delay)
        {
            yield return new WaitForSeconds(delay);

            // 부메랑 생성
            GameObject boomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);

            // BoomerangProjectile 컴포넌트 추가/설정
            BoomerangProjectile projectile = boomerang.GetComponent<BoomerangProjectile>();
            if (projectile == null)
            {
                projectile = boomerang.AddComponent<BoomerangProjectile>();
            }

            // 초기 방향 설정 (타겟이 있으면 타겟 방향, 없으면 정면)
            Vector3 initialDirection;
            if (target != null)
            {
                initialDirection = (target.transform.position - transform.position).normalized;
            }
            else
            {
                initialDirection = transform.forward;
            }

            // 부메랑 초기화
            projectile.Initialize(
                transform,              // 발사 위치 (플레이어)
                initialDirection,       // 발사 방향
                damage,                 // 데미지
                range,                  // 최대 거리
                projectileSpeed,        // 이동 속도
                projectileSize,         // 크기
                isPiercing,             // 관통 여부
                rotationSpeed          // 회전 속도
            );

            activeBoomerangs.Add(projectile);

            Debug.Log($"[Boomerang] 발사! 타겟: {(target != null ? target.name : "없음")}");
        }

        /// <summary>
        /// 비활성 부메랑 정리
        /// </summary>
        private void CleanupInactiveBoomerangs()
        {
            activeBoomerangs.RemoveAll(boomerang => boomerang == null || !boomerang.IsActive);
        }

        public override void LevelUp()
        {
            base.LevelUp();
            Debug.Log($"[Boomerang] 레벨업! 현재 레벨: {currentLevel}");
        }
    }

    
}