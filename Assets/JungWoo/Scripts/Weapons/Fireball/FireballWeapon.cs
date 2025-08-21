using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Fireball 무기 - 무작위 방향으로 폭발 투사체 발사
    /// </summary>
    public class FireballWeapon : RangedWeapon
    {
        [Header("Fireball Specific")]
        [SerializeField] private GameObject fireballPrefab;            // 파이어볼 프리팹
        [SerializeField] private GameObject explosionEffectPrefab;     // 폭발 이펙트

        [Header("Runtime")]
        private List<FireballProjectile> activeFireballs;              // 활성 파이어볼 목록
        private float attackTimer = 0f;                                // 공격 타이머

        protected override void Start()
        {
            weaponName = "Fireball";
            base.Start();

            // 리스트 초기화
            activeFireballs = new List<FireballProjectile>();

            
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

            // 비활성 파이어볼 정리
            CleanupInactiveFireballs();
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
                Debug.LogError("[Fireball] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetRangedWeapon("Fireball", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                Debug.Log($"[Fireball] Lv.{currentLevel} 로드 - Damage: {damage}, ExplosionDamage: {explosionDamage}, DoT: {dotDamage}");
            }
            else
            {
                Debug.LogError($"[Fireball] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행
        /// </summary>
        protected override void PerformAttack()
        {
            // projectileCount 만큼 파이어볼 발사
            for (int i = 0; i < projectileCount; i++)
            {
                LaunchFireball();
            }
        }

        /// <summary>
        /// 파이어볼 발사
        /// </summary>
        private void LaunchFireball()
        {
            // 무작위 방향 생성
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;

            // 파이어볼 생성
            GameObject fireball = Instantiate(fireballPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
           

            // FireballProjectile 컴포넌트 추가/설정
            FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
            if (projectile == null)
            {
                projectile = fireball.AddComponent<FireballProjectile>();
            }

            // 파이어볼 초기화
            projectile.Initialize(
                randomDirection,        // 발사 방향
                damage,                 // 직접 데미지
                explosionDamage,        // 폭발 데미지
                explosionRadius,        // 폭발 범위
                dotDamage,              // DoT 데미지 (레벨 3부터)
                duration,               // DoT 지속시간
                range,                  // 최대 거리
                projectileSpeed,        // 이동 속도
                projectileSize,         // 투사체 크기
                explosionEffectPrefab   // 폭발 이펙트
            );

            activeFireballs.Add(projectile);

            Debug.Log($"[Fireball] 발사! 방향: {randomDirection}");
        }

        

        /// <summary>
        /// 비활성 파이어볼 정리
        /// </summary>
        private void CleanupInactiveFireballs()
        {
            activeFireballs.RemoveAll(fireball => fireball == null || !fireball.IsActive);
        }

        public override void LevelUp()
        {
            base.LevelUp();
            Debug.Log($"[Fireball] 레벨업! 현재 레벨: {currentLevel}");
        }
    }

    /// <summary>
    /// 파이어볼 투사체 컴포넌트
    /// </summary>
    public class FireballProjectile : MonoBehaviour
    {
        private Vector3 moveDirection;         // 이동 방향
        private float directDamage;           // 직접 데미지
        private float explosionDamage;        // 폭발 데미지
        private float explosionRadius;        // 폭발 범위
        private float dotDamage;              // DoT 데미지
        private float dotDuration;            // DoT 지속시간
        private float maxRange;               // 최대 거리
        private float moveSpeed;              // 이동 속도
        private float projectileSize;         // 투사체 크기
        private GameObject explosionEffect;   // 폭발 이펙트

        private Vector3 startPosition;        // 시작 위치
        private float traveledDistance;       // 이동한 거리
        private bool hasExploded;             // 폭발 여부

        public bool IsActive { get; private set; }

        /// <summary>
        /// 파이어볼 초기화
        /// </summary>
        public void Initialize(Vector3 direction, float damage, float explDamage, float explRadius,
                             float dot, float duration, float range, float speed, float size, GameObject effect)
        {
            moveDirection = direction.normalized;
            directDamage = damage;
            explosionDamage = explDamage;
            explosionRadius = explRadius;
            dotDamage = dot;
            dotDuration = duration;
            maxRange = range;
            projectileSize = size;
            explosionEffect = effect;
            moveSpeed = speed;               // projectileSpeed는 이동 시간(초)

            startPosition = transform.position;
            traveledDistance = 0f;
            hasExploded = false;
            IsActive = true;

            // 크기 설정
            transform.localScale = Vector3.one * projectileSize;

            

            Debug.Log($"[FireballProjectile] 초기화 - 직접: {directDamage}, 폭발: {explosionDamage}, DoT: {dotDamage}");
        }

        void Update()
        {
            if (!IsActive || hasExploded) return;

            // 이동
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            transform.position += movement;

            // 이동 거리 계산
            traveledDistance += movement.magnitude;

            // 최대 거리 도달 시 폭발
            if (traveledDistance >= maxRange)
            {
                Explode(transform.position);
            }
        }

        /// <summary>
        /// 적과 충돌 시
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive || hasExploded) return;

            // Enemy 레이어 체크
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // 직접 데미지
                var enemy = other.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(directDamage);
                    Debug.Log($"[FireballProjectile] 직접 타격: {other.name} - 데미지: {directDamage}");
                }

                // 충돌 위치에서 폭발
                Explode(transform.position);
            }
        }

        /// <summary>
        /// 폭발 처리
        /// </summary>
        private void Explode(Vector3 position)
        {
            if (hasExploded) return;
            hasExploded = true;

            Debug.Log($"[FireballProjectile] 폭발! 위치: {position}, 범위: {explosionRadius}");

            // 폭발 범위 내 적에게 데미지
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(position, explosionRadius, enemyLayer);

            foreach (var enemyCollider in enemies)
            {
                var enemy = enemyCollider.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    // 폭발 데미지
                    enemy.TakeDamage(explosionDamage);
                    Debug.Log($"[FireballProjectile] 폭발 데미지: {enemyCollider.name} - {explosionDamage}");
                }
            }

            // DoT 구역 생성 (레벨 3부터)
            if (dotDamage > 0 && dotDuration > 0)
            {
                CreateDotZone(position);
            }

            // 폭발 이펙트
            CreateExplosionEffect(position);

            // 투사체 제거
            IsActive = false;
            Destroy(gameObject);
        }

        /// <summary>
        /// DoT 구역 생성
        /// </summary>
        private void CreateDotZone(Vector3 position)
        {
            GameObject dotZone = new GameObject($"FireballDotZone_{Time.time}");
            dotZone.transform.position = position;

            FireDotZone zone = dotZone.AddComponent<FireDotZone>();
            zone.Initialize(explosionRadius, dotDamage, dotDuration);

            Debug.Log($"[FireballProjectile] DoT 구역 생성 - 데미지: {dotDamage}/초, 지속: {dotDuration}초");
        }

        /// <summary>
        /// 폭발 이펙트 생성
        /// </summary>
        private void CreateExplosionEffect(Vector3 position)
        {
            if (explosionEffect != null)
            {
                GameObject effect = Instantiate(explosionEffect, position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
    }

    /// <summary>
    /// 화염 DoT 구역
    /// </summary>
    public class FireDotZone : MonoBehaviour
    {
        private float radius;
        private float dotDamage;
        private float duration;
        private float endTime;
        private float lastDotTime;

        public void Initialize(float rad, float dot, float dur)
        {
            radius = rad;
            dotDamage = dot;
            duration = dur;
            endTime = Time.time + duration;
            lastDotTime = Time.time;

            // 트리거 콜라이더
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = radius;
            collider.isTrigger = true;
        }

        void Update()
        {
            if (Time.time >= endTime)
            {
                Destroy(gameObject);
                return;
            }

            // 1초마다 DoT
            if (Time.time - lastDotTime >= 1f)
            {
                ApplyDot();
                lastDotTime = Time.time;
            }
        }

        private void ApplyDot()
        {
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, radius, enemyLayer);

            foreach (var enemyCollider in enemies)
            {
                var enemy = enemyCollider.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(dotDamage);
                }
            }
        }
    }
}