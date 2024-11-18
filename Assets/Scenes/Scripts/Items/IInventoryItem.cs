using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem : iItem
{
    public bool isStackable();

    public IInventoryItem GetItem();
}
