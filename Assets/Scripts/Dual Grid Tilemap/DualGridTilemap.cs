using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TilesType
{
    public string typeName;
    public Tile tileToRepresend;
}

[System.Serializable]
public class TypesOfNeighboringTiles
{
    public string tileType_TopLeft;
    public string tileType_TopRight;
    public string tileType_BottomLeft;
    public string tileType_BottomRight;
}

[System.Serializable]
public class RandomTileSettings
{
    public Tile tileToRepresend;
    public float chanceToApear;
}

[System.Serializable]
public class TileRuls
{
    public Tile tile;
    [Space(5)]
    public TypesOfNeighboringTiles typesOfNeighboring;
    [Space(5)]
    public bool isRandom;
    public int countOfVariants;
    public List<RandomTileSettings> randomTileSettings;
}

public class DualGridTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap invisibleTilemap;
    [Space(5)]
    [SerializeField] public List<TilesType> tileTypes = new List<TilesType>(1) { new TilesType() {typeName = "None", tileToRepresend = null } };
    [Space(5)]
    [SerializeField] private List<TileRuls> tileRuls;


    void Start()
    {
        DrawVisibleMap();
    }

    public void DrawVisibleMap()
    {
        invisibleTilemap.color = new Color(1, 1, 1, 0);

        Tilemap visibleTilemap = VisibleTilemap();

        BoundsInt tilemapBorders = invisibleTilemap.cellBounds;
        Vector3Int tilemapSize = tilemapBorders.size;

        for (int xOffset = 0; xOffset < tilemapSize.x; xOffset++)
        {
            for (int yOffset = 0; yOffset < tilemapSize.y; yOffset++)
            {
                Vector3Int positionOfGetTile = new Vector3Int(tilemapBorders.xMin + xOffset, tilemapBorders.yMin + yOffset);
                Vector3Int positionOfSetTile = new Vector3Int(tilemapBorders.xMin + xOffset - 1, tilemapBorders.yMin + yOffset - 1);
                TileBase tileToPlace = invisibleTilemap.GetTile(positionOfGetTile);
                visibleTilemap.SetTile(positionOfSetTile, tileToPlace);
            }
        }
    }

    Tilemap VisibleTilemap()
    {
        GameObject visibleTilemap = new GameObject();
        visibleTilemap.name = "Visible Tilemap";
        visibleTilemap.AddComponent<Tilemap>();
        visibleTilemap.AddComponent<TilemapRenderer>();

        visibleTilemap.transform.position = new Vector3(0.5f, 0.5f);
        visibleTilemap.transform.parent = invisibleTilemap.transform.parent;

        return visibleTilemap.GetComponent<Tilemap>();
    }

    private void OnDrawGizmos()
    {
        if (invisibleTilemap == null) return;
        BoundsInt tilemapBorders = invisibleTilemap.cellBounds;
        Vector2 positionOfFirstCell = new Vector2(tilemapBorders.xMin, tilemapBorders.yMin);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(positionOfFirstCell, 0.2f);
    }
}
