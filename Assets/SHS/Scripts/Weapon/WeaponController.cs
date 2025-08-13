using DiceSurvivor.Manager;
using UnityEngine;

namespace DiceSurvivor.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        [Header("------Weapon-------")]
        [SerializeField]private string weaponName = "Hammer";
        [SerializeField]private int weaponLevel = 1;

        [Header("------WeaponStat------")]
        public WeaponStats currentWeaponStats;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void Start()
        {
            LoadWeaponData();
        }
        #endregion

        #region Custom Methods
        public void LoadWeaponData()
        {
            currentWeaponStats = DataTableManager.Instance.GetMeleeWeapon(weaponName, weaponLevel);
        }
        #endregion
    }

}
