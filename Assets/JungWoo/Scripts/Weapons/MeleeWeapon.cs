using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Battle;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    /// 근접 무기의 기본 클래스
    /// 각 근접 무기는 이 클래스를 상속받아 구현
    /// </summary>
    public class MeleeWeapon : MonoBehaviour
    {
        [Header("무기 설정")]
        [SerializeField] private string weaponName;           // 무기 이름 (JSON의 무기명과 일치해야 함)
        [SerializeField] private Transform player;            // 플레이어 Transform
        [SerializeField] private LayerMask enemyLayer;       // 적 레이어

        [Header("공격 이펙트")]
        [SerializeField] private GameObject attackEffectPrefab;  // 공격 이펙트 프리팹
        [SerializeField] private Transform attackPoint;          // 공격 시작 지점

        [Header("현재 무기 상태")]
        [SerializeField] private int currentLevel = 1;          // 현재 무기 레벨
        private WeaponStats currentStats;                       // 현재 레벨의 스탯
        private float lastAttackTime;                            // 마지막 공격 시간
        private bool isAttacking = false;                        // 공격 중 여부

        // 공격 타입 열거형
        public enum AttackType
        {
            Sweep,      // 휩쓸기 (Scythe, Greatsword)
            Thrust,     // 찌르기 (Spear, Staff)
            Slam,       // 내려치기 (Hammer)
            Whip        // 채찍질 (Whip)
        }

        [Header("공격 타입")]
        [SerializeField] private AttackType attackType = AttackType.Sweep;

        private void Start()
        {
            // 플레이어가 설정되지 않았다면 찾기
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }

            // 초기 무기 데이터 로드
            LoadWeaponData();
        }

        private void Update()
        {
            // 쿨다운 체크 후 자동 공격
            if (Time.time - lastAttackTime >= currentStats.cooldown && !isAttacking)
            {
                // 가장 가까운 적 찾기
                GameObject nearestEnemy = FindNearestEnemy();
                Debug.Log("적 발견");
                if (nearestEnemy != null)
                {
                    StartCoroutine(PerformAttack(nearestEnemy.transform.position));
                }
            }
        }

        /// <summary>
        /// JSON에서 현재 레벨의 무기 데이터 로드
        /// </summary>
        private void LoadWeaponData()
        {
            currentStats = DataTableManager.Instance.GetMeleeWeapon(weaponName, currentLevel);
            if (currentStats == null)
            {
                Debug.LogError($"무기 데이터를 찾을 수 없습니다: {weaponName} Lv.{currentLevel}");
                return;
            }

            Debug.Log($"{weaponName} Lv.{currentLevel} 로드 완료 - 데미지: {currentStats.damage}, 쿨다운: {currentStats.cooldown}");
        }

        /// <summary>
        /// 가장 가까운 적 찾기
        /// </summary>
        private GameObject FindNearestEnemy()
        {
            Collider[] enemies = Physics.OverlapSphere(player.position, currentStats.range, enemyLayer);

            GameObject nearest = null;
            float minDistance = float.MaxValue;

            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(player.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy.gameObject;
                }
            }

            return nearest;
        }

        /// <summary>
        /// 공격 수행 코루틴
        /// </summary>
        private IEnumerator PerformAttack(Vector3 targetPosition)
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            // 타겟 방향으로 회전
            Vector3 direction = (targetPosition - player.position).normalized;
            player.rotation = Quaternion.LookRotation(direction);

            // 공격 타입에 따른 처리
            switch (attackType)
            {
                case AttackType.Sweep:
                    yield return StartCoroutine(SweepAttack());
                    break;
                case AttackType.Thrust:
                    yield return StartCoroutine(ThrustAttack());
                    break;
                case AttackType.Slam:
                    yield return StartCoroutine(SlamAttack());
                    break;
                case AttackType.Whip:
                    yield return StartCoroutine(WhipAttack());
                    break;
            }

            isAttacking = false;
        }

        /// <summary>
        /// 휩쓸기 공격 (Scythe, Greatsword)
        /// </summary>
        private IEnumerator SweepAttack()
        {
            // 공격 애니메이션 시간
            float attackDuration = 0.3f;
            float elapsedTime = 0f;

            // 시작 각도와 종료 각도
            float startAngle = -45f;
            float endAngle = 45f;

            while (elapsedTime < attackDuration)
            {
                float t = elapsedTime / attackDuration;
                float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

                // 현재 각도에서 적 검출
                Vector3 attackDirection = Quaternion.Euler(0, currentAngle, 0) * player.forward;
                RaycastHit[] hits = Physics.SphereCastAll(
                    player.position,
                    currentStats.range,
                    attackDirection,
                    0.1f,
                    enemyLayer
                );

                // 데미지 처리
                foreach (RaycastHit hit in hits)
                {
                    ApplyDamage(hit.collider.gameObject);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 공격 이펙트 생성
            if (attackEffectPrefab != null)
            {
                GameObject effect = Instantiate(attackEffectPrefab, attackPoint.position, player.rotation);
                Destroy(effect, 1f);
            }
        }

        /// <summary>
        /// 찌르기 공격 (Spear, Staff)
        /// </summary>
        private IEnumerator ThrustAttack()
        {
            // 전방으로 빠르게 찌르기
            Vector3 attackEndPoint = player.position + player.forward * currentStats.range;

            // 관통 여부에 따라 처리
            if (currentStats.isPiercing)
            {
                // 관통 공격 - 라인 상의 모든 적에게 데미지
                RaycastHit[] hits = Physics.RaycastAll(player.position, player.forward, currentStats.range, enemyLayer);
                foreach (RaycastHit hit in hits)
                {
                    ApplyDamage(hit.collider.gameObject);
                }
            }
            else
            {
                // 단일 타겟 공격
                RaycastHit hit;
                if (Physics.Raycast(player.position, player.forward, out hit, currentStats.range, enemyLayer))
                {
                    ApplyDamage(hit.collider.gameObject);
                }
            }

            // 공격 이펙트
            if (attackEffectPrefab != null)
            {
                GameObject effect = Instantiate(attackEffectPrefab, attackEndPoint, player.rotation);
                effect.transform.localScale = new Vector3(1, 1, currentStats.range);
                Destroy(effect, 0.5f);
            }

            yield return new WaitForSeconds(0.2f);
        }

        /// <summary>
        /// 내려치기 공격 (Hammer)
        /// </summary>
        private IEnumerator SlamAttack()
        {
            // 공격 준비 시간
            yield return new WaitForSeconds(0.3f);

            // 타격 지점
            Vector3 slamPoint = player.position + player.forward * (currentStats.range / 2);

            // 폭발 범위가 있는 경우
            if (currentStats.explosionRadius > 0)
            {
                // 범위 내 모든 적에게 데미지
                Collider[] enemies = Physics.OverlapSphere(slamPoint, currentStats.explosionRadius, enemyLayer);
                foreach (Collider enemy in enemies)
                {
                    // 폭발 데미지 적용
                    float damageToApply = currentStats.damage + currentStats.explosionDamage;
                    ApplyDamage(enemy.gameObject, damageToApply);

                    // 넉백 효과
                    Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                    if (enemyRb != null)
                    {
                        Vector3 knockbackDirection = (enemy.transform.position - slamPoint).normalized;
                        enemyRb.AddForce(knockbackDirection * 10f, ForceMode.Impulse);
                    }
                }

                // 폭발 이펙트
                if (attackEffectPrefab != null)
                {
                    GameObject effect = Instantiate(attackEffectPrefab, slamPoint, Quaternion.identity);
                    effect.transform.localScale = Vector3.one * currentStats.explosionRadius * 2;
                    Destroy(effect, 1f);
                }
            }
            else
            {
                // 일반 내려치기
                Collider[] enemies = Physics.OverlapSphere(slamPoint, currentStats.range / 2, enemyLayer);
                foreach (Collider enemy in enemies)
                {
                    ApplyDamage(enemy.gameObject);
                }
            }
        }

        /// <summary>
        /// 채찍 공격 (Whip)
        /// </summary>
        private IEnumerator WhipAttack()
        {
            // 긴 범위의 원호 공격
            float attackDuration = 0.4f;
            float elapsedTime = 0f;

            List<GameObject> hitEnemies = new List<GameObject>();

            while (elapsedTime < attackDuration)
            {
                float t = elapsedTime / attackDuration;

                // 채찍의 움직임을 시뮬레이션
                float currentRange = Mathf.Lerp(0, currentStats.range, t);
                float swingAngle = Mathf.Sin(t * Mathf.PI) * 60f; // 좌우로 휘두르기

                Vector3 attackDirection = Quaternion.Euler(0, swingAngle, 0) * player.forward;
                Vector3 attackPoint = player.position + attackDirection * currentRange;

                // 범위 내 적 검출
                Collider[] enemies = Physics.OverlapSphere(attackPoint, 1f, enemyLayer);
                foreach (Collider enemy in enemies)
                {
                    if (!hitEnemies.Contains(enemy.gameObject))
                    {
                        ApplyDamage(enemy.gameObject);
                        hitEnemies.Add(enemy.gameObject);
                    }
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 데미지 적용
        /// </summary>
        private void ApplyDamage(GameObject enemy, float damage = -1)
        {
            if (damage < 0) damage = currentStats.damage;

            // IDamageable 인터페이스를 통한 데미지 처리
            IBattle damageable = enemy.GetComponent<IBattle>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);

                // 데미지 텍스트 표시 (옵션)
                ShowDamageText(enemy.transform.position, damage);
            }
        }

        /// <summary>
        /// 데미지 텍스트 표시
        /// </summary>
        private void ShowDamageText(Vector3 position, float damage)
        {
            // DamageTextManager가 있다면 사용
            // DamageTextManager.Instance?.ShowDamage(position, damage);
        }

        /// <summary>
        /// 무기 레벨업
        /// </summary>
        public void LevelUp()
        {
            if (currentLevel >= 8)
            {
                Debug.Log($"{weaponName}은(는) 이미 최대 레벨입니다.");
                return;
            }

            currentLevel++;
            LoadWeaponData();

            // 레벨업 이펙트
            Debug.Log($"{weaponName} 레벨업! Lv.{currentLevel}");
            Debug.Log($"새로운 능력: {currentStats.description}");
        }

        /// <summary>
        /// 현재 무기 정보 가져오기
        /// </summary>
        public WeaponStats GetCurrentStats() => currentStats;
        public int GetCurrentLevel() => currentLevel;
        public string GetWeaponName() => weaponName;

        /// <summary>
        /// 기즈모 그리기 (공격 범위 시각화)
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (player == null) return;

            // 공격 범위 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, currentStats?.range ?? 5f);

            // 폭발 범위 표시 (Hammer)
            if (currentStats != null && currentStats.explosionRadius > 0)
            {
                Gizmos.color = Color.yellow;
                Vector3 explosionCenter = player.position + player.forward * (currentStats.range / 2);
                Gizmos.DrawWireSphere(explosionCenter, currentStats.explosionRadius);
            }
        }
    }
}