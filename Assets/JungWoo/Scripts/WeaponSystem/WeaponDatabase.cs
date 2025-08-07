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
        [Header("근접 무기")]
        public List<WeaponData> Scythe = new List<WeaponData>();
        public List<WeaponData> Staff = new List<WeaponData>();
        public List<WeaponData> Spear = new List<WeaponData>();
        public List<WeaponData> Greatsword = new List<WeaponData>();
        public List<WeaponData> Hammer = new List<WeaponData>();
        public List<WeaponData> Whip = new List<WeaponData>();

        [Header("원거리 무기")]
        public List<WeaponData> Boomerang = new List<WeaponData>();
        public List<WeaponData> Fireball = new List<WeaponData>();
        public List<WeaponData> Chakram = new List<WeaponData>();
        public List<WeaponData> PoisonFlask = new List<WeaponData>();
        public List<WeaponData> Laser = new List<WeaponData>();

        [Header("범위 무기")]
        public List<WeaponData> KillingAura = new List<WeaponData>();
        public List<WeaponData> Icicle = new List<WeaponData>();
        public List<WeaponData> LightningStaff = new List<WeaponData>();
        public List<WeaponData> Asteroid = new List<WeaponData>();

        public List<WeaponData> OrbitBlade = new List<WeaponData>();

        

        //public WeaponLevelData HolyCircle;
        // 추가 무기들...
    }
}