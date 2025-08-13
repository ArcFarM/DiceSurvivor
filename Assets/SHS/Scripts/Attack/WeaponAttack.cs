using DiceSurvivor.Weapon;
using UnityEngine;

namespace DiceSurvivor.Attack
{
    public class WeaponAttack : MonoBehaviour
    {
        #region Variables
        
        public WeaponController weaponData;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        protected virtual void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
            {
                Debug.Log($"적 피해 받음 : {weaponData.currentWeaponStats.damage}");
            }
        }
        #endregion

        #region Custom Methods
        #endregion
    }

}
