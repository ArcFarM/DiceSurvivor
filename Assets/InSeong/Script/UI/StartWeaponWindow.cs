using UnityEngine;
using System.Collections.Generic;

namespace DiceSurvivor.UI
{
    /// <summary>
    // StartWeaponPanel에 무작위로 근접 무기를 뽑아서 넘겨 주는 역할
    public class StartWeaponWindow : MonoBehaviour
    {
        #region Variables
        [SerializeField] TestItemArray meleeWeaponArray;
        StartWeaponPanel[] weaponSelections;


        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        void Start()
        {
            weaponSelections = GetComponentsInChildren<StartWeaponPanel>();
            AssignRandomWeapons();
        }
        #endregion

        #region Custom Methods
        void AssignRandomWeapons()
        {
            var items = meleeWeaponArray.items;
            var usedIndices = new HashSet<int>();
            var rnd = new System.Random();

            // 패널 수만큼 중복 없이 무작위로 뽑아서 할당
            for (int i = 0; i < weaponSelections.Length && i < items.Count; i++)
            {
                int idx;
                do
                {
                    idx = rnd.Next(items.Count);
                } while (usedIndices.Contains(idx));
                usedIndices.Add(idx);

                items[idx].SetWeaponStats(1);
                weaponSelections[i].SetRandomWeapon(items[idx]);
            }
        }
        #endregion
    }
}