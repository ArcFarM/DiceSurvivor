using UnityEngine;

namespace DiceSurvivor.WeaponSystem
{
    /// <summary>
    /// 무기 효과 ScriptableObject 
    /// </summary>
    public abstract class EffectSOBase : ScriptableObject
    {
        public abstract void ApplyEffect(int level, GameObject owner, LevelData data, Vector3 mouseDirection);

        // WeaponSkillSO.effect를 EffectSOBase로 선언
        public EffectSOBase effect;
    }
}