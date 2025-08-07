/*using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    /// 모든 타입의 무기 데이터를 통합 관리하는 매니저
    /// </summary>
    public class UnifiedWeaponDataManager : MonoBehaviour
    {
        // 싱글톤 패턴
        private static UnifiedWeaponDataManager instance;
        public static UnifiedWeaponDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<UnifiedWeaponDataManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UnifiedWeaponDataManager");
                        instance = go.AddComponent<UnifiedWeaponDataManager>();
                    }
                }
                return instance;
            }
        }

        [Header("JSON 파일 설정")]
        [SerializeField] private string jsonFileName = "DataTable.json";

        [System.Serializable]
        public class WeaponStats
        {
            public string id;
            public string name;
            public string type;
            public int level;
            public float damage;
            public float cooldown;
            public float radius;
            public float range;
            public float projectileSize;
            public float projectileSpeed;
            public int projectileCount;
            public float explosionRadius;
            public float explosionDamage;
            public float dotDamage;
            public float duration;
            public bool isPiercing;
            public bool canReturn;
            public string description;
        }
        [Serializable]
        public class PassiveSkill
        {
            public string id;
            public string name;
            public string type;
            public int? level;
            public string effect;
            public float? value;
            public string unit;
            public float cooldown;
            public string evolutionFor;
            public bool isConsumable;
            public string description;
        }

        
        

        [System.Serializable]
        private class WeaponCategory
        {
            private Dictionary<string, Dictionary<int, WeaponStats>> weaponData;

            public WeaponCategory()
            {
                weaponData = new Dictionary<string, Dictionary<int, WeaponStats>>();
            }

            public void AddWeapon(string weaponName, int level, WeaponStats stats)
            {
                if (!weaponData.ContainsKey(weaponName))
                {
                    weaponData[weaponName] = new Dictionary<int, WeaponStats>();
                }
                weaponData[weaponName][level] = stats;
            }

            public WeaponStats GetWeapon(string weaponName, int level)
            {
                if (weaponData.ContainsKey(weaponName) && weaponData[weaponName].ContainsKey(level))
                {
                    return weaponData[weaponName][level];
                }
                return null;
            }

            public Dictionary<int, WeaponStats> GetAllLevelsOfWeapon(string weaponName)
            {
                return weaponData.ContainsKey(weaponName) ? weaponData[weaponName] : null;
            }

            public List<string> GetWeaponNames()
            {
                return new List<string>(weaponData.Keys);
            }
        }

        // 패시브 범주별 데이터 구조
        [Serializable]
        public class PassiveCategory
        {
            private Dictionary<string, PassiveSkill> passiveData;

            public PassiveCategory()
            {
                passiveData = new Dictionary<string, PassiveSkill>();
            }

            public void AddPassive(string passiveName, PassiveSkill passive)
            {
                passiveData[passiveName] = passive;
            }

            public PassiveSkill GetPassive(string passiveName)
            {
                return passiveData.ContainsKey(passiveName) ? passiveData[passiveName] : null;
            }

            public List<string> GetPassiveNames()
            {
                return new List<string>(passiveData.Keys);
            }
        }

        // 무기 타입별 캐시
        private Dictionary<string, List<WeaponStats>> meleeWeapons = new Dictionary<string, List<WeaponStats>>();
        private Dictionary<string, List<WeaponStats>> rangedWeapons = new Dictionary<string, List<WeaponStats>>();
        private Dictionary<string, List<WeaponStats>> splashWeapons = new Dictionary<string, List<WeaponStats>>();

        // 전체 무기 캐시 (빠른 검색용)
        private Dictionary<string, List<WeaponStats>> allWeaponsCache = new Dictionary<string, List<WeaponStats>>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadWeaponData();
        }

        /// <summary>
        /// JSON 파일에서 무기 데이터 로드
        /// </summary>
        private void LoadWeaponData()
        {
            // StreamingAssets 폴더 확인
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Debug.LogError($"StreamingAssets 폴더가 없습니다. Assets 폴더 안에 'StreamingAssets' 폴더를 생성하세요.");
                Debug.LogError($"예상 경로: {Application.streamingAssetsPath}");
                return;
            }

            string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

            // 디버그 정보 출력
            Debug.Log($"StreamingAssets 경로: {Application.streamingAssetsPath}");
            Debug.Log($"JSON 파일 전체 경로: {filePath}");

            if (!File.Exists(filePath))
            {
                Debug.LogError($"JSON 파일을 찾을 수 없습니다: {filePath}");
                Debug.LogError("해결 방법:");
                Debug.LogError("1. Assets 폴더 안에 'StreamingAssets' 폴더를 생성하세요");
                Debug.LogError("2. DataTable.json 파일을 StreamingAssets 폴더에 넣으세요");

                // StreamingAssets 폴더의 파일 목록 출력
                if (Directory.Exists(Application.streamingAssetsPath))
                {
                    string[] files = Directory.GetFiles(Application.streamingAssetsPath);
                    Debug.Log($"StreamingAssets 폴더의 파일들: {string.Join(", ", files)}");
                }
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);

                // JSON을 Dictionary로 파싱 (Unity의 JsonUtility는 복잡한 구조를 잘 못 다룸)
                // 실제로는 Newtonsoft.Json이나 다른 JSON 라이브러리 사용 권장
                ParseJsonManually(jsonContent);

                Debug.Log($"무기 데이터 로드 완료! 근접: {meleeWeapons.Count}, 원거리: {rangedWeapons.Count}, 범위: {splashWeapons.Count}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON 파싱 오류: {e.Message}");
            }
        }

        /// <summary>
        /// JSON을 수동으로 파싱 (실제로는 JSON 라이브러리 사용 권장)
        /// </summary>
        private void ParseJsonManually(string jsonContent)
        {
            // 간단한 예시 - 실제로는 더 복잡한 파싱 필요
            // Unity의 JsonUtility 한계로 인해 실제 프로젝트에서는 
            // Newtonsoft.Json 같은 외부 라이브러리 사용을 권장합니다

            // 임시로 하드코딩된 데이터로 테스트
            BuildTestCache();
        }


        /// <summary>
        /// 테스트용 캐시 구성
        /// </summary>
        private void BuildTestCache()
        {
            // 근접 무기 예시
            meleeWeapons["Scythe"] = new List<WeaponStats>();
            meleeWeapons["Staff"] = new List<WeaponStats>();
            meleeWeapons["Spear"] = new List<WeaponStats>();
            meleeWeapons["Greatsword"] = new List<WeaponStats>();
            meleeWeapons["Hammer"] = new List<WeaponStats>();
            meleeWeapons["Whip"] = new List<WeaponStats>();

            // 원거리 무기 예시
            rangedWeapons["Boomerang"] = new List<WeaponStats>();
            rangedWeapons["Fireball"] = new List<WeaponStats>();
            rangedWeapons["Chakram"] = new List<WeaponStats>();
            rangedWeapons["PoisonFlask"] = new List<WeaponStats>();
            rangedWeapons["Laser"] = new List<WeaponStats>();

            // 범위 무기 예시
            splashWeapons["KillingAura"] = new List<WeaponStats>();
            splashWeapons["Icicle"] = new List<WeaponStats>();
            splashWeapons["LightningStaff"] = new List<WeaponStats>();
            splashWeapons["Asteroid"] = new List<WeaponStats>();

            // 전체 캐시에 추가
            foreach (var kvp in meleeWeapons)
                allWeaponsCache[kvp.Key] = kvp.Value;
            foreach (var kvp in rangedWeapons)
                allWeaponsCache[kvp.Key] = kvp.Value;
            foreach (var kvp in splashWeapons)
                allWeaponsCache[kvp.Key] = kvp.Value;
        }

        #region 공통 메서드

        /// <summary>
        /// 무기 데이터 가져오기 (타입 무관)
        /// </summary>
        public WeaponStats GetWeaponData(string weaponName, int level)
        {
            if (allWeaponsCache.ContainsKey(weaponName))
            {
                var weaponLevels = allWeaponsCache[weaponName];
                if (level > 0 && level <= weaponLevels.Count)
                {
                    return weaponLevels[level - 1];
                }
            }

            Debug.LogWarning($"무기 데이터를 찾을 수 없습니다: {weaponName} Lv.{level}");
            return null;
        }

        /// <summary>
        /// 무기의 타입 확인
        /// </summary>
        public WeaponType GetWeaponType(string weaponName)
        {
            if (meleeWeapons.ContainsKey(weaponName))
                return WeaponType.Melee;
            else if (rangedWeapons.ContainsKey(weaponName))
                return WeaponType.Ranged;
            else if (splashWeapons.ContainsKey(weaponName))
                return WeaponType.Splash;

            return WeaponType.None;
        }

        /// <summary>
        /// 최대 레벨 가져오기
        /// </summary>
        public int GetMaxLevel(string weaponName)
        {
            if (allWeaponsCache.ContainsKey(weaponName))
            {
                return allWeaponsCache[weaponName].Count;
            }
            return 0;
        }

        /// <summary>
        /// 레벨업 가능 여부 확인
        /// </summary>
        public bool CanLevelUp(string weaponName, int currentLevel)
        {
            return currentLevel < GetMaxLevel(weaponName);
        }

        #endregion

        #region 근접 무기 전용 메서드

        /// <summary>
        /// 근접 무기 데이터 가져오기
        /// </summary>
        public WeaponStats GetMeleeWeaponData(string weaponName, int level)
        {
            if (meleeWeapons.ContainsKey(weaponName))
            {
                var weaponLevels = meleeWeapons[weaponName];
                if (level > 0 && level <= weaponLevels.Count)
                {
                    return weaponLevels[level - 1];
                }
            }
            return null;
        }

        /// <summary>
        /// 모든 근접 무기 이름 가져오기
        /// </summary>
        public List<string> GetAllMeleeWeaponNames()
        {
            return new List<string>(meleeWeapons.Keys);
        }

        /// <summary>
        /// 근접 무기의 공격 타입 가져오기
        /// </summary>
        public MeleeWeapon.AttackType GetMeleeAttackType(string weaponName)
        {
            switch (weaponName.ToLower())
            {
                case "scythe":
                case "greatsword":
                    return MeleeWeapon.AttackType.Sweep;
                case "spear":
                case "staff":
                    return MeleeWeapon.AttackType.Thrust;
                case "hammer":
                    return MeleeWeapon.AttackType.Slam;
                case "whip":
                    return MeleeWeapon.AttackType.Whip;
                default:
                    return MeleeWeapon.AttackType.Sweep;
            }
        }

        #endregion

        #region 원거리 무기 전용 메서드

        /// <summary>
        /// 원거리 무기 데이터 가져오기
        /// </summary>
        public WeaponStats GetRangedWeaponData(string weaponName, int level)
        {
            if (rangedWeapons.ContainsKey(weaponName))
            {
                var weaponLevels = rangedWeapons[weaponName];
                if (level > 0 && level <= weaponLevels.Count)
                {
                    return weaponLevels[level - 1];
                }
            }
            return null;
        }

        /// <summary>
        /// 모든 원거리 무기 이름 가져오기
        /// </summary>
        public List<string> GetAllRangedWeaponNames()
        {
            return new List<string>(rangedWeapons.Keys);
        }

        #endregion

        #region 범위 무기 전용 메서드

        /// <summary>
        /// 범위 무기 데이터 가져오기
        /// </summary>
        public WeaponStats GetSplashWeaponData(string weaponName, int level)
        {
            if (splashWeapons.ContainsKey(weaponName))
            {
                var weaponLevels = splashWeapons[weaponName];
                if (level > 0 && level <= weaponLevels.Count)
                {
                    return weaponLevels[level - 1];
                }
            }
            return null;
        }

        /// <summary>
        /// 모든 범위 무기 이름 가져오기
        /// </summary>
        public List<string> GetAllSplashWeaponNames()
        {
            return new List<string>(splashWeapons.Keys);
        }

        #endregion

        /// <summary>
        /// 무기 타입 열거형
        /// </summary>
        public enum WeaponType
        {
            None,
            Melee,
            Ranged,
            Splash
        }
    }
}*/