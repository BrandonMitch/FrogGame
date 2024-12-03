using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Player Inventory", menuName = "Player/Player Inventory")]
public class PlayerInventory : ScriptableObject, IInventory
{
    [HideInInspector] public InventoryManager inventory;

    public bool AddItem(IInventoryItem newItem, int qty)
    {
        return inventory.AddItem(newItem, qty);
    }

    public void ClearItems()
    {
        inventory.ClearItems();
    }

    public bool hasRoomForItem(IInventoryItem newItem, int qty, int maxItemsPerStack)
    {
        return inventory.hasRoomForItem(newItem, qty, maxItemsPerStack);
    }

    public (Dictionary<IInventoryItem, int>, List<IInventoryItem>) InventoryContents()
    {
        return inventory.InventoryContents();
    }
    public IInventoryItem ItemInHand
    {
        get
        {
            return inventory.SelectedSlot.Item?.GetItem();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Print item in hand")]
    public void PrintItemInHand()
    {
        Debug.Log("Item in hand: " + ItemInHand?.GetName());
    }
#endif
}
