using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using JetBrains.Annotations;
using System.Data;

[System.Serializable]
public class TypeOfTiles
{
    public string tileName = "None";
    public Tile tileToRepresend = null;
}

[System.Serializable]
public class RandomTile
{
    public Tile tile;
    public float chance;
}

[System.Serializable]
public class TileRule
{
    public string topLeftTile;
    public string topRightTile;
    public string bottomLeftTile;
    public string bottomRightTile;

    public bool Equals(TileRule other)
    {
        if (other == null)
            return false;

        return
            topLeftTile == other.topLeftTile &&
            topRightTile == other.topRightTile &&
            bottomLeftTile == other.bottomLeftTile &&
            bottomRightTile == other.bottomRightTile;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as TileRule);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (topLeftTile != null ? topLeftTile.GetHashCode() : 0);
            hash = hash * 23 + (topRightTile != null ? topRightTile.GetHashCode() : 0);
            hash = hash * 23 + (bottomLeftTile != null ? bottomLeftTile.GetHashCode() : 0);
            hash = hash * 23 + (bottomRightTile != null ? bottomRightTile.GetHashCode() : 0);
            return hash;
        }
    }
}

[System.Serializable]
public class TilesToRule
{
    public List<RandomTile> randomTiles = new List<RandomTile>();
    [Space(5)]
    public TileRule tileRule;
}

[ExecuteInEditMode]
public class DualGridTilemap : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap invisibleTilemaps;
    [SerializeField] private Tilemap visibleTilemap;

    [Header("Type of tiles")]
    [SerializeField] private List<TypeOfTiles> typeOfTiles = new List<TypeOfTiles>();

    [Header("Rules for tiles")]
    [SerializeField] private List<TilesToRule> tilesToRule = new List<TilesToRule>();

    Dictionary<Tile, string> tileNameDictionary = new Dictionary<Tile, string>();
    Dictionary<TileRule, List<RandomTile>> tileToRule = new Dictionary<TileRule, List<RandomTile>>();

    private void Awake()
    {
        invisibleTilemaps.color = new Color(1,1,1,0);
    }

    private void OnValidate()
    {
        tileNameDictionary = new Dictionary<Tile, string>();
        tileToRule = new Dictionary<TileRule, List<RandomTile>>();

        foreach (TypeOfTiles currentType in typeOfTiles)
            if (currentType.tileToRepresend != null && !tileNameDictionary.ContainsKey(currentType.tileToRepresend))
                tileNameDictionary.Add(currentType.tileToRepresend, currentType.tileName);

        foreach (TilesToRule currentRule in tilesToRule)
            if (currentRule.randomTiles.Count > 0 && !tileToRule.ContainsKey(currentRule.tileRule))
                tileToRule.Add(currentRule.tileRule, currentRule.randomTiles);
    }

    private void OnEnable()
    {
        if (invisibleTilemaps != null)
            Tilemap.tilemapTileChanged += OnTilemapTileChanged;
    }

    private void OnDisable()
    {
        if (invisibleTilemaps != null)
            Tilemap.tilemapTileChanged -= OnTilemapTileChanged;
    }

    private void OnTilemapTileChanged(Tilemap changedTilemap, Tilemap.SyncTile[] syncTiles)
    {
        if (changedTilemap != invisibleTilemaps)
            return;

        foreach (Tilemap.SyncTile syncTile in syncTiles)
        {
            Vector3Int position = syncTile.position;
            TileBase newTile = syncTile.tile;

            SetTileWithRule(position);
            SetTileWithRule(position + new Vector3Int(0, -1, 0));
            SetTileWithRule(position + new Vector3Int(-1, -1, 0));
            SetTileWithRule(position + new Vector3Int(-1, 0, 0));
        }
    }

    private void SetTileWithRule(Vector3Int position)
    {
        TileRule currentRule = new TileRule();
        if (invisibleTilemaps.GetTile<Tile>(position + new Vector3Int(0, 1, 0)) == null) currentRule.topLeftTile = "Null";
        else tileNameDictionary.TryGetValue(invisibleTilemaps.GetTile<Tile>(position + new Vector3Int(0, 1, 0)), out currentRule.topLeftTile);

        if (invisibleTilemaps.GetTile<Tile>(position + new Vector3Int(1, 1, 0)) == null) currentRule.topRightTile = "Null";
        else tileNameDictionary.TryGetValue(invisibleTilemaps.GetTile<Tile>(position + new Vector3Int(1, 1, 0)), out currentRule.topRightTile);

        if (invisibleTilemaps.GetTile<Tile>(position) == null) currentRule.bottomLeftTile = "Null";
        else tileNameDictionary.TryGetValue(invisibleTilemaps.GetTile<Tile>(position), out currentRule.bottomLeftTile);

        if (invisibleTilemaps.GetTile<Tile>(position + new Vector3Int(1, 0, 0)) == null) currentRule.bottomRightTile = "Null";
        else tileNameDictionary.TryGetValue(invisibleTilemaps.GetTile<Tile>(position + new Vector3Int(1, 0, 0)), out currentRule.bottomRightTile);

        List<RandomTile> posibleRandomTiles;
        tileToRule.TryGetValue(currentRule, out posibleRandomTiles);

        if (posibleRandomTiles == null)
            visibleTilemap.SetTile(position, null);
        else
        {
            Tile tileToSet = GetRandomTile(posibleRandomTiles);
            visibleTilemap.SetTile(position, tileToSet);
        }
    }

    private Tile GetRandomTile(List<RandomTile> tiles)
    {
        if (tiles == null || tiles.Count == 0)
            return null;

        float totalChance = 0f;
        foreach (var rt in tiles)
            totalChance += rt.chance;

        if (totalChance <= 0f)
            return tiles[0].tile;

        float rand = UnityEngine.Random.Range(0f, totalChance);

        float cumulative = 0f;
        foreach (var rt in tiles)
        {
            cumulative += rt.chance;
            if (rand <= cumulative)
                return rt.tile;
        }

        return tiles[tiles.Count - 1].tile;
    }



    public List<TypeOfTiles> GetTypeOfTiles()
    {
        return typeOfTiles;
    }
    public Tile GetTileByName(string name)
    {
        if (name == "Null")
            return null;

        foreach (TypeOfTiles t in typeOfTiles)
            if (t.tileName == name)
                return t.tileToRepresend;

        return null;
    }
}
