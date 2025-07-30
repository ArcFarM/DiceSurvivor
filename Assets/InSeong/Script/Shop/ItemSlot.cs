using DiceSurvivor.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    #region Variables
    //임시 아이템용 스크립트
    //해당 칸에 들어갈 아이템 정보
    public TestItem currentItem;
    //아이템 정보를 보여줄 UI 요소들
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText;
    public Image itemImageFrame;
    #endregion

    #region Properties
    #endregion

    #region Unity Event Methods
    private void Start() {
        changeInfo();
        if(ShopManagerTest.Instance != null) {
            GetComponent<Button>().onClick.AddListener(
                () => ShopManagerTest.Instance.BuyItem(this)
            );
        }
    }
    #endregion

    #region Custom Methods
    //아이템 칸의 내용물을 저장된 아이템 정보로 변경
    public void changeInfo() {
        if (currentItem != null) {
            itemNameText.text = currentItem.itemName;
            itemTypeText.text = currentItem.type.ToString();
            itemImageFrame.sprite = currentItem.itemImage;
            GetComponent<Button>().interactable = currentItem.canIBuy; //아이템 구매 가능 여부에 따라 버튼 활성화
        }
        else { //칸을 비워야 하는 경우
            itemNameText.text = "";
            itemTypeText.text = "";
            itemImageFrame.sprite = null;
            GetComponent<Button>().interactable = false; //버튼 비활성화
        }
    }
    #endregion
}
