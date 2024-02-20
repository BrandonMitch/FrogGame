using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public Vector3 lookat = new Vector3(1, 1, 0);
    private Vector3 lookAtDefaultVector = new Vector3(1, 1, 0);
    public bool lookAtmouse = false;
    public bool lookAtDefault = true;
    private void Start()
    {
        if (lookAtmouse)
        {
            lookat = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            lookat = lookAtDefaultVector;
        }
        lookat.z = 0;
        lookAtDefaultVector.z = 0;
    }
    protected void Update()
    {
        if (lookAtDefault && (lookat != lookAtDefaultVector))
        {
            lookat = lookAtDefaultVector;
        }
        else if (lookAtmouse)
        {
            lookat = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lookat.z = 0;
        }

        Vector3 difference = lookat - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z );


    }
}
