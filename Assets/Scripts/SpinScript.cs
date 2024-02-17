using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public Vector3 lookat = new Vector3(1, 1);
    private Vector3 lookAtDefaultVector = new Vector3(1, 1);
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
    }
    protected void Update()
    {
        if (lookAtDefault && (lookat != lookAtDefaultVector))
        {
            lookat = lookAtDefaultVector;
        }
        else if(lookAtmouse)
        {
                lookat = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        Vector3 difference = lookat - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + 30);

    }
}
