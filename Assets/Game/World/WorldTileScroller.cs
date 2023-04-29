using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTileScroller : MonoBehaviour
{

    public List<WorldTile> CenterTiles = new List<WorldTile>();
    public List<WorldTile> OuterTiles = new List<WorldTile>();

    private List<WorldTile> activeTiles = new List<WorldTile>();

    public float Speed = 20f;
    public float TileSpacing = 100f;
    public int NumberOfTilesPerAxisToRenderInEachDirection = 3; // Radius

    private float currentOffset = 0f;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = -NumberOfTilesPerAxisToRenderInEachDirection; i < NumberOfTilesPerAxisToRenderInEachDirection; i++)
        {
            SpawnColumn( i * TileSpacing);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentOffset += Time.deltaTime * Speed;
        foreach (var tile in activeTiles)
        {
            tile.gameObject.transform.localPosition += new Vector3(-Speed, 0, 0) * Time.deltaTime;
        }

        if (currentOffset > TileSpacing) {
            currentOffset -= TileSpacing;
            SpawnColumn(((NumberOfTilesPerAxisToRenderInEachDirection - 1) * TileSpacing) - currentOffset);

            // Delete oldest column
            int n = (NumberOfTilesPerAxisToRenderInEachDirection * 2) - 1;
            for (int i = 0; i < n; i++)
            {
                Destroy(activeTiles[i].gameObject);
            }
            activeTiles.RemoveRange(0, n);
        }
    }

    void SpawnColumn(float x)
    {
        SpawnRandomTile(CenterTiles, x, 0);

        for (int i = 1; i < NumberOfTilesPerAxisToRenderInEachDirection; i++)
        {
            SpawnRandomTile(OuterTiles, x,  i * TileSpacing);
            SpawnRandomTile(OuterTiles, x, -i * TileSpacing);
        }

    }

    void SpawnRandomTile(List<WorldTile> tiles, float x, float z)
    {
        var tile = Instantiate(PickRandomTile(tiles), gameObject.transform);
        activeTiles.Add(tile);
        tile.gameObject.transform.localPosition = new Vector3(x, 0, z);
    }

    WorldTile PickRandomTile(List<WorldTile> tiles)
    {
        return tiles[Random.Range(0, tiles.Count)];
    }
}
