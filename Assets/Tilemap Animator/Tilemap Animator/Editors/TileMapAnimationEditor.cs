using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMapAnimation))]
public class TileMapAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileMapAnimation myScript = (TileMapAnimation)target;
        if (GUILayout.Button("Intro Animation"))
        {
            myScript.IntroAnimation();
        }

        if (GUILayout.Button("Outro Animation"))
        {
            myScript.OutroAnimation();
        }
    }
}
