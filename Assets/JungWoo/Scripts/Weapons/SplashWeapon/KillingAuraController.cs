using UnityEngine;
using DiceSurvivor.Manager;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// KillingAura 무기 컨트롤러 - 데이터 관리 및 레벨 시스템
    /// </summary>
    public class KillingAuraController : MonoBehaviour
    {
        [Header("Weapon Reference")]
        [SerializeField] private KillingAuraWeapon killingAuraWeapon;

        [Header("Level System")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 8;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        // 현재 무기 스탯
        private WeaponStats currentStats;

        void Awake()
        {
            // KillingAuraWeapon 컴포넌트 찾기
            if (killingAuraWeapon == null)
            {
                killingAuraWeapon = GetComponent<KillingAuraWeapon>();
                if (killingAuraWeapon == null)
                {
                    killingAuraWeapon = gameObject.AddComponent<KillingAuraWeapon>();
                }
            }
        }

        void Start()
        {
            InitializeWeapon();
        }

        /// <summary>
        /// 무기 초기화
        /// </summary>
        private void InitializeWeapon()
        {
            LoadWeaponData();

            if (debugMode)
            {
                PrintWeaponInfo();
            }
        }

        /// <summary>
        /// 무기 데이터 로드
        /// </summary>
        private void LoadWeaponData()
        {
            var dataManager = DataTableManager.Instance;
            if (dataManager == null)
            {
                Debug.LogError("DataTableManager not found!");
                return;
            }

            currentStats = dataManager.GetSplashWeapon("KillingAura", currentLevel);

            if (currentStats != null)
            {
                killingAuraWeapon.UpdateWeaponStats(currentStats);
                Debug.Log($"KillingAura Lv.{currentLevel} loaded successfully!");
            }
            else
            {
                Debug.LogError($"Failed to load KillingAura Lv.{currentLevel} data!");
            }
        }

        /// <summary>
        /// 레벨 업
        /// </summary>
        public void LevelUp()
        {
            if (currentLevel >= maxLevel)
            {
                Debug.Log($"KillingAura is already at max level ({maxLevel})!");
                return;
            }

            currentLevel++;
            LoadWeaponData();
            UpdateChildScale("KillingAuraParticle", new Vector3(0.04f,0, 0.04f));
            

            if (debugMode)
            {
                Debug.Log($"KillingAura leveled up to Lv.{currentLevel}!");
                PrintWeaponInfo();
            }
        }


        /// <summary>
        /// 특정 레벨로 설정
        /// </summary>
        public void SetLevel(int level)
        {
            if (level < 1 || level > maxLevel)
            {
                Debug.LogError($"Invalid level: {level}. Must be between 1 and {maxLevel}");
                return;
            }

            currentLevel = level;
            LoadWeaponData();

            if (debugMode)
            {
                Debug.Log($"KillingAura set to Lv.{currentLevel}");
                PrintWeaponInfo();
            }
        }

        /// <summary>
        /// 현재 레벨 반환
        /// </summary>
        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        /// <summary>
        /// 최대 레벨 도달 여부
        /// </summary>
        public bool IsMaxLevel()
        {
            return currentLevel >= maxLevel;
        }

        /// <summary>
        /// 현재 무기 스탯 반환
        /// </summary>
        public WeaponStats GetCurrentStats()
        {
            return currentStats;
        }

        /// <summary>
        /// 다음 레벨 스탯 미리보기
        /// </summary>
        public WeaponStats GetNextLevelStats()
        {
            if (IsMaxLevel())
            {
                Debug.Log("Already at max level!");
                return null;
            }

            var dataManager = DataTableManager.Instance;
            if (dataManager == null) return null;

            return dataManager.GetSplashWeapon("KillingAura", currentLevel + 1);
        }

        /// <summary>
        /// 무기 정보 출력
        /// </summary>
        private void PrintWeaponInfo()
        {
            if (currentStats == null) return;

            Debug.Log("=== KillingAura Info ===");
            Debug.Log($"Level: {currentLevel}/{maxLevel}");
            Debug.Log($"Radius: {currentStats.radius}");
            Debug.Log($"DoT Damage: {currentStats.dotDamage}");
            Debug.Log($"Slow Duration: {currentStats.duration}");
            Debug.Log($"Description: {currentStats.description}");

            // 다음 레벨 정보
            if (!IsMaxLevel())
            {
                var nextStats = GetNextLevelStats();
                if (nextStats != null)
                {
                    Debug.Log($"Next Level Preview: {nextStats.description}");
                }
            }
        }

        void UpdateChildScale(string childName, Vector3 scaleMultiplier)
        {
            // 자식 이름으로 자식 Transform 찾기
            Transform child = transform.Find(childName);

            if (child != null)
            {
                // 자식의 기존 localScale 가져오기
                Vector3 originalScale = child.localScale;

                // 원하는 방식으로 변환
                //Vector3 newScale = Vector3.Scale(originalScale, scaleMultiplier);

                // 변경 적용
                child.localScale += scaleMultiplier;

                Debug.Log($"{child.name}의 새로운 스케일: {child.localScale}");
                if (currentLevel == 1)
                {
                    child.localScale = originalScale;
                }
            }
            else
            {
                Debug.LogWarning($"{childName}이라는 자식 오브젝트를 찾을 수 없습니다.");
            }
        }
        
        

            /// <summary>
            /// 디버그용 키 입력 처리
            /// </summary>
            void Update()
        {
            if (!debugMode) return;

            // 테스트용 레벨업 (L키)
            if (Input.GetKeyDown(KeyCode.L))
            {
                LevelUp();
            }

            // 테스트용 레벨 리셋 (R키)
            if (Input.GetKeyDown(KeyCode.R))
            {
                SetLevel(1);
            }

            // 테스트용 최대 레벨 (M키)
            if (Input.GetKeyDown(KeyCode.M))
            {
                SetLevel(maxLevel);
            }
        }
        
        /// <summary>
        /// 에디터 메뉴 - 레벨업
        /// </summary>
        [ContextMenu("Level Up")]
        private void EditorLevelUp()
        {
            LevelUp();
        }

        /// <summary>
        /// 에디터 메뉴 - 레벨 리셋
        /// </summary>
        [ContextMenu("Reset Level")]
        private void EditorResetLevel()
        {
            SetLevel(1);
        }

        /// <summary>
        /// 에디터 메뉴 - 최대 레벨
        /// </summary>
        [ContextMenu("Set Max Level")]
        private void EditorSetMaxLevel()
        {
            SetLevel(maxLevel);
        }

        /// <summary>
        /// 에디터 메뉴 - 무기 정보 출력
        /// </summary>
        [ContextMenu("Print Weapon Info")]
        private void EditorPrintInfo()
        {
            PrintWeaponInfo();
        }
    }
}