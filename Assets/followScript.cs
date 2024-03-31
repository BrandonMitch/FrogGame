using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followScript : MonoBehaviour
{
    public GameObject soccerBall;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = soccerBall.transform.position + new Vector3(0, -0.18f, 0) ;
    }
}
