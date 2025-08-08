using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Weapon;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// KillingAura 무기 - 자기 주변 원형 범위에 지속적으로 DoT 데미지를 주는 무기
    /// </summary>
    public class KillingAuraWeapon : SplashWeapon
    {
        [Header("KillingAura Specific")]
        [SerializeField] private float dotInterval = 1f; // DoT 데미지 간격 (1초)
        [SerializeField] private bool showAuraVisual = true; // 시각화 옵션

        // DoT 관리용 딕셔너리 (적 -> 마지막 데미지 시간)
        private Dictionary<GameObject, float> lastDotTime = new Dictionary<GameObject, float>();

        // 범위 내 적 배열 관리
        private List<GameObject> enemiesInAura = new List<GameObject>();

        // DoT 타이머 (상시 작동)
        private float dotTimer = 0f;

        // Aura 시각화용
        private GameObject auraVisual;

        protected override void Start()
        {
            weaponName = "KillingAura";
            base.Start();

            // Aura 시각화 생성
            if (showAuraVisual)
            {
                //CreateAuraVisual();
            }
        }

        protected override void Update()
        {
            // 플레이어 체크
            if (player == null) return;

            // DoT 타이머 업데이트 (상시 작동)
            dotTimer += Time.deltaTime;

            // 1초마다 DoT 데미지 적용
            if (dotTimer >= dotInterval)
            {
                ApplyDotToAllEnemiesInRange();
                dotTimer = 0f; // 타이머 리셋
            }

           
            // 범위를 벗어난 적 정리
            CleanupNullEnemies();
        }

        /// <summary>
        /// 무기 초기화
        /// </summary>
        protected override void InitializeWeapon()
        {
            LoadWeaponData();
            SetupAuraCollider();
        }

        /// <summary>
        /// Aura Collider 설정 (Layer 감지용)
        /// </summary>
        private void SetupAuraCollider()
        {
            // 기존 콜라이더 제거
            Collider[] colliders = GetComponents<Collider>();
            foreach (var col in colliders)
            {
                Destroy(col);
            }

            // 새 Sphere Collider 추가 (트리거)
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;

            Debug.Log($"KillingAura Collider 설정 - 반경: {radius}");
        }

        /// <summary>
        /// 무기 데이터 로드
        /// </summary>
        protected override void LoadWeaponData()
        {
            var dataManager = DataTableManager.Instance;
            if (dataManager == null)
            {
                Debug.LogError("DataTableManager를 찾을 수 없습니다!");
                return;
            }

            // DataTable에서 KillingAura 데이터 로드
            var weaponStats = dataManager.GetSplashWeapon("KillingAura", currentLevel);
            if (weaponStats != null)
            {
                UpdateWeaponStats(weaponStats);

                // Collider 크기 업데이트
                SphereCollider sphereCollider = GetComponent<SphereCollider>();
                if (sphereCollider != null)
                {
                    sphereCollider.radius = radius;
                }

                // 시각화 업데이트
                UpdateAuraVisual();
            }
            else
            {
                Debug.LogError($"KillingAura Lv.{currentLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 범위 내 모든 적에게 DoT 데미지 및 감속 적용 (1초마다 실행)
        /// </summary>
        private void ApplyDotToAllEnemiesInRange()
        {
            // 배열 복사본으로 작업 (순회 중 수정 방지)
            List<GameObject> enemiesCopy = new List<GameObject>(enemiesInAura);

            // 적이 없으면 리턴
            if (enemiesCopy.Count == 0) return;

            Debug.Log($"[KillingAura] DoT 타이머 발동! 범위 내 적: {enemiesCopy.Count}명");

            int damageAppliedCount = 0;

            foreach (var enemy in enemiesCopy)
            {
                // null 체크
                if (enemy == null)
                {
                    continue;
                }

                // DoT 데미지 적용
                if (dotDamage > 0)
                {
                    // Enemy 컴포넌트 직접 찾아서 데미지 적용
                    var enemyComponent = enemy.GetComponent<Test.WJEnemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.TakeDamage(dotDamage);
                        damageAppliedCount++;
                    }
                }

                // 감속 효과 적용 (duration 값 사용)
                if (duration > 0)
                {
                    ApplySlow(enemy, duration);
                }
            }

            Debug.Log($"[KillingAura] DoT 데미지 적용 완료: {damageAppliedCount}명에게 {dotDamage} 데미지");
        }

        /// <summary>
        /// null이 된 적 제거
        /// </summary>
        private void CleanupNullEnemies()
        {
            // null 적 제거
            enemiesInAura.RemoveAll(enemy => enemy == null);

            // DoT 시간 딕셔너리에서도 제거
            List<GameObject> toRemove = new List<GameObject>();
            foreach (var kvp in lastDotTime)
            {
                if (kvp.Key == null)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var enemy in toRemove)
            {
                lastDotTime.Remove(enemy);
            }
        }

        /// <summary>
        /// 트리거 진입 - Enemy Layer 감지
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // Enemy 레이어 체크
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Debug.Log($"{other.gameObject.name} - 현재 적 수: {enemiesInAura.Count}");
                // 배열에 추가 (중복 방지)
                if (!enemiesInAura.Contains(other.gameObject))
                {
                    enemiesInAura.Add(other.gameObject);
                    Debug.Log($"[KillingAura] Enemy 진입: {other.gameObject.name} - 현재 적 수: {enemiesInAura.Count}");

                    // 즉시 초기 데미지 적용
                    /*if (dotDamage > 0)
                    {
                        var enemyComp = other.gameObject.GetComponent<Test.WJEnemy>();
                        if (enemyComp != null)
                        {
                            //enemyComp.TakeDamage(dotDamage);
                            Debug.Log($"[KillingAura] 진입 즉시 데미지 {dotDamage} 적용: {other.gameObject.name}");
                        }
                    }*/

                    // 즉시 감속 적용
                    if (duration > 0)
                    {
                        ApplySlow(other.gameObject, duration);
                    }
                }
            }
        }

        /// <summary>
        /// 트리거 체류 - 지속적인 감속 효과 갱신
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            // Enemy 레이어만 처리
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // 감속 효과 갱신 (매 프레임)
                if (duration > 0 && other.gameObject != null)
                {
                    // 감속 효과 지속적으로 갱신
                    ApplySlow(other.gameObject, duration);
                }
            }
        }

        /// <summary>
        /// 트리거 퇴장 - Enemy가 범위를 벗어남
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            // Enemy 레이어 체크
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // 배열에서 제거
                if (enemiesInAura.Remove(other.gameObject))
                {
                    Debug.Log($"Enemy 퇴장 (KillingAura): {other.gameObject.name} - 남은 적 수: {enemiesInAura.Count}");
                }

                // DoT 시간 기록 제거
                if (lastDotTime.ContainsKey(other.gameObject))
                {
                    lastDotTime.Remove(other.gameObject);
                }
            }
        }

        /// <summary>
        /// Aura 시각화 생성
        /// </summary>
        private void CreateAuraVisual()
        {
            if (auraVisual == null)
            {
                // 원형 평면 시각화
                auraVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                auraVisual.name = "KillingAuraVisual";
                auraVisual.transform.SetParent(transform);
                auraVisual.transform.localPosition = Vector3.zero;
                auraVisual.transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);

                // 콜라이더 제거 (시각화만)
                Destroy(auraVisual.GetComponent<Collider>());

                // 반투명 빨간색 머티리얼 설정
                var renderer = auraVisual.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.color = new Color(1f, 0f, 0f, 0.2f); // 반투명 빨간색
                    mat.SetFloat("_Mode", 3); // Transparent
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    renderer.material = mat;
                }
            }
        }

        /// <summary>
        /// Aura 시각화 업데이트
        /// </summary>
        private void UpdateAuraVisual()
        {
            if (auraVisual != null)
            {
                auraVisual.transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);
            }
        }

        /// <summary>
        /// 공격 수행 (사용하지 않음 - KillingAura는 지속 효과)
        /// </summary>
        protected override void PerformAttack()
        {
            // KillingAura는 지속 효과이므로 이 메서드는 사용하지 않음
        }

        /// <summary>
        /// 지속 공격 (사용하지 않음 - Update에서 직접 처리)
        /// </summary>
        protected override void ContinuousAttack()
        {
            // Update에서 직접 처리하므로 사용하지 않음
        }

        /// <summary>
        /// 레벨 업
        /// </summary>
        public override void LevelUp()
        {
            base.LevelUp();
            Debug.Log($"KillingAura 레벨업! 현재 레벨: {currentLevel}");
        }

        /// <summary>
        /// 현재 범위 내 적 수 반환
        /// </summary>
        public int GetEnemyCount()
        {
            CleanupNullEnemies();
            return enemiesInAura.Count;
        }

        /// <summary>
        /// 범위 내 적 배열 반환
        /// </summary>
        public List<GameObject> GetEnemiesInRange()
        {
            CleanupNullEnemies();
            return new List<GameObject>(enemiesInAura);
        }

        /// <summary>
        /// Gizmo 그리기 (디버그용)
        /// </summary>
        protected override void OnDrawGizmosSelected()
        {
            // 공격 범위 표시
            Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, radius);

            // 범위 내 적과의 연결선 표시
            Gizmos.color = Color.red;
            foreach (var enemy in enemiesInAura)
            {
                if (enemy != null)
                {
                    Gizmos.DrawLine(transform.position, enemy.transform.position);
                }
            }
        }

        /// <summary>
        /// 오브젝트 파괴 시 정리
        /// </summary>
        private void OnDestroy()
        {
            // 배열 정리
            enemiesInAura.Clear();
            lastDotTime.Clear();

            // 시각화 오브젝트 제거
            if (auraVisual != null)
            {
                Destroy(auraVisual);
            }
        }
    }
}