using DiceSurvivor.Attack;
using DiceSurvivor.Audio;
using DiceSurvivor.Manager;
using DiceSurvivor.Type;
using UnityEngine;

namespace DiceSurvivor.Weapon
{
    public class RangedWeaponController : MonoBehaviour
    {
        #region Variables
        // 참조
        private Animator animator;
        private AttackEffectSpawn attackEffect;

        [Header("------Weapon-------")]
        [SerializeField] private string weaponName = "Laser";
        [SerializeField] private int weaponLevel = 1;
        [SerializeField] private int maxLevel = 8;
        [SerializeField] private SfxType sfxType;

        // 무기 타입 (자동 감지)
        private WeaponType weaponType;
        private string weaponTypeString; // "Wp_Me", "Wp_Ra", "Wp_Sp"

        private int currentLevel;

        [Header("------WeaponStat------")]
        [SerializeField] public WeaponStats currentWeaponStats;

        public event System.Action<WeaponStats> OnWeaponLoaded;
        #endregion

        #region Properties
        public WeaponType CurrentWeaponType => weaponType;
        public string WeaponName => weaponName;
        public int WeaponLevel => weaponLevel;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            animator = this.GetComponent<Animator>();
            attackEffect = this.GetComponentInChildren<AttackEffectSpawn>();
        }

        private void Start()
        {
            // 초기 무기 타입 감지 및 데이터 로드
            DetectAndLoadWeaponType();
            currentLevel = weaponLevel;
        }

        private void Update()
        {
            // 레벨이 변경되었을 때
            if (weaponLevel != currentLevel)
            {
                UpdateWeaponData();
                currentLevel = weaponLevel;
            }
        }

        private void OnEnable()
        {
            if (animator != null)
            {
                // ItemManager에서 레벨 동기화
                int dictLevel = ItemManager.Instance.GetItemLevel(weaponName);
                if (weaponLevel < dictLevel)
                {
                    weaponLevel = dictLevel;
                    UpdateWeaponData(); // 레벨 변경 시 데이터 업데이트
                }
                animator.SetBool("IsAttack", true);
            }
        }

        private void OnDisable()
        {
            if (animator != null)
            {
                animator.SetBool("IsAttack", false);
            }
        }
        #endregion

        #region Custom Methods

