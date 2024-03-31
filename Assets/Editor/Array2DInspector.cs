using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSOValues))]
public class Array2DInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSOValues script = (GridSOValues)target;

        // Check if the array is not null
        if (script.values != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("2D Array Contents", EditorStyles.boldLabel);

            // Display the array contents
            for (int i = 0; i < script.values.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < script.values[i].Length; j++)
                {
                    EditorGUIUtility.labelWidth = 10f;
                    script.values[i][j] = EditorGUILayout.IntField(i + "," + j, script.values[i][j]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            AssetDatabase.SaveAssets(); // Save the changes to the asset
            Repaint();
        }
    }
}
