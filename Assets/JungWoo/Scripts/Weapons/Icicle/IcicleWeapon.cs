using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Icicle 무기 - 원거리 투척 + 폭발 + 도트 데미지
    /// </summary>
    public class IcicleWeapon : SplashWeaponBase
    {
        [Header("Icicle Range Settings")]
        [SerializeField] private float minDistance = 5f;           // 최소 투척 거리
        [SerializeField] private float maxDistance = 15f;          // 최대 투척 거리

        [Header("Projectile Settings")]
        [SerializeField] private GameObject iciclePrefab;          // 고드름 프리팹
        [SerializeField] private float projectileHeight = 10f;     // 투사체 시작 높이
        [SerializeField] private float fallSpeed = 20f;            // 낙하 속도

        [Header("Effect Settings")]
        [SerializeField] private GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹
        [SerializeField] private GameObject dotEffectPrefab;       // DoT 이펙트 프리팹

        [Header("Runtime")]
        private float attackTimer = 0f;                            // 공격 타이머
        private List<IcicleProjectile> activeProjectiles;          // 활성 투사체 목록
        private List<DotZone> activeDotZones;                      // 활성 DoT 구역 목록

        protected override void Start()
        {
            weaponName = "Icicle";
            base.Start();

            // 리스트 초기화
            activeProjectiles = new List<IcicleProjectile>();
            activeDotZones = new List<DotZone>();

            // 프리팹이 없으면 기본 생성
            if (iciclePrefab == null)
            {
                CreateDefaultIciclePrefab();
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

            // 비활성 DoT 구역 정리
            CleanupInactiveDotZones();
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
                Debug.LogError("[Icicle] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetSplashWeapon("Icicle", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                //Debug.Log($"[Icicle] Lv.{currentLevel} 로드 완료 - 폭발 데미지: {explosionDamage}, DoT: {dotDamage}, 지속시간: {duration}");
            }
            else
            {
                Debug.LogError($"[Icicle] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행
        /// </summary>
        protected override void PerformAttack()
        {
            // projectileCount 만큼 투사체 생성
            for (int i = 0; i < projectileCount; i++)
            {
                LaunchIcicle();
            }
        }

        /// <summary>
        /// 고드름 투척
        /// </summary>
        private void LaunchIcicle()
        {
            // 무작위 목표 지점 선택 (플레이어로부터 일정 거리)
            Vector3 targetPosition = GetRandomTargetPosition();

            // 투사체 시작 위치 (목표 지점 위)
            Vector3 startPosition = targetPosition + Vector3.up * projectileHeight;

            // 고드름 생성
            GameObject icicle = Instantiate(iciclePrefab, startPosition, Quaternion.identity);

            // IcicleProjectile 컴포넌트 추가/설정
            IcicleProjectile projectile = icicle.GetComponent<IcicleProjectile>();
            if (projectile == null)
            {
                projectile = icicle.AddComponent<IcicleProjectile>();
            }

            // 투사체 설정
            projectile.Initialize(targetPosition, fallSpeed, explosionRadius, explosionDamage,
                                 dotDamage, duration, explosionEffectPrefab, dotEffectPrefab);

            activeProjectiles.Add(projectile);

            //Debug.Log($"[Icicle] 고드름 발사! 목표: {targetPosition}");
        }

        /// <summary>
        /// 무작위 목표 지점 선택
        /// </summary>
        private Vector3 GetRandomTargetPosition()
        {
            // 플레이어로부터 무작위 방향
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            // 무작위 거리 (최소~최대 사이)
            float randomDistance = Random.Range(minDistance, maxDistance);

            // 목표 위치 계산
            Vector3 offset = new Vector3(
                Mathf.Cos(randomAngle) * randomDistance,
                0f,
                Mathf.Sin(randomAngle) * randomDistance
            );

            return player.position + offset;
        }

        /// <summary>
        /// 기본 고드름 프리팹 생성
        /// </summary>
        private void CreateDefaultIciclePrefab()
        {
            iciclePrefab = new GameObject("IciclePrefab");

            // 기본 메시 (원뿔 모양으로 고드름 표현)
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(iciclePrefab.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(0.3f, 2f, 0.3f);
            visual.transform.localRotation = Quaternion.Euler(0, 0, 180); // 뒤집어서 고드름처럼

            // 하늘색 머티리얼
            Renderer renderer = visual.GetComponent<Renderer>();
            renderer.material.color = new Color(0.5f, 0.8f, 1f); // 얼음색

            // 콜라이더 제거 (투사체는 트리거로만 사용)
            Destroy(visual.GetComponent<Collider>());

            iciclePrefab.SetActive(false);
        }

        /// <summary>
        /// 비활성 DoT 구역 정리
        /// </summary>
        private void CleanupInactiveDotZones()
        {
            activeDotZones.RemoveAll(zone => zone == null || !zone.IsActive);
        }

        /// <summary>
        /// 지속 공격 (사용하지 않음)
        /// </summary>
        protected override void ContinuousAttack()
        {
            // Icicle은 쿨다운 기반이므로 사용하지 않음
        }

        public override void LevelUp()
        {
            base.LevelUp();
            Debug.Log($"[Icicle] 레벨업! 현재 레벨: {currentLevel}");
        }
    }

    /// <summary>
    /// 고드름 투사체 컴포넌트
    /// </summary>
    public class IcicleProjectile : MonoBehaviour
    {
        private Vector3 targetPosition;
        private float fallSpeed;
        private float explosionRadius;
        private float explosionDamage;
        private float dotDamage;
        private float duration;
        private GameObject explosionEffectPrefab;
        private GameObject dotEffectPrefab;
        private bool hasExploded = false;

        /// <summary>
        /// 투사체 초기화
        /// </summary>
        public void Initialize(Vector3 target, float speed, float radius, float explDamage,
                              float dot, float dur, GameObject explEffect, GameObject dotEffect)
        {
            targetPosition = target;
            fallSpeed = speed;
            explosionRadius = radius;
            explosionDamage = explDamage;
            dotDamage = dot;
            duration = dur;
            explosionEffectPrefab = explEffect;
            dotEffectPrefab = dotEffect;
        }

        void Update()
        {
            if (hasExploded) return;

            // 목표 지점으로 낙하
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

            // 목표 도달 체크
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Explode();
            }
        }

        /// <summary>
        /// 폭발 처리
        /// </summary>
        private void Explode()
        {
            if (hasExploded) return;
            hasExploded = true;

            //Debug.Log($"[Icicle] 폭발! 위치: {transform.position}, 범위: {explosionRadius}");

            // 1차 폭발 데미지
            ApplyExplosionDamage();

            // 폭발 이펙트 생성
            if (explosionEffectPrefab != null)
            {
                GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
            }

            // DoT 구역 생성
            CreateDotZone();

            // 투사체 제거
            Destroy(gameObject);
        }

        /// <summary>
        /// 폭발 데미지 적용
        /// </summary>
        private void ApplyExplosionDamage()
        {
            // 폭발 범위 내 적 탐색
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

            //Debug.Log($"[Icicle] 폭발 범위 내 적: {enemies.Length}명");

            foreach (var enemyCollider in enemies)
            {
                var enemy = enemyCollider.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                    //Debug.Log($"[Icicle] {enemyCollider.name}에게 폭발 데미지 {explosionDamage} 적용");
                }
            }
        }

        /// <summary>
        /// DoT 구역 생성
        /// </summary>
        private void CreateDotZone()
        {
            if (dotDamage <= 0 || duration <= 0) return;

            // DoT 구역 오브젝트 생성
            GameObject dotZone = new GameObject($"IcicleDotZone_{Time.time}");
            dotZone.transform.position = transform.position;

            // DotZone 컴포넌트 추가
            DotZone zone = dotZone.AddComponent<DotZone>();
            zone.Initialize(explosionRadius, dotDamage, duration, dotEffectPrefab);

            //Debug.Log($"[Icicle] DoT 구역 생성 - 데미지: {dotDamage}/초, 지속: {duration}초");
        }
    }

    /// <summary>
    /// DoT 구역 컴포넌트
    /// </summary>
    public class DotZone : MonoBehaviour
    {
        private float radius;
        private float dotDamage;
        private float duration;
        private float endTime;
        private float lastDotTime;
        private GameObject effectObject;
        private ParticleSystem particleEffect;

        public bool IsActive { get; private set; }

        /// <summary>
        /// DoT 구역 초기화
        /// </summary>
        public void Initialize(float rad, float dot, float dur, GameObject effectPrefab)
        {
            radius = rad;
            dotDamage = dot;
            duration = dur;
            endTime = Time.time + duration;
            lastDotTime = Time.time;
            IsActive = true;

            // 파티클 이펙트 생성
            CreateParticleEffect(effectPrefab);

            // 트리거 콜라이더 추가
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = radius;
            collider.isTrigger = true;
        }

        /// <summary>
        /// 파티클 이펙트 생성
        /// </summary>
        private void CreateParticleEffect(GameObject effectPrefab)
        {
            if (effectPrefab != null)
            {
                // 프리팹에서 파티클 생성
                effectObject = Instantiate(effectPrefab, transform.position, Quaternion.identity, transform);

                // 프리팹의 ParticleSystem 찾기
                particleEffect = effectObject.GetComponent<ParticleSystem>();
                if (particleEffect == null)
                {
                    particleEffect = effectObject.GetComponentInChildren<ParticleSystem>();
                }

                // 프리팹 파티클의 startLifetime을 duration과 연동
                if (particleEffect != null)
                {
                    // 파티클 시스템 정지 (설정 변경을 위해)
                    particleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                    var main = particleEffect.main;
                    main.startLifetime = duration;  // startLifetime을 duration과 연동
                    main.duration = duration;        // 전체 지속시간도 duration과 연동
                    main.loop = false;              // 루프 비활성화

                    // 파티클 재생
                    particleEffect.Play();

                    //Debug.Log($"[DotZone] 프리팹 파티클 설정 - startLifetime: {duration}초");
                }
            }
            else
            {
                /*// 기본 파티클 생성
                effectObject = new GameObject("DotEffect");
                effectObject.transform.SetParent(transform);
                effectObject.transform.localPosition = Vector3.zero;

                particleEffect = effectObject.AddComponent<ParticleSystem>();

                // 파티클 메인 설정 (생성 직후라 Stop 불필요)
                var main = particleEffect.main;
                main.duration = duration;
                main.startLifetime = duration;  // startLifetime을 duration과 연동
                main.loop = false;
                main.startSpeed = 0.5f;
                main.startSize = 0.2f;
                main.startColor = new Color(0.5f, 0.8f, 1f, 0.5f); // 반투명 하늘색

                // 파티클 모양 설정
                var shape = particleEffect.shape;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = radius;

                // 이미션 설정
                var emission = particleEffect.emission;
                emission.rateOverTime = 10f;

                Debug.Log($"[DotZone] 기본 파티클 생성 - startLifetime: {duration}초");*/
            }

            // 이펙트 크기를 explosionRadius와 연동
            effectObject.transform.localScale = Vector3.one * radius;

            // 자식 파티클 시스템들도 duration 연동 (프리팹에 여러 파티클이 있는 경우)
            ParticleSystem[] childParticles = effectObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var childParticle in childParticles)
            {
                // 자기 자신은 제외 (이미 처리함)
                if (childParticle == particleEffect) continue;

                // 파티클 정지 후 설정 변경
                childParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                var childMain = childParticle.main;
                childMain.startLifetime = duration;  // 모든 자식 파티클의 startLifetime도 연동
                childMain.duration = duration;

                // 파티클 재생
                childParticle.Play();
            }

            //Debug.Log($"[DotZone] 파티클 최종 설정 - Scale: {effectObject.transform.localScale}, Duration: {duration}, 자식 파티클 수: {childParticles.Length}");
        }

        void Update()
        {
            // 지속 시간 체크
            if (Time.time >= endTime)
            {
                Deactivate();
                return;
            }

            // 1초마다 DoT 데미지
            if (Time.time - lastDotTime >= 1f)
            {
                ApplyDotDamage();
                lastDotTime = Time.time;
            }
        }

        /// <summary>
        /// 범위 내 적에게 DoT 데미지 적용
        /// </summary>
        private void ApplyDotDamage()
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
                //Debug.Log($"[DotZone] {enemies.Length}명에게 DoT 데미지 {dotDamage} 적용");
            }
        }

        /// <summary>
        /// DoT 구역 비활성화
        /// </summary>
        private void Deactivate()
        {
            IsActive = false;
            //Debug.Log($"[DotZone] 종료 - 위치: {transform.position}");
            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            // DoT 범위 표시
            Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}