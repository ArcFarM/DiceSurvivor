using UnityEngine;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    /// 적이 죽었을 때 드롭되는 경험치 오브
    /// </summary>
    public class ExperienceOrb : MonoBehaviour
    {
        [Header("경험치 설정")]
        [SerializeField] private float experienceValue = 10f;      // 경험치 양
        [SerializeField] private float attractSpeed = 10f;         // 플레이어에게 끌려가는 속도
        [SerializeField] private float attractDistance = 3f;       // 끌려가기 시작하는 거리

        [Header("생명 주기")]
        [SerializeField] private float lifeTime = 30f;            // 자동 소멸 시간

        [Header("비주얼")]
        [SerializeField] private GameObject visualEffect;          // 시각 효과
        [SerializeField] private AnimationCurve bounceCurve;      // 통통 튀는 애니메이션

        private Transform target;                                  // 목표 (플레이어)
        private bool isBeingCollected = false;                     // 수집 중인지 여부
        private float spawnTime;                                   // 생성 시간
        private Vector3 initialPosition;                           // 초기 위치

        private void Start()
        {
            spawnTime = Time.time;
            initialPosition = transform.position;

            // 랜덤한 방향으로 약간 튀기
            GetComponent<Rigidbody>()?.AddForce(
                new Vector3(Random.Range(-2f, 2f), 5f, Random.Range(-2f, 2f)),
                ForceMode.Impulse
            );
        }

        private void Update()
        {
            // 생명 시간 체크
            if (Time.time - spawnTime > lifeTime && !isBeingCollected)
            {
                FadeAndDestroy();
                return;
            }

            // 수집 중이면 플레이어에게 이동
            if (isBeingCollected && target != null)
            {
                MoveToTarget();
            }
            else
            {
                // 통통 튀는 애니메이션
                if (bounceCurve != null && bounceCurve.length > 0)
                {
                    float bounceHeight = bounceCurve.Evaluate((Time.time - spawnTime) % 1f);
                    transform.position = new Vector3(
                        transform.position.x,
                        initialPosition.y + bounceHeight * 0.3f,
                        transform.position.z
                    );
                }
            }
        }

        /// <summary>
        /// 경험치 오브 수집 시작
        /// </summary>
        public void Collect(Transform collector)
        {
            if (isBeingCollected) return;

            target = collector;
            isBeingCollected = true;

            // 콜라이더 비활성화로 중복 수집 방지
            GetComponent<Collider>().enabled = false;
        }

        /// <summary>
        /// 플레이어에게 이동
        /// </summary>
        private void MoveToTarget()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // 목표를 향해 가속하며 이동
            float distance = Vector3.Distance(transform.position, target.position);
            float currentSpeed = attractSpeed * (1 + (attractDistance - distance));

            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                currentSpeed * Time.deltaTime
            );

            // 플레이어에 도착하면 경험치 부여
            if (distance < 0.5f)
            {
                GiveExperience();
            }
        }

        /// <summary>
        /// 경험치 부여
        /// </summary>
        private void GiveExperience()
        {
            //ExperienceManager.Instance?.GainExperience(experienceValue);

            // 수집 효과
            ShowCollectEffect();

            Destroy(gameObject);
        }

        /// <summary>
        /// 수집 효과 표시
        /// </summary>
        private void ShowCollectEffect()
        {
            // 파티클 효과 재생
            // ParticleManager.Instance?.PlayExperienceCollectEffect(transform.position);

            // 사운드 재생
            // AudioManager.Instance?.PlaySound("ExperienceCollect");
        }

        /// <summary>
        /// 시간 초과로 사라질 때
        /// </summary>
        private void FadeAndDestroy()
        {
            // 페이드 아웃 효과
            // 구현은 프로젝트에 따라
            Destroy(gameObject);
        }

        /// <summary>
        /// 경험치 값 설정
        /// </summary>
        public void SetExperienceValue(float value)
        {
            experienceValue = value;

            // 경험치 양에 따라 크기 조절
            float scale = 1f + (value / 100f);
            transform.localScale = Vector3.one * Mathf.Clamp(scale, 0.5f, 2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 플레이어가 가까이 왔을 때 자동 수집
            if (other.CompareTag("Player") && !isBeingCollected)
            {
                Collect(other.transform);
            }
        }
    }
}