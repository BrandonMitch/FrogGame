using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventory 
{
    public bool hasRoomForItem(IInventoryItem newItem, int qty, int maxItemsPerStack);
    public bool AddItem(IInventoryItem newItem, int qty);
    public (Dictionary<IInventoryItem, int>, List<IInventoryItem>) InventoryContents();

    public void ClearItems();

}
