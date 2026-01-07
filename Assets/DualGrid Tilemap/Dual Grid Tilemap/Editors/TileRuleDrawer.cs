using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(TileRule))]
public class TileRuleDrawer : PropertyDrawer
{
    const float CELL_SIZE = 64f;
    const float PADDING = 4f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return CELL_SIZE * 2 + PADDING * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty topLeft = property.FindPropertyRelative("topLeftTile");
        SerializedProperty topRight = property.FindPropertyRelative("topRightTile");
        SerializedProperty bottomLeft = property.FindPropertyRelative("bottomLeftTile");
        SerializedProperty bottomRight = property.FindPropertyRelative("bottomRightTile");

        DualGridTilemap grid = property.serializedObject.targetObject as DualGridTilemap;
        if (grid == null)
        {
            EditorGUI.LabelField(position, "TileRule works only inside DualGridTilemap");
            EditorGUI.EndProperty();
            return;
        }

        Rect gridRect = position;

        DrawCell(
            new Rect(gridRect.x, gridRect.y, CELL_SIZE, CELL_SIZE),
            topLeft,
            grid
        );

        DrawCell(
            new Rect(gridRect.x + CELL_SIZE + PADDING, gridRect.y, CELL_SIZE, CELL_SIZE),
            topRight,
            grid
        );

        DrawCell(
            new Rect(gridRect.x, gridRect.y + CELL_SIZE + PADDING, CELL_SIZE, CELL_SIZE),
            bottomLeft,
            grid
        );

        DrawCell(
            new Rect(gridRect.x + CELL_SIZE + PADDING, gridRect.y + CELL_SIZE + PADDING, CELL_SIZE, CELL_SIZE),
            bottomRight,
            grid
        );

        EditorGUI.EndProperty();
    }

    private void DrawCell(Rect rect, SerializedProperty tileNameProperty, DualGridTilemap grid)
    {
        GUI.Box(rect, GUIContent.none);

        string tileName = tileNameProperty.stringValue;
        Tile tile = grid.GetTileByName(tileName);

        if (tile != null && tile.sprite != null)
        {
            Sprite sprite = tile.sprite;
            Texture2D tex = sprite.texture;
            Rect texCoords = sprite.textureRect;

            // Зберігаємо пропорції
            float spriteAspect = texCoords.width / texCoords.height;
            float cellAspect = rect.width / (rect.height - 20); // враховуємо місце під назву

            float drawWidth, drawHeight;
            if (spriteAspect > cellAspect)
            {
                drawWidth = rect.width - 8;
                drawHeight = drawWidth / spriteAspect;
            }
            else
            {
                drawHeight = rect.height - 28;
                drawWidth = drawHeight * spriteAspect;
            }

            // Центруємо спрайт в клітинці
            Rect iconRect = new Rect(
                rect.x + (rect.width - drawWidth) / 2,
                rect.y + (rect.height - 20 - drawHeight) / 2,
                drawWidth,
                drawHeight
            );

            GUI.DrawTextureWithTexCoords(
                iconRect,
                tex,
                new Rect(
                    texCoords.x / tex.width,
                    texCoords.y / tex.height,
                    texCoords.width / tex.width,
                    texCoords.height / tex.height
                )
            );
        }

        // Підпис тайлу
        Rect labelRect = new Rect(rect.x, rect.yMax - 20, rect.width, 18);
        EditorGUI.LabelField(labelRect, tileName, EditorStyles.centeredGreyMiniLabel);

        // Кнопка для меню
        if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Null"), tileName == "Null", () =>
            {
                tileNameProperty.stringValue = "Null";
                tileNameProperty.serializedObject.ApplyModifiedProperties();
            });

            foreach (TypeOfTiles type in grid.GetTypeOfTiles())
            {
                string name = type.tileName;
                menu.AddItem(new GUIContent(name), tileName == name, () =>
                {
                    tileNameProperty.stringValue = name;
                    tileNameProperty.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
    }

}
