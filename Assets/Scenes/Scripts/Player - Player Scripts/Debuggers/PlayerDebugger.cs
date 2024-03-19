using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebugger : MonoBehaviour
{
    private GameObject playerObject = null;
    private Player player = null;
    private TongueStateMachine tongueStateMachine;
    private PlayerStateMachine stateMachine;
    void Start()
    {
        playerObject = GameObject.FindWithTag("Player");

        if(playerObject != null)
        {
            Debug.Log("found player");
            player = playerObject.GetComponent<Player>();
            stateMachine = player.stateMachine;
            tongueStateMachine = player.tongueStateMachine;
        }
        else
        {
            Debug.LogError("did not find player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
