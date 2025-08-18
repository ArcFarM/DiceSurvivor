using UnityEngine;
using DiceSurvivor.Manager;
using System.Collections.Generic;
//StartWeaponPanel에 무작위로 근접 무기를 뽑아서 넘겨 주는 역할

namespace DiceSurvivor.UI
{
    public class StartWeaponWindow : MonoBehaviour
    {
        #region Variables
        StartWeaponPanel[] weaponSelections;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        void Start()
        {
            weaponSelections = GetComponentsInChildren<StartWeaponPanel>();
            AddRandomWeapon();
        }
        #endregion

        #region Custom Methods
        void AddRandomWeapon()
        {
            // 무작위 무기 추가
            List<string> WeaponNames = DataTableManager.Instance.GetDT.MeleeWeapons.GetWeaponNames();
            int size = WeaponNames.Count;
            int[] randomIndex = new int[size];
            for (int i = 0; i < size; i++)
            {
                randomIndex[i] = i;
            }

            for (int i = size - 1; i >= 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (randomIndex[i], randomIndex[j]) = (randomIndex[j], randomIndex[i]);
            }

            for (int i = 0; i < weaponSelections.Length; i++)
            {
                string randomName = WeaponNames[randomIndex[i]];
                //weaponSelections[i].AddRandomWeapon(DataTableManager.Instance.GetMeleeWeapon(randomName, 1));
            }
        }
        #endregion
    }
}