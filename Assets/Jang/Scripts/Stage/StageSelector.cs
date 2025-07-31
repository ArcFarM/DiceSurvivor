using UnityEngine;

public class StageSelector : MonoBehaviour
{
    public static StageSelector Instance;        // 싱글톤 인스턴스

    public PlayerMover player;                   // 이동시킬 플레이어
    public SceneFader sceneFader;                // 씬 전환 연출
    public StageNode startingNode;               // 시작할 노드 (인스펙터에서 설정)

    private StageNode selectedNode;              // 현재 선택된 노드
    [SerializeField]
    private bool loadLastStagePosition = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StageNode found = null;

        if (loadLastStagePosition)
        {
            string lastNodeName = PlayerPrefs.GetString("LastStageNode", "");
            if (!string.IsNullOrEmpty(lastNodeName))
            {
                foreach (StageNode node in FindObjectsOfType<StageNode>())
                {
                    if (node.name == lastNodeName)
                    {
                        found = node;
                        break;
                    }
                }
            }
        }

        if (found != null)
        {
            selectedNode = found;
        }
        else if (startingNode != null)
        {
            selectedNode = startingNode;
        }
        else
        {
            selectedNode = FindClosestNodeToPlayer();
        }

        player.MoveTo(selectedNode.transform.position);

        Debug.Log($"[선택 씬 시작] 시작 노드: {selectedNode.name}");
    }

    private void Update()
    {
        // 이동 중이면 입력 무시
        if (player == null || player.IsMoving()) return;

        // 방향키(WASD) 또는 화살표 입력 감지
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.magnitude > 0.1f)
        {
            TryMoveByInput(input);
        }

        // Z 또는 Enter 키 누르면 스테이지 진입
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedNode != null && player.IsAtTarget())
            {
                sceneFader.FadeTo(selectedNode.sceneName);
            }
        }
    }

    private void LateUpdate()
    {
        if (!player.IsMoving())
        {
            StageNode closest = FindClosestNodeToPlayer();
            float dist = Vector3.Distance(player.transform.position, closest.transform.position);

            if (dist < 0.2f && closest != null)
            {
                selectedNode = closest;
            }
        }
    }


    // 노드를 마우스로 클릭했을 때 처리
    public void OnNodeClicked(StageNode node)
    {
        // 플레이어가 도착한 상태 + 클릭한 노드가 selectedNode와 정확히 같은 경우만 씬 진입
        if (node == selectedNode && player.IsAtTarget() &&
            Vector3.Distance(player.transform.position, node.transform.position) < 0.1f)
        {
            PlayerPrefs.SetString("LastStageNode", node.name);
            PlayerPrefs.Save();

            sceneFader.FadeTo(node.sceneName);
        }
        else
        {
            selectedNode = node;
            player.MoveTo(node.transform.position);
        }
    }

    // 키보드 입력 방향에 따라 이동 시도
    private void TryMoveByInput(Vector2 input)
    {
        if (selectedNode == null) return;

        Vector3 currentPos = selectedNode.transform.position;
        Vector3 inputDir = new Vector3(input.x, 0f, input.y).normalized;

        StageNode bestNode = null;
        float bestDot = 0.7f; // 방향 일치도 기준

        foreach (StageNode node in selectedNode.connectedNodes)
        {
            //if (!node.IsUnlocked()) continue;

            Vector3 dir = (node.transform.position - currentPos).normalized;
            float dot = Vector3.Dot(inputDir, dir); // 방향 비교

            if (dot > bestDot)
            {
                bestDot = dot;
                bestNode = node;
            }
        }

        if (bestNode != null)
        {
            selectedNode = bestNode;
            player.MoveTo(bestNode.transform.position);
        }
    }

    // 플레이어 위치 기준으로 가장 가까운 노드 찾기
    private StageNode FindClosestNodeToPlayer()
    {
        StageNode closest = null;
        float minDist = float.MaxValue;

        foreach (StageNode node in FindObjectsOfType<StageNode>())
        {
            float dist = Vector3.Distance(player.transform.position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }
}
