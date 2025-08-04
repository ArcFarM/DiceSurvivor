using System.Collections.Generic;
using UnityEngine;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    ///  각 무기의 이름 데이터베이스
    /// </summary>
    [System.Serializable]
    public class WeaponDatabase
    {
        public List<WeaponData> KillingAura;
        public List<WeaponData> Icicle;
        public List<WeaponData> LightningStaff;
        public List<WeaponData> Asteroid;
        public List<WeaponData> OrbitBlade;
        //public WeaponLevelData HolyCircle;
        // 추가 무기들...
    }
}