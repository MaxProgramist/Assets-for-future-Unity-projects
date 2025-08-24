using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapAnimation: MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;

    [Header("Universal")]
    [SerializeField] private float timeBtwNextStep = 0.01f;

    [Header("Intro Animation")]
    [SerializeField] private Vector2Int introAnimationPosition = Vector2Int.zero;
    [Space(10)]
    [SerializeField] private AnimationType introAnimationType = AnimationType.Circle;
    [SerializeField] private float introAnimationDuration = 0.001f;
    [Space(10)]
    [SerializeField] private PositionType introAnimation_PositionType = PositionType.Global;
    [SerializeField] private Vector2 introAnimation_Position = Vector2.zero;
    [SerializeField] private AnimationCurve introAnimationCurve_Position = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(7)]
    [SerializeField] private Vector3 introAnimation_Rotation = Vector2.zero;
    [SerializeField] private AnimationCurve introAnimationCurve_Rotation = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(7)]
    [SerializeField] private Vector2 introAnimation_Scale = Vector2.one;
    [SerializeField] private AnimationCurve introAnimationCurve_Scale = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(7)]
    [SerializeField] private Color introAnimation_Color = Color.white;
    [SerializeField] private AnimationCurve introAnimationCurve_Color = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(20)]

    [Header("Outro Animation")]
    [SerializeField] private Vector2Int outroAnimationPosition = Vector2Int.zero;
    [Space(10)]
    [SerializeField] private AnimationType outroAnimationType = AnimationType.Circle;
    [SerializeField] private float outroAnimationDuration = 0.001f;
    [Space(10)]
    [SerializeField] private PositionType outroAnimation_PositionType = PositionType.Global;
    [SerializeField] private Vector2 outroAnimation_Position = Vector2.zero;
    [SerializeField] private AnimationCurve outroAnimationCurve_Position = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(7)]
    [SerializeField] private Vector3 outroAnimation_Rotation = Vector2.zero;
    [SerializeField] private AnimationCurve outroAnimationCurve_Rotation = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(7)]
    [SerializeField] private Vector2 outroAnimation_Scale = Vector2.one;
    [SerializeField] private AnimationCurve outroAnimationCurve_Scale = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(7)]
    [SerializeField] private Color outroAnimation_Color = Color.white;
    [SerializeField] private AnimationCurve outroAnimationCurve_Color = AnimationCurve.Linear(0, 0, 1, 1);
    [Space(20)]

    [Header("Debugger")]
    [SerializeField] private Color startPosOfIntro_Color = Color.white;
    [SerializeField] private float startPosOfIntro_Radius = 0.25f;
    [Space(7)]
    [SerializeField] private Color startPosOfOutro_Color = Color.black;
    [SerializeField] private float startPosOfOutro_Radius = 0.25f;

    [HideInInspector] public bool isAnimating = false;

    enum AnimationType { Circle, Square }

    enum PositionType { Global, Local }

    void Start()
    {
        
    }

    public void IntroAnimation()
    {
        if (isAnimating) return;
        
        StartCoroutine(MapAnimationIntro());
    }

    public void OutroAnimation()
    {
        if (isAnimating) return;

        StartCoroutine(MapAnimationOutro());
    }

    IEnumerator MapAnimationIntro()
    {
        isAnimating = true;

        tilemap.CompressBounds();
        BoundsInt tilemapBorders = tilemap.cellBounds;
        Vector3Int tilemapSize = tilemapBorders.size;
        TileBase[,] mapTilebases = GetTileMap();

        Vector2Int startPos = introAnimationPosition;

        tilemap.ClearAllTiles();

        int distanceFromCentre = 0, countOfVisited = 0;
        bool[,] visitedCells = new bool[tilemapSize.x, tilemapSize.y];

        while (countOfVisited < tilemapSize.x * tilemapSize.y)
        {
            for (int x = 0; x < tilemapSize.x; x++)
            {
                for (int y = 0; y < tilemapSize.y; y++)
                {
                    Vector2Int currentPos = new Vector2Int(x, y);

                    if (visitedCells[x, y]) continue;
                    if (!CanPlaceTile(startPos, new Vector2Int(x, y), distanceFromCentre, true)) continue;

                    TileBase tile = mapTilebases[x, y];
                    if (tile != null)
                    {
                        Vector3Int currentRealWorldPos = new Vector3Int(x + tilemapBorders.xMin, y + tilemapBorders.yMin);
                        StartCoroutine(AnimateTile(true, (Vector2Int)currentRealWorldPos, tile));
                    }

                    visitedCells[x,y] = true;

                    countOfVisited++;
                }
            }
            distanceFromCentre++;

            yield return new WaitForSeconds(timeBtwNextStep);
        }

        isAnimating = false;
    }

    IEnumerator MapAnimationOutro()
    {
        isAnimating = true;

        tilemap.CompressBounds();
        BoundsInt tilemapBorders = tilemap.cellBounds;
        Vector3Int tilemapSize = tilemapBorders.size;
        TileBase[,] mapTilebases = GetTileMap();

        Vector2Int startPos = outroAnimationPosition;

        int distanceFromCentre = 0, countOfVisited = 0;
        bool[,] visitedCells = new bool[tilemapSize.x, tilemapSize.y];

        while (countOfVisited < tilemapSize.x * tilemapSize.y)
        {
            for (int x = 0; x < tilemapSize.x; x++)
            {
                for (int y = 0; y < tilemapSize.y; y++)
                {
                    Vector2Int currentPos = new Vector2Int(x, y);
                    TileBase tile = mapTilebases[x, y];

                    if (visitedCells[x, y]) continue;
                    if (tile == null) continue;
                    if (!CanPlaceTile(startPos, new Vector2Int(x, y), distanceFromCentre, false)) continue;

                    Vector3Int currentRealWorldPos = new Vector3Int(x + tilemapBorders.xMin, y + tilemapBorders.yMin);
                    StartCoroutine(AnimateTile(false, (Vector2Int)currentRealWorldPos, tile));

                    visitedCells[x, y] = true;

                    countOfVisited++;
                }
            }
            distanceFromCentre++;

            yield return new WaitForSeconds(timeBtwNextStep);
        }

        isAnimating = false;
    }

    IEnumerator AnimateTile(bool intro, Vector2Int tilePosition, TileBase tile)
    {
        if (!intro)
            tilemap.SetTile((Vector3Int)tilePosition, null);

        Tile tileForSprite = tile as Tile;

        GameObject newTile = new GameObject();

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileForSprite.sprite;

        Vector2 startPositon, endPosition;
        Vector3 startRotation, endRotation;
        Vector2 startScale, endScale;
        Color startColor, endColor;
        float duration;
        AnimationCurve animationCurve_Position;
        AnimationCurve animationCurve_Rotation;
        AnimationCurve animationCurve_Scale;
        AnimationCurve animationCurve_Color;
        Vector2 tileOffset = tilemap.tileAnchor;

        if (intro)
        {
            startPositon = introAnimation_Position + tileOffset + ((introAnimation_PositionType == PositionType.Global) ? 1:0) * tilePosition;
            endPosition = tilePosition + tileOffset;
            startRotation = introAnimation_Rotation;
            endRotation = new Vector2(tilemap.transform.rotation.x, tilemap.transform.rotation.y);
            startScale = introAnimation_Scale;
            endScale = tilemap.transform.localScale;
            startColor = introAnimation_Color;
            endColor = tilemap.color;

            duration = introAnimationDuration;
            animationCurve_Position = introAnimationCurve_Position;
            animationCurve_Rotation = introAnimationCurve_Rotation;
            animationCurve_Scale = introAnimationCurve_Scale;
            animationCurve_Color = introAnimationCurve_Color;
        } else
        {
            startPositon = tilePosition + tileOffset;
            endPosition = outroAnimation_Position + tileOffset + ((outroAnimation_PositionType == PositionType.Global) ? 1 : 0) * tilePosition;
            startRotation = new Vector2(tilemap.transform.rotation.x, tilemap.transform.rotation.y);
            endRotation = outroAnimation_Rotation;
            startScale = tilemap.transform.localScale;
            endScale = outroAnimation_Scale;
            startColor = tilemap.color;
            endColor = outroAnimation_Color;

            duration = outroAnimationDuration;
            animationCurve_Position = outroAnimationCurve_Position;
            animationCurve_Rotation = outroAnimationCurve_Rotation;
            animationCurve_Scale = outroAnimationCurve_Scale;
            animationCurve_Color = outroAnimationCurve_Color;
        }

        float timePassed = 0, t = timePassed / duration;

        while (t <= 1)
        {
            float curveT_Position = animationCurve_Position.Evaluate(t);
            float curveT_Rotation = animationCurve_Rotation.Evaluate(t);
            float curveT_Scale = animationCurve_Scale.Evaluate(t);
            float curveT_Color = animationCurve_Color.Evaluate(t);

            newTile.transform.position = Vector2.LerpUnclamped(startPositon, endPosition, curveT_Position);
            newTile.transform.rotation = Quaternion.LerpUnclamped(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), curveT_Rotation);
            newTile.transform.localScale = Vector2.LerpUnclamped(startScale, endScale, curveT_Scale);
            newTile.GetComponent<SpriteRenderer>().color = Color.LerpUnclamped(startColor, endColor, curveT_Color);

            timePassed += Time.deltaTime;
            t = timePassed / duration;

            yield return null;
        }

        Destroy(newTile);

        if (intro)
            tilemap.SetTile((Vector3Int)tilePosition, tile);
    }

    bool CanPlaceTile(Vector2Int startPos, Vector2Int tilePos, int a, bool intro)
    {
        if ((intro && introAnimationType == AnimationType.Circle) || 
            (!intro && outroAnimationType == AnimationType.Circle))
        {
            return Vector2.Distance((Vector2)startPos, (Vector2)tilePos) <= a;
        }
        else if ((intro && introAnimationType == AnimationType.Square) ||
            (!intro && outroAnimationType == AnimationType.Square))
        {
            return Mathf.Abs(startPos.x - tilePos.x) <= a && Mathf.Abs(startPos.y - tilePos.y) <= a;
        }

        return false;
    }

    TileBase[,] GetTileMap()
    {
        tilemap.CompressBounds();
        BoundsInt tilemapBorders = tilemap.cellBounds;
        Vector3Int tilemapSize = tilemapBorders.size;

        TileBase[,] mapTilebases = new TileBase[tilemapSize.x, tilemapSize.y];

        for (int x = 0; x < tilemapSize.x; x++)
        {
            for (int y = 0; y < tilemapSize.y; y++)
            {
                Vector3Int posOfTile = new Vector3Int(x + tilemapBorders.xMin, y + tilemapBorders.yMin);
                mapTilebases[x, y] = tilemap.GetTile(posOfTile);
            }
        }

        return mapTilebases;
    }

    private void OnDrawGizmos()
    {
        if (tilemap == null) return;

        BoundsInt tilemapBorders = tilemap.cellBounds;
        Vector3 startDrawPos = new Vector3(introAnimationPosition.x + tilemapBorders.xMin + 0.5f, introAnimationPosition.y + tilemapBorders.yMin + 0.5f);

        Gizmos.color = startPosOfIntro_Color;
        Gizmos.DrawSphere(startDrawPos, startPosOfIntro_Radius);

        Vector3 endDrawPos = new Vector3(outroAnimationPosition.x + tilemapBorders.xMin + 0.5f, outroAnimationPosition.y + tilemapBorders.yMin + 0.5f);

        Gizmos.color = startPosOfOutro_Color;
        Gizmos.DrawSphere(endDrawPos, startPosOfOutro_Radius);
    }
}
