using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugMouse : MonoBehaviour
{
    float mouseX;
    float mouseY;


    // Update is called once per frame

    Resolution screenSize;
    private void Start()
    {
        var mousePos = Input.mousePosition;
        screenSize = Screen.currentResolution;
        DebugGUI.SetGraphProperties("mousX", "mouseX", 0, screenSize.width, 1, new Color(1f,0.3f,0.3f), true);
        DebugGUI.SetGraphProperties("mousY", "mouseY", 0, screenSize.height, 1, new Color(0f,1f,1f), true);
    }
    void Update()
    {
        var mousePos = Input.mousePosition;
        mouseX = Mathf.Clamp(mousePos.x, 0, screenSize.width);
        mouseY = Mathf.Clamp(mousePos.y, 0, screenSize.height);
        DebugGUI.Graph("mousX", mouseX);
        DebugGUI.Graph("mousY", mouseY);
    }
    void OnDestroy()
    {
        DebugGUI.RemoveGraph("mousX");
        DebugGUI.RemoveGraph("mousY");
    }
}
