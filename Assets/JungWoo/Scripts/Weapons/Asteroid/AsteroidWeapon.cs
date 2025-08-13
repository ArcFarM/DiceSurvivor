using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Asteroid 무기 - 플레이어 주위를 공전하는 소행성
    /// </summary>
    public class AsteroidWeapon : SplashWeaponBase
    {
        [Header("Asteroid Specific")]
        [SerializeField] private GameObject asteroidPrefab;            // 소행성 프리팹
        [SerializeField] private float orbitHeight = 0.2f;               // 공전 높이

        [Header("Runtime")]
        private float spawnTimer = 0f;                                 // 스폰 타이머
        private List<AsteroidOrbit> activeAsteroids;                   // 활성 소행성 목록
        private Coroutine deactivateCoroutine;                         // 비활성화 코루틴

        protected override void Start()
        {
            weaponName = "Asteroid";
            base.Start();

            // 리스트 초기화
            activeAsteroids = new List<AsteroidOrbit>();

        }

        protected override void Update()
        {
            if (player == null) return;

            // 쿨다운 체크
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= cooldown)
            {
                PerformAttack();
                spawnTimer = 0f;
            }

            // 활성 소행성들 업데이트
            UpdateActiveAsteroids();

            // 비활성 소행성 정리
            CleanupInactiveAsteroids();
        }

        /// <summary>
        /// 활성 소행성 업데이트
        /// </summary>
        private void UpdateActiveAsteroids()
        {
            for (int i = activeAsteroids.Count - 1; i >= 0; i--)
            {
                if (activeAsteroids[i] == null || !activeAsteroids[i].IsActive)
                {
                    activeAsteroids.RemoveAt(i);
                }
                else
                {
                    activeAsteroids[i].UpdateOrbit();
                }
            }
        }

        /// <summary>
        /// 무기 초기화
        /// </summary>
        protected override void InitializeWeapon()
        {
            LoadWeaponData();

            // 초기 소행성 생성
            PerformAttack();
        }

        /// <summary>
        /// 무기 데이터 로드
        /// </summary>
        protected override void LoadWeaponData()
        {
            var dataManager = DataTableManager.Instance;
            if (dataManager == null)
            {
                Debug.LogError("[Asteroid] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetSplashWeapon("Asteroid", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                Debug.Log($"[Asteroid] Lv.{currentLevel} 로드 - Count: {projectileCount}, Size: {projectileSize}, Speed: {projectileSpeed}, Duration: {duration}");
            }
            else
            {
                Debug.LogError($"[Asteroid] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행 - 소행성 생성
        /// </summary>
        protected override void PerformAttack()
        {
            // 기존 소행성 제거
            //CleanupAllAsteroids();

            // projectileCount 만큼 소행성 생성
            float angleStep = projectileCount;

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = angleStep * i;
                CreateAsteroid(angle);
            }

            // duration 후 비활성화
            if (deactivateCoroutine != null)
            {
                StopCoroutine(deactivateCoroutine);
            }
            deactivateCoroutine = StartCoroutine(DeactivateAfterDuration());

            Debug.Log($"[Asteroid] {projectileCount}개 소행성 생성 - Duration: {duration}초");
        }

        /// <summary>
        /// 소행성 생성
        /// </summary>
        private void CreateAsteroid(float startAngle)
        {
            // 소행성 생성 (플레이어가 아닌 월드 공간에 생성)
            GameObject asteroid = Instantiate(asteroidPrefab);
            asteroid.name = $"Asteroid";

            // AsteroidOrbit 컴포넌트 추가/설정
            AsteroidOrbit orbit = asteroid.GetComponent<AsteroidOrbit>();
            if (orbit == null)
            {
                orbit = asteroid.AddComponent<AsteroidOrbit>();
            }

            // 소행성 초기화
            orbit.Initialize(
                player,                 // 중심점 (플레이어)
                radius,                 // 공전 반경
                projectileSize,         // 크기
                damage,                 // 데미지
                projectileSpeed,        // 회전 속도
                startAngle,            // 시작 각도
                orbitHeight            // 공전 높이
            );

            activeAsteroids.Add(orbit);
        }

        /// <summary>
        /// duration 후 비활성화
        /// </summary>
        private IEnumerator DeactivateAfterDuration()
        {
            yield return new WaitForSeconds(duration);

            Debug.Log($"[Asteroid] Duration 종료 - 소행성 비활성화");
            CleanupAllAsteroids();
        }

        /// <summary>
        /// 비활성 소행성 정리
        /// </summary>
        private void CleanupInactiveAsteroids()
        {
            activeAsteroids.RemoveAll(asteroid => asteroid == null || !asteroid.IsActive);
        }

        /// <summary>
        /// 모든 소행성 제거
        /// </summary>
        private void CleanupAllAsteroids()
        {
            foreach (var asteroid in activeAsteroids)
            {
                if (asteroid != null)
                {
                    asteroid.Deactivate();
                }
            }
            activeAsteroids.Clear();
        }

        /// <summary>
        /// 지속 공격 (사용하지 않음)
        /// </summary>
        protected override void ContinuousAttack()
        {
            // Asteroid는 쿨다운 기반이므로 사용하지 않음
        }

        public override void LevelUp()
        {
            base.LevelUp();

            // 레벨업 시 즉시 새로운 소행성 생성
            CleanupAllAsteroids();
            PerformAttack();
            spawnTimer = 0f;

            Debug.Log($"[Asteroid] 레벨업! 현재 레벨: {currentLevel}");
        }

        protected override void OnDrawGizmosSelected()
        {
            // 공전 궤도 표시
            Gizmos.color = new Color(0.5f, 0.3f, 0.2f, 0.3f);
            if (player != null)
            {
                Gizmos.DrawWireSphere(player.position, radius);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }

        private void OnDestroy()
        {
            if (deactivateCoroutine != null)
            {
                StopCoroutine(deactivateCoroutine);
            }
            CleanupAllAsteroids();
        }
    }

    /// <summary>
    /// 소행성 공전 컴포넌트
    /// </summary>
    public class AsteroidOrbit : MonoBehaviour
    {
        private Transform centerPoint;         // 공전 중심 (플레이어)
        private float orbitRadius;             // 공전 반경
        private float asteroidSize;            // 소행성 크기
        private float damage;                  // 데미지
        private float orbitSpeed;              // 공전 속도
        private float currentAngle;            // 현재 각도
        private float orbitHeight;             // 공전 높이

        private HashSet<GameObject> hitEnemies; // 이미 타격한 적 목록

        public bool IsActive { get; private set; }

        /// <summary>
        /// 소행성 초기화
        /// </summary>
        public void Initialize(Transform center, float radius, float size, float dmg,
                             float speed, float startAngle, float height)
        {
            centerPoint = center;
            orbitRadius = radius;
            asteroidSize = size;
            damage = dmg;
            orbitSpeed = speed * 50f; // 속도 조정 (도/초)
            currentAngle = startAngle;
            orbitHeight = height;

            hitEnemies = new HashSet<GameObject>();
            IsActive = true;

            // 크기 설정
            transform.localScale = Vector3.one * asteroidSize;

            // Collider 크기 조정
            SphereCollider collider = GetComponent<SphereCollider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<SphereCollider>();
                collider.isTrigger = true;
            }

            // 초기 위치 설정
            UpdatePosition();

            Debug.Log($"[AsteroidOrbit] 초기화 완료 - 위치: {transform.position}");
        }

        /// <summary>
        /// 공전 업데이트 (AsteroidWeapon에서 호출)
        /// </summary>
        public void UpdateOrbit()
        {
            if (!IsActive || centerPoint == null) return;

            // 공전
            currentAngle += orbitSpeed * Time.deltaTime;
            if (currentAngle >= 360f)
            {
                currentAngle -= 360f;
                // 한 바퀴 돌면 타격 목록 초기화
                hitEnemies.Clear();
            }

            UpdatePosition();
        }

        /// <summary>
        /// 위치 업데이트
        /// </summary>
        private void UpdatePosition()
        {
            if (centerPoint == null) return;

            // 원형 공전 위치 계산
            float radian = currentAngle * Mathf.Deg2Rad;
            float x = centerPoint.position.x + Mathf.Cos(radian) * orbitRadius;
            float z = centerPoint.position.z + Mathf.Sin(radian) * orbitRadius;
            float y = centerPoint.position.y + orbitHeight;

            transform.position = new Vector3(x, y, z);
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
                // 이미 타격한 적인지 체크
                if (!hitEnemies.Contains(other.gameObject))
                {
                    // 데미지 적용
                    var enemy = other.GetComponent<WJEnemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        hitEnemies.Add(other.gameObject);

                        Debug.Log($"[AsteroidOrbit] {other.name}에게 데미지 {damage} 적용");

                        /*// 타격 이펙트
                        CreateHitEffect(other.transform.position);*/
                    }
                }
            }
        }

        /// <summary>
        /// 타격 이펙트 생성
        /// </summary>
        /*private void CreateHitEffect(Vector3 position)
        {
            // 간단한 이펙트
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = position;
            effect.transform.localScale = Vector3.one * 0.5f;

            // 콜라이더 제거
            Destroy(effect.GetComponent<Collider>());

            // 노란색 반투명
            Renderer renderer = effect.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(1f, 1f, 0f, 0.5f);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            renderer.material = mat;

            // 0.2초 후 제거
            Destroy(effect, 0.2f);
        }*/

        /// <summary>
        /// 소행성 비활성화
        /// </summary>
        public void Deactivate()
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
            float fadeTime = 0.5f;
            float elapsedTime = 0f;
            Vector3 originalScale = transform.localScale;

            while (elapsedTime < fadeTime)
            {
                if (this != null && transform != null)
                {
                    transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / fadeTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (centerPoint != null)
            {
                // 공전 궤도 표시
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(centerPoint.position, orbitRadius);

                // 현재 위치에서 중심까지 선
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, centerPoint.position);
            }
        }
    }
}