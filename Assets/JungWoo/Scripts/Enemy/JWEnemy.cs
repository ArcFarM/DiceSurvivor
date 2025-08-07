using UnityEngine;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    /// 적 캐릭터 스크립트
    /// IDamageable 인터페이스를 구현하여 데미지를 받을 수 있음
    /// </summary>
    public class JWEnemy : MonoBehaviour, IDamageable
    {
        [Header("적 스탯")]
        [SerializeField] private float maxHealth = 100f;           // 최대 체력
        [SerializeField] private float currentHealth;              // 현재 체력
        [SerializeField] private float moveSpeed = 3f;             // 이동 속도
        [SerializeField] private float damage = 10f;               // 공격력

        [Header("경험치 드롭")]
        [SerializeField] private GameObject experienceOrbPrefab;    // 경험치 오브 프리팹
        [SerializeField] private float experienceValue = 20f;       // 드롭할 경험치 양
        [SerializeField] private int experienceOrbCount = 1;        // 드롭할 경험치 오브 개수
        [SerializeField] private float orbSpreadRadius = 1f;        // 경험치 오브 퍼짐 반경

        [Header("비주얼")]
        [SerializeField] private GameObject deathEffectPrefab;      // 사망 이펙트
        [SerializeField] private Renderer enemyRenderer;            // 적 렌더러 (데미지 표시용)

        private Transform player;                                   // 플레이어 Transform
        private Rigidbody rb;                                      // Rigidbody 컴포넌트
        private bool isDead = false;                               // 사망 여부

        // 데미지 표시용
        private Color originalColor;
        private float damageFlashDuration = 0.1f;

        private void Start()
        {
            // 초기화
            currentHealth = maxHealth;
            rb = GetComponent<Rigidbody>();

            // 플레이어 찾기
            player = GameObject.FindGameObjectWithTag("Player").transform;

            // 원본 색상 저장
            if (enemyRenderer != null)
            {
                originalColor = enemyRenderer.material.color;
            }
        }

        private void Update()
        {
            if (isDead || player == null) return;

            // 플레이어를 향해 이동
            MoveTowardsPlayer();
        }

        /// <summary>
        /// 플레이어를 향해 이동
        /// </summary>
        private void MoveTowardsPlayer()
        {
            Vector3 direction = (player.position - transform.position).normalized;

            // 이동
            if (rb != null)
            {
                rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }

            // 플레이어를 바라보기
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        /// <summary>
        /// 데미지 받기 (IDamageable 인터페이스 구현)
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;

            // 데미지 효과
            ShowDamageEffect();

            // 체력 체크
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// 데미지 효과 표시
        /// </summary>
        private void ShowDamageEffect()
        {
            // 색상 플래시
            if (enemyRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }

            // 넉백 효과
            if (rb != null)
            {
                Vector3 knockbackDirection = (transform.position - player.position).normalized;
                rb.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// 데미지 색상 플래시 코루틴
        /// </summary>
        private System.Collections.IEnumerator DamageFlash()
        {
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(damageFlashDuration);
            enemyRenderer.material.color = originalColor;
        }

        /// <summary>
        /// 적 사망 처리
        /// </summary>
        private void Die()
        {
            isDead = true;

            // 경험치 드롭
            DropExperience();

            // 사망 효과
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            // 사망 사운드
            // AudioManager.Instance?.PlaySound("EnemyDeath");

            // 오브젝트 제거
            Destroy(gameObject);
        }

        /// <summary>
        /// 경험치 오브 드롭
        /// </summary>
        private void DropExperience()
        {
            if (experienceOrbPrefab == null) return;

            // 설정된 개수만큼 경험치 오브 생성
            for (int i = 0; i < experienceOrbCount; i++)
            {
                // 랜덤한 위치에 생성
                Vector3 randomOffset = Random.insideUnitSphere * orbSpreadRadius;
                randomOffset.y = 0; // 높이는 고정

                Vector3 spawnPosition = transform.position + randomOffset;

                GameObject orb = Instantiate(experienceOrbPrefab, spawnPosition, Quaternion.identity);

                // 경험치 값 설정
                ExperienceOrb expOrb = orb.GetComponent<ExperienceOrb>();
                if (expOrb != null)
                {
                    // 여러 개로 나누어 드롭하는 경우 경험치 분배
                    float individualValue = experienceValue / experienceOrbCount;
                    expOrb.SetExperienceValue(individualValue);
                }
            }
        }

        /// <summary>
        /// 플레이어와 충돌 시 데미지 주기
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // 플레이어에게 데미지 주기
                IDamageable playerDamageable = collision.gameObject.GetComponent<IDamageable>();
                playerDamageable?.TakeDamage(damage);
            }
        }

        /// <summary>
        /// 적 초기화 (오브젝트 풀링용)
        /// </summary>
        public void Initialize(float health, float speed, float experienceReward)
        {
            maxHealth = health;
            currentHealth = health;
            moveSpeed = speed;
            experienceValue = experienceReward;
            isDead = false;

            // 렌더러 색상 초기화
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = originalColor;
            }
        }

        /// <summary>
        /// 현재 체력 비율 가져오기
        /// </summary>
        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// 데미지를 받을 수 있는 인터페이스
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}