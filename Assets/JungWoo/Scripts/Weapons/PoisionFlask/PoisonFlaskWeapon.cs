using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Enemy;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// PoisonFlask 무기 - 곡선 투척 DoT 투사체
    /// </summary>
    public class PoisonFlaskWeapon : RangedWeaponBase
    {
        [Header("PoisonFlask Specific")]
        [SerializeField] private GameObject potionPrefab;              // 포션 프리팹
        [SerializeField] private GameObject poisonGasPrefab;           // 독가스 DoT 프리팹
        [SerializeField] private float arcHeight = 3f;                 // 포물선 높이
        [SerializeField] private float throwDuration = 1f;             // 투척 시간

        [Header("Runtime")]
        private List<PoisonFlaskProjectile> activeFlasks;              // 활성 플라스크 목록
        private float attackTimer = 0f;                                // 공격 타이머

        protected override void Awake()
        {
            base.Awake(); 
        }

        private void Start()
        {
            activeFlasks = new List<PoisonFlaskProjectile>();
        }

        protected override void Update()
        {
            // 쿨다운 체크
            attackTimer += Time.deltaTime;

            if (attackTimer >= cooldown)
            {
                Attack();
                attackTimer = 0f;
            }

            // 비활성 플라스크 정리
            CleanupInactiveFlasks();
        }

        protected override void ShootProjectile(GameObject target, int projectileCount)
        {
            // 가장 가까운 적들 찾기
            List<GameObject> closestEnemies = FindClosestEnemies(projectileCount);

            // 각 타겟에게 플라스크 투척
            for (int i = 0; i < projectileCount; i++)
            {
                if (i < closestEnemies.Count)
                {
                    target = closestEnemies[i];
                }

                ThrowFlask(target);
            }
        }

        /// <summary>
        /// 가장 가까운 N개의 적 찾기
        /// </summary>
        private List<GameObject> FindClosestEnemies(int count)
        {
            List<GameObject> enemies = new List<GameObject>();

            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] allEnemies = Physics.OverlapSphere(transform.position, 50f, enemyLayer);

            // 거리순 정렬
            System.Array.Sort(allEnemies, (a, b) =>
            {
                float distA = Vector3.Distance(transform.position, a.transform.position);
                float distB = Vector3.Distance(transform.position, b.transform.position);
                return distA.CompareTo(distB);
            });

            // 상위 N개 선택
            for (int i = 0; i < Mathf.Min(count, allEnemies.Length); i++)
            {
                enemies.Add(allEnemies[i].gameObject);
            }

            return enemies;
        }

        /// <summary>
        /// 플라스크 투척
        /// </summary>
        private void ThrowFlask(GameObject target)
        {
            // 타겟 위치 결정
            Vector3 targetPosition;
            if (target != null)
            {
                targetPosition = target.transform.position;
            }
            else
            {
                // 타겟이 없으면 전방 랜덤 위치
                float randomAngle = Random.Range(-30f, 30f);
                Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * transform.forward;
                targetPosition = transform.position + direction * Weapon.range;
            }

            // 플라스크 생성
            GameObject flask = Instantiate(potionPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            flask.name = $"PoisonFlask_{Time.time}";

            // PoisonFlaskProjectile 컴포넌트 추가/설정
            PoisonFlaskProjectile projectile = flask.GetComponent<PoisonFlaskProjectile>();
            if (projectile == null)
            {
                projectile = flask.AddComponent<PoisonFlaskProjectile>();
            }

            // 플라스크 초기화
            projectile.Initialize(
                transform.position,     // 시작 위치
                targetPosition,         // 목표 위치
                arcHeight,              // 포물선 높이
                throwDuration,          // 투척 시간
                Weapon.explosionDamage,        // 폭발 데미지
                Weapon.explosionRadius,        // 폭발 범위
                Weapon.dotDamage,              // DoT 데미지
                Weapon.duration,               // DoT 지속시간
                Weapon.projectileSize,         // 크기
                poisonGasPrefab        // 독가스 프리팹
            );

            activeFlasks.Add(projectile);

            Debug.Log($"[PoisonFlask] 투척! 타겟: {(target != null ? target.name : "없음")}");
        }

        /// <summary>
        /// 비활성 플라스크 정리
        /// </summary>
        private void CleanupInactiveFlasks()
        {
            activeFlasks.RemoveAll(flask => flask == null || !flask.IsActive);
        }
    }

    /// <summary>
    /// 독 플라스크 투사체 컴포넌트
    /// </summary>
    public class PoisonFlaskProjectile : MonoBehaviour
    {
        private Vector3 startPosition;         // 시작 위치
        private Vector3 targetPosition;        // 목표 위치
        private float arcHeight;               // 포물선 높이
        private float throwDuration;           // 투척 시간
        private float explosionDamage;         // 폭발 데미지
        private float explosionRadius;         // 폭발 범위
        private float dotDamage;               // DoT 데미지
        private float dotDuration;             // DoT 지속시간
        private float projectileSize;          // 크기
        private GameObject poisonGasPrefab;    // 독가스 프리팹

        private float elapsedTime;             // 경과 시간
        private bool hasLanded;                // 착지 여부

        public bool IsActive { get; private set; }

        /// <summary>
        /// 플라스크 초기화
        /// </summary>
        public void Initialize(Vector3 start, Vector3 target, float arc, float duration,
                             float explDamage, float explRadius, float dot, float dotDur, float size, GameObject gasPrefab)
        {
            startPosition = start;
            targetPosition = target;
            targetPosition.y = 0f; // 바닥 높이로 조정
            arcHeight = arc;
            throwDuration = duration;
            explosionDamage = explDamage;
            explosionRadius = explRadius;
            dotDamage = dot;
            dotDuration = dotDur;
            projectileSize = size;
            poisonGasPrefab = gasPrefab;

            elapsedTime = 0f;
            hasLanded = false;
            IsActive = true;

            // 크기 설정
            transform.localScale = Vector3.one * projectileSize;

            Debug.Log($"[PoisonFlaskProjectile] 초기화 - 폭발: {explosionDamage}, DoT: {dotDamage}");
        }

        void Update()
        {
            if (!IsActive || hasLanded) return;

            // 포물선 이동
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / throwDuration;

            if (progress >= 1f)
            {
                // 착지
                transform.position = targetPosition;
                Land();
            }
            else
            {
                // 포물선 계산
                Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, progress);
                currentPos.y += arcHeight * 4f * progress * (1f - progress); // 포물선 높이
                transform.position = currentPos;

                // 회전
                transform.Rotate(Vector3.right * 360f * Time.deltaTime);
            }
        }

        /// <summary>
        /// 바닥 충돌 감지
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive || hasLanded) return;

            // Ground 레이어 체크
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Land();
            }
        }

        /// <summary>
        /// 착지 처리
        /// </summary>
        private void Land()
        {
            if (hasLanded) return;
            hasLanded = true;

            Debug.Log($"[PoisonFlaskProjectile] 착지! 위치: {transform.position}");

            // 1차 폭발 데미지
            ApplyExplosionDamage();

            // DoT 구역 생성
            CreatePoisonZone();

            // 플라스크 제거
            IsActive = false;
            Destroy(gameObject);
        }

        /// <summary>
        /// 폭발 데미지 적용
        /// </summary>
        private void ApplyExplosionDamage()
        {
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

            Debug.Log($"[PoisonFlaskProjectile] 폭발! 범위: {explosionRadius}, 적: {enemies.Length}명");

            foreach (var enemyCollider in enemies)
            {
                var enemy = enemyCollider.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                    Debug.Log($"[PoisonFlaskProjectile] 폭발 데미지: {enemyCollider.name} - {explosionDamage}");
                }
            }

        }

        /// <summary>
        /// 독 구역 생성
        /// </summary>
        private void CreatePoisonZone()
        {
            GameObject poisonZone = new GameObject($"PoisonZone_{Time.time}");
            poisonZone.transform.position = transform.position;

            PoisonDotZone zone = poisonZone.AddComponent<PoisonDotZone>();
            zone.Initialize(explosionRadius, dotDamage, dotDuration, projectileSize, poisonGasPrefab);

            Debug.Log($"[PoisonFlaskProjectile] 독 구역 생성 - DoT: {dotDamage}/초, 지속: {dotDuration}초");
        }
       
    }

    /// <summary>
    /// 독 DoT 구역
    /// </summary>
    public class PoisonDotZone : MonoBehaviour
    {
        private float radius;
        private float dotDamage;
        private float duration;
        private float projectileSize;
        private GameObject gasPrefab;
        private GameObject gasEffect;

        private float endTime;
        private float lastDotTime;

        public void Initialize(float rad, float dot, float dur, float size, GameObject prefab)
        {
            radius = rad;
            dotDamage = dot;
            duration = dur;
            projectileSize = size;
            gasPrefab = prefab;

            endTime = Time.time + duration;
            lastDotTime = Time.time;

            // 트리거 콜라이더
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = radius;
            collider.isTrigger = true;

            // 독가스 프리팹이 있으면 사용, 없으면 기본 시각화
            if (gasPrefab != null)
            {
                CreateGasEffectFromPrefab();
            }
        }

        /// <summary>
        /// 독가스 프리팹으로 이펙트 생성
        /// </summary>
        private void CreateGasEffectFromPrefab()
        {
            // 프리팹 인스턴스 생성
            gasEffect = Instantiate(gasPrefab, transform.position, Quaternion.identity, transform);

            // 프리팹 스케일을 projectileSize와 연동
            gasEffect.transform.localScale = Vector3.one * projectileSize;

            // 자식 ParticleSystem들의 startLifetime을 duration과 연동
            ParticleSystem[] particleSystems = gasEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                // 파티클 시스템 정지
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                // main 모듈 설정
                var main = ps.main;
                main.startLifetime = duration;  // startLifetime을 duration과 연동
                main.loop = false;              // 루프 비활성화

                // shape 모듈에서 radius 조정 (원형인 경우)
                var shape = ps.shape;
                if (shape.shapeType == ParticleSystemShapeType.Circle ||
                    shape.shapeType == ParticleSystemShapeType.Sphere)
                {
                    shape.radius = radius;
                }

                // 파티클 재생
                ps.Play();

                Debug.Log($"[PoisonDotZone] ParticleSystem 설정 - StartLifetime: {duration}초, Scale: {projectileSize}");
            }

            Debug.Log($"[PoisonDotZone] 독가스 프리팹 생성 - Scale: {gasEffect.transform.localScale}, Duration: {duration}초");
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

            if (enemies.Length > 0)
            {
                Debug.Log($"[PoisonDotZone] {enemies.Length}명에게 DoT 데미지 {dotDamage} 적용");
            }
        }
    }
}