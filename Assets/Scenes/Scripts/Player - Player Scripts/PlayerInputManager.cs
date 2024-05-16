using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
public class PlayerInputManager : MonoBehaviour
{
    private bool _leftMouseDown = false;
    private bool _leftMouseUp = false;
    private bool _leftMouseButton = false;
    private bool _rightMouseDown = false;
    private bool _rightMouseUp = false;
    private bool _rightMouseButton = false;
    private bool _releaseKeyDown = false;
    private Vector2 _currentMovementInputs = Vector2.zero;
    [Header("Controller Type")]
    [SerializeField] public ControllerMode controllerMode = ControllerMode.Mouse;
    public enum ControllerMode
    {
        [Description("Uses normal mouse and meyboard input")]
        Mouse,
        [Description("Uses only controller input")]
        Controller,
        [Description("Uses both mouse inputs and controller inputs, great for debugging")]
        Combined, 
    };
    [Space]
    [Header("Variable Names")]
    [SerializeField] private string leftClickButton =         "Fire1";
    [SerializeField] private string rightClickButton =        "Fire2";
    [SerializeField] private string horizontalMov =      "Horizontal";
    [SerializeField] private string verticalMov =          "Vertical";
    [SerializeField] private KeyCode releaseKey =           KeyCode.K;
    [Space]
    [SerializeField] private string leftClickCTRL =         "Fire1c1";
    [SerializeField] private string rightClickCTRL =        "Fire2c1";
    [SerializeField] private string horizontalMovCTRL ="Horizontalc1";
    [SerializeField] private string verticalMovCTRL =    "Verticalc1";
    [SerializeField] private string releaseKeyCTRL =      "Retractc1";
    [SerializeField] private string controllerXName =       "stick2X";
    [SerializeField] private string controllerYName =       "stick2Y";


