/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Laser 무기 - 가장 먼 적을 향해 관통 레이저 발사
    /// </summary>
    public class LaserWeapon : RangedWeapon
    {
        [Header("Laser Specific")]
        [SerializeField] private GameObject laserPrefab;               // 레이저 프리팹
        [SerializeField] private float laserDuration = 0.5f;           // 레이저 지속 시간
        [SerializeField] private float laserWidth = 0.5f;              // 레이저 폭

        [Header("Runtime")]
        private List<LaserBeam> activeLasers;                          // 활성 레이저 목록
        private float attackTimer = 0f;                                // 공격 타이머

        protected override void Start()
        {
            weaponName = "Laser";
            base.Start();

            // 리스트 초기화
            activeLasers = new List<LaserBeam>();

            // 프리팹이 없으면 기본 생성
            if (laserPrefab == null)
            {
                CreateDefaultLaserPrefab();
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

            // 비활성 레이저 정리
            CleanupInactiveLasers();
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
                Debug.LogError("[Laser] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetRangedWeapon("Laser", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                Debug.Log($"[Laser] Lv.{currentLevel} 로드 - Damage: {damage}, Count: {projectileCount}, Size: {projectileSize}");
            }
            else
            {
                Debug.LogError($"[Laser] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행
        /// </summary>
        protected override void PerformAttack()
        {
            // projectileCount 만큼 레이저 발사
            for (int i = 0; i < projectileCount; i++)
            {
                StartCoroutine(FireLaserWithDelay(i * 0.1f));
            }
        }

        /// <summary>
        /// 딜레이 후 레이저 발사
        /// </summary>
        private IEnumerator FireLaserWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // 가장 먼 적 찾기
            GameObject farthestEnemy = FindFarthestEnemy();

            Vector3 targetDirection;
            if (farthestEnemy != null)
            {
                targetDirection = (farthestEnemy.transform.position - transform.position).normalized;
            }
            else
            {
                // 적이 없으면 전방으로
                targetDirection = transform.forward;
            }

            FireLaser(targetDirection);
        }

        /// <summary>
        /// 가장 먼 적 찾기
        /// </summary>
        private GameObject FindFarthestEnemy()
        {
            GameObject farthest = null;
            float maxDistance = 0f;

            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, range, enemyLayer);

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthest = enemy.gameObject;
                }
            }

            return farthest;
        }

        /// <summary>
        /// 레이저 발사
        /// </summary>
        private void FireLaser()
        {
            // 레이저 끝점 계산
            Vector3 endPosition = startPosition + direction * maxRange;

            // LineRenderer 위치 설정
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);

            // 레이캐스트로 경로상의 모든 적 검출
            RaycastHit[] hits = Physics.RaycastAll(startPosition, direction, maxRange);

            // Enemy 레이어만 필터링하고 데미지 적용
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (!hitEnemies.Contains(hit.collider.gameObject))
                    {
                        var enemy = hit.collider.GetComponent<DiceSurvivor.EnemySystem.Enemy>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(damage);
                            hitEnemies.Add(hit.collider.gameObject);

                            Debug.Log($"[LaserBeam] {hit.collider.name}에게 데미지 {damage} 적용");

                            // 타격 이펙트
                            CreateHitEffect(hit.point);
                        }
                    }
                }
            }

            // BoxCast로 더 넓은 범위 검사 (레이저 두께 고려)
            Vector3 halfExtents = new Vector3(beamWidth / 2f, beamWidth / 2f, maxRange / 2f);
            Vector3 center = startPosition + direction * (maxRange / 2f);

            Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.LookRotation(direction), LayerMask.GetMask("Enemy"));

            foreach (var collider in colliders)
            {
                if (!hitEnemies.Contains(collider.gameObject))
                {
                    var enemy = collider.GetComponent<DiceSurvivor.EnemySystem.Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        hitEnemies.Add(collider.gameObject);

                        Debug.Log($"[LaserBeam] 범위 내 {collider.name}에게 데미지 {damage} 적용");
                    }
                }
            }

            Debug.Log($"[LaserBeam] 레이저 발사 완료 - 타격 적: {hitEnemies.Count}명");
        }

        void Update()
        {
            if (!IsActive) return;

            // 지속 시간 체크
            if (Time.time >= endTime)
            {
                Deactivate();
            }

            // 레이저 깜빡임 효과 (옵션)
            if (lineRenderer != null)
            {
                float alpha = Mathf.PingPong(Time.time * 3f, 1f);
                Color startColor = lineRenderer.startColor;
                Color endColor = lineRenderer.endColor;
                startColor.a = alpha;
                endColor.a = alpha * 0.8f;
                lineRenderer.startColor = startColor;
                lineRenderer.endColor = endColor;
            }
        }

        /// <summary>
        /// 타격 이펙트 생성
        /// </summary>
        private void CreateHitEffect(Vector3 position)
        {
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = position;
            effect.transform.localScale = Vector3.one * 0.5f;

            Destroy(effect.GetComponent<Collider>());

            // 파란색 반투명
            Renderer renderer = effect.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.5f, 0.8f, 1f, 0.5f);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            renderer.material = mat;

            Destroy(effect, 0.3f);
        }

        /// <summary>
        /// 레이저 비활성화
        /// </summary>
        private void Deactivate()
        {
            IsActive = false;

            // 페이드 아웃 효과
            StartCoroutine(FadeOutAndDestroy());
        }

        /// <summary>
        /// 페이드 아웃 후 제거
        /// </summary>
        private IEnumerator FadeOutAndDestroy()
        {
            float fadeTime = 0.2f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                if (lineRenderer != null)
                {
                    float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                    Color startColor = lineRenderer.startColor;
                    Color endColor = lineRenderer.endColor;
                    startColor.a = alpha;
                    endColor.a = alpha;
                    lineRenderer.startColor = startColor;
                    lineRenderer.endColor = endColor;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
aser(Vector3 direction)
        {
    // 레이저 생성
    GameObject laser = Instantiate(laserPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    laser.name = $"Laser_{Time.time}";

    // LaserBeam 컴포넌트 추가/설정
    LaserBeam beam = laser.GetComponent<LaserBeam>();
    if (beam == null)
    {
        beam = laser.AddComponent<LaserBeam>();
    }

    // 레이저 초기화
    beam.Initialize(
        transform.position + Vector3.up * 0.5f,    // 시작 위치
        direction,                                  // 발사 방향
        damage,                                     // 데미지
        range,                                      // 최대 거리
        projectileSize,                             // 레이저 두께
        laserDuration                               // 지속 시간
    );

    activeLasers.Add(beam);

    Debug.Log($"[Laser] 발사! 방향: {direction}");
}

/// <summary>
/// 기본 레이저 프리팹 생성
/// </summary>
private void CreateDefaultLaserPrefab()
{
    laserPrefab = new GameObject("LaserPrefab");

    // LineRenderer로 레이저 표현
    LineRenderer lineRenderer = laserPrefab.AddComponent<LineRenderer>();
    lineRenderer.startWidth = 0.5f;
    lineRenderer.endWidth = 0.5f;
    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    lineRenderer.startColor = new Color(0, 0.8f, 1f);
    lineRenderer.endColor = new Color(0, 0.5f, 1f);

    laserPrefab.SetActive(false);
}

/// <summary>
/// 비활성 레이저 정리
/// </summary>
private void CleanupInactiveLasers()
{
    activeLasers.RemoveAll(laser => laser == null || !laser.IsActive);
}

public override void LevelUp()
{
    base.LevelUp();
    Debug.Log($"[Laser] 레벨업! 현재 레벨: {currentLevel}");
}
    }
    
    /// <summary>
    /// 레이저 빔 컴포넌트
    /// </summary>
    public class LaserBeam : MonoBehaviour
{
    private Vector3 startPosition;         // 시작 위치
    private Vector3 direction;             // 방향
    private float damage;                  // 데미지
    private float maxRange;                // 최대 거리
    private float beamWidth;               // 빔 두께
    private float duration;                // 지속 시간

    private LineRenderer lineRenderer;     // 라인 렌더러
    private HashSet<GameObject> hitEnemies; // 타격한 적 목록
    private float endTime;                 // 종료 시간

    public bool IsActive { get; private set; }

    /// <summary>
    /// 레이저 초기화
    /// </summary>
    public void Initialize(Vector3 start, Vector3 dir, float dmg, float range, float width, float dur)
    {
        startPosition = start;
        direction = dir.normalized;
        damage = dmg;
        maxRange = range;
        beamWidth = width;
        duration = dur;

        endTime = Time.time + duration;
        hitEnemies = new HashSet<GameObject>();
        IsActive = true;

        // LineRenderer 설정
        SetupLineRenderer();

        // 레이저 발사
        FireLaser();
    }

    /// <summary>
    /// LineRenderer 설정
    /// </summary>
    private void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 레이저 외관 설정
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.positionCount = 2;

        // 머티리얼 설정
        if (lineRenderer.material == null)
        {
            Material laserMat = new Material(Shader.Find("Sprites/Default"));
            laserMat.color = new Color(0, 0.8f, 1f);
            lineRenderer.material = laserMat;
        }

        // 색상 설정
        lineRenderer.startColor = new Color(0.5f, 0.8f, 1f, 1f);
        lineRenderer.endColor = new Color(0.3f, 0.6f, 1f, 0.8f);
    }

    /// <summary>
    /// 레이저 발사
    /// </summary>
    private void FireL*/