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

    public bool Equals(TypesOfNeighboringTiles other)
    {
        return other != null &&
            tileType_TopLeft == other.tileType_TopLeft &&
            tileType_TopRight == other.tileType_TopRight &&
            tileType_BottomLeft == other.tileType_BottomLeft &&
            tileType_BottomRight == other.tileType_BottomRight;
    }
}

[System.Serializable]
public class TileRuls
{
    public Tile tile;
    [Space(5)]
    public TypesOfNeighboringTiles typesOfNeighboring;
}

public class DualGridTilemap : MonoBehaviour
{
    [SerializeField] private List<Tilemap> invisibleTilemaps;
    [SerializeField] private GameObject gridObject;
    [Space(5)]
    [SerializeField] public List<TilesType> tileTypes = new List<TilesType>(1) { new TilesType() {typeName = "None", tileToRepresend = null } };
    [Space(5)]
    [SerializeField] private List<TileRuls> tileRuls;

    Dictionary<Tile, string> dictionaryOfTilesName = new Dictionary<Tile, string>();
    Tilemap invisibleTilemap;
    Color invisibleColor = new Color(1, 1, 1, 0);

    void Start()
    {
        for (int indexOfTileType = 0; indexOfTileType < tileTypes.Count; indexOfTileType++)
        {
            if (tileTypes[indexOfTileType].typeName == "None") continue;

            if (tileTypes[indexOfTileType].tileToRepresend == null) Debug.LogError("You forget to set tile example for type: " + tileTypes[indexOfTileType].typeName);

            dictionaryOfTilesName.Add(tileTypes[indexOfTileType].tileToRepresend, tileTypes[indexOfTileType].typeName);
        }

        DrawVisibleMap();
    }

    public Tilemap DrawVisibleMap()
    {
        GameObject objectForInvisibleTilemap = new GameObject();
        objectForInvisibleTilemap.AddComponent<Tilemap>();
        invisibleTilemap = objectForInvisibleTilemap.GetComponent<Tilemap>();

        CopyTilemap();

        invisibleTilemap.color = invisibleColor;

        Tilemap visibleTilemap = VisibleTilemap();

        BoundsInt tilemapBorders = invisibleTilemap.cellBounds;
        Vector3Int tilemapSize = tilemapBorders.size;

        for (int xOffset = 0; xOffset <= tilemapSize.x; xOffset++)
        {
            for (int yOffset = 0; yOffset <= tilemapSize.y; yOffset++)
            {
                Vector3Int positionOfSetTile = new Vector3Int(tilemapBorders.xMin + xOffset - 1, tilemapBorders.yMin + yOffset - 1);

                Vector3Int positionOfGetTile_TopRight = new Vector3Int(tilemapBorders.xMin + xOffset, tilemapBorders.yMin + yOffset);
                Vector3Int positionOfGetTile_TopLeft = new Vector3Int(tilemapBorders.xMin + xOffset - 1, tilemapBorders.yMin + yOffset);
                Vector3Int positionOfGetTile_BottomRight = new Vector3Int(tilemapBorders.xMin + xOffset, tilemapBorders.yMin + yOffset - 1);
                Vector3Int positionOfGetTile_BottomLeft = new Vector3Int(tilemapBorders.xMin + xOffset - 1, tilemapBorders.yMin + yOffset - 1);

                Tile topRightTile = invisibleTilemap.GetTile(positionOfGetTile_TopRight) as Tile;
                Tile topLeftTile = invisibleTilemap.GetTile(positionOfGetTile_TopLeft) as Tile;
                Tile bottomRightTile = invisibleTilemap.GetTile(positionOfGetTile_BottomRight) as Tile;
                Tile bottomLeftTile = invisibleTilemap.GetTile(positionOfGetTile_BottomLeft) as Tile;


                TypesOfNeighboringTiles typesOfNeighboringTiles = new TypesOfNeighboringTiles();
                if (topRightTile == null) typesOfNeighboringTiles.tileType_TopRight = "None";
                else typesOfNeighboringTiles.tileType_TopRight = dictionaryOfTilesName[topRightTile];

                if (topLeftTile == null) typesOfNeighboringTiles.tileType_TopLeft = "None";
                else typesOfNeighboringTiles.tileType_TopLeft = dictionaryOfTilesName[topLeftTile];

                if (bottomRightTile == null) typesOfNeighboringTiles.tileType_BottomRight = "None";
                else typesOfNeighboringTiles.tileType_BottomRight = dictionaryOfTilesName[bottomRightTile];
                
                if (bottomLeftTile == null) typesOfNeighboringTiles.tileType_BottomLeft = "None";
                else typesOfNeighboringTiles.tileType_BottomLeft = dictionaryOfTilesName[bottomLeftTile];


                TileRuls tileRule = tileRuls.Find(t => t.typesOfNeighboring.Equals(typesOfNeighboringTiles));
                if (tileRule != null)
                {
                    Tile tileToPlace = tileRule.tile;
                    visibleTilemap.SetTile((Vector3Int)positionOfSetTile, tileToPlace);
                }
            }
        }

        Destroy(objectForInvisibleTilemap);

        return visibleTilemap;
    }

    Tilemap VisibleTilemap()
    {
        GameObject visibleTilemap = new GameObject();
        visibleTilemap.name = "Visible Tilemap";
        visibleTilemap.AddComponent<Tilemap>();
        visibleTilemap.AddComponent<TilemapRenderer>();

        visibleTilemap.transform.position = new Vector3(0.5f, 0.5f);
        visibleTilemap.transform.parent = gridObject.transform;

        return visibleTilemap.GetComponent<Tilemap>();
    }

    void CopyTilemap()
    {
        foreach (Tilemap currentTilemap in invisibleTilemaps)
        {
            currentTilemap.color = invisibleColor;
            BoundsInt tilemapBorders = currentTilemap.cellBounds;
            int startX = tilemapBorders.xMin, startY = tilemapBorders.yMin;
            int endX = tilemapBorders.xMax, endY = tilemapBorders.yMax;
            for (int currentX = startX; currentX < endX; currentX++)
            {
                for (int currentY = startY; currentY < endY; currentY++)
                {
                    Vector3Int currentPos = new Vector3Int(currentX, currentY);
                    Tile currentTile = currentTilemap.GetTile(currentPos) as Tile;
                    if (currentTile != null)
                        invisibleTilemap.SetTile(currentPos, currentTile);
                }
            }
        }
    }
}
