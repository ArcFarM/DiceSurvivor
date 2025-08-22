using DiceSurvivor.Attack;
using DiceSurvivor.Audio;
using DiceSurvivor.Manager;
using UnityEngine;
using UnityEngine.Android;

namespace DiceSurvivor.Weapon
{
    public class SplashWeaponController : MonoBehaviour
    {
        #region Variables
        //참조
        private AttackEffectSpawn attackEffect;

        [Header("------Weapon-------")]
        [SerializeField] private string weaponName = "KillingAura";
        [SerializeField] private int weaponLevel = 1;
        [SerializeField] private int maxLevel = 8;
        [SerializeField] private SfxType sfxType;

        private int currentLevel;

        [Header("------WeaponStat------")]
        [SerializeField] public WeaponStats currentWeaponStats;

        public event System.Action<WeaponStats> OnWeaponLoaded;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            
            attackEffect = this.GetComponentInChildren<AttackEffectSpawn>();
        }

        private void Start()
        {
            LoadWeaponData();

            currentLevel = weaponLevel;
        }
        private void Update()
        {
            if (weaponLevel != currentLevel)
            {
                LoadWeaponData();
                currentLevel = weaponLevel;
            }
        }

        private void OnEnable()
        {
            int dictLevel = ItemManager.Instance.GetItemLevel(weaponName);
            if (weaponLevel < dictLevel) weaponLevel = dictLevel;
        }
        private void OnDisable()
        {

        }
        #endregion

        #region Custom Methods
        public void LoadWeaponData()
        {
            currentWeaponStats = DataTableManager.Instance.GetMeleeWeapon(weaponName, weaponLevel);
            OnWeaponLoaded?.Invoke(currentWeaponStats);     //Weapon 값 넘겨주기
        }

        public void AttackEffectSpawn()
        {
            attackEffect.SpawnAttackEffect();
        }

        public void SpearAndStaffAttack()
        {
            this.GetComponentInChildren<SpearStaffAttack>().ExecuteAttack();
        }
        public void PlaySfx()
        {
            SfxManager.Instance.PlaySfx(sfxType);
        }
        #endregion
    }

}
