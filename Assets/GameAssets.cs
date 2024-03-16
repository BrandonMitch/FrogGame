using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From code monkey tutorial
// Referencing Assets through code | Unity Tutorial
public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;
    public static GameAssets instance
    {
        get {
            if (_instance == null) _instance = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _instance;
        }
    }

}
