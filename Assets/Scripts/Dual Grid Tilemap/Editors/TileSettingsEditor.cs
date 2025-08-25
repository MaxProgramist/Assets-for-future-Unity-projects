using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSettings))]
public class TileSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Виводимо Tile
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tileType"));

        EditorGUILayout.Space();

        // Виводимо 4 enum у формі квадрата
        SerializedProperty topLeftProp = serializedObject.FindProperty("tileType_TopLeft");
        SerializedProperty topRightProp = serializedObject.FindProperty("tileType_TopRight");
        SerializedProperty bottomLeftProp = serializedObject.FindProperty("tileType_BottomLeft");
        SerializedProperty bottomRightProp = serializedObject.FindProperty("tileType_BottomRight");

        EditorGUILayout.LabelField("Tile Type Corners", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(topLeftProp, new GUIContent("Top Left"));
        EditorGUILayout.PropertyField(topRightProp, new GUIContent("Top Right"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(bottomLeftProp, new GUIContent("Bottom Left"));
        EditorGUILayout.PropertyField(bottomRightProp, new GUIContent("Bottom Right"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Далі решта полів
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isAnimate"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isRandomTile"));

        serializedObject.ApplyModifiedProperties();
    }
}
