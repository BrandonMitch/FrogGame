using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class TongueStateMachine 
{
    public TongueState CurrentTongueState { get; set; }
    [SerializeField]  public GameObject tongueEndPrefab { get; set; }
    [SerializeField]  public GameObject tongeHitCollisionPrefab;

    [SerializeField]  private TongueData tongueData;
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public Transform parentTransform = null;

    public GameObject endOfTongue;
    public Rigidbody2D endOfTongueRB;

    public Vector3 aimLocation;

    private ArrayList tonguePoints = new ArrayList();
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
        tongeHitCollisionPrefab = tongueData.tongueHitCollisionPrefab;
        InitializeBaseOfTongue();

        CurrentTongueState = startingState;
        CurrentTongueState.EnterState();
        activeCoroutine = false;
    }


    public void ChangeState(TongueState newState)
    {
        CurrentTongueState.ExitState();
        CurrentTongueState = newState;
        CurrentTongueState.EnterState();
    }

    public void TurnOffEndOfTongueRB()
    {
        endOfTongueRB.simulated = false;
    }
    public void DestroyEndOfTongue()
    {
        Debug.Log("Destroy end of tongue called"); 
        if (endOfTongue != null)
        {
            GameObject.Destroy(endOfTongue);
            endOfTongue = null;
        }


        if (tonguePoints.Count == 2)
        {
            Debug.Log("Trying to destryoing the end of the tognue");
            TongueHitData lastElement = (TongueHitData)tonguePoints[1];
            if (lastElement.type != TonguePointType.endOfTongue) { Debug.LogError("End of tongue is not the correct type"); return; }
            tonguePoints.RemoveAt(1); // this clears the tongue points lists
        }
        else {
            if (tonguePoints.Count > 2)
            {
                Debug.LogError("error in trying to destroy end of tongue, the end is destroyed before all the mid points are removed");
            }
            else
            {
                if(tonguePoints.Count == 1)
                {
                    return; // All tongue points already destroyed
                }
            }
        }

    }

    public bool isTongueRetracting()
    {
        return CurrentTongueState.isRetracting();
    }
    public bool isTongueOff()
    {
        return CurrentTongueState.isOff();
    }
    public bool isTongueAiming()
    {
        return CurrentTongueState.isAiming();
    }
    /* IntializeTongueStates is used for intializing tongue states that need certain transforms on the start call instead of the awake call. Typically used to grab transforms and lineRenderer
     *  @param states : is a list of tongue states that you want to be intialized
     *  Intialize() must be implemented in the tonguestate
     */
    // TODO: make private
    public void IntializeTongueStates(TongueState[] states)
    {
        foreach (TongueState state in states)
        {
            state.Intialize();
        }
    }

    #region Tongue Collision Point Managment
    // the base of the tongue is point 0 on the line renderer,
    // the rotation point of the tongue is always point 0 on the line renderer.
    private void InitializeBaseOfTongue()
    { 
        tonguePoints.Add(new TongueHitData(parentTransform, TonguePointType.baseOfTongue)); // the base of the tongue is point 0 on the line renderer,
    }
    public GameObject IntializeEndOfTongue()
    {
        if (isTongueRetracting()) { return null; } 
        if (endOfTongue != null) Debug.LogError("problem in trying to make new end of tongue, this object should be null");

        // Instantiate new end of tongue.

        endOfTongue = GameObject.Instantiate(tongueEndPrefab, parentTransform.position, Quaternion.identity);

        // Now we need to add this to array list containing all of the tongue points.
        tonguePoints.Add(new TongueHitData(endOfTongue.transform, TonguePointType.endOfTongue, endOfTongue));

        // Now we need to set the rigid body to the new end of tongue, and shut it off
        endOfTongueRB = endOfTongue.GetComponent<Rigidbody2D>();
        endOfTongueRB.simulated = false;
        return endOfTongue;
    }
    public void AddNewTongueCollisionPoint(TongueHitData newTonguePoint)
    {
        tonguePoints.Insert(1, newTonguePoint);
        lineRenderer.positionCount++;
    }
    public void DestroyTongueMidPoint(float t)
    {
        int points = tonguePoints.Count;
        if (points > 2)
        {
            TongueHitData lastElement = (TongueHitData)tonguePoints[points - 2];
            if (lastElement != null && lastElement.DestroyPoint())
            {
                if (lastElement.type != TonguePointType.tongueHitPoint){Debug.LogError("Tongue midpoints is not the correct type");return;} 
                GameObject TgameObject = lastElement.getGameObject();
                if (TgameObject != null)
                {
                    tonguePoints.Remove(lastElement); // Remove from the list
                    GameObject.Destroy(TgameObject, t); // Destroy the GameObject
                    lineRenderer.positionCount--;
                }
                else
                {
                    Debug.LogWarning("GameObject is null, cannot destroy.");

                }
                return;
            }
            else
            {
                Debug.LogError("Failed to destroy last tongue point.");
            }
        }
        else
        {
            Debug.LogError("Error in trying to destroy tongue mid point: Not enough points.");
        }
    }

    #region Rendering the tongue
    bool debugRenderer = true;
    bool activeCoroutine = false;
    bool tongueRenderIsStarted = false;
    Coroutine drawTongueCoroutine = null;
    public void TwoPointTongueRenderer()
    {

        // Don't do anything if there is only one tongue point.
        int nPoints = tonguePoints.Count;
        if (nPoints == 1) { return; }

        // We need to get the player to start a coroutine.
        // This is because the rendering had some issues if we don't wait for the fixed update.
        // This caused the tongue position to lag behind the rigid body and make the tongue float ahead or behind the player
        Player player = CurrentTongueState.GetPlayer();
        if (!activeCoroutine)
        {
            drawTongueCoroutine = player.StartCoroutine(drawTongue(nPoints, player));
        }
    }
    public void StartTongueRenderer()
    {
        tongueRenderIsStarted = true;
    }
    public void StopTongueRenderer()
    {
        tongueRenderIsStarted = false;
        TurnOffLineRenderer();
    }
    private void TurnOffLineRenderer()
    {
        // Stop the tongue renderer coroutine so it does not play next frame
        if (activeCoroutine && drawTongueCoroutine != null) {
            CurrentTongueState.GetPlayer().StopCoroutine(drawTongueCoroutine);
        }
        activeCoroutine = false;
        drawTongueCoroutine = null;

        // Shut off the renderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
            tongueRenderIsStarted = false;
        }
        else
        {
            Debug.LogError("Line renderer null");
        }
    }
    private void TurnOnLineRenderer()
    {
        lineRenderer.enabled = true;
    }
    IEnumerator drawTongue(int nPoints, Player player)
    {
        activeCoroutine = true;
        yield return new WaitForFixedUpdate();
        if (debugRenderer)
        {
            Tracer.DrawCircle(parentTransform.position, 0.05f, 5, Color.yellow);
            Tracer.DrawCircle(player.GetPlayerRigidBody().position, 0.05f, 7, Color.green);
        }
        // First turn on the tongue renderer
        if (tongueRenderIsStarted)
        {
            if (nPoints > 2)
            {
                MultiPointTongueRenderer(nPoints);
            }
            else
            {
                lineRenderer.SetPosition(0, parentTransform.position);
                lineRenderer.SetPosition(tonguePoints.Count - 1, endOfTongue.transform.position);
            }
            // TODO: Doesn't need to be called every frame but requires more switches
            TurnOnLineRenderer();
        }
        activeCoroutine = false;
        drawTongueCoroutine = null;
    }
    private void MultiPointTongueRenderer(int nPoints)
    {
        lineRenderer.SetPosition(0, parentTransform.position);
        lineRenderer.SetPosition(tonguePoints.Count - 1, endOfTongue.transform.position);
        for (int i = 1; (i < (nPoints - 1)) && (i < lineRenderer.positionCount); i++)
        {
            TongueHitData point = (TongueHitData)tonguePoints[i];
            lineRenderer.SetPosition(i, point.getPos());
        }
    }

    #endregion
    public TongueHitData GetPointBeforeEndOfTongue()
    {
        TongueHitData p = (TongueHitData)tonguePoints[tonguePoints.Count - 2];
        return p;
    }

    public TongueHitData GetRotationPoint()
    {
        TongueHitData p = (TongueHitData)tonguePoints[1];
        return p;
    }
    public RopeBase getRope()
    {
        return (RopeBase)tongueData;
    }

    public Vector3 GetParentTransformPosition()
    {
        return parentTransform.position;
    }
    public Vector3 GetEndOfTongueTransformPosition()
    {
        return endOfTongue.transform.position;
    }
    public Transform GetEndOfTongueTransform()
    {
        return endOfTongue.transform;
    }
    public Transform GetParentTransform()
    {
        if (parentTransform == null)
        {
            return null;
        }
        else
        {
            return parentTransform;
        }
    }

    private void DestroyWholeTongueBesidesBase()
    {
        int points = tonguePoints.Count;
        if(points == 1) { return;  } // don't destroy base of tongue
        if(points == 2) { DestroyEndOfTongue(); return; } // the only point left should be end of tongue
        while(TongueMidPointCount() > 0) // Destroy all mid points
        {
            DestroyTongueMidPoint(0);
        }
        points = tonguePoints.Count;
        if(points == 2)
        {
            DestroyEndOfTongue();
        }
    }
    public void EnterOffStateImmediately(Player player)
    {
        DestroyWholeTongueBesidesBase();
        player.tongueStateMachine.ChangeState(player.tongueOffState);
    }
    private int TongueMidPointCount()
    {
        int points = tonguePoints.Count;
        int midPoints = points - 2;
        if(midPoints < 0)
        {
            return 0;
        }
        else
        {
            return midPoints;
        }
    }
    public int TonguePointCount()
    {
        return tonguePoints.Count;
    }
    #endregion
}

