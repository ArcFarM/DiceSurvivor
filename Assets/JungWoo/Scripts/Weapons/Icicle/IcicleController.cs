using UnityEngine;
using DiceSurvivor.Manager;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Icicle 무기 컨트롤러 - 데이터 관리 및 레벨 시스템
    /// </summary>
    public class IcicleController : MonoBehaviour
    {
        [Header("Weapon Reference")]
        [SerializeField] private IcicleWeapon icicleWeapon;

        [Header("Level System")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 8;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        // 현재 무기 스탯
        private WeaponStats currentStats;

        void Awake()
        {
            // IcicleWeapon 컴포넌트 찾기
            if (icicleWeapon == null)
            {
                icicleWeapon = GetComponent<IcicleWeapon>();
                if (icicleWeapon == null)
                {
                    icicleWeapon = gameObject.AddComponent<IcicleWeapon>();
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
                Debug.LogError("[IcicleController] DataTableManager를 찾을 수 없습니다!");
                return;
            }

            currentStats = dataManager.GetSplashWeapon("Icicle", currentLevel);

            if (currentStats != null)
            {
                icicleWeapon.UpdateWeaponStats(currentStats);
                Debug.Log($"[IcicleController] Icicle Lv.{currentLevel} 로드 성공!");
            }
            else
            {
                Debug.LogError($"[IcicleController] Icicle Lv.{currentLevel} 데이터 로드 실패!");
            }
        }

        /// <summary>
        /// 레벨 업
        /// </summary>
        public void LevelUp()
        {
            if (currentLevel >= maxLevel)
            {
                Debug.Log($"[IcicleController] 이미 최대 레벨({maxLevel})입니다!");
                return;
            }

            currentLevel++;
            LoadWeaponData();

            if (debugMode)
            {
                Debug.Log($"[IcicleController] 레벨업! 현재 레벨: {currentLevel}");
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
                Debug.LogError($"[IcicleController] 잘못된 레벨: {level} (1~{maxLevel} 범위)");
                return;
            }

            currentLevel = level;
            LoadWeaponData();

            if (debugMode)
            {
                Debug.Log($"[IcicleController] 레벨 설정: {currentLevel}");
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
                Debug.Log("[IcicleController] 이미 최대 레벨입니다!");
                return null;
            }

            var dataManager = DataTableManager.Instance;
            if (dataManager == null) return null;

            return dataManager.GetSplashWeapon("Icicle", currentLevel + 1);
        }

        /// <summary>
        /// 무기 정보 출력
        /// </summary>
        private void PrintWeaponInfo()
        {
            if (currentStats == null) return;

            Debug.Log("=== Icicle 정보 ===");
            Debug.Log($"레벨: {currentLevel}/{maxLevel}");
            Debug.Log($"쿨다운: {currentStats.cooldown}초");
            Debug.Log($"투사체 수: {currentStats.projectileCount}");
            Debug.Log($"폭발 범위: {currentStats.explosionRadius}");
            Debug.Log($"폭발 데미지: {currentStats.explosionDamage}");
            Debug.Log($"DoT 데미지: {currentStats.dotDamage}");
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

            // 테스트용 레벨업 (I키)
            if (Input.GetKeyDown(KeyCode.I))
            {
                LevelUp();
            }

            // 테스트용 레벨 리셋 (O키)
            if (Input.GetKeyDown(KeyCode.O))
            {
                SetLevel(1);
            }

            // 테스트용 최대 레벨 (P키)
            if (Input.GetKeyDown(KeyCode.P))
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