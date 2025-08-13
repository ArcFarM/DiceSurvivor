using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Test;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// LightningStaff 무기 - 범위 내 가장 먼 적에게 번개 공격
    /// </summary>
    public class LightningStaffWeapon : SplashWeaponBase
    {
        [Header("LightningStaff Specific")]
        [SerializeField] private float targetRandomOffset = 0.5f;        // 타겟 주변 랜덤 오프셋
        [SerializeField] private float secondaryDamageDelay = 0.3f;    // 2차 데미지 지연 시간
        [SerializeField] private GameObject lightningPrefab;           // 번개 프리팹
        [SerializeField] private GameObject lightningEffectPrefab;     // 번개 이펙트 프리팹

        [Header("Runtime")]
        private float attackTimer = 0f;                                // 공격 타이머
        private List<LightningBolt> activeBolts;                       // 활성 번개 목록

        protected override void Start()
        {
            weaponName = "LightningStaff";
            base.Start();

            // 리스트 초기화
            activeBolts = new List<LightningBolt>();

            
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

            // 비활성 번개 정리
            CleanupInactiveBolts();
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
                Debug.LogError("[LightningStaff] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            var weaponStats = dataManager.GetSplashWeapon("LightningStaff", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);
                Debug.Log($"[LightningStaff] Lv.{currentLevel} 로드 - Damage: {damage}, ExplosionDamage: {explosionDamage}, Radius: {radius}");
            }
            else
            {
                Debug.LogError($"[LightningStaff] Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 공격 수행
        /// </summary>
        protected override void PerformAttack()
        {
            // radius 범위 내에서 가장 먼 적들 찾기
            List<GameObject> farthestEnemies = GetFarthestEnemiesInRange();

            if (farthestEnemies.Count == 0)
            {
                Debug.Log("[LightningStaff] 범위 내에 적이 없습니다.");
                return;
            }

            // projectileCount 만큼 번개 생성
            for (int i = 0; i < projectileCount && i < farthestEnemies.Count; i++)
            {
                // 랜덤하게 선택
                int randomIndex = Random.Range(0, farthestEnemies.Count);
                GameObject target = farthestEnemies[randomIndex];

                if (target != null)
                {
                    StrikeLightning(target);
                    farthestEnemies.RemoveAt(randomIndex); // 중복 방지
                }
            }
        }

        /// <summary>
        /// radius 범위 내에서 가장 먼 적들 찾기
        /// </summary>
        private List<GameObject> GetFarthestEnemiesInRange()
        {
            List<GameObject> enemiesInRange = new List<GameObject>();

            // radius 범위 내 모든 적 찾기
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, radius, enemyLayer);

            // 거리 계산 및 정렬
            System.Array.Sort(enemies, (a, b) =>
            {
                float distA = Vector3.Distance(transform.position, a.transform.position);
                float distB = Vector3.Distance(transform.position, b.transform.position);
                return distB.CompareTo(distA); // 내림차순 (먼 순서)
            });

            // GameObject 리스트로 변환
            foreach (var collider in enemies)
            {
                enemiesInRange.Add(collider.gameObject);
            }

            Debug.Log($"[LightningStaff] 범위({radius}) 내 적: {enemiesInRange.Count}명");

            return enemiesInRange;
        }

        /// <summary>
        /// 번개 공격
        /// </summary>
        private void StrikeLightning(GameObject target)
        {
            // 타겟 주변 랜덤 위치 계산
            Vector2 randomCircle = Random.insideUnitCircle * targetRandomOffset;
            Vector3 strikePosition = target.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // 번개 생성
            GameObject lightning = Instantiate(lightningPrefab, strikePosition, Quaternion.identity);

            // LightningBolt 컴포넌트 추가/설정
            LightningBolt bolt = lightning.GetComponent<LightningBolt>();
            if (bolt == null)
            {
                bolt = lightning.AddComponent<LightningBolt>();
            }

            // 번개 초기화
            bolt.Initialize(strikePosition, damage, projectileSize, explosionDamage,
                          explosionRadius, secondaryDamageDelay, lightningEffectPrefab);

            activeBolts.Add(bolt);

            Debug.Log($"[LightningStaff] 번개 생성! 타겟: {target.name}, 위치: {strikePosition}");
        }

        /// <summary>
        /// 비활성 번개 정리
        /// </summary>
        private void CleanupInactiveBolts()
        {
            //activeBolts.RemoveAll(bolt => bolt == null || !bolt.IsActive);
        }

        /// <summary>
        /// 지속 공격 (사용하지 않음)
        /// </summary>
        protected override void ContinuousAttack()
        {
            // LightningStaff는 쿨다운 기반이므로 사용하지 않음
        }

        public override void LevelUp()
        {
            base.LevelUp();
            Debug.Log($"[LightningStaff] 레벨업! 현재 레벨: {currentLevel}");
        }

        protected override void OnDrawGizmosSelected()
        {
            // 공격 범위 표시
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    /// <summary>
    /// 번개 볼트 컴포넌트
    /// </summary>
    public class LightningBolt : MonoBehaviour
    {
        private Vector3 targetPosition;
        private float primaryDamage;
        private float primaryRadius;
        private float secondaryDamage;
        private float secondaryRadius;
        private float secondaryDelay;
        private GameObject effectPrefab;

        private bool hasStruck = false;
        private bool hasExploded = false;

        public bool IsActive { get; private set; }

        /// <summary>
        /// 번개 초기화
        /// </summary>
        public void Initialize(Vector3 target, float damage1, float radius1,
                             float damage2, float radius2, float delay, GameObject effect)
        {
            targetPosition = target;
            primaryDamage = damage1;        //낙뢰 데미지
            primaryRadius = radius1;        //낙뢰 범위
            secondaryDamage = damage2;      //낙뢰후 폭발 데미지
            secondaryRadius = radius2;      //낙뢰후 폭발 범위
            secondaryDelay = delay;
            effectPrefab = effect;
            IsActive = true;

            // 번개 낙하 시작
            StartCoroutine(LightningStrike());
        }

        /// <summary>
        /// 번개 낙하 코루틴
        /// </summary>
        private IEnumerator LightningStrike()
        {
            // 번개 낙하 애니메이션 (0.2초)
            float fallTime = 0.2f;
            float elapsedTime = 0f;
            Vector3 startPos = transform.position;

            while (elapsedTime < fallTime)
            {
                transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / fallTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;

            // 1차 데미지 (projectileSize 범위)
            ApplyPrimaryDamage();

            // 지연 후 2차 데미지
            yield return new WaitForSeconds(secondaryDelay);

            // 2차 폭발 데미지 (explosionRadius 범위)
            ApplySecondaryDamage();

            // 번개 제거
            yield return new WaitForSeconds(0.5f);
            IsActive = false;
            Destroy(gameObject);
        }

        /// <summary>
        /// 1차 데미지 적용 (projectileSize 범위)
        /// </summary>
        private void ApplyPrimaryDamage()
        {
            if (hasStruck) return;
            hasStruck = true;

            Debug.Log($"[LightningBolt] 1차 데미지 적용 - 위치: {transform.position}, 범위: {primaryRadius}, 데미지: {primaryDamage}");

            // projectileSize 범위 내 적에게 데미지
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, primaryRadius, enemyLayer);

            foreach (var enemyCollider in enemies)
            {
                var enemy = enemyCollider.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(primaryDamage);
                    Debug.Log($"[LightningBolt] 1차 데미지 {primaryDamage} → {enemyCollider.name}");
                }
            }

            // 1차 이펙트
            CreateEffect(primaryRadius, Color.yellow);
        }

        /// <summary>
        /// 2차 폭발 데미지 적용 (explosionRadius 범위)
        /// </summary>
        private void ApplySecondaryDamage()
        {
            if (hasExploded) return;
            hasExploded = true;

            Debug.Log($"[LightningBolt] 2차 폭발 데미지 - 위치: {transform.position}, 범위: {secondaryRadius}, 데미지: {secondaryDamage}");

            // explosionRadius 범위 내 적에게 데미지
            int enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, secondaryRadius, enemyLayer);

            foreach (var enemyCollider in enemies)
            {
                var enemy = enemyCollider.GetComponent<WJEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(secondaryDamage);
                    Debug.Log($"[LightningBolt] 2차 폭발 데미지 {secondaryDamage} → {enemyCollider.name}");
                }
            }

            // 2차 이펙트
            CreateEffect(secondaryRadius, new Color(0.5f, 0.8f, 1f));
        }

        /// <summary>
        /// 이펙트 생성
        /// </summary>
        private void CreateEffect(float radius, Color color)
        {
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
                effect.transform.localScale = Vector3.one * radius;
                Destroy(effect);
            }
        }

        void OnDrawGizmosSelected()
        {
            // 1차 범위 (노란색)
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, primaryRadius);

            // 2차 범위 (파란색)
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, secondaryRadius);
        }
    }
}