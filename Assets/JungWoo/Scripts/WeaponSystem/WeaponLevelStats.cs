//using System.Collections.Generic;
//using UnityEngine;


//namespace DiceSurvivor.WeaponSystem
//{
//    /// <summary>
//    /// 무기 test1 
//    /// 
//    /// </summary>
//    public class WeaponBehaviour : MonoBehaviour, IWeaponLevelUpstat
//    {
//        #region Variables
        
//        public float damage;                //데미지  
//        public float cooldown;              //공격 간격
//        public int projectileCount;         //투사체 갯수
//        public float projectileSpeed;       //투사체 속도
//        public float attackRange;           //투사체 반경
//        public EffectData effects;          //투사체 이펙트
//        #endregion

//        #region Property

//        public int Level { get; set; } = 1;
//        #endregion

//        public void ApplyLevelData(LevelData data)
//        {
//            if (data == null)
//                return;

//            this.Level = data.level;
//            this.damage = data.damage;
//            this.cooldown = data.cooldown;
//            this.projectileSpeed = data.projectileSpeed;
//            this.effects = data.effects;

//        }
//    }
//}