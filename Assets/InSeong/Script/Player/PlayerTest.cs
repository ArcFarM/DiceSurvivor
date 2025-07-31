using UnityEngine;
using System.Collections.Generic;

namespace DiceSurvivor.Player {
    public class PlayerTest : MonoBehaviour {
        #region Variables
        //플레이어의 체력
        [SerializeField] private int health = 100;
        //플레이어가 소지한 근접 무기
        [SerializeField] TestItem meleeWeapon;
        //플레이어가 소지한 보조 무기
        [SerializeField] List<TestItem> subWeapons = new List<TestItem>();
        //플레이어가 소지한 패시브 스킬
        [SerializeField] List<TestItem> passives = new List<TestItem>();


        //플레이어가 소지한 아이템의 레벨 정보와 최대치 정보
        public Dictionary<string, int> itemLevelDict = new Dictionary<string, int>();

        //플레이어가 최대로 가질 수 있는 무기와 스킬의 수
        [SerializeField] private int maxWeapons = 1;
        [SerializeField] private int maxSubWeapons = 3;
        [SerializeField] private int maxPassives = 3;

        public Dictionary<TestItem.ItemType, int> itemCountDict;
        #endregion

        #region Properties
        //플레이어의 체력을 반환하는 프로퍼티
        public int Health {
            get { return health; }
            set { health = Mathf.Max(0, value); }
        }
        //플레이어가 소지한 근접 무기를 반환하는 프로퍼티
        public TestItem MeleeWeapon {
            get { return meleeWeapon; }
            set { meleeWeapon = value; }
        }
        //플레이어가 소지한 보조 무기를 반환하는 프로퍼티
        public List<TestItem> SubWeapons {
            get { return subWeapons; }
            set { subWeapons = value; }
        }
        //플레이어가 소지한 패시브 스킬을 반환하는 프로퍼티
        public List<TestItem> Passives {
            get { return passives; }
            set { passives = value; }
        }

        //플레이어가 최대로 가질 수 있는 무기와 스킬의 수를 반환하는 메서드지만 프로퍼티처럼 사용
        public int GetMaxCount(TestItem.ItemType itemType) {
            return itemCountDict.TryGetValue(itemType, out int count) ? count : 0;
        }
        public int GetCurrCount(TestItem.ItemType itemType) {
            switch(itemType) {
                case TestItem.ItemType.MeleeWeapon:
                    return meleeWeapon != null ? 1 : 0;
                case TestItem.ItemType.SubWeapon:
                    return subWeapons.Count;
                case TestItem.ItemType.Passive:
                    return passives.Count;
                default:
                    return 0;
            }
        }
        #endregion

        #region Unity Event Methods
        private void Awake() {
            itemCountDict = new Dictionary<TestItem.ItemType, int>() {
                { TestItem.ItemType.MeleeWeapon, maxWeapons},
                { TestItem.ItemType.SubWeapon, maxSubWeapons },
                { TestItem.ItemType.Passive, maxPassives }
            };
            SyncItem();
        }
        #endregion

        #region Custom Methods
        //아이템 구매/진화 시 상점 상태와 동기화
        public void SyncItem() {
            //인스펙터에서 할당된 아이템 정보를 샵에 넘겨줌

        }
        #endregion
    }

}
