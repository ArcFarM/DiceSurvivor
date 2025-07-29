using DiceSurvivor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Weapon")]
public class WeaponData : ScriptableObject
{
    /// <summary>
    /// Weapon�� ������ ������ �� �ִ� Data
    /// </summary>
    #region Variables
    public Weapon weapon;               //������ ������ ����ִ� Ŭ����

    public SkillType skillType;         //��ų Ÿ�� Weapon, Passive
    public int evolutionTargetId;       //��ȭ ��� ���� Id
    #endregion

    #region Custom Method

    #endregion
}
