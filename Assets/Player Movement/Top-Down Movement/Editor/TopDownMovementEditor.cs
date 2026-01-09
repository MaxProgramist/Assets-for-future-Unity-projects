using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TopDownMovement))]
public class TopDownMovementEditor : Editor
{
    SerializedProperty movementType;

    SerializedProperty movementMaxSpeed;
    SerializedProperty movementAcceleration;
    SerializedProperty movementDeceleration;
    SerializedProperty movementDirections;

    SerializedProperty lenghtOfStep;
    SerializedProperty timeBtwSteps;
    SerializedProperty movementAnimationCurve;
    SerializedProperty movementAnimationDuration;
    SerializedProperty wallLayer;

    SerializedProperty rigidbody;

    void OnEnable()
    {
        movementType = serializedObject.FindProperty("movementType");

        movementMaxSpeed = serializedObject.FindProperty("movementMaxSpeed");
        movementAcceleration = serializedObject.FindProperty("movementAcceleration");
        movementDeceleration = serializedObject.FindProperty("movementDeceleration");
        movementDirections = serializedObject.FindProperty("movementDirections");

        lenghtOfStep = serializedObject.FindProperty("lenghtOfStep");
        timeBtwSteps = serializedObject.FindProperty("timeBtwSteps");
        movementAnimationCurve = serializedObject.FindProperty("movementAnimationCurve");
        movementAnimationDuration = serializedObject.FindProperty("movementAnimationDuration");
        wallLayer = serializedObject.FindProperty("wallLayer");

        rigidbody = serializedObject.FindProperty("rigidbody");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(movementType);
        EditorGUILayout.Space(6);

        TopDownMovement.MovementType currentMovementType =
            (TopDownMovement.MovementType)movementType.enumValueIndex;

        if (currentMovementType == TopDownMovement.MovementType.Continuously)
        {
            DrawContinuouslySettings();
        }
        else if (currentMovementType == TopDownMovement.MovementType.StepByStep)
        {
            DrawStepByStepSettings();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(rigidbody);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawContinuouslySettings()
    {
        EditorGUILayout.PropertyField(movementMaxSpeed);
        EditorGUILayout.PropertyField(movementAcceleration);
        EditorGUILayout.PropertyField(movementDeceleration);
        EditorGUILayout.PropertyField(movementDirections);
    }

    private void DrawStepByStepSettings()
    {
        EditorGUILayout.PropertyField(lenghtOfStep);
        EditorGUILayout.PropertyField(timeBtwSteps);
        EditorGUILayout.PropertyField(movementAnimationCurve);
        EditorGUILayout.PropertyField(movementAnimationDuration);
        EditorGUILayout.PropertyField(wallLayer);
    }
}
