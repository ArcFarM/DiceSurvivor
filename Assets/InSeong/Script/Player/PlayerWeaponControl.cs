using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Weapon;
using DiceSurvivor.Manager; 

namespace DiceSurvivor.Player
{
    public class PlayerWeaponControl : MonoBehaviour
    {
        #region Variables
        List<WeaponController> weapons = new List<WeaponController>();
        float timer = 0;
        public float cooldown = 0.3f;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        void Start()
        {
            foreach (Transform objects in gameObject.transform)
            {
                var weapon = objects.gameObject.GetComponent<WeaponController>();
                if (weapon != null)
                {
                    weapons.Add(weapon);
                }
            }
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                CheckEnabledWeapon();
                timer = 0;
            }
        }
        #endregion

        #region Custom Methods
        void CheckEnabledWeapon()
        {
            ItemManager im = ItemManager.Instance;
            foreach (WeaponController weapon in weapons)
            {
                if (weapon.gameObject.activeSelf) continue;
                if (weapon.currentWeaponStats.type == "MeleeWeapon")
                {
                    if (im.GetMeleeWeapon != null && im.GetMeleeWeapon.itemName == weapon.currentWeaponStats.name)
                    {
                        weapon.gameObject.SetActive(true);
                    }
                }
                else
                {
                    foreach (var item in im.GetSubWeapons)
                    {
                        if (item != null && item.itemName == weapon.currentWeaponStats.name)
                        {
                            weapon.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }
            Debug.Log("활성화 된 무기 찾기 완료");
        }
        #endregion
    }
}