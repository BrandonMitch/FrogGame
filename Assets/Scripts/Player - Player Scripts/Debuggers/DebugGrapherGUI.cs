using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugGrapherGUI : MonoBehaviour
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

    [DebugGUIGraph(group: 0, min: -1, max: 1, r: 0, g: 1, b: 0, autoScale: true)]
    float SinField;


    #region FPS Grapher
    [SerializeField] private GameObject DebugGrapherFPSPrefab;
    private GameObject DebugFPSGrapher = null;
    [SerializeField] private bool launchWithFPSGrapher = true;

    [ContextMenu("Start DebugFPSGrapher")]
    public void StartDebugFPSGrapher()
    {
        if (DebugFPSGrapher == null)
        {
            DebugFPSGrapher = GameObject.Instantiate(DebugGrapherFPSPrefab, transform);
        }
    }
    [ContextMenu("Stop DebugerFPSGrapher")]
    public void StopDebugFPSGrapher()
    {
        if (DebugFPSGrapher != null)
        {
            Destroy(DebugFPSGrapher);
            DebugFPSGrapher = null;
        }
    }
    #endregion

    #region Mouse Grapher
    [SerializeField] private GameObject DebugGrapherMousePrefab;
    private GameObject DebugMouseGrapher = null;
    [SerializeField] private bool launchWithMouseGrapher = true;
    [ContextMenu("Start DebugMouseGrapher")]
    public void StartDebugMouseGrapher()
    {
        if (DebugMouseGrapher == null)
        {
            DebugMouseGrapher = GameObject.Instantiate(DebugGrapherMousePrefab, transform);
        }
    }
    [ContextMenu("Stop DebugMouseGrapher")]
    public void StopDebugMouseGrapher()
    {
        if (DebugMouseGrapher != null)
        {
            Destroy(DebugMouseGrapher);
            DebugMouseGrapher = null;
        }
    }
    #endregion

    void Awake()
    {

        // Log (as opposed to LogPersistent) will disappear automatically after some time.
        DebugGUI.Log("Hello! I will disappear after some time!");
    }
    private void Start()
    {
        if (launchWithFPSGrapher) StartDebugFPSGrapher();
        if (launchWithMouseGrapher) StartDebugMouseGrapher();
    }
    private void Update()
    {
        // Update the fields our attributes are graphing
        SinField = Mathf.Sin(Time.time * 6);

        if (Input.GetKeyDown(KeyCode.P))
        {
            Destroy(this);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(DebugGUI.ExportGraphs());
        }
    }

    void OnDestroy()
    {

    }

}
