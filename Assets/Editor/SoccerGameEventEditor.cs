using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoccerGameEvent))]
public class SoccerGameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Cast target to your derived class type
        SoccerGameEvent derivedGameEvent = (SoccerGameEvent)target;

        // Add a button to invoke the Raise function
        if (GUILayout.Button("Raise Event"))
        {
            derivedGameEvent.Raise();
        }
    }
}