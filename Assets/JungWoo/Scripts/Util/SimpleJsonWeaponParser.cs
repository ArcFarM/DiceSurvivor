/*using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    /// JSON 파싱을 위한 도우미 클래스
    /// </summary>
    public static class SimpleJsonWeaponParser
    {
        /// <summary>
        /// JSON 문자열에서 특정 무기의 데이터를 추출
        /// </summary>
        public static List<UnifiedWeaponDataManager.WeaponStats> ExtractWeaponData(string jsonContent, string weaponName)
        {
            List<UnifiedWeaponDataManager.WeaponStats> weaponList = new List<UnifiedWeaponDataManager.WeaponStats>();

            try
            {
                // 무기 이름으로 해당 섹션 찾기
                string pattern = $"\"{weaponName}\"\\s*:\\s*\\[(.*?)\\]";
                Match match = Regex.Match(jsonContent, pattern, RegexOptions.Singleline);

                if (match.Success)
                {
                    string weaponArrayContent = match.Groups[1].Value;

                    // 각 레벨의 데이터 추출
                    string levelPattern = @"\{([^{}]*)\}";
                    MatchCollection levelMatches = Regex.Matches(weaponArrayContent, levelPattern);

                    foreach (Match levelMatch in levelMatches)
                    {
                        string levelData = "{" + levelMatch.Groups[1].Value + "}";

                        // 개별 무기 스탯 파싱
                        var stats = ParseSingleWeaponStats(levelData, weaponName);
                        if (stats != null)
                        {
                            weaponList.Add(stats);
                        }
                    }
                }

                Debug.Log($"{weaponName} 무기 데이터 추출 완료: {weaponList.Count}개 레벨");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{weaponName} 파싱 오류: {e.Message}");
            }

            return weaponList;
        }

        /// <summary>
        /// 단일 무기 스탯 파싱
        /// </summary>
        private static UnifiedWeaponDataManager.WeaponStats ParseSingleWeaponStats(string jsonData, string weaponName)
        {
            try
            {
                var stats = new UnifiedWeaponDataManager.WeaponStats();

                // 각 필드 추출
                stats.id = ExtractStringValue(jsonData, "id");
                stats.name = ExtractStringValue(jsonData, "name");
                stats.type = ExtractStringValue(jsonData, "type");
                stats.level = ExtractIntValue(jsonData, "level");
                stats.damage = ExtractFloatValue(jsonData, "damage");
                stats.cooldown = ExtractFloatValue(jsonData, "cooldown");
                stats.radius = ExtractFloatValue(jsonData, "radius");
                stats.range = ExtractFloatValue(jsonData, "range");
                stats.projectileSize = ExtractFloatValue(jsonData, "projectileSize");
                stats.projectileSpeed = ExtractFloatValue(jsonData, "projectileSpeed");
                stats.projectileCount = ExtractIntValue(jsonData, "projectileCount");
                stats.explosionRadius = ExtractFloatValue(jsonData, "explosionRadius");
                stats.explosionDamage = ExtractFloatValue(jsonData, "explosionDamage");
                stats.dotDamage = ExtractFloatValue(jsonData, "dotDamage");
                stats.duration = ExtractFloatValue(jsonData, "duration");
                stats.isPiercing = ExtractBoolValue(jsonData, "isPiercing");
                stats.canReturn = ExtractBoolValue(jsonData, "canReturn");
                stats.description = ExtractStringValue(jsonData, "description");

                return stats;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 문자열 값 추출
        /// </summary>
        private static string ExtractStringValue(string json, string key)
        {
            string pattern = $"\"{key}\"\\s*:\\s*\"([^\"]+)\"";
            Match match = Regex.Match(json, pattern);
            return match.Success ? match.Groups[1].Value : "";
        }

        /// <summary>
        /// 정수 값 추출
        /// </summary>
        private static int ExtractIntValue(string json, string key)
        {
            string pattern = $"\"{key}\"\\s*:\\s*([0-9]+)";
            Match match = Regex.Match(json, pattern);
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        /// <summary>
        /// 실수 값 추출
        /// </summary>
        private static float ExtractFloatValue(string json, string key)
        {
            string pattern = $"\"{key}\"\\s*:\\s*([0-9.]+)";
            Match match = Regex.Match(json, pattern);
            return match.Success ? float.Parse(match.Groups[1].Value) : 0f;
        }

        /// <summary>
        /// 불린 값 추출
        /// </summary>
        private static bool ExtractBoolValue(string json, string key)
        {
            string pattern = $"\"{key}\"\\s*:\\s*(true|false)";
            Match match = Regex.Match(json, pattern);
            return match.Success ? bool.Parse(match.Groups[1].Value) : false;
        }

        /// <summary>
        /// 모든 근접 무기 이름 목록
        /// </summary>
        public static List<string> GetMeleeWeaponNames()
        {
            return new List<string> { "Scythe", "Staff", "Spear", "Greatsword", "Hammer", "Whip" };
        }

        /// <summary>
        /// 모든 원거리 무기 이름 목록
        /// </summary>
        public static List<string> GetRangedWeaponNames()
        {
            return new List<string> { "Boomerang", "Fireball", "Chakram", "PoisonFlask", "Laser" };
        }

        /// <summary>
        /// 모든 범위 무기 이름 목록
        /// </summary>
        public static List<string> GetSplashWeaponNames()
        {
            return new List<string> { "KillingAura", "Icicle", "LightningStaff", "Asteroid" };
        }
    }
}*/