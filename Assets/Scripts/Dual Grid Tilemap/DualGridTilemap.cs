using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DualGridTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap invisibleTilemap;
    [SerializeField] private Tilemap visibleTilemap;



    void Start()
    {
        DrawVisibleMap();
    }

    public void DrawVisibleMap()
    {
        invisibleTilemap.color = new Color(1, 1, 1, 0);
    }
}
