using System.Collections;
using UnityEngine;

/// <summary>
/// �÷��̾� �ֺ��� �ִ� Ÿ���� �ֱ������� Ȯ���Ͽ�
/// ���� �Ÿ� �ȿ� �ִ� Ÿ���� Ȱ��ȭ�ϰ�, ��� Ÿ���� ��Ȱ��ȭ�ϴ� Ŭ����.
/// </summary>
public class PlayerTileTracker : MonoBehaviour
{
    #region Variables
    private Transform player;                 // �÷��̾� Transform (�ڱ� �ڽ�)
    [SerializeField]private float checkInterval = 0.3f;       // Ÿ�� ���� �ֱ� (�� ����)
    #endregion

    #region Unity Event Method
    void Start()
    {
        player = transform;                   // �÷��̾� �ڽ��� Transform ����
        StartCoroutine(UpdateTilesRoutine()); // Ÿ�� ���� �ڷ�ƾ ����
    }
    #endregion

    #region Custom Method

    /// <summary>
    /// ���� �ð� �������� �ֺ� Ÿ�� ���¸� �����ϴ� �ڷ�ƾ
    /// </summary>
    IEnumerator UpdateTilesRoutine()
    {
        while (true)
        {
            UpdateNearbyTiles();                         // �ֺ� Ÿ�� ����
            yield return new WaitForSeconds(checkInterval); // ���� �ð� ���
        }
    }

    /// <summary>
    /// �÷��̾� �ֺ� Ÿ���� Ȯ���ϰ� �Ÿ� ���̸� Ȱ��ȭ, �ٱ��̸� ��Ȱ��ȭ
    /// </summary>
    void UpdateNearbyTiles()
    {
        int chunkSize = MapChunkManager.Instance.GetChunkSize();         // Ÿ��(ûũ) ũ��
        float activationRange = MapChunkManager.Instance.GetActivationRange(); // Ÿ�� Ȱ��ȭ �Ÿ� ����

        // �÷��̾� ��ġ�� �������� ���� � ûũ(Ÿ��) ���� �ִ��� ��ǥ ���
        Vector2Int playerCoord = new(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        // Ȱ��ȭ ���� ������ �󸶳� ���� ûũ�� Ȯ������ ���
        int range = Mathf.CeilToInt(activationRange / chunkSize);

        // range ���� �ȿ� �ִ� ûũ���� �ݺ��ϸ� Ȯ��
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                // �ֺ� ûũ ��ǥ ���
                Vector2Int coord = new(playerCoord.x + x, playerCoord.y + z);

                // �ش� ûũ�� ���� ��ġ ��� (�߽��� ����)
                Vector3 tileWorldPos = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);

                // �÷��̾���� �Ÿ� ���
                float distance = Vector3.Distance(player.position, tileWorldPos);

                if (distance <= activationRange)
                    MapChunkManager.Instance.GetOrCreateTileAt(coord);   // ���� ���̸� Ÿ�� ���� or Ȱ��ȭ
                else
                    MapChunkManager.Instance.DeactivateTile(coord);     // ���� ���̸� Ÿ�� ��Ȱ��ȭ
            }
        }
    }
    #endregion
}