    // This implementation allows us to get last input if we make get[control] return [control]
    [SerializeField] public bool isPaused = false;
    public void FrameUpdate()
    {
        // Check if the editor is in play mode and the user presses the space key
        if (Application.isPlaying && Input.GetKeyDown(KeyCode.P))
        {
            PauseUnPausePlayer();
        }
        // Move forward one frame
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.O)) 
            {
                this.StartCoroutine(AdvanceOneFixedFrame());
            } else if(Input.GetKeyDown(KeyCode.L))
            {
                this.StartCoroutine(AdvanceOneFrame());
            }
        }
    }
    public void PauseUnPausePlayer()
    {
        if (Time.timeScale == 0f)
        {
            isPaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0f;
        }
    }
    IEnumerator AdvanceOneFixedFrame()
    {
        PauseUnPausePlayer();
        yield return new WaitForFixedUpdate();
        PauseUnPausePlayer();
    }
    IEnumerator AdvanceOneFrame()
    {
        PauseUnPausePlayer();
        yield return null;
        PauseUnPausePlayer();
    }
    public bool LeftMouseDown
    {
        get {
            UpdateLeftMouseDown();
            return _leftMouseDown;
        }
    }
    public bool LeftMouseUp { 
        get {
            UpdateLeftMouseUp();
            return _leftMouseUp;
        }
    }
    public bool LeftMouseButton {   
        get {
            UpdateLeftMouseButton();
            return _leftMouseButton;
        }
    }
    public bool RightMouseDown {    
        get {
            UpdateRightMouseDown();
            return _rightMouseButton;
        }
    }
    public bool RightMouseUp {      
        get {
            UpdateRightMouseUp();
            return _rightMouseUp;
        }
    }
    public bool RightMouseButton {  
        get {
            UpdateRightMouseButton();
            return _rightMouseButton;
        }
    }
    public bool ReleaseKeyDown {
        get {
            UpdateReleaseKeyDown();
            return _releaseKeyDown;
        }
    }
    public Vector2 CurrentMovementInputs {
        get
        {
            Vector2 movVec;
            switch (controllerMode)
            {
                case ControllerMode.Mouse:
                    movVec = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw(horizontalMov), Input.GetAxisRaw(verticalMov)), 1.0f);
                    break;
                case ControllerMode.Controller:
                    movVec = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw(horizontalMovCTRL), Input.GetAxisRaw(verticalMovCTRL)), 1.0f);
                    break;
                case ControllerMode.Combined:
                    Vector2 vec1 = new Vector2(Input.GetAxisRaw(horizontalMov), Input.GetAxisRaw(verticalMov));
                    Vector2 vec2 = new Vector2(Input.GetAxisRaw(horizontalMovCTRL), Input.GetAxisRaw(verticalMovCTRL));
                    movVec = Vector2.ClampMagnitude(vec1 + vec2, 1.0f);
                    break;
                default:
                    InvalidControllerMode();
                    Debug.Break();
                    return Vector2.zero;
            }
            return movVec;
        }
    }

    public float ControllerX {
        get{
            return Input.GetAxis(controllerXName);
        }
    }
    public float ControllerY {
        get{
            return Input.GetAxis(controllerYName);
        }
    }
    private void UpdateLeftMouseDown()
    {
        // Attack is started
        string inputName;
        switch (controllerMode)
        {
            case ControllerMode.Mouse:
                inputName = leftClickButton;
                break;
            case ControllerMode.Controller:
                inputName = leftClickCTRL;
                break;
            case ControllerMode.Combined:
                _leftMouseDown = Input.GetButtonDown(leftClickButton) || Input.GetButtonDown(leftClickCTRL);
                return;
            default:
                InvalidControllerMode();
                return;
        }
        _leftMouseDown = Input.GetButtonDown(inputName);
    }
    private void UpdateLeftMouseButton()
    {
        // Trying to charge
        string inputName;
        switch (controllerMode)
        {
            case ControllerMode.Mouse:
                inputName = leftClickButton;
                break;
            case ControllerMode.Controller:
                inputName = leftClickCTRL;
                break;
            case ControllerMode.Combined:
                _leftMouseButton = Input.GetButton(leftClickButton) || Input.GetButton(leftClickCTRL);
                return;
            default:
                InvalidControllerMode();
                return;
        }
        _leftMouseButton = Input.GetButton(inputName);
    }
    private void UpdateLeftMouseUp()
    {
        // Trying to charge
        string inputName;
        switch (controllerMode)
        {
            case ControllerMode.Mouse:
                inputName = leftClickButton;
                break;
            case ControllerMode.Controller:
                inputName = leftClickCTRL;
                break;
            case ControllerMode.Combined:
                _leftMouseUp = Input.GetButtonUp(leftClickButton) || Input.GetButtonUp(leftClickCTRL);
                return;
            default:
                InvalidControllerMode();
                return;
        }
        _leftMouseUp = Input.GetButtonUp(inputName);
    }
    private void UpdateRightMouseUp()
    {
        // Spitting out the tongue on release
        string inputName;
        switch (controllerMode)
        {
            case ControllerMode.Mouse:
                inputName = rightClickButton;
                break;
            case ControllerMode.Controller:
                inputName = rightClickCTRL;
                break;
            case ControllerMode.Combined:
                _rightMouseUp = Input.GetButtonUp(rightClickButton) || Input.GetButtonUp(rightClickCTRL);
                return;
            default:
                InvalidControllerMode();
                return;
        }
        _rightMouseUp = Input.GetButtonUp(inputName);
    }
    private void UpdateRightMouseDown()
    {
        // Start aiming the tongue
        string inputName;
        switch (controllerMode)
        {
            case ControllerMode.Mouse:
                inputName = rightClickButton;
                break;
            case ControllerMode.Controller:
                inputName = rightClickCTRL;
                break;
            case ControllerMode.Combined:
                _rightMouseDown = Input.GetButtonDown(rightClickButton) || Input.GetButtonDown(rightClickCTRL);
                return;
            default:
                InvalidControllerMode();
                return;
        }
        _rightMouseDown = Input.GetButtonDown(inputName);
    }
    private void UpdateRightMouseButton()
    {
        // Aiming the tongue
        string inputName;
        switch (controllerMode)
        {
            case ControllerMode.Mouse:
                inputName = rightClickButton;
                break;
            case ControllerMode.Controller:
                inputName = rightClickCTRL;
                break;
            case ControllerMode.Combined:
                _rightMouseButton = Input.GetButton(rightClickButton) || Input.GetButton(rightClickCTRL);
                return;
            default:
                InvalidControllerMode();
                return;
        }
        _rightMouseButton = Input.GetButton(inputName);
    }
    private void UpdateReleaseKeyDown()
    {
        switch (controllerMode)
        {
            case ControllerMode.Mouse:
                _releaseKeyDown = Input.GetKeyDown(releaseKey);
                return;
            case ControllerMode.Controller:
                _releaseKeyDown = Input.GetAxis(releaseKeyCTRL) > 0;
                return;
            case ControllerMode.Combined:
                _releaseKeyDown = Input.GetKeyDown(releaseKey)|| (Input.GetAxis(releaseKeyCTRL) > 0);
                return;
            default:
                InvalidControllerMode();
                return;
        }
    }

    private void InvalidControllerMode()
    {
        Debug.LogError("Invalid controller mode: " + controllerMode.ToString() + controllerMode.GetHashCode());
    }
}
