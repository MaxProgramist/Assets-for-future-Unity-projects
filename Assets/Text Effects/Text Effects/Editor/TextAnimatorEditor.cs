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

        if (GUILayout.Button("➕ Add Effect"))
        {
            GenericMenu menu = new GenericMenu();

            var effectTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TextEffect).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in effectTypes)
            {
                if (animator.GetEffects().Any(e => e != null && e.GetType() == type))
                    continue;

                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    TextEffect newEffect = CreateInstance(type) as TextEffect;
                    animator.GetEffects().Add(newEffect);

                    Undo.RegisterCreatedObjectUndo(newEffect, "Add Effect");
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

                if (animator.GetEffects()[i] != null)
                    EditorGUILayout.LabelField(animator.GetEffects()[i].GetType().Name, bigLabel);
                else
                    EditorGUILayout.LabelField("Missing Effect", bigLabel);

                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    Undo.DestroyObjectImmediate(animator.GetEffects()[i]);
                    animator.GetEffects().RemoveAt(i);
                    EditorUtility.SetDirty(animator);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
