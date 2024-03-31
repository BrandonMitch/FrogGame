using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(GenericStat))]
public class GenericStatEditor : Editor
{
    // Serialized Properties
/*    SerializedProperty healthProp;
    SerializedProperty attackProp;
    SerializedProperty defenseProp;

    void OnEnable()
    {
        // Initialize Serialized Properties
        healthProp = serializedObject.FindProperty("health");
        attackProp = serializedObject.FindProperty("attack");
        defenseProp = serializedObject.FindProperty("defense");
    }

    public override void OnInspectorGUI()
    {
        // Update Serialized Object
        serializedObject.Update();

        // Display fields in the Inspector
        EditorGUILayout.PropertyField(healthProp);
        EditorGUILayout.PropertyField(attackProp);
        EditorGUILayout.PropertyField(defenseProp);

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
*/
}

