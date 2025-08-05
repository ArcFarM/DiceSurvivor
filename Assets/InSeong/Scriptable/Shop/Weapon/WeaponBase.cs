using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBase", menuName = "Scriptable Objects/WeaponBase")]
public class WeaponBase : ScriptableObject
{
    /// <summary>
    /// 상점에 사용할 아이템들의 기본 정보
    /// 

    //참조할 무기 데이터
    DiceSurvivor.WeaponDataSystem.WeaponData weaponData;
    //구매 가능 여부
    bool canIBuy = true;
    //최대 레벨
    int maxLevel = 8;
    //아이템 이미지
    public Sprite itemImage;

}
