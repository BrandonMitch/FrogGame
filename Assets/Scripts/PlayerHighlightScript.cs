using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHighlightScript : MonoBehaviour
{
    // Player Class of the actual player
    public Player player;

    private void Start()
    {
        // Set to off on start 
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        // Set the sprite to the spirte on player
        gameObject.GetComponent<SpriteRenderer>().sprite = player.playerOverlaySprite;


    }
    
}
