using UnityEngine;

/// <summary>
/// 상점 테스트용 임시 아이템.
/// </summary>
[CreateAssetMenu(fileName = "TestItem", menuName = "Scriptable Objects/TestItem")]
public class TestItem : ScriptableObject
{
    public enum ItemType {
        MeleeWeapon,
        SubWeapon,
        Passive
    }
    //임시 아이템의 각 속성들
    public string ID;
    public string itemName;
    [TextArea(5, 50)]
    public string description;
    public ItemType type;
    //아이템 최대 레벨
    public int maxLevel = 8;
    //아이템 구매 가능 여부
    public bool canIBuy = true;
    //아이템 이미지
    public Sprite itemImage;

}
