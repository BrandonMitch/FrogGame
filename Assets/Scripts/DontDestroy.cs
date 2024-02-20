using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        //Debug.Log(this.name +" --- " + GetInstanceID() );
        DontDestroyOnLoad(gameObject);

    }
}
