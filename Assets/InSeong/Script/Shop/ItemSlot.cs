using DiceSurvivor.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    #region Variables
    //임시 아이템용 스크립트
    //해당 칸에 들어갈 아이템 정보
    public TestItem currentItem;
    //아이템 정보를 보여줄 UI 요소들
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText;
    public Image itemImageFrame;

    [Header("Detail Display")]
    public ItemDetailDisplay detailDisplay; // 인스펙터에서 detail 오브젝트 할당
    #endregion

    #region Properties
    #endregion

    #region Unity Event Methods
    private void Start() {
        changeInfo();
        if (ShopManagerTest.Instance != null) {
            GetComponent<Button>().onClick.AddListener(
                () => ShopManagerTest.Instance.BuyItem(this)
            );
        }
    }

    // 마우스를 올렸을 때
    public void OnPointerEnter(PointerEventData eventData) {
        if (currentItem != null && detailDisplay != null) {
            detailDisplay.ShowDetail(currentItem);
        }
    }

    // 마우스를 뗐을 때
    public void OnPointerExit(PointerEventData eventData) {
        if (detailDisplay != null) {
            detailDisplay.HideDetail();
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

            // 이미지를 보이게 함 (Alpha = 1)
            Color imageColor = itemImageFrame.color;
            imageColor.a = 1f;
            itemImageFrame.color = imageColor;

            GetComponent<Button>().interactable = currentItem.canIBuy; //아이템 구매 가능 여부에 따라 버튼 활성화
        }
        else { //칸을 비워야 하는 경우
            itemNameText.text = "";
            itemTypeText.text = "";
            itemImageFrame.sprite = null;

            // 이미지를 투명하게 함 (Alpha = 0)
            Color imageColor = itemImageFrame.color;
            imageColor.a = 0f;
            itemImageFrame.color = imageColor;

            GetComponent<Button>().interactable = false; //버튼 비활성화
        }
    }
    #endregion
}