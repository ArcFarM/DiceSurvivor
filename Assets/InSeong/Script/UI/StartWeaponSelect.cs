using UnityEngine;
using UnityEngine.UI;
using DiceSurvivor.Manager;

namespace DiceSurvivor.UI
{
    public class StartWeaponPanel : MonoBehaviour
    {
        //시작 근접무기 설정 이벤트에 사용될 무기 선택 패널
        #region Variables
        [SerializeField] Button selectButton;
        [SerializeField] Image weaponImage;
        [SerializeField] TestItem weapon;
        //TODO : weapon의 정보를 부착할 공간들 변수 추가하기
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void Start()
        {
            //무기 정보를 받아서 UI에 표시
            selectButton.onClick.AddListener(onButtonClicked);
        }
        #endregion

        #region Custom Methods
        public void SetRandomWeapon(TestItem item)
        {
            weapon = item;
            weaponImage.sprite = weapon.itemImage;
        }
        public void onButtonClicked()
        {
            //담겨 있는 무기 정보를 아이템 매니저로 넘겨서 플레이어 근접 무기로 할당
            ItemManager.Instance.BuyItem(weapon);
        }
        #endregion
    }
}