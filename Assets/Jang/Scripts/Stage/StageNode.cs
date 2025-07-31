using UnityEngine;

public class StageNode : MonoBehaviour
{
    public int stageID;                     // 현재 노드의 스테이지 번호
    public string sceneName;                // 해당 스테이지에 연결된 씬 이름
    public GameObject lockIcon;             // 잠금 아이콘 오브젝트 (잠긴 경우 표시용)
    public StageNode[] connectedNodes;      // 연결된 이웃 노드들 (이동 가능한 방향들)

  /*  private void Start()
    {
        // 잠겨 있으면 아이콘 표시
        if (!IsUnlocked() && lockIcon != null)
        {
            lockIcon.SetActive(true);
        }
    }

    // 이전 스테이지를 클리어했는지 확인
    public bool IsUnlocked()
    {
        if (stageID == 0) return true; // 첫 번째 스테이지는 항상 열려 있음
        return PlayerPrefs.GetInt($"Stage{stageID - 1}_Cleared", 0) == 1;
    }*/

    // 마우스로 클릭했을 때 처리
    private void OnMouseDown()
    {
       /* if (!IsUnlocked())
        {
            Debug.Log(" 잠긴 스테이지입니다.");
            return;
        }*/

        // StageSelector에 클릭 통보
        StageSelector.Instance.OnNodeClicked(this);
    }
}
