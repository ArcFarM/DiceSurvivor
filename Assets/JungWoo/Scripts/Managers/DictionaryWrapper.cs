using DiceSurvivor.WeaponSystem;
using UnityEngine;

namespace DiceSurvivor.JsonWrapper
{
    [System.Serializable]
    public class DictionaryWrapper : MonoBehaviour
    {
        public string id;
        public LevelData[] levelData;
    }
}