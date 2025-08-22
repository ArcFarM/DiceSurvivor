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
    public class AsteroidWeapon : SplashWeaponBases
    {
        [Header("Asteroid Specific")]
        [SerializeField] private GameObject asteroidPrefab;            // 소행성 프리팹
        [SerializeField] private float orbitHeight = 0.2f;               // 공전 높이

        [Header("Runtime")]
        private float spawnTimer = 0f;                                 // 스폰 타이머
        private List<AsteroidOrbit> activeAsteroids;                   // 활성 소행성 목록
        private Coroutine deactivateCoroutine;                         // 비활성화 코루틴
        private const float MinCooldown = 0.1f;                 // 1프레임 생성 방지용 최소 쿨다운

        private bool HasActiveWave => activeAsteroids != null && activeAsteroids.Count > 0;

        private void Awake()
        {
            // 이중 안전장치
            if (activeAsteroids == null) activeAsteroids = new List<AsteroidOrbit>();
        }

        protected override void Start()
        {
            weaponName = "Asteroid";
            base.Start();

            // 리스트 초기화
            activeAsteroids = new List<AsteroidOrbit>();
            if (asteroidPrefab == null)
            {
                Debug.LogError("[Asteroid] asteroidPrefab이 비어 있습니다. 인스펙터에서 지정하세요.");
            }

        }

        protected override void Update()
        {
            if (player == null) return;

            // 쿨다운 체크
            spawnTimer += Time.deltaTime;

            // 활성 파동이 없을 때만 새 파동 생성 (프레임당 무한 생성 방지)
            float cd = Mathf.Max(cooldown, MinCooldown);
            if (!HasActiveWave && spawnTimer >= cd)
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
            if (activeAsteroids == null) return; // NRE 1차 방어
            
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

            /*// 시작 시 한 번만 소환 (이미 활성 파동이 있으면 중복 소환 금지)
            if (!HasActiveWave)
            {
                PerformAttack();
                spawnTimer = 0f;
            }*/
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
            if (asteroidPrefab == null || player == null) return;

            //if (HasActiveWave) return;

            // 기존 소행성 제거
            //CleanupAllAsteroids();

            // projectileCount 만큼 소행성 생성
            int count = Mathf.Max(1, projectileCount);
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i;
                CreateAsteroid(angle);
            }

            // duration 경과 후 전체 제거
            if (deactivateCoroutine != null) StopCoroutine(deactivateCoroutine);
            if (duration > 0f)
                deactivateCoroutine = StartCoroutine(DeactivateAfterDuration());

            Debug.Log($"[Asteroid] {projectileCount}개 소행성 생성 - Duration: {duration}초");
        }

        /// <summary>
        /// 소행성 생성
        /// </summary>
        private void CreateAsteroid(float startAngle)
        {
            // 소행성 생성 (플레이어가 아닌 월드 공간에 생성)
            GameObject asteroid = Instantiate(asteroidPrefab, player.position, Quaternion.identity);
            asteroid.name = $"Asteroid";

            asteroid.transform.SetParent(this.transform, worldPositionStays: true);

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

            
            if (duration < cooldown)
            {
                CleanupAllAsteroids();
                Debug.Log($"[Asteroid] Duration 종료 - 소행성 비활성화");
            }
            
        }

        /// <summary>
        /// 비활성 소행성 정리
        /// </summary>
        private void CleanupInactiveAsteroids()
        {
            if (activeAsteroids == null) return;
            activeAsteroids.RemoveAll(asteroid => asteroid == null || !asteroid.IsActive);
        }

        /// <summary>
        /// 모든 소행성 제거
        /// </summary>
        private void CleanupAllAsteroids()
        {
            if (activeAsteroids == null) return;
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
            spawnTimer = 0f;
            PerformAttack();

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
    
    
}