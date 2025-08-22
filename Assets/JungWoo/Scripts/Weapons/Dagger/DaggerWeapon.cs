using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Dagger 무기 - 연속 발사형
    /// </summary>
    public class DaggerWeapon : RangedWeapon
    {
        [Header("Dagger Specific")]
        [SerializeField] private GameObject DaggerPrefab;             // Dagger 프리팹
        [SerializeField] private float spawnHeightOffset = 0.5f;       // 투사체 발사 높이

        [Header("Runtime")]
        private List<DaggerProjectile> activeDaggers;                // 활성 Dagger 목록
        private float attackTimer = 0f;                                // 공격 타이머

        protected override void Start()
        {
            weaponName = "Dagger";
            base.Start();

            // 리스트 초기화
            activeDaggers = new List<DaggerProjectile>();

            
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

            // 비활성 차크람 정리
            CleanupInactiveDaggers();
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
                Debug.LogError("[Dagger] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetRangedWeapon("Dagger", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                Debug.Log($"[Dagger] Lv.{currentLevel} 로드 - Damage: {damage}, Count: {projectileCount}");
            }
            else
            {
                Debug.LogError($"[Dagger] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행
        /// </summary>
        protected override void PerformAttack()
        {
            // 가장 가까운 적 찾기
            GameObject closestEnemy = FindClosestEnemy();

            Vector3 targetDirection;
            if (closestEnemy != null)
            {
                targetDirection = (closestEnemy.transform.position - transform.position).normalized;
            }
            else
            {
                targetDirection = transform.forward;
            }

            // 병렬 발사를 위한 오프셋 계산
            Vector3 perpDirection = Vector3.Cross(targetDirection, Vector3.up).normalized;
            float totalOffset = (projectileCount - 1) * 0.2f;
            Vector3 startOffset = -perpDirection * (totalOffset / 2f);

            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 currentOffset = startOffset + perpDirection * (i * 0.2f);
                LaunchDagger(targetDirection, currentOffset);
            }
            //LaunchDagger(targetDirection);
            
        }

        /// <summary>
        /// 차크람 발사
        /// </summary>
        private void LaunchDagger(Vector3 direction, Vector3 offset)
        {
            // 차크람 생성
            GameObject Dagger = Instantiate(DaggerPrefab, transform.position + offset + Vector3.up * 0.5f * spawnHeightOffset, Quaternion.identity);

            // DaggerProjectile 컴포넌트 추가/설정
            DaggerProjectile projectile = Dagger.GetComponent<DaggerProjectile>();
            if (projectile == null)
            {
                projectile = Dagger.AddComponent<DaggerProjectile>();
            }

            // 차크람 초기화
            projectile.Initialize(
                direction,              // 발사 방향
                damage,                 // 데미지
                range,                  // 최대 거리
                projectileSpeed,        // 이동 속도
                projectileSize         // 크기
            );

            activeDaggers.Add(projectile);

            Debug.Log($"[Dagger] 발사! 방향: {direction}");
        }

        /// <summary>
        /// 비활성 차크람 정리
        /// </summary>
        private void CleanupInactiveDaggers()
        {
            activeDaggers.RemoveAll(Dagger => Dagger == null || !Dagger.IsActive);
        }

        public override void LevelUp()
        {
            base.LevelUp();
            Debug.Log($"[Dagger] 레벨업! 현재 레벨: {currentLevel}");
        }
    }

    /// <summary>
    /// 차크람 투사체 컴포넌트
    /// </summary>
    public class DaggerProjectile : MonoBehaviour
    {
        private Vector3 moveDirection;         // 이동 방향
        private float damage;                  // 데미지
        private float maxRange;                // 최대 거리
        private float moveSpeed;               // 이동 속도
        private float projectileSize;          // 크기

        private Vector3 startPosition;         // 시작 위치
        private float traveledDistance;        // 이동한 거리
        private bool hasHitEnemy;              // 적 타격 여부

        public bool IsActive { get; private set; }

        /// <summary>
        /// 차크람 초기화
        /// </summary>
        public void Initialize(Vector3 direction, float dmg, float range, float speed, float size)
        {
            moveDirection = direction.normalized;
            damage = dmg;
            maxRange = range;
            moveSpeed = speed;
            projectileSize = size;

            startPosition = transform.position;
            traveledDistance = 0f;
            hasHitEnemy = false;
            IsActive = true;

            // 크기 설정
            transform.localScale = Vector3.one * projectileSize;

            // 투사체가 바라보는 방향 설정
            transform.LookAt(transform.position + moveDirection);

            Debug.Log($"[DaggerProjectile] 초기화 - 데미지: {damage}, 범위: {maxRange}, 속도: {moveSpeed}");
        }

        void Update()
        {
            if (!IsActive) return;

            // 이동
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            transform.position += movement;

            // 이동 거리 계산
            traveledDistance += movement.magnitude;

            // 최대 거리 도달 시 제거
            if (traveledDistance >= maxRange)
            {
                Deactivate();
            }
        }

        /// <summary>
        /// 적과 충돌 시
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive) return;

            // Enemy 레이어 체크
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // 즉시 데미지
                var enemy = other.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    hasHitEnemy = true;

                    Debug.Log($"[DaggerProjectile] {other.name}에게 데미지 {damage} 적용");

                    // 타격 이펙트
                    //CreateHitEffect(other.transform.position);

                    // 적과 부딪히면 즉시 사라짐
                    Deactivate();
                }
            }
        }

        /// <summary>
        /// 차크람 비활성화
        /// </summary>
        private void Deactivate()
        {
            
            Destroy(gameObject);
        }
    }
}