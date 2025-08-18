using DiceSurvivor.Manager;
using DiceSurvivor.SHS;
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
        private void Start()
        {
            animator = this.GetComponent<Animator>();
            attackEffect = this.GetComponentInChildren<AttackEffectSpawn>();

            LoadWeaponData();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭 시
            {
                animator.SetTrigger("IsAttack"); // 공격 애니메이션 트리거 실행
            }
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
        #endregion
    }

}