        /// <summary>
        /// 무기 타입을 자동으로 감지하고 데이터를 로드
        /// </summary>
        private void DetectAndLoadWeaponType()
        {
            // 먼저 각 카테고리에서 무기를 찾아 타입 결정
            if (TryDetectWeaponType())
            {
                LoadWeaponDataByType();
            }
            else
            {
                Debug.LogError($"[WeaponController] {weaponName} 무기를 데이터베이스에서 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 무기 이름으로 타입을 감지
        /// </summary>
        private bool TryDetectWeaponType()
        {
            var dataManager = DataTableManager.Instance;
            if (dataManager == null)
            {
                Debug.LogError("[WeaponController] DataTableManager를 찾을 수 없습니다!");
                return false;
            }

            // Melee 무기 확인
            var meleeWeapon = dataManager.GetMeleeWeapon(weaponName, 1);
            if (meleeWeapon != null)
            {
                weaponType = WeaponType.MeleeWeapon;
                weaponTypeString = "Wp_Me";
                Debug.Log($"[WeaponController] {weaponName}은(는) 근접 무기입니다.");
                return true;
            }

            // Ranged 무기 확인
            var rangedWeapon = dataManager.GetRangedWeapon(weaponName, 1);
            if (rangedWeapon != null)
            {
                weaponType = WeaponType.RangedWeapon;
                weaponTypeString = "Wp_Ra";
                Debug.Log($"[WeaponController] {weaponName}은(는) 원거리 무기입니다.");
                return true;
            }

            // Splash 무기 확인
            var splashWeapon = dataManager.GetSplashWeapon(weaponName, 1);
            if (splashWeapon != null)
            {
                weaponType = WeaponType.SplashWeapon;
                weaponTypeString = "Wp_Sp";
                Debug.Log($"[WeaponController] {weaponName}은(는) 범위 무기입니다.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 감지된 타입에 따라 무기 데이터 로드
        /// </summary>
        private void LoadWeaponDataByType()
        {
            switch (weaponType)
            {
                case WeaponType.MeleeWeapon:
                    LoadMeleeWeaponData();
                    break;
                case WeaponType.RangedWeapon:
                    LoadRangedWeaponData();
                    break;
                case WeaponType.SplashWeapon:
                    LoadSplashWeaponData();
                    break;
                default:
                    Debug.LogError($"[WeaponController] 알 수 없는 무기 타입: {weaponType}");
                    break;
            }
        }

        /// <summary>
        /// 무기 데이터 업데이트 (레벨 변경 시)
        /// </summary>
        private void UpdateWeaponData()
        {
            // 타입이 이미 결정되어 있으므로 해당 타입의 데이터만 로드
            LoadWeaponDataByType();

            // 무기 속성 업데이트 로그
            if (currentWeaponStats != null)
            {
                LogWeaponStats();
            }
        }

        /// <summary>
        /// 근접 무기 데이터 로드
        /// </summary>
        public void LoadMeleeWeaponData()
        {
            currentWeaponStats = DataTableManager.Instance.GetMeleeWeapon(weaponName, weaponLevel);
            if (currentWeaponStats != null)
            {
                OnWeaponLoaded?.Invoke(currentWeaponStats);
                Debug.Log($"[Melee] {weaponName} Lv.{weaponLevel} 로드 완료");
            }
            else
            {
                Debug.LogError($"[Melee] {weaponName} Lv.{weaponLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 원거리 무기 데이터 로드
        /// </summary>
        public void LoadRangedWeaponData()
        {
            currentWeaponStats = DataTableManager.Instance.GetRangedWeapon(weaponName, weaponLevel);
            if (currentWeaponStats != null)
            {
                OnWeaponLoaded?.Invoke(currentWeaponStats);
                Debug.Log($"[Ranged] {weaponName} Lv.{weaponLevel} 로드 완료");
            }
            else
            {
                Debug.LogError($"[Ranged] {weaponName} Lv.{weaponLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 범위 무기 데이터 로드
        /// </summary>
        public void LoadSplashWeaponData()
        {
            currentWeaponStats = DataTableManager.Instance.GetSplashWeapon(weaponName, weaponLevel);
            if (currentWeaponStats != null)
            {
                OnWeaponLoaded?.Invoke(currentWeaponStats);
                Debug.Log($"[Splash] {weaponName} Lv.{weaponLevel} 로드 완료");
            }
            else
            {
                Debug.LogError($"[Splash] {weaponName} Lv.{weaponLevel} 데이터를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 현재 무기 속성 로그 출력
        /// </summary>
        private void LogWeaponStats()
        {
            Debug.Log($"===== {weaponName} Lv.{weaponLevel} 속성 =====");
            Debug.Log($"타입: {weaponTypeString}");
            Debug.Log($"데미지: {currentWeaponStats.damage}");
            Debug.Log($"쿨다운: {currentWeaponStats.cooldown}");

            if (currentWeaponStats.range > 0)
                Debug.Log($"사거리: {currentWeaponStats.range}");

            if (currentWeaponStats.projectileCount > 0)
                Debug.Log($"투사체 수: {currentWeaponStats.projectileCount}");

            if (currentWeaponStats.projectileSpeed > 0)
                Debug.Log($"투사체 속도: {currentWeaponStats.projectileSpeed}");

            if (currentWeaponStats.explosionRadius > 0)
                Debug.Log($"폭발 반경: {currentWeaponStats.explosionRadius}");

            if (currentWeaponStats.isPiercing)
                Debug.Log($"관통: {currentWeaponStats.isPiercing}");

            Debug.Log("================================");
        }

        /// <summary>
        /// 수동으로 무기 타입 변경 (디버그용)
        /// </summary>
        [ContextMenu("Refresh Weapon Type")]
        public void RefreshWeaponType()
        {
            DetectAndLoadWeaponType();
        }

        /// <summary>
        /// 공격 이펙트 생성
        /// </summary>
        public void AttackEffectSpawn()
        {
            attackEffect?.SpawnAttackEffect();
        }

        /// <summary>
        /// 창/지팡이 타입 공격
        /// </summary>
        public void SpearAndStaffAttack()
        {
            var spearStaff = this.GetComponentInChildren<SpearStaffAttack>();
            spearStaff?.ExecuteAttack();
        }

        /// <summary>
        /// 사운드 재생
        /// </summary>
        public void PlaySfx()
        {
            SfxManager.Instance?.PlaySfx(sfxType);
        }

        /// <summary>
        /// 레벨업 처리
        /// </summary>
        public void LevelUp()
        {
            if (weaponLevel < maxLevel)
            {
                weaponLevel++;
                UpdateWeaponData();
                Debug.Log($"[WeaponController] {weaponName} 레벨업! 현재 레벨: {weaponLevel}");
            }
            else
            {
                Debug.Log($"[WeaponController] {weaponName}은(는) 최대 레벨입니다.");
            }
        }
        #endregion
    }
}