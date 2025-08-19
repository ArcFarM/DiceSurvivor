using DiceSurvivor.Attack;
using DiceSurvivor.Manager;
using UnityEngine;

namespace DiceSurvivor.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;
        private AttackEffectSpawn attackEffect;        

        [Header("------Weapon-------")]
        [SerializeField]private string weaponName = "Hammer";
        [SerializeField]private int weaponLevel = 1;

        [Header("------WeaponStat------")]
        [SerializeField]public WeaponStats currentWeaponStats;

        public event System.Action<WeaponStats> OnWeaponLoaded;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            animator = this.GetComponent<Animator>();
            attackEffect = this.GetComponentInChildren<AttackEffectSpawn>();            
        }

        private void Start()
        {
            LoadWeaponData();
        }

        private void OnEnable()
        {
            int dictLevel = ItemManager.Instance.GetItemLevel(weaponName);
            if (weaponLevel < dictLevel) weaponLevel = dictLevel;
            animator.SetBool("IsAttack",true); // 공격 애니메이션 트리거 실행
        }
        private void OnDisable()
        {
            animator.SetBool("IsAttack", false);
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
        #endregion
    }

}
