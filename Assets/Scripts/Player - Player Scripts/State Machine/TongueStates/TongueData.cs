using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueData : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform parentTransform;
    public GameObject tongueEndPrefab;
    public Transform startOfTongueTransform;
    public void getInformation()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        parentTransform = GetComponentInParent<Transform>();
    }
}
