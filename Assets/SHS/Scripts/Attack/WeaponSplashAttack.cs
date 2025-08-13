using UnityEngine;

namespace DiceSurvivor.Attack
{
    public class WeaponSplashAttack : WeaponAttack
    {
        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                Debug.Log($"적 피해 받음 : {weaponData.currentWeaponStats.explosionDamage}");
            }
        }
        #endregion

        #region Custom Methods
        #endregion
    }
}
