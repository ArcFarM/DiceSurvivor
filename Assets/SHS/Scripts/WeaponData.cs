using DiceSurvivor.SHS;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Weapon")]
public class WeaponData : ScriptableObject
{
    /// <summary>
    /// Weapon에 정보를 설정할 수 있는 Data
    /// </summary>
    #region Variables
    public Weapon weapon;               //무기의 정보가 들어있는 클래스

    public SkillType skillType;         //스킬 타입 Weapon, Passive
    public int evolutionTargetId;       //진화 대상 무기 Id
    #endregion

    #region Custom Method

    #endregion
}
