using UnityEngine;
using DiceSurvivor.Manager;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Boomerang 무기 컨트롤러 - 데이터 관리 및 레벨 시스템
    /// </summary>
    public class BoomerangController : MonoBehaviour
    {
        [Header("Weapon Reference")]
        [SerializeField] private BoomerangWeapon boomerangWeapon;

        [Header("Level System")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 8;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        // 현재 무기 스탯
        private WeaponStats currentStats;

        void Awake()
        {
            // BoomerangWeapon 컴포넌트 찾기
            if (boomerangWeapon == null)
            {
                boomerangWeapon = GetComponent<BoomerangWeapon>();
                if (boomerangWeapon == null)
                {
                    boomerangWeapon = gameObject.AddComponent<BoomerangWeapon>();
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
            if (debugMode)
            {
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
                Debug.Log("[BoomerangController] 이미 최대 레벨입니다!");
                return null;
            }

            var dataManager = DataTableManager.Instance;
            if (dataManager == null) return null;

            return dataManager.GetRangedWeapon("Boomerang", currentLevel + 1);
        }

        /// <summary>
        /// 무기 정보 출력
        /// </summary>
        private void PrintWeaponInfo()
        {
            if (currentStats == null) return;

            Debug.Log("=== Boomerang 정보 ===");
            Debug.Log($"레벨: {currentLevel}/{maxLevel}");
            Debug.Log($"쿨다운: {currentStats.cooldown}초");
            Debug.Log($"데미지: {currentStats.damage}");
            Debug.Log($"범위: {currentStats.range}");
            Debug.Log($"투사체 크기: {currentStats.projectileSize}");
            Debug.Log($"투사체 속도: {currentStats.projectileSpeed}");
            Debug.Log($"투사체 개수: {currentStats.projectileCount}");
            Debug.Log($"관통: {currentStats.isPiercing}");
            Debug.Log($"돌아옴: {currentStats.canReturn}");
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
        /// 에디터 메뉴 - 무기 정보 출력
        /// </summary>
        [ContextMenu("Print Weapon Info")]
        private void EditorPrintInfo()
        {
            PrintWeaponInfo();
        }
    }
}