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
    public int NumColumns = 3; // Radius
    public int NumRows = 3; // Radius

    private float currentOffset = 0f;


    // Start is called before the first frame update
    void Start()
    {
        var rowStartingPoint = -((TileSpacing * NumRows) / 2.0f) + TileSpacing / 2.0f;
        for (int i = 0; i < NumRows; i++)
        {
            SpawnRow( i * TileSpacing + rowStartingPoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentOffset += Time.deltaTime * Speed;
        foreach (var tile in activeTiles)
        {
            tile.gameObject.transform.localPosition += new Vector3(0, 0, -Speed) * Time.deltaTime;
        }

        if (currentOffset > TileSpacing) {
            currentOffset -= TileSpacing;

            var rowStartingPoint = -((TileSpacing * NumRows) / 2.0f) + TileSpacing / 2.0f;
            var newRowOffset = rowStartingPoint + ((NumRows - 1) * TileSpacing) - currentOffset;
            SpawnRow(newRowOffset);

            // Delete oldest row
            for (int i = 0; i < NumColumns; i++)
            {
                Destroy(activeTiles[i].gameObject);
            }
            activeTiles.RemoveRange(0, NumColumns);
        }
    }

    void SpawnRow(float verticalOffset)
    {
        float totalHorizontalOffset = NumColumns * TileSpacing;

        for (int i = 0; i < NumColumns; i++)
        {
            float horizontalOffset = (i * TileSpacing + TileSpacing / 2.0f) - (totalHorizontalOffset / 2.0f);

            if (i == 0 || i == NumColumns - 1)
            {
                SpawnRandomTile(OuterTiles, horizontalOffset, verticalOffset);
            }
            else
            {
                SpawnRandomTile(CenterTiles, horizontalOffset, verticalOffset);
            }
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
