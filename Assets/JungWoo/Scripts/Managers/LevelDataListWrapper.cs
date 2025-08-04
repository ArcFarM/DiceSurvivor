using DiceSurvivor.WeaponSystem;
using System.Collections.Generic;
using UnityEngine;

namespace DiceSurvivor.JsonWrapper
{
    [System.Serializable]
    public class LevelDataListWrapper : MonoBehaviour
    {
        public DictionaryWrapper[] items;

        public Dictionary<string, List<LevelData>> ToDict()
        {
            var dict = new Dictionary<string, List<LevelData>>();
            foreach (var item in items)
                dict[item.id] = new List<LevelData>(item.levelData);
            return dict;
        }
    }
}