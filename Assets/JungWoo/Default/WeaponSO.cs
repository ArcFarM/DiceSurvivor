using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.ItemSystem;

namespace DiceSurvivor.WeaponSystem
{
    /// <summary>
    /// 무기 데이터를 관리하는 클래스
    /// 속성: 무기 대미지, 곡격 간격, 사거리, 외 이팩트등
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "Game/WeaponSkill")]
    public class WeaponSO : ItemData
    {
        [Header("기본 공격 속성")]
        public float damage;                // 공격력
        public float cooldown;              // 공격 간격 (초 단위)
        public float range;                 // 사거리
        public float projectileSpeed;       // 투사체 속도
        public EffectSOBase effect;      //효과 ScriptableObject

        [Header("이펙트")]
        public List<EffectData> effects; // 무기에 부착된 효과 목록
    }
}