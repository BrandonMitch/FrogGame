using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugGrapherGUI))]
public class customScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);

        DebugGrapherGUI script = (DebugGrapherGUI)target;

        if (GUILayout.Button("Start DebugFPSGrapher"))
        {
            script.StartDebugFPSGrapher();
        }

        if (GUILayout.Button("Stop DebugerFPSGrapher"))
        {
            script.StopDebugFPSGrapher();
        }
        GUILayout.Space(20);

        if (GUILayout.Button("Start Mouse Debugger"))
        {
            script.StartDebugMouseGrapher();
        }

        if (GUILayout.Button("Stop Mouse Debugger"))
        {
            script.StopDebugMouseGrapher();
        }

    }
}
