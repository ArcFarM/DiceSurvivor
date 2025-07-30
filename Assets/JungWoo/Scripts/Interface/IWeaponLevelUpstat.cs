using UnityEngine;


namespace DiceSurvivor.WeaponSystem
{
    /// <summary>
    /// 공통 효과 인터페이스
    /// </summary>
    public interface IWeaponLevelUpstat
    {
        int Level { get; set; }
        void ApplyLevelData(LevelData data);
    }
}