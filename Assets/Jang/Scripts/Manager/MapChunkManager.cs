using System.Collections.Generic;
using UnityEngine;
using MainGame.Manager;

public class MapChunkManager : SingletonManager<MapChunkManager>
{
    [SerializeField] private GameObject[] tilePrefabs;

    [SerializeField] private int chunkSize = 10;
    [SerializeField] private float tileActivationRange = 25f;

    private Dictionary<Vector2Int, GameObject> tilePool = new();

    public GameObject GetOrCreateTileAt(Vector2Int coord)
    {
        if (tilePool.TryGetValue(coord, out GameObject tile))
        {
            tile.SetActive(true); // 다시 켜기
            return tile;
        }
        GameObject prefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];

        Vector3 position = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
        tile = Instantiate(prefab, position, Quaternion.identity);
        tile.name = $"Tile_{coord.x}_{coord.y}";
        tilePool[coord] = tile;
        return tile;
    }

    public void DeactivateTile(Vector2Int coord)
    {
        if (tilePool.TryGetValue(coord, out GameObject tile))
        {
            tile.SetActive(false); // 끄기
        }
    }

    public IEnumerable<Vector2Int> GetAllTileCoords() => tilePool.Keys;

    public float GetActivationRange() => tileActivationRange;

    public int GetChunkSize() => chunkSize;
}
