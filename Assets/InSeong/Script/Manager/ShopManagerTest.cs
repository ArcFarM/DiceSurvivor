using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DiceSurvivor.Player;
using TMPro;

namespace DiceSurvivor.Manager {

    public class ShopManagerTest : SingletonManager<ShopManagerTest> {
        #region Variables
        //아이템 정보가 담긴 배열과 아이템 칸이 담긴 배열
        public TestItemArray testItems;
        public ItemSlot[] itemSlots;

        //현재 아이템 레벨 정보가 담긴 딕셔너리
        Dictionary<string, int> currItemLevelDict;

        //아이템 삭제 가능 횟수
        [SerializeField] int vanishChance = 5;
        //이번 상점에서 새로고침을 한 횟수 (구매 시 초기화)
        int consecutiveRerollCount = 0;

        //아이템의 가격
        Dictionary<TestItem.ItemType, int> itemCosts;

        //아이템 정보가 담긴 싱글톤
        ItemManager im => ItemManager.Instance;

        //상점 잠금 여부
        bool isShopLocked = false;

        //플레이어 참조
        PlayerTest player;

        //현재 보유 금액
        [SerializeField]
        int currentGold = 0;
        #region UIElements
        public Button rerollButton;
        public Button lockButton;
        public Button vanishButton;
        public TextMeshProUGUI goldText;
        #endregion
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        protected override void Awake() {
            base.Awake();
            //플레이어는 게임 전체에서 단 하나이므로 findfirstobjectbytype 사용
            player = FindFirstObjectByType<PlayerTest>().GetComponent<PlayerTest>();
            //딕셔너리 초기화
            currItemLevelDict = new Dictionary<string, int>();
            itemCosts = new Dictionary<TestItem.ItemType, int> {
                { TestItem.ItemType.MeleeWeapon, 10 },
                { TestItem.ItemType.SubWeapon, 5 },
                { TestItem.ItemType.Passive, 3 }
            };
            RefreshGold();
            //json파일을 통해 무기 설명문 데이터를 딕셔너리에 저장
            /*
             jsonFile json;
            Dictionary<string, string[]> weapondetails = new Dictionary<string, string[]>();
            >> json 안에 있는 정보를 string(weaponID)별로 따와서 LIst<string> 혹은 string[maxLEvel] 형태로 저장
            */
        }

        private void OnEnable() {
            FillItem();
            RefreshGold();
        }

        
        #endregion

        #region Custom Methods
        //구매 버튼 눌렀을 때 실행
        public void BuyItem(ItemSlot slot) {
            //해당 칸 아이템 정보를 플레이어 한테 넘김
            TestItem itemToBuy = slot.currentItem;
            if (itemToBuy == null) return;

            // 1. 금액 확인
            int itemCost = 0;
            if (itemCosts.TryGetValue(itemToBuy.type, out int cost)) {
                itemCost = cost;
            } else Debug.LogError("해당 아이템 구매 시도 시 오류 발생 : " + itemToBuy);

            if (currentGold < itemCost) return;

            // 2. 해당 아이템을 보유하고 있는 지 확인
            bool hasItem = im.GetItemLevel(itemToBuy.itemName) > 0;

            // 3-1. 보유한 아이템이라면 최대 레벨 이상인 지 확인
            if (hasItem) {
                if(im.GetItemLevel(itemToBuy.itemName) >= itemToBuy.maxLevel) {
                    VanishMaxLevelItem(itemToBuy);
                    return;
                }
            } else {
                //3-2. 보유하지 않은 아이템이라면 남은 자리가 있는 지 확인
                if (im.GetCurrentItemCount(itemToBuy.type) + 1 >= im.GetMaxItemCount(itemToBuy.type)) {
                    return;
                }
            }

            // 4. 구매 처리
            currentGold -= itemCost;
            im.BuyItem(itemToBuy);
            RefreshGold();

            //5. 아이템 칸 정보 갱신
            ClearItem(slot);
        }

        //상점 열림 또는 새로고침 버튼 누르면 실행
        void FillItem() {
            if (isShopLocked) return;
            //현재 상점에 있는 아이템 정보를 전부 지우고 새로 채움
            foreach(ItemSlot slot in itemSlots) {
                ClearItem(slot);
            }
            foreach(ItemSlot slot in itemSlots) {
                AddItem(slot);
            }
        }

        void AddItem(ItemSlot slot) {
            //아이템 칸에 무작위로 구매 가능한 아이템을 채움
            int randomIndex = Random.Range(0, testItems.items.Length);
            TestItem randomItem = testItems.items[randomIndex];

            while (!randomItem.canIBuy) {
                randomIndex = Random.Range(0, testItems.items.Length);
                randomItem = testItems.items[randomIndex];
            }

            slot.currentItem = randomItem;
            slot.changeInfo();
        }

        //새로고침 했을 때 상점 아이템을 새로 채우기
        void RerollShop() {
            //잠금 여부와 상관 없이 아이템을 새로 채운다
            if(isShopLocked) ToggleLockShop();
            FillItem();
        }


        //상점 잠금 및 잠금 해제
        void ToggleLockShop() {
            if (isShopLocked) {
                isShopLocked = false;
            }
            else {
                isShopLocked = true;
            }
        }

        //아이템 영구 삭제 (게임당 가능한 횟수 정해져 있음)
        void VanishItem(ItemSlot slot) {
            if(vanishChance <= 0) {
                return;
            }
            else {
                vanishChance--;
                slot.currentItem.canIBuy = false;
                ClearItem(slot);
            }

            if(vanishChance <= 0) {
                //버튼 비활성화
                //vanishButton.interactable = false;
            }
        }

        //플레이어 아이템 레벨 정보 업데이트 (PlayerTest에서 호출)
        public void UpdatePlayerItemLevel(string itemName, int level) {
            if (currItemLevelDict.ContainsKey(itemName)) {
                currItemLevelDict[itemName] = level;
            }
            else {
                currItemLevelDict.Add(itemName, level);
            }
        }

        //최대 레벨에 도달한 아이템 삭제
        void VanishMaxLevelItem(TestItem item) {
            string itemName = item.itemName;
            int maxLevel = item.maxLevel;

            if (currItemLevelDict.TryGetValue(itemName, out int value)) {
                //아이템 레벨이 최대 레벨에 도달했을 경우 더 이상 상점에 안나오게 만들기
                if (value >= maxLevel) {
                    item.canIBuy = false;
                }
                else return;
            }
            else return;
        }

        //진화 조건을 만족했을 경우 진화 아이템 상점 풀에 추가하기
        void AddEvolvedWeapon() {
            //구매 시 플레이어 아이템 칸과 상호작용하여 진화 무기로 교체
        }
        //아이템 칸 정보 지우기
        void ClearItem(ItemSlot slot) {
            slot.currentItem = null;
            slot.changeInfo();
        }

        //플레이어 무기 칸이 최대치에 도달한 경우 갖고 있지 않은 아이템은 모두 삭제
        void ClearAllUnused() {

        }

        //골드 정보 갱신
        void RefreshGold() {
            //UI에 표시
            if (goldText != null) {
                goldText.text = currentGold.ToString();
            }
        }
        #endregion
    }
}
