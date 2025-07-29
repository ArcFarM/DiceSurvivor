using UnityEngine;
using System.Collections.Generic;

public class MapExpander : MonoBehaviour
{
    public Transform player;
    public GameObject mapChunkPrefab;
    public int chunkSize = 20;
    public int checkRadius = 1;
    public float unloadDistance = 50f;

    private Dictionary<Vector2Int, GameObject> spawnedChunks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        SpawnChunk(Vector2Int.zero);
    }

    void Update()
    {
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(player.position);

        // 주변 Chunk 생성
        for (int x = -checkRadius; x <= checkRadius; x++)
        {
            for (int z = -checkRadius; z <= checkRadius; z++)
            {
                Vector2Int coord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + z);
                if (!spawnedChunks.ContainsKey(coord))
                {
                    SpawnChunk(coord);
                }
            }
        }

        // 멀어진 Chunk 제거
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var kvp in spawnedChunks)
        {
            Vector3 chunkWorldPos = new Vector3(kvp.Key.x * chunkSize, 0, kvp.Key.y * chunkSize);
            float distance = Vector3.Distance(player.position, chunkWorldPos);

            if (distance > unloadDistance)
            {
                Destroy(kvp.Value);
                chunksToRemove.Add(kvp.Key);
            }
        }

        foreach (var coord in chunksToRemove)
        {
            spawnedChunks.Remove(coord);
        }
    }

    void SpawnChunk(Vector2Int coord)
    {
        Vector3 spawnPos = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
        GameObject chunk = Instantiate(mapChunkPrefab, spawnPos, Quaternion.identity);
        spawnedChunks.Add(coord, chunk);
    }

    Vector2Int GetChunkCoordFromPosition(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / chunkSize);
        int y = Mathf.FloorToInt(pos.z / chunkSize);
        return new Vector2Int(x, y);
    }
}
