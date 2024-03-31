using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Cast target to GameEvent
        GameEvent gameEvent = (GameEvent)target;

        // Add a button to invoke the Raise function
        if (GUILayout.Button("Raise Event"))
        {
            gameEvent.Raise();
        }
    }
}
