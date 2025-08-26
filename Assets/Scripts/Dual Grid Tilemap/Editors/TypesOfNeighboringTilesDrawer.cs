using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
[CustomPropertyDrawer(typeof(TypesOfNeighboringTiles))] 
public class TypesOfNeighboringTilesDrawer : PropertyDrawer 
{ 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) { 
        float padding = 2f; 
        float lineHeight = EditorGUIUtility.singleLineHeight; 
        float squareWidth = (position.width - padding * 3) / 2; 
        DualGridTilemap parentScript = property.serializedObject.targetObject as DualGridTilemap; 
        List<TilesType> types = parentScript.tileTypes; 
        GUIContent[] options = new GUIContent[types.Count]; 
        for (int i = 0; i < types.Count; i++) { 
            Texture preview = null; 
            if (types[i].tileToRepresend != null && types[i].tileToRepresend.sprite != null) { 
                preview = AssetPreview.GetAssetPreview(types[i].tileToRepresend.sprite); 
                if (preview == null) preview = AssetPreview.GetMiniThumbnail(types[i].tileToRepresend.sprite); 
            } 
            if (preview == null) { 
                preview = new Texture2D(32, 32, TextureFormat.ARGB32, false); 
                Color[] pixels = new Color[32 * 32]; 
                for (int p = 0; p < pixels.Length; p++) 
                    pixels[p] = Color.clear; 
                ((Texture2D)preview).SetPixels(pixels); 
                ((Texture2D)preview).Apply(); 
            } 
            options[i] = new GUIContent(types[i].typeName, preview); 
        } 
        DrawTilePopup(property, "tileType_TopLeft", options, new Rect(position.x, position.y, squareWidth, lineHeight)); 
        DrawTilePopup(property, "tileType_TopRight", options, new Rect(position.x + squareWidth + padding, position.y, squareWidth, lineHeight)); 
        DrawTilePopup(property, "tileType_BottomLeft", options, new Rect(position.x, position.y + lineHeight + padding, squareWidth, lineHeight)); 
        DrawTilePopup(property, "tileType_BottomRight", options, new Rect(position.x + squareWidth + padding, position.y + lineHeight + padding, squareWidth, lineHeight)); 
    } 
    private void DrawTilePopup(SerializedProperty property, string fieldName, GUIContent[] options, Rect rect) 
    { 
        SerializedProperty prop = property.FindPropertyRelative(fieldName); 
        int selectedIndex = Mathf.Max(0, System.Array.FindIndex(options, o => o.text == prop.stringValue)); 
        selectedIndex = EditorGUI.Popup(rect, selectedIndex, options); 
        prop.stringValue = options[selectedIndex].text; 
    } 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
    { 
        float lineHeight = EditorGUIUtility.singleLineHeight; 
        float padding = 2f; 
        return lineHeight * 2 + padding; 
    } 
}