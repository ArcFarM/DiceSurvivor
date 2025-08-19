using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class StageNodeClickable : MonoBehaviour, IPointerClickHandler
{
    public StageNode node;
    Animator lockAnimator;

    void Awake()
    {
        if (node == null) node = GetComponent<StageNode>();
        if (node != null && node.LockIcon != null)
            lockAnimator = node.LockIcon.GetComponentInChildren<Animator>();
    }

    public void OnPointerClick(PointerEventData eventData) => HandleClick();
    void OnMouseDown() => HandleClick(); // EventSystem 없을 때 백업

    void HandleClick()
    {
        if (node == null) return;

        if (!node.IsUnlocked())
        {
            // 잠긴 상태면 애니메이터 트리거만 쏨
            if (lockAnimator != null)
            {
                lockAnimator.ResetTrigger("Shake");
                lockAnimator.SetTrigger("Shake");
            }

            // 이동은 허용 (입장은 StageSelector에서 막힘)
            StageSelector.Instance?.OnNodeClicked(node);
            return;
        }

        // 언락일 때
        StageSelector.Instance?.OnNodeClicked(node);
    }
}
