using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Weapon;

namespace DiceSurvivor.WeaponSystem
{
    /// <summary>
    /// Asteroid 무기 컨트롤러 - 데이터 관리 및 레벨 시스템
    /// </summary>
    public class AsteroidController : MonoBehaviour
    {
        [Header("Weapon Reference")]
        [SerializeField] private AsteroidWeapon asteroidWeapon;

        [Header("Level System")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 8;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        // 현재 무기 스탯
        private WeaponStats currentStats;

        void Awake()
        {
            // AsteroidWeapon 컴포넌트 찾기
            if (asteroidWeapon == null)
            {
                asteroidWeapon = GetComponent<AsteroidWeapon>();
                if (asteroidWeapon == null)
                {
                    asteroidWeapon = gameObject.AddComponent<AsteroidWeapon>();
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
                Debug.LogError("[AsteroidController] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            currentStats = dataManager.GetSplashWeapon("Asteroid", currentLevel);

            if (currentStats != null)
            {
                asteroidWeapon.UpdateWeaponStats(currentStats);
                Debug.Log($"[AsteroidController] Asteroid Lv.{currentLevel} 로드 성공!");
            }
            else
            {
                Debug.LogError($"[AsteroidController] Asteroid Lv.{currentLevel} 데이터 로드 실패!");
            }
        }

        /// <summary>
        /// 레벨 업
        /// </summary>
        public void LevelUp()
        {
            if (currentLevel >= maxLevel)
            {
                Debug.Log($"[AsteroidController] 이미 최대 레벨({maxLevel})입니다!");
                return;
            }

            currentLevel++;
            LoadWeaponData();

            // 레벨업 시 AsteroidWeapon의 LevelUp 호출 (소행성 재생성)
            if (asteroidWeapon != null)
            {
                asteroidWeapon.LevelUp();
            }

            if (debugMode)
            {
                Debug.Log($"[AsteroidController] 레벨업! 현재 레벨: {currentLevel}");
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
                Debug.LogError($"[AsteroidController] 잘못된 레벨: {level} (1~{maxLevel} 범위)");
                return;
            }

            currentLevel = level;
            LoadWeaponData();

            if (debugMode)
            {
                Debug.Log($"[AsteroidController] 레벨 설정: {currentLevel}");
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
                Debug.Log("[AsteroidController] 이미 최대 레벨입니다!");
                return null;
            }

            var dataManager = DataTableManager.Instance;
            if (dataManager == null) return null;

            return dataManager.GetSplashWeapon("Asteroid", currentLevel + 1);
        }

        /// <summary>
        /// 무기 정보 출력
        /// </summary>
        private void PrintWeaponInfo()
        {
            if (currentStats == null) return;

            Debug.Log("=== Asteroid 정보 ===");
            Debug.Log($"레벨: {currentLevel}/{maxLevel}");
            Debug.Log($"쿨다운: {currentStats.cooldown}초");
            Debug.Log($"공전 반경: {currentStats.radius}");
            Debug.Log($"소행성 개수: {currentStats.projectileCount}");
            Debug.Log($"소행성 크기: {currentStats.projectileSize}");
            Debug.Log($"회전 속도: {currentStats.projectileSpeed}");
            Debug.Log($"데미지: {currentStats.damage}");
            Debug.Log($"지속 시간: {currentStats.duration}초");
            Debug.Log($"설명: {currentStats.description}");

            // 다음 레벨 정보
            if (!IsMaxLevel())
            {
                var nextStats = GetNextLevelStats();
                if (nextStats != null)
                {
                    Debug.Log($"다음 레벨: {nextStats.description}");
                }
            }
        }

        /// <summary>
        /// 디버그용 키 입력 처리
        /// </summary>
        void Update()
        {
            if (!debugMode) return;

            // 테스트용 레벨업 (G키)
            if (Input.GetKeyDown(KeyCode.G))
            {
                LevelUp();
            }

            // 테스트용 레벨 리셋 (H키)
            if (Input.GetKeyDown(KeyCode.H))
            {
                SetLevel(1);
            }

            // 테스트용 최대 레벨 (J키)
            if (Input.GetKeyDown(KeyCode.J))
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