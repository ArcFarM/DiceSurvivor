using UnityEngine;

namespace DiceSurvivor.Common
{
    //이펙트 종류 enum값 정의
    public enum EffectType
    {
        None = -1,
        NORMAL,
    }

    //사운드 종류 enum값 정의
    public enum SoundType
    {
        None = -1,
        MUSIC,
        SFX,
    }

    //캐릭터 속성 enum값 정의
    public enum CharacterAttibute
    {
        Agility,            //민첩
        Strength,           //힘
        Health,             //생병력
        Exp,                //경험치
    }

    //아이템 타입 enum값 정의
    public enum ItemType
    {
        None = -1,
        rangedWeapon = 0,   //원거리 무기
        meleeWeapon = 1,    //근거리 무기
        wideRangeWeapon = 2,//광역 무기
        passiveItem = 3,    //패세브 아이템
        Food,               //물약
        Default,
    }

    //인벤토리 타입 
    public enum InventoryType
    {
        Inventory,              //창고형
        Equipment,              //장착형
    }


}
