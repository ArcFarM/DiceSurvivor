using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RandomBoxSpawner : MonoBehaviour
{
    #region Variables
    public List<GameObject> boxPrefabs; // ���� ������ �ڽ�
    public float minSpawnDelay = 10f;   // �ּ� ��� �ð�
    public float maxSpawnDelay = 30f;   // �ִ� ��� �ð�
    public List<Transform> spawnPoints; // ���� ���� ������ ��ġ��

    public Transform player;

    private GameObject currentBox;
    #endregion

    #region Custom Method
    void Start()
    {
        StartCoroutine(SpawnBoxRoutine());
    }
    #endregion

    #region Custom Method
    IEnumerator SpawnBoxRoutine()
    {
        while (true)
        {
            // ���ڰ� �̹� �����ϸ� ��ٸ�
            while (currentBox != null)
                yield return null;

            // ���� �ð� ���� ���
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            SpawnBox();
        }
    }

    public  void SpawnBox()
    {
        if (boxPrefabs.Count == 0) return;

        // ���� ������ ����
        GameObject selectedBox = boxPrefabs[Random.Range(0, boxPrefabs.Count)];

        // �÷��̾� �ֺ� ���� ��ġ ���
        Vector3 playerPos = player.position;
        float radius = 10f;
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 spawnPos = new Vector3(playerPos.x + randomCircle.x, playerPos.y, playerPos.z + randomCircle.y);

        currentBox = Instantiate(selectedBox, spawnPos, Quaternion.identity);

        Box boxScript = currentBox.GetComponent<Box>();
        if (boxScript != null)
            boxScript.onBoxDestroyed += OnBoxDestroyed;
    }

    public void OnBoxDestroyed()
    {
        currentBox = null;
    }
    #endregion
}
