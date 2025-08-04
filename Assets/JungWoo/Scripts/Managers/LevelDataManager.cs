using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.JsonWrapper;

namespace DiceSurvivor.WeaponSystem
{
    public class LevelDataManager : MonoBehaviour
    {
        #region Variables
        public static LevelDataManager Instance { get; private set; }

        private Dictionary<string, List<LevelData>> levelDataDict;
        #endregion


        #region Unity Event Method
        void Awake()
        {
            Instance = this;
            LoadData();
        }
        #endregion

        #region Custom Method

        void LoadData()
        {
            TextAsset json = Resources.Load<TextAsset>("ranged_weapons");
            var raw = JsonUtility.FromJson<LevelDataListWrapper>(json.text);
            levelDataDict = raw.ToDict();
        }

        public LevelData GetLevelData(string id, int level)
        {
            if (levelDataDict.TryGetValue(id, out var list))
            {
                int idx = Mathf.Clamp(level - 1, 0, list.Count - 1);
                return list[idx];
            }
            return null;
        }

        #endregion
        // Json 파싱용 래퍼
        [System.Serializable]
        public class LevelDataListWrapper
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

        [System.Serializable]
        public class DictionaryWrapper
        {
            public string id;
            public LevelData[] levelData;
        }
    }
}