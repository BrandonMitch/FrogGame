using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueStateMachine 
{
    public TongueState CurrentTongueState { get; set; }
    [SerializeField] public GameObject tongueEndPrefab { get; set; }

    [SerializeField]  private TongueData tongueData;
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public Transform parentTransform;

    public GameObject endOfTongue;
    public Rigidbody2D endOfTongueRB;
    public enum LatchMovementType
    {
        None,
        Waiting,
        LungeForward,
        LungeLeft,
        LungeRight,
        LungeBack,
    };
    [SerializeField] public LatchMovementType movementType;

    public Vector3 aimLocation;


    public TongueStateMachine(TongueData tongueData)
    {
        this.tongueData = tongueData;
    }
    public void Intialize(TongueState startingState)
    {
        tongueData.getInformation(); // get information from mono behavior
        lineRenderer = tongueData.lineRenderer;
        parentTransform = tongueData.parentTransform;
        tongueEndPrefab = tongueData.tongueEndPrefab;

        CurrentTongueState = startingState;
        CurrentTongueState.EnterState();
    }
    public void ChangeState(TongueState newState)
    {
        CurrentTongueState.ExitState();
        CurrentTongueState = newState;
        CurrentTongueState.EnterState();
    }
    public void TurnOffLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            Debug.Log("Line renderer null, try again");
        }
    }
}

