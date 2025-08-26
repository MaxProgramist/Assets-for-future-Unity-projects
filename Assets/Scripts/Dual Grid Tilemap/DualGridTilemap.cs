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
    public List<RandomTileSettings> randomTileSettings;
}

public class DualGridTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap invisibleTilemap;
    [Space(5)]
    [SerializeField] public List<TilesType> tileTypes = new List<TilesType>(1) { new TilesType() {typeName = "None", tileToRepresend = null } };
    [Space(5)]
    [SerializeField] private List<TileRuls> tileRuls;

    Dictionary<Tile, string> dictionaryOfTilesName = new Dictionary<Tile, string>();

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

    public void DrawVisibleMap()
    {
        invisibleTilemap.color = new Color(1, 1, 1, 0);

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
