using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Enemy;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Laser 무기 - 순간 발사 레이저 (0.1초 플래시)
    /// </summary>
    public class LaserWeapon : RangedWeapon
    {
        [Header("Laser Specific")]
        [SerializeField] private GameObject laserBeamPrefab;          // 레이저 투사체 프리팹
        [SerializeField] private GameObject hitEffectPrefab;          // 타격 이펙트 프리팹
        [SerializeField] private float burstDelay = 0.2f;             // 연발 간격

        [Header("Laser Width Settings")]
        [SerializeField] private float baseWidth = 0.5f;              // 기본 레이저 두께
        [SerializeField] private float widthPerLevel = 0.2f;          // 레벨당 두께 증가량
        [SerializeField] private float maxWidth = 2.0f;               // 최대 레이저 두께

        [Header("Runtime")]
        private List<LaserBeam> activeLasers;                         // 활성 레이저 목록
        private float attackTimer = 0f;                               // 공격 타이머
        private Coroutine burstFireCoroutine;                         // 연발 코루틴
        private bool isFiring = false;                                // 발사 중 플래그
        private float currentLaserWidth;                              // 현재 레이저 두께

        protected override void Start()
        {
            weaponName = "Laser";
            base.Start();

            activeLasers = new List<LaserBeam>();
            CalculateLaserWidth();
        }

        protected override void Update()
        {
            if (player == null) return;

            attackTimer += Time.deltaTime;

            // cooldown이 지나고 발사 중이 아닐 때만 새로운 공격 시작
            if (attackTimer >= cooldown && !isFiring)
            {
                PerformAttack();
                attackTimer = 0f;
            }

            CleanupInactiveLasers();
        }

        protected override void InitializeWeapon()
        {
            LoadWeaponData();
            CalculateLaserWidth();
        }

        protected override void LoadWeaponData()
        {
            var dataManager = DataTableManager.Instance;
            if (dataManager == null)
            {
                Debug.LogError("[Laser] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetRangedWeapon("Laser", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);

                // 레벨에 따른 연발 간격 조정
                burstDelay = Mathf.Max(0.05f, 0.1f / projectileCount);

                // 레벨에 따른 레이저 두께 계산
                CalculateLaserWidth();

                // 관통은 항상 true
                isPiercing = true;

                Debug.Log($"[Laser] Lv.{currentLevel} 로드");
                Debug.Log($"  - Damage: {damage}, Count: {projectileCount}, Range: {range}");
                Debug.Log($"  - BurstDelay: {burstDelay}, LaserWidth: {currentLaserWidth}");
            }
            else
            {
                Debug.LogError($"[Laser] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        private void CalculateLaserWidth()
        {
            currentLaserWidth = baseWidth + (widthPerLevel * (currentLevel - 1));
            currentLaserWidth = Mathf.Min(currentLaserWidth, maxWidth);
        }

        protected override void PerformAttack()
        {
            // cooldown마다 가장 먼 적 찾기
            GameObject farthestEnemy = FindFarthestEnemy(range);

            // 이전 연발이 진행 중이면 중단
            if (burstFireCoroutine != null)
            {
                StopCoroutine(burstFireCoroutine);
            }

            // 연발 발사 시작
            burstFireCoroutine = StartCoroutine(BurstFire(farthestEnemy));
        }

        /// <summary>
        /// 연발 발사 코루틴 - 같은 방향으로 연속 발사
        /// </summary>
        private IEnumerator BurstFire(GameObject targetEnemy)
        {
            isFiring = true;

            // 발사 방향 결정 (이번 공격 턴 동안 고정)
            Vector3 fireDirection;
            if (targetEnemy != null)
            {
                fireDirection = (targetEnemy.transform.position - player.position).normalized;
                Debug.Log($"[Laser] 타겟 발견: {targetEnemy.name}");
            }
            else
            {
                // 적이 없으면 플레이어 전방
                fireDirection = player.forward;
                Debug.Log("[Laser] 타겟 없음 - 전방 발사");
            }

            // projectileCount만큼 연속 발사 (같은 방향)
            for (int i = 0; i < projectileCount; i++)
            {
                LaunchLaserBeam(fireDirection);

                // 마지막 발사가 아니면 짧은 딜레이
                if (i < projectileCount - 1)
                {
                    yield return new WaitForSeconds(burstDelay);
                }
            }

            isFiring = false;
            burstFireCoroutine = null;
        }

        /// <summary>
        /// 레이저 발사 함수 - 순간 발사
        /// </summary>
        private void LaunchLaserBeam(Vector3 direction)
        {
            // 레이저 빔 생성 (플레이어 위치에 생성)
            GameObject laser = Instantiate(laserBeamPrefab, player.position, Quaternion.identity);
            laser.name = $"LaserBeam_Flash_{System.DateTime.Now.Ticks}";

            LaserBeam laserBeam = laser.GetComponent<LaserBeam>();
            if (laserBeam == null)
            {
                laserBeam = laser.AddComponent<LaserBeam>();
            }

            // 레이저 초기화 (0.1초 플래시)
            laserBeam.Initialize(player, direction, damage, range,
                hitEffectPrefab, currentLaserWidth);

            activeLasers.Add(laserBeam);
        }

        /// <summary>
        /// 범위 내에서 가장 먼 적을 찾습니다.
        /// </summary>
        private GameObject FindFarthestEnemy(float searchRange)
        {
            GameObject farthest = null;
            float maxDistance = 0f;
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, searchRange, enemyLayer);

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance > maxDistance && distance <= searchRange)
                {
                    maxDistance = distance;
                    farthest = enemy.gameObject;
                }
            }

            return farthest;
        }

        private void CleanupInactiveLasers()
        {
            activeLasers.RemoveAll(laser => laser == null || !laser.IsActive);
        }

        public override void LevelUp()
        {
            base.LevelUp();

            // 레벨업 시 연발 간격과 레이저 두께 재계산
            LoadWeaponData();

            Debug.Log($"[Laser] 레벨업! 현재 레벨: {currentLevel}");
            Debug.Log($"  - 연발 수: {projectileCount}, 연발 간격: {burstDelay}초");
            Debug.Log($"  - 레이저 두께: {currentLaserWidth}");
        }

        void OnDestroy()
        {
            // 코루틴 정리
            if (burstFireCoroutine != null)
            {
                StopCoroutine(burstFireCoroutine);
            }

            // 활성 레이저 정리
            foreach (var laser in activeLasers)
            {
                if (laser != null && laser.gameObject != null)
                {
                    Destroy(laser.gameObject);
                }
            }
            activeLasers.Clear();
        }
    }
}