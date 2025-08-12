using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Battle;
using DiceSurvivor.Manager;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Scythe 무기 전용 클래스
    /// 휩쓸기(Sweep) 공격 패턴을 사용하는 근접 무기
    /// </summary>
    public class ScytheWeapon : MonoBehaviour
    {
        [Header("무기 기본 설정")]
        [SerializeField] private Transform player;              // 플레이어 Transform
        [SerializeField] private LayerMask enemyLayer;          // 적 레이어

        [Header("공격 이펙트")]
        [SerializeField] private GameObject sweepEffectPrefab;  // 휩쓸기 이펙트 프리팹
        [SerializeField] private Transform weaponPivot;         // 무기 회전 중심점
        [SerializeField] private GameObject scytheModel;        // 낫 3D 모델

        [Header("공격 설정")]
        [SerializeField] private float sweepAngle = 120f;       // 휩쓸기 각도
        [SerializeField] private float attackSpeed = 0.5f;      // 공격 애니메이션 속도

        // 현재 무기 스탯 (외부에서 설정)
        private WeaponStats currentStats;
        private float lastAttackTime;
        private bool isAttacking = false;
        private HashSet<GameObject> hitEnemiesThisAttack;       // 이번 공격에 맞은 적들

        private void Start()
        {
            // 플레이어가 설정되지 않았다면 찾기
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }

            // 피격 적 리스트 초기화
            hitEnemiesThisAttack = new HashSet<GameObject>();
        }

        private void Update()
        {
            // 무기 데이터가 설정되지 않았으면 대기
            if (currentStats == null) return;

            // 쿨다운 체크 후 자동 공격
            if (Time.time - lastAttackTime >= currentStats.cooldown && !isAttacking)
            {
                StartCoroutine(PerformScytheAttack());
            }
        }

        /// <summary>
        /// 무기 스탯 설정 (외부에서 호출)
        /// </summary>
        /// <param name="stats">설정할 무기 스탯</param>
        public void SetWeaponStats(WeaponStats stats)
        {
            currentStats = stats;


            // 무기 모델 크기 조정 (레벨에 따라)
            /*if (scytheModel != null && stats != null)
            {
                float scaleMultiplier = 1f + (stats.level - 1) * 0.1f; // 레벨당 10% 크기 증가
                scytheModel.transform.localScale = Vector3.one * scaleMultiplier;
            }*/

            Debug.Log($"Scythe 무기 스탯 설정 - Lv.{stats.level}, 데미지: {stats.damage}, 범위: {stats.range}");
        }

        /// <summary>
        /// 공격 범위 내 적이 있는지 확인
        /// </summary>
        /*private bool HasEnemyInRange()
        {
            Collider[] enemies = Physics.OverlapSphere(player.position, currentStats.range, enemyLayer);
            return enemies.Length > 0;
        }*/

        /// <summary>
        /// Scythe 휩쓸기 공격 수행
        /// </summary>
        private IEnumerator PerformScytheAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            hitEnemiesThisAttack.Clear(); // 피격 리스트 초기화

            /*// 가장 가까운 적 방향으로 회전
            //GameObject nearestEnemy = FindNearestEnemy();
            *//*if (nearestEnemy != null)
            {
                
            }*//*
            Vector3 direction = (nearestEnemy.transform.position - player.position).normalized;
            direction.y = 0; // 수평 회전만
            player.rotation = Quaternion.LookRotation(direction);*/

            // 휩쓸기 애니메이션
            float startAngle = -sweepAngle / 2f;
            float endAngle = sweepAngle / 2f;
            float elapsedTime = 0f;

            // 공격 시작 이펙트
            PlayAttackStartEffect();

            while (elapsedTime < attackSpeed)
            {
                float t = elapsedTime / attackSpeed;
                float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

                // 무기 모델 회전 (있다면)
                if (weaponPivot != null)
                {
                    weaponPivot.localRotation = Quaternion.Euler(0, currentAngle, 0);
                }

                // 현재 각도에서 적 검출 및 데미지 처리
                DetectAndDamageEnemies(currentAngle);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 무기 위치 초기화
            if (weaponPivot != null)
            {
                weaponPivot.localRotation = Quaternion.identity;
            }

            // 공격 종료 이펙트
            PlayAttackEndEffect();

            isAttacking = false;
        }

        /// <summary>
        /// 특정 각도에서 적 검출 및 데미지 처리
        /// </summary>
        private void DetectAndDamageEnemies(float angle)
        {
            // 현재 각도의 방향 벡터
            Vector3 attackDirection = Quaternion.Euler(0, angle, 0) * player.forward;

            // 부채꼴 범위 내의 적 검출
            Collider[] enemies = Physics.OverlapSphere(player.position, currentStats.range, enemyLayer);

            foreach (Collider enemy in enemies)
            {

                // 이미 맞은 적은 스킵
                if (hitEnemiesThisAttack.Contains(enemy.gameObject))
                    continue;
                
                // 데미지 적용
                ApplyDamageToEnemy(enemy.gameObject);
                hitEnemiesThisAttack.Add(enemy.gameObject);
                

                /*// 적이 부채꼴 범위 내에 있는지 확인
                Vector3 directionToEnemy = (enemy.transform.position - player.position).normalized;
                float angleToEnemy = Vector3.Angle(player.forward, directionToEnemy);

                if (angleToEnemy <= sweepAngle / 2f)
                {
                    

                    // 피격 이펙트
                    PlayHitEffect(enemy.transform.position);
                }*/
            }
        }

        /// <summary>
        /// 가장 가까운 적 찾기
        /// </summary>
       /* private GameObject FindNearestEnemy()
        {
            Collider[] enemies = Physics.OverlapSphere(player.position, currentStats.range * 1.5f, enemyLayer);

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
        }*/

        /// <summary>
        /// 적에게 데미지 적용
        /// </summary>
        private void ApplyDamageToEnemy(GameObject enemy)
        {
            // IBattle 인터페이스를 통한 데미지 처리
            IBattle damageable = enemy.GetComponent<IBattle>();
            if (damageable != null)
            {
                // 기본 데미지 적용
                float finalDamage = currentStats.damage;

                /*// 레벨에 따른 추가 데미지 (옵션)
                float levelBonus = (currentStats.level - 1) * 2f;
                finalDamage += levelBonus;*/

                damageable.TakeDamage(finalDamage);

                // 디버그 메시지
                Debug.Log($"Scythe가 {enemy.name}에게 {finalDamage} 데미지를 입혔습니다!");
            }
        }

        /// <summary>
        /// 공격 시작 이펙트 재생
        /// </summary>
        private void PlayAttackStartEffect()
        {
            if (sweepEffectPrefab != null && weaponPivot != null)
            {
                // 휩쓸기 이펙트 생성
                GameObject effect = Instantiate(sweepEffectPrefab, weaponPivot.position, weaponPivot.rotation);
                effect.transform.SetParent(weaponPivot);

                /*// 이펙트 크기를 공격 범위에 맞게 조정
                effect.transform.localScale = Vector3.one * (currentStats.range / 3f);*/

                Destroy(effect, attackSpeed);
            }
        }

        /// <summary>
        /// 공격 종료 이펙트 재생
        /// </summary>
        private void PlayAttackEndEffect()
        {
            // 추가 이펙트가 필요한 경우 여기에 구현
        }

        /// <summary>
        /// 피격 이펙트 재생
        /// </summary>
        private void PlayHitEffect(Vector3 position)
        {
            // 피격 이펙트나 사운드 재생
            // 예: ParticleManager.Instance?.PlayHitEffect(position);
        }

        /// <summary>
        /// 현재 무기 정보 가져오기
        /// </summary>
        public WeaponStats GetCurrentStats() => currentStats;
        public bool IsAttacking() => isAttacking;

        /// <summary>
        /// 무기 활성화/비활성화
        /// </summary>
        public void SetWeaponActive(bool active)
        {
            enabled = active;
            if (scytheModel != null)
                scytheModel.SetActive(active);
        }

        /// <summary>
        /// 기즈모 그리기 (공격 범위 시각화)
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (player == null || currentStats == null) return;

            // 공격 범위 원
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(player.position, currentStats.range);

            // 휩쓸기 범위 표시
            Vector3 leftBoundary = Quaternion.Euler(0, -sweepAngle / 2f, 0) * player.forward * currentStats.range;
            Vector3 rightBoundary = Quaternion.Euler(0, sweepAngle / 2f, 0) * player.forward * currentStats.range;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(player.position, player.position + leftBoundary);
            Gizmos.DrawLine(player.position, player.position + rightBoundary);

            // 부채꼴 호 그리기
            int segments = 20;
            Vector3 previousPoint = player.position + leftBoundary;

            for (int i = 1; i <= segments; i++)
            {
                float angle = Mathf.Lerp(-sweepAngle / 2f, sweepAngle / 2f, (float)i / segments);
                Vector3 currentPoint = player.position + Quaternion.Euler(0, angle, 0) * player.forward * currentStats.range;
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }
    }
}