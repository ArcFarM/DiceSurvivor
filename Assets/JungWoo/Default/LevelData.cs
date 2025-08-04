using System.Collections.Generic;
using UnityEngine;

namespace DiceSurvivor.WeaponSystem
{
    /// <summary>
    /// 레벨 상승때의 데이터 속성을 관리하는 클래스
    /// 속성: 레벨, 데미지, 공격 간격, 투사체 속도, 외 이펙트등...
    /// </summary>
    [System.Serializable]
    public class LevelData
    {
        public float damage;                //데미지  
        public float cooldown;              //공격 간격
        public float range;                 // 사거리
        public float projectileSpeed;       //투사체 속도

        public EffectData effects;    //투사체 이펙트
    }
}