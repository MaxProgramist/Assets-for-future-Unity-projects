using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    None
};

[System.Serializable]
public class TileSettings
{
    public Tile tileType;
    [Space(3)]
    public TileType tileType_TopLeft;
    public TileType tileType_TopRight;
    public TileType tileType_BottomLeft;
    public TileType tileType_BottomRight;
    [Space(3)]
    public bool isAnimate;
    public bool isRandomTile;
}

public class DualGridTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap invisibleTilemap;
    [Space(5)]
    [SerializeField] private List<TileSettings> tileSettings = new List<TileSettings>();

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
        BoundsInt tilemapBorders = invisibleTilemap.cellBounds;
        Vector2 positionOfFirstCell = new Vector2(tilemapBorders.xMin, tilemapBorders.yMin);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(positionOfFirstCell, 0.2f);
    }
}
