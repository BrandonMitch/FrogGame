using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TongueData : RopeBase, IRope
{
    public LineRenderer lineRenderer;
    public Transform parentTransform;
    public GameObject tongueEndPrefab;
    public Transform startOfTongueTransform;
    public GameObject tongueHitCollisionPrefab;
    public void getInformation()
    {   
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        parentTransform = GetComponentInParent<Transform>();

        InitializeRope();
    }
}
