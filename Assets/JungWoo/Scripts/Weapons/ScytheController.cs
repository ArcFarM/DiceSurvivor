using UnityEngine;
using DiceSurvivor.Manager;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Scythe 무기의 데이터 로드 및 레벨 관리를 담당하는 컨트롤러
    /// </summary>
    public class ScytheController : MonoBehaviour
    {
        [Header("무기 컴포넌트")]
        [SerializeField] private ScytheWeapon scytheWeapon;    // Scythe 무기 컴포넌트

        [Header("무기 설정")]
        [SerializeField] private string weaponName = "Scythe"; // 무기 이름 (JSON 데이터와 일치)
        [SerializeField] private int currentLevel = 1;         // 현재 무기 레벨

        private WeaponStats currentStats;                       // 현재 무기 스탯

        private void Awake()
        {
            // ScytheWeapon 컴포넌트가 없으면 찾기
            if (scytheWeapon == null)
            {
                
                if (scytheWeapon == null)
                {
                    Debug.LogError("ScytheWeapon 컴포넌트를 찾을 수 없습니다!");
                }
            }
        }

        private void Start()
        {
            scytheWeapon = GetComponent<ScytheWeapon>();
            // 초기 무기 데이터 로드 및 설정
            LoadAndApplyWeaponData();
        }

        /// <summary>
        /// 무기 데이터를 로드하고 ScytheWeapon에 적용
        /// </summary>
        private void LoadAndApplyWeaponData()
        {
            // DataTableManager에서 무기 데이터 가져오기
            currentStats = DataTableManager.Instance.GetMeleeWeapon(weaponName, currentLevel);

            if (currentStats != null)
            {
                // ScytheWeapon에 데이터 전달
                scytheWeapon.SetWeaponStats(currentStats);

                Debug.Log($"{weaponName} Lv.{currentLevel} 데이터 로드 및 적용 완료");
                Debug.Log($"데미지: {currentStats.damage}, 쿨다운: {currentStats.cooldown}, 범위: {currentStats.range}");
                Debug.Log($"설명: {currentStats.description}");
            }
            else
            {
                Debug.LogError($"{weaponName} Lv.{currentLevel} 데이터를 찾을 수 없습니다!");

                
            }
        }

        /// <summary>
        /// 무기 레벨업
        /// </summary>
        public void LevelUp()
        {
            // 최대 레벨 체크
            if (currentLevel >= 8)
            {
                Debug.Log($"{weaponName}은(는) 이미 최대 레벨입니다.");
                return;
            }

            // 레벨 증가 -> TODO
            if (CanLevelUp())
            {
                
                //currentLevel++;
            }

            // 새로운 레벨의 데이터 로드 및 적용
            LoadAndApplyWeaponData();

            // 레벨업 이펙트 (옵션)
            PlayLevelUpEffect();
        }

        /// <summary>
        /// 특정 레벨로 설정
        /// </summary>
        public void SetLevel(int level)
        {
            if (level < 1 || level > 8)
            {
                Debug.LogError($"유효하지 않은 레벨: {level}. 레벨은 1~8 사이여야 합니다.");
                return;
            }

            currentLevel = level;
            LoadAndApplyWeaponData();
        }

        /// <summary>
        /// 레벨업 이펙트 재생
        /// </summary>
        private void PlayLevelUpEffect()
        {
            // 레벨업 파티클 효과
            // ParticleManager.Instance?.PlayLevelUpEffect(transform.position);

            // 레벨업 사운드
            // AudioManager.Instance?.PlaySound("WeaponLevelUp");

            Debug.Log($"{weaponName} 레벨업! 현재 레벨: {currentLevel}");
        }

        /// <summary>
        /// 현재 무기 정보 가져오기
        /// </summary>
        public int GetCurrentLevel() => currentLevel;
        public WeaponStats GetCurrentStats() => currentStats;
        public string GetWeaponName() => weaponName;

        /// <summary>
        /// 레벨업 가능 여부 확인
        /// </summary>
        public bool CanLevelUp()
        {
            return currentLevel < 8;
        }

        /// <summary>
        /// 디버그용 - 무기 정보 출력
        /// </summary>
        [ContextMenu("Print Weapon Info")]
        private void PrintWeaponInfo()
        {
            if (currentStats != null)
            {
                Debug.Log("=== Scythe 무기 정보 ===");
                Debug.Log($"이름: {currentStats.name}");
                Debug.Log($"레벨: {currentStats.level}");
                Debug.Log($"데미지: {currentStats.damage}");
                Debug.Log($"쿨다운: {currentStats.cooldown}");
                Debug.Log($"범위: {currentStats.range}");
                Debug.Log($"설명: {currentStats.description}");
            }
            else
            {
                Debug.Log("무기 데이터가 로드되지 않았습니다.");
            }
        }
    }
}