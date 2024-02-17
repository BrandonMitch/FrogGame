using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Collectable
{
    public Sprite emptyChest;
    public int pesosAmount = 5;

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            GameManager.instance.pesos += pesosAmount;
            GameManager.instance.ShowText("+" + pesosAmount + " pesos!", 25, new Color(0f, 0.5f, 0.5f), transform.position, Vector3.up * 25 /*every second, 50 pixels in hieght*/, 1.0f);
        }
    }
}
