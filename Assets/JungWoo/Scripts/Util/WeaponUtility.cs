using UnityEngine;

/// <summary>
/// 무기 관련 유틸리티 클래스
/// </summary>
namespace DiceSurvivor.WeaponDataSystem
{
    public static class WeaponUtility
    {
        /// <summary>
        /// 무기 타입별 설명을 반환
        /// </summary>
        public static string GetWeaponTypeDescription(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Melee:
                    return "근접전에 특화된 무기입니다.";
                case WeaponType.Projectile:
                    return "원거리 공격이 가능한 무기입니다.";
                case WeaponType.Orbit:
                    return "플레이어 주변을 공전하는 무기입니다.";
                case WeaponType.DonutArea:
                    return "플레이어 주변에 지속적인 피해를 주는 무기입니다.";
                default:
                    return "알 수 없는 무기 타입입니다.";
            }
        }

        /// <summary>
        /// 데미지 계산 (크리티컬, 버프 등 적용)
        /// </summary>
        public static float CalculateFinalDamage(float baseDamage, bool isCritical = false, float damageMultiplier = 1f)
        {
            float finalDamage = baseDamage * damageMultiplier;

            if (isCritical)
            {
                finalDamage *= 2f; // 크리티컬 데미지 2배
            }

            return finalDamage;
        }

        /// <summary>
        /// 크리티컬 확률 계산
        /// </summary>
        public static bool CalculateCritical(float criticalChance = 0.1f)
        {
            return UnityEngine.Random.Range(0f, 1f) < criticalChance;
        }

        /// <summary>
        /// 레벨에 따른 데미지 스케일링
        /// </summary>
        public static float GetDamageScaling(int level, float baseScaling = 1.1f)
        {
            return Mathf.Pow(baseScaling, level - 1);
        }

        /// <summary>
        /// 거리에 따른 데미지 감소 계산
        /// </summary>
        public static float CalculateDistanceDamage(float baseDamage, float distance, float maxRange, float minDamagePercent = 0.5f)
        {
            if (distance >= maxRange) return 0f;

            float damagePercent = Mathf.Lerp(1f, minDamagePercent, distance / maxRange);
            return baseDamage * damagePercent;
        }

        /// <summary>
        /// 무기 이름을 표시용 이름으로 변환
        /// </summary>
        public static string GetDisplayName(string weaponName)
        {
            switch (weaponName.ToLower())
            {
                case "scythe": return "낫";
                case "staff": return "지팡이";
                case "spear": return "창";
                case "greatsword": return "대검";
                case "hammer": return "해머";
                case "whip": return "채찍";
                case "boomerang": return "부메랑";
                case "fireball": return "파이어볼";
                case "chakram": return "차크람";
                case "poisonflask": return "독 플라스크";
                case "laser": return "레이저";
                case "killingaura": return "살상 오라";
                case "icicle": return "고드름";
                case "lightningstaff": return "번개 지팡이";
                case "asteroid": return "운석";
                case "orbitblade": return "공전 칼날";
                default: return weaponName;
            }
        }

        /// <summary>
        /// 무기 등급 계산 (레벨 기반)
        /// </summary>
        public static WeaponGrade GetWeaponGrade(int level)
        {
            if (level <= 2) return WeaponGrade.Common;
            if (level <= 4) return WeaponGrade.Uncommon;
            if (level <= 6) return WeaponGrade.Rare;
            return WeaponGrade.Epic;
        }

        /// <summary>
        /// 등급별 색상 반환
        /// </summary>
        public static UnityEngine.Color GetGradeColor(WeaponGrade grade)
        {
            switch (grade)
            {
                case WeaponGrade.Common: return UnityEngine.Color.white;
                case WeaponGrade.Uncommon: return UnityEngine.Color.green;
                case WeaponGrade.Rare: return UnityEngine.Color.blue;
                case WeaponGrade.Epic: return UnityEngine.Color.magenta;
                case WeaponGrade.Legendary: return UnityEngine.Color.yellow;
                default: return UnityEngine.Color.gray;
            }
        }
    }

    /// <summary>
    /// 무기 등급 열거형
    /// </summary>
    public enum WeaponGrade
    {
        Common,     // 일반
        Uncommon,   // 고급
        Rare,       // 희귀
        Epic,       // 영웅
        Legendary   // 전설
    }

    /// <summary>
    /// 무기 상태 정보 클래스
    /// </summary>
    [System.Serializable]
    public class WeaponStatus
    {
        public string weaponName;
        public int currentLevel;
        public int maxLevel;
        public float experience;
        public float requiredExperience;
        public WeaponGrade grade;
        public bool isUnlocked;
        public bool isEquipped;

        public WeaponStatus(string name, int maxLvl)
        {
            weaponName = name;
            currentLevel = 1;
            maxLevel = maxLvl;
            experience = 0f;
            requiredExperience = 100f;
            grade = WeaponGrade.Common;
            isUnlocked = false;
            isEquipped = false;
        }

        /// <summary>
        /// 경험치 추가 및 레벨업 처리
        /// </summary>
        public bool AddExperience(float exp)
        {
            if (currentLevel >= maxLevel) return false;

            experience += exp;

            if (experience >= requiredExperience)
            {
                LevelUp();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 레벨업 처리
        /// </summary>
        private void LevelUp()
        {
            if (currentLevel >= maxLevel) return;

            currentLevel++;
            experience = 0f;
            requiredExperience = Mathf.RoundToInt(requiredExperience * 1.5f); // 다음 레벨 필요 경험치 증가
            grade = WeaponUtility.GetWeaponGrade(currentLevel);
        }

        /// <summary>
        /// 진행률 계산 (0~1)
        /// </summary>
        public float GetProgressPercent()
        {
            if (currentLevel >= maxLevel) return 1f;
            return experience / requiredExperience;
        }
    }
}