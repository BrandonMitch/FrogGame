using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugerFPS : MonoBehaviour
{
    /* * * *
    * 
    *   [DebugGUIGraph]
    *   Renders the variable in a graph on-screen. Attribute based graphs will updates every Update.
    *    Lets you optionally define:
    *        max, min  - The range of displayed values
    *        r, g, b   - The RGB color of the graph (0~1)
    *        group     - Graphs can be grouped into the same window and overlaid
    *        autoScale - If true the graph will readjust min/max to fit the data
    *   
    *   [DebugGUIPrint]
    *    Draws the current variable continuously on-screen as 
    *    $"{GameObject name} {variable name}: {value}"
    *   
    *   For more control, these features can be accessed manually.
    *    DebugGUI.SetGraphProperties(key, ...) - Set the properties of the graph with the provided key
    *    DebugGUI.Graph(key, value)            - Push a value to the graph
    *    DebugGUI.LogPersistent(key, value)    - Print a persistent log entry on screen
    *    DebugGUI.Log(value)                   - Print a temporary log entry on screen
    *    
    *   See DebugGUI.cs for more info
    * 
    * * * */

    Queue<float> deltaTimeBuffer = new();
    float smoothDeltaTime => deltaTimeBuffer.Sum() / deltaTimeBuffer.Count;
    void Awake()
    {
        // Init smooth DT
        for (int i = 0; i < 10; i++)
        {
            deltaTimeBuffer.Enqueue(0);
        }

        // Set propertie using graph key
        // (key,label, min, max, group, color, autoscale)
        DebugGUI.SetGraphProperties("smoothFrameRate", "SmoothFPS", 0, 200, 5, new Color(0, 1, 1), false);
        DebugGUI.SetGraphProperties("frameRate", "FPS", 0, 200, 5, new Color(1, 0.5f, 1), false);
    }

    // Update is called once per frame
    void Update()
    {
        // Update smooth delta time queue
        deltaTimeBuffer.Dequeue();
        deltaTimeBuffer.Enqueue(Time.deltaTime);

        // Manual persistent logging
        DebugGUI.LogPersistent("smoothFrameRate", "SmoothFPS: " + (1 / smoothDeltaTime).ToString("F3"));
        DebugGUI.LogPersistent("frameRate", "FPS: " + (1 / Time.deltaTime).ToString("F3"));

        if (smoothDeltaTime != 0)
        {
            DebugGUI.Graph("smoothFrameRate", 1 / smoothDeltaTime);
        }
        if (Time.deltaTime != 0)
        {
            DebugGUI.Graph("frameRate", 1 / Time.deltaTime);
        }

    }
    void OnDestroy()
    {
        // Clean up our logs and graphs when this object leaves tree
        DebugGUI.RemoveGraph("frameRate");
        DebugGUI.RemoveGraph("smoothFrameRate");

        DebugGUI.RemovePersistent("frameRate");
        DebugGUI.RemovePersistent("smoothFrameRate");
    }

}
