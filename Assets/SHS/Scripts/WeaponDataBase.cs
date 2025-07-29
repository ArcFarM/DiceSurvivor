using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponDataBase", menuName = "Weapon System/WeaponDataBase")]
public class WeaponDataBase : ScriptableObject
{
    /// <summary>
    /// Weapon들을 모아놓는 DataBase
    /// </summary>
    #region Variables
    public WeaponData[] weaponObjects;
    #endregion

    #region Custom Method

    #endregion
}
