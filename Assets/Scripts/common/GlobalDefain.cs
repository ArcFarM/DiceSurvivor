using UnityEngine;

namespace DiceSurvivorB.Common
{
    //����Ʈ ���� enum�� ����
    public enum EffectType
    {
        None = -1,
        NORMAL,
    }

    //���� ���� enum�� ����
    public enum SoundType
    {
        None = -1,
        MUSIC,
        SFX,
    }

    //ĳ���� �Ӽ� enum�� ����
    public enum CharacterAttibute
    {
        Agility,            //��ø
        Strength,           //��
        Health,             //������
        Exp,                //����ġ
    }

    //������ Ÿ�� enum�� ����
    public enum ItemType
    {
        None = -1,
        rangedWeapon = 0,   //���Ÿ� ����
        meleeWeapon = 1,    //�ٰŸ� ����
        wideRangeWeapon = 2,//���� ����
        passiveItem = 3,    //�м��� ������
        Food,               //����
        Default,
    }

    //�κ��丮 Ÿ�� 
    public enum InventoryType
    {
        Inventory,              //â����
        Equipment,              //������
    }


}
