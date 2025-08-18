using DiceSurvivor.Weapon;
using UnityEngine;

namespace DiceSurvivor.Attack
{
    public class WeaponSplashAttack : WeaponAttackBase
    {
        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                Debug.Log($"적 피해 받음 : {Weapon.explosionDamage}");
            }
        }
        #endregion

        #region Custom Methods
        #endregion
    }
}
