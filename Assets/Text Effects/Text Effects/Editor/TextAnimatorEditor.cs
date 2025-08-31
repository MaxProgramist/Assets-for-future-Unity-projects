using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextAnimator))]
public class TextAnimatorEditor : Editor
{
    private TextAnimator animator;

    void OnEnable()
    {
        animator = (TextAnimator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effects Manager", EditorStyles.boldLabel);

        if (GUILayout.Button("+ Add Effect"))
        {
            GenericMenu menu = new GenericMenu();

            string[] guids = AssetDatabase.FindAssets("t:TextEffect");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextEffect asset = AssetDatabase.LoadAssetAtPath<TextEffect>(path);

                if (animator.GetEffects().Contains(asset)) continue;

                menu.AddItem(new GUIContent(asset.name), false, () =>
                {
                    animator.GetEffects().Add(asset);
                    EditorUtility.SetDirty(animator);
                });
            }

            if (menu.GetItemCount() == 0)
                menu.AddDisabledItem(new GUIContent("No more effects available"));

            menu.ShowAsContext();
        }

        if (animator.GetEffects().Count > 0)
        {
            for (int i = 0; i < animator.GetEffects().Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUIStyle bigLabel = new GUIStyle(EditorStyles.boldLabel);
                bigLabel.fontSize = 12;

                var effect = animator.GetEffects()[i];
                if (effect != null)
                    EditorGUILayout.LabelField(effect.name, bigLabel);
                else
                    EditorGUILayout.LabelField("Missing Effect", bigLabel);


                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    animator.GetEffects().RemoveAt(i);
                    EditorUtility.SetDirty(animator);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
