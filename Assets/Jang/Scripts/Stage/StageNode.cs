using UnityEngine;

public class StageNode : MonoBehaviour
{
    [Header("Stage Info")]
    public string sceneName;

    [Header("Progression")]
    public int stageIndex = 0;

    [Header("Lock State")]
    [SerializeField] private bool locked = false;

    [Tooltip("자물쇠 아이콘(가능하면 할당). 없으면 자식에서 lockIconChildName으로 자동 탐색")]
    [SerializeField] private Transform lockIcon;
    [SerializeField] private string lockIconChildName = "LockIcon";

    [SerializeField] private Renderer[] tintTargets;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = new(0.55f, 0.55f, 0.55f);

    public StageNode[] connectedNodes;

    public bool IsUnlocked() => !locked;

    /// 파괴/미할당 시 자동 재탐색하는 안전 게터
    public Transform LockIcon
    {
        get
        {
            // Unity의 파괴된 오브젝트는 == null 비교를 통과합니다.
            if (lockIcon == null)
            {
                var found = string.IsNullOrEmpty(lockIconChildName)
                    ? null
                    : transform.Find(lockIconChildName);
                if (found) lockIcon = found;
            }
            return lockIcon;
        }
        set { lockIcon = value; }
    }

    private void Start() => ApplyVisual();
    private void OnValidate() => ApplyVisual();

    public void SetLocked(bool value)
    {
        locked = value;
        ApplyVisual();
    }

    public void ForceLockVisualRefresh() => ApplyVisual();

    private void ApplyVisual()
    {
        var icon = LockIcon;
        if (icon) icon.gameObject.SetActive(!IsUnlocked());

        if (tintTargets != null)
        {
            var c = IsUnlocked() ? unlockedColor : lockedColor;
            foreach (var r in tintTargets)
            {
                if (!r) continue;
                if (r.material && r.material.HasProperty("_Color"))
                    r.material.color = c;
            }
        }
    }
}
