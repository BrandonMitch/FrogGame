using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Item/ItemBase")]
public class Item : ScriptableObject, IInventoryItem
{
    public new string name;
    public string description;
    public Sprite image;

    [Header("Only UI")]
    public bool stackable = false;

    #region Inventory Item Implementation
    public string GetDescription()
    {
        return description;
    }

    public IInventoryItem GetItem()
    {
        return this;
    }

    public string GetName()
    {
        return name;
    }

    public Sprite GetSprite()
    {
        return image;
    }

    public bool isStackable()
    {
        return stackable;
    }
    #endregion
    public override string ToString()
    {
        string s = name;
        return s;
    }


}
