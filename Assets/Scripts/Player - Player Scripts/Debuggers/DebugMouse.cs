using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugMouse : MonoBehaviour
{
    float mouseX;
    float mouseY;
    float joyStick2x;
    float joyStick2y;

    // Update is called once per frame

    Resolution screenSize;
    private void Start()
    {
        var mousePos = Input.mousePosition;
        screenSize = Screen.currentResolution;
        DebugGUI.SetGraphProperties("mousX", "mouseX", 0, screenSize.width, 1, new Color(1f,0.3f,0.3f), true);
        DebugGUI.SetGraphProperties("mousY", "mouseY", 0, screenSize.height, 1, new Color(0f,1f,1f), true);
        DebugGUI.SetGraphProperties("joystick2x", "jostick2X", 0, screenSize.width, 1, new Color(1f, 0f, 0.3f), true);
        DebugGUI.SetGraphProperties("joystick2y", "joystick2Y", 0, screenSize.height, 1, new Color(0f, 0f, 1f), true);
    }
    void Update()
    {
        var mousePos = Input.mousePosition;
        mouseX = Mathf.Clamp(mousePos.x, 0, screenSize.width);
        mouseY = Mathf.Clamp(mousePos.y, 0, screenSize.height);
        joyStick2x = Input.GetAxis("stick2X");
        joyStick2y = -1*Input.GetAxis("stick2Y");
        DebugGUI.Graph("mousX", mouseX);
        DebugGUI.Graph("mousY", mouseY);
        DebugGUI.Graph("joystick2x", joyStick2x);
        DebugGUI.Graph("joystick2y", joyStick2y);
    }
    void OnDestroy()
    {
        DebugGUI.RemoveGraph("mousX");
        DebugGUI.RemoveGraph("mousY");
        DebugGUI.RemoveGraph("joystick2X");
        DebugGUI.RemoveGraph("joystick2Y");
    }
}
