using DiceSurvivor.Manager;
using DiceSurvivor.Weapon;
using UnityEngine;

namespace DiceSurvivor.Attack
{
    public abstract class WeaponAttackBase : MonoBehaviour
    {
        #region Variables
        //참조
        protected WeaponController weaponController;

        [SerializeField]
        protected WeaponStats weapon;
        #endregion

        #region Properties
        public WeaponStats Weapon
        {
            get
            {
                return weapon;
            }
            set
            {
                weapon = value;
            }
        }
        #endregion

        #region Unity Event Methods
        protected virtual void Awake()
        {
            weaponController = GetComponentInParent<WeaponController>();            
            if (weaponController != null)
            {
                weaponController.OnWeaponLoaded += weaponStats =>
                {
                    Weapon = weaponStats;
                    //Debug.Log($"무기 로딩 완료: {Weapon.id}");
                };
            }
        }

        #endregion

        #region Custom Methods
        public virtual void Mthd() { }
        #endregion
    }

}
