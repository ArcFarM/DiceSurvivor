using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// PoisonFlask 무기 - 곡선 투척 DoT 투사체
    /// </summary>
    public class PoisonFlaskWeapon : RangedWeapon
    {
        [Header("PoisonFlask Specific")]
        [SerializeField] private GameObject potionPrefab;              // 포션 프리팹
        [SerializeField] private float arcHeight = 3f;                 // 포물선 높이
        [SerializeField] private float throwDuration = 1f;             // 투척 시간

        [Header("Runtime")]
        private List<PoisonFlaskProjectile> activeFlasks;              // 활성 플라스크 목록
        private float attackTimer = 0f;                                // 공격 타이머

        protected override void Start()
        {
            weaponName = "PoisonFlask";
            base.Start();

            // 리스트 초기화
            activeFlasks = new List<PoisonFlaskProjectile>();

            // 프리팹이 없으면 기본 생성
            if (potionPrefab == null)
            {
                CreateDefaultPotionPrefab();
            }
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

            // 비활성 플라스크 정리
            CleanupInactiveFlasks();
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
                Debug.LogError("[PoisonFlask] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetRangedWeapon("PoisonFlask", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                Debug.Log($"[PoisonFlask] Lv.{currentLevel} 로드 - ExplosionDamage: {explosionDamage}, DoT: {dotDamage}");
            }
            else
            {
                Debug.LogError($"[PoisonFlask] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행
        /// </summary>
        protected override void PerformAttack()
        {
            // 가장 가까운 적들 찾기
            List<GameObject> closestEnemies = FindClosestEnemies(projectileCount);

            // 각 타겟에게 플라스크 투척
            for (int i = 0; i < projectileCount; i++)
            {
                GameObject target = null;
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
                targetPosition = transform.position + direction * range;
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
                explosionDamage,        // 폭발 데미지
                explosionRadius,        // 폭발 범위
                dotDamage,              // DoT 데미지
                duration,               // DoT 지속시간
                projectileSize          // 크기
            );

            activeFlasks.Add(projectile);

            Debug.Log($"[PoisonFlask] 투척! 타겟: {(target != null ? target.name : "없음")}");
        }

        /// <summary>
        /// 기본 포션 프리팹 생성
        /// </summary>
        private void CreateDefaultPotionPrefab()
        {
            potionPrefab = new GameObject("PotionPrefab");

            // 기본 메시 (원통 - 플라스크 모양)
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(potionPrefab.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);

            // 포션 색상 (독 - 녹색)
            Renderer renderer = visual.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.8f, 0.2f);
            renderer.material = mat;

            // Collider 설정
            CapsuleCollider collider = potionPrefab.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                collider = potionPrefab.AddComponent<CapsuleCollider>();
            }
            collider.isTrigger = true;
            collider.radius = 0.15f;
            collider.height = 0.5f;

            // Rigidbody 추가
            Rigidbody rb = potionPrefab.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = potionPrefab.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;

            potionPrefab.SetActive(false);
        }

        /// <summary>
        /// 비활성 플라스크 정리
        /// </summary>
        private void CleanupInactiveFlasks()
        {
            activeFlasks.RemoveAll(flask => flask == null || !flask.IsActive);
        }

        public override void LevelUp()
        {
            base.LevelUp();
            Debug.Log($"[PoisonFlask] 레벨업! 현재 레벨: {currentLevel}");
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

        private float elapsedTime;             // 경과 시간
        private bool hasLanded;                // 착지 여부

        public bool IsActive { get; private set; }

        /// <summary>
        /// 플라스크 초기화
        /// </summary>
        public void Initialize(Vector3 start, Vector3 target, float arc, float duration,
                             float explDamage, float explRadius, float dot, float dotDur, float size)
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

            // 폭발 이펙트
            CreateExplosionEffect();
        }

        /// <summary>
        /// 독 구역 생성
        /// </summary>
        private void CreatePoisonZone()
        {
            GameObject poisonZone = new GameObject($"PoisonZone_{Time.time}");
            poisonZone.transform.position = transform.position;

            PoisonDotZone zone = poisonZone.AddComponent<PoisonDotZone>();
            zone.Initialize(explosionRadius, dotDamage, dotDuration);

            Debug.Log($"[PoisonFlaskProjectile] 독 구역 생성 - DoT: {dotDamage}/초, 지속: {dotDuration}초");
        }

        /// <summary>
        /// 폭발 이펙트 생성
        /// </summary>
        private void CreateExplosionEffect()
        {
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = transform.position;
            effect.transform.localScale = Vector3.one * explosionRadius * 2f;

            Destroy(effect.GetComponent<Collider>());

            Renderer renderer = effect.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            renderer.material = mat;

            Destroy(effect, 0.5f);
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

            // 시각화
            CreateVisual();
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

        private void CreateVisual()
        {
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);

            Destroy(visual.GetComponent<Collider>());

            Renderer renderer = visual.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.8f, 0.2f, 0.2f);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            renderer.material = mat;
        }
    }
}