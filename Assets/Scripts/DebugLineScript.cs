using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLineScript : MonoBehaviour
{
    public Player player;
    public float timeOnScreen = 0.5f;
    Vector3 mousPos;
    Vector3 playerPos;
    Vector3 xy0 = new Vector3(1, 1, 0);
    void Update() 
    { 
        if (Input.GetButtonDown("Fire1"))
        {
            mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousPos.z = 0;
            playerPos = player.transform.position;
            Vector3 normPos = (mousPos - playerPos).normalized;
            Debug.DrawLine(playerPos,playerPos + normPos, Color.white,1.0f);
            Debug.Log("(X" + normPos.x + ",Y" + normPos.y +")");
        }
    }
}
