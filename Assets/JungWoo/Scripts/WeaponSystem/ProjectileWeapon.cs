using UnityEngine;

namespace DiceSurvivor.WeaponDataSystem
{
    public class ProjectileWeapon : MonoBehaviour
    {
        #region Variables
        [Header("무기 설정")]
        public string weaponName = "Asteroid";      
        public int weaponLevel = 1;

        [Header("컴포넌트")]
        public Transform player;
        public Transform firePoint;
        public GameObject bulletPrefab;

        [Header("타겟팅")]
        public LayerMask enemyLayer = -1;

        //참조
        private WeaponData weaponData;          
        private Bullet bullet;
        private float lastFireTime;

        #endregion

        #region Unity Event Method
        void Start()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (firePoint == null)
                firePoint = transform;

            LoadWeaponData();
        }

        void Update()
        {
            if (weaponData == null || player == null) return;

            // 플레이어 따라다니기
            transform.position = player.position;

            // 자동 공격
            HandleAutoFire();
        }
        #endregion
        #region Custom Method
        void LoadWeaponData()
        {

            weaponData = WeaponDataManager.Instance.GetWeaponData(weaponName, weaponLevel);
            if (weaponData == null)
            {
                Debug.LogError($"무기 '{weaponName}' 레벨 {weaponLevel}의 데이터를 찾을 수 없습니다.");
                return;
            }
        }

        void HandleAutoFire()
        {
            float attackSpeed = weaponData.GetAttackSpeed();
            float fireInterval = 1f / attackSpeed;

            if (Time.time - lastFireTime >= fireInterval)
            {
                Fire();
                lastFireTime = Time.time;
            }
        }

        void Fire()
        {
            Transform target = FindNearestEnemy();
            if (target == null)
            {
                Debug.Log("No Enemy");
                return;
            }

            int projectileCount = weaponData.projectileCount;

            if (projectileCount == 1)
            {
                // 단일 투사체
                FireProjectile(target.position);
            }
            else
            {
                // 다중 투사체 (부채꼴 모양)
                FireMultipleProjectiles(target.position, projectileCount);
            }
        }

        // 단일 투사체
        void FireProjectile(Vector3 targetPosition)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            if (bulletScript == null)
                Debug.Log("No Bullet");
                bulletScript = bullet.AddComponent<Bullet>();

            Vector3 direction = (targetPosition - firePoint.position).normalized;

            //투사체 속성 참조
            bulletScript.Initialize( // 총알 초기화
                direction, // 발사 방향
                weaponData.damage, // 데미지
                weaponData.radius, // 사거리
                weaponData.penetration, // 관통력
                weaponData.knockback, // 넉백 힘
                0f, // 크리티컬 확률
                1f, // 크리티컬 배수
                weaponData.hasAreaDamage, // 범위 데미지 여부
                weaponData.areaDamageRadius // 범위 데미지 반경
            );
        }

        // 다중 투사체 (부채꼴 모양)
        void FireMultipleProjectiles(Vector3 targetPosition, int count)
        {
            Vector3 baseDirection = (targetPosition - firePoint.position).normalized; // 기본 발사 방향
            float spreadAngle = 30f; // 전체 확산 각도 (도)
            float angleStep = spreadAngle / (count - 1); // 각 투사체 사이의 각도 간격
            float startAngle = -spreadAngle / 2f; // 시작 각도

            for (int i = 0; i < count; i++) // 설정된 개수만큼 반복
            {
                float angle = startAngle + (angleStep * i); // 현재 투사체의 각도 계산
                Vector3 direction = Quaternion.Euler(0, angle, 0) * baseDirection; // 각도만큼 회전한 방향 벡터

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); // 총알 생성
                Bullet bulletScript = bullet.GetComponent<Bullet>(); // Bullet 컴포넌트 가져오기

                if (bulletScript == null) // Bullet 컴포넌트가 없으면
                    bulletScript = bullet.AddComponent<Bullet>(); // Bullet 컴포넌트 추가

                bulletScript.Initialize( // 총알 초기화
                    direction, // 발사 방향
                    weaponData.damage, // 데미지
                    weaponData.radius, // 사거리
                    weaponData.penetration, // 관통력
                    weaponData.knockback, // 넉백 힘
                    0f, // 크리티컬 확률
                    1f, // 크리티컬 배수
                    weaponData.hasAreaDamage, // 범위 데미지 여부
                    weaponData.areaDamageRadius // 범위 데미지 반경
                    );
            }
        }

        Transform FindNearestEnemy() // 가장 가까운 적을 찾는 함수
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, weaponData.radius * 10f, enemyLayer); // 탐지 범위 내의 적들 찾기
            Transform nearest = null; // 가장 가까운 적
            float nearestDistance = float.MaxValue; // 가장 가까운 거리

            foreach (Collider enemy in enemies) // 모든 적에 대해 반복
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position); // 거리 계산
                if (distance < nearestDistance) // 더 가까운 적이면
                {
                    nearestDistance = distance; // 최단 거리 업데이트
                    nearest = enemy.transform; // 가장 가까운 적 업데이트
                }
            }

            return nearest; // 가장 가까운 적 반환
        }

        public void LevelUp() // 레벨업 함수
        {
            if (WeaponDataManager.Instance.CanLevelUp(weaponName, weaponLevel)) // 레벨업 가능하면
            {
                weaponLevel++; // 레벨 증가
                LoadWeaponData(); // 새로운 레벨 데이터 로드
                Debug.Log($"{weaponData.weaponName} 레벨 {weaponLevel}로 업그레이드!"); // 업그레이드 로그 출력
            }
            else // 레벨업 불가능하면
            {
                Debug.Log($"{weaponName}은 이미 최대 레벨입니다."); // 최대 레벨 로그 출력
            }
        }

        #endregion
    }
}

