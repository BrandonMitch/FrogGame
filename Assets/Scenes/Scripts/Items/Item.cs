using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Item/ItemBase")]
public class Item : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite image;

    [Header("Only UI")]
    public bool stackable = false;

    public override string ToString()
    {
        string s = name;
        return s;
    }
}
