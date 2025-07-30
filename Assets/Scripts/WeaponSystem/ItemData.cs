using DiceSurvivorB.Common;
using UnityEngine;

namespace DiceSurvivorB.ItemSystem
{
    /// <summary>
    /// 아이템 기본 내용 주석
    /// 내용: 이름, 아이디, 아이콘, 가격, 진화 조건
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("공통 속성")]
        public string id;                     // 아이템 아이디
        public string itemName;               // 아이템 이름
        public Sprite icon;                   // 인벤토리용 아이콘
        public GameObject modelPrefab;        // 월드에 배치하거나 장착할 prefab 모델
        public ItemType type;                 // 무기혹은 패시브

        [Header("상점 정보")]
        public int buyPrice;                  // 구매 가격
        public int sellPrice;                 // 판매 가격
        public bool isCanBuy;                 // 구매 가능 조건

        [TextArea(10, 10)]
        public string description;            // 툴팁에 들어갈 설명문
    }
}