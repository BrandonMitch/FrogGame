using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // TODO: There is an error when trying to set active
    // NOTE: May be useful to make a hotbar slot array and a full inventory slot arrary and then combine them
    public int maxStackedItems = 4;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    int selectedSlot = -1;
    private void Start()
    {
        ChangeSelectedSlot(0);
    }
    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }
    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if(isNumber && number > 0 && number < 7)
            {
                ChangeSelectedSlot(number - 1);
            }
        }
    }
    // When we add a new item, we will search the inventory for an empty slot
    public bool AddItem(IInventoryItem item)
    {
        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.GetItem() != null &&
                itemInSlot.GetItem().Equals(item) &&
                itemInSlot.count < maxStackedItems &&
                itemInSlot.GetItem().isStackable() == true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        // Find an empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                // If the item in slot is null, then we can spawn the new item in the slots
                SpawnNewItem(item, slot, count:1);
                return true;
            }   
        }
        return false;
    }
    public bool AddItem(IInventoryItem item, int count)
    {
        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.GetItem() != null &&
                itemInSlot.GetItem().Equals(item) &&
                itemInSlot.count + count < maxStackedItems &&
                itemInSlot.GetItem().isStackable() == true)
            {
                itemInSlot.count += count;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        // Find an empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            // item slot not intiatialized
            if (itemInSlot == null)
            {
                // If the item in slot is null, then we can spawn the new item in the slots
                SpawnNewItem(item, slot, count);
                return true;
            }
            else // fill the item in the slot if there it is a null item
            {
                if(itemInSlot.GetItem() == null)
                {
                    itemInSlot.InitializeItem(item, count);
                }
                else
                {
                    continue; // there's already an item in there
                }
            }

        }
        return false;
    }
    [ContextMenu("ReorganizeInventory()")]
    public void ReorganizeInventory()
    {
        Dictionary<IInventoryItem, int> itemCounts;
        List<IInventoryItem> unstackable;
        (itemCounts, unstackable) = InventoryContents();

        ClearInventoryy();

        // add all stackable items first
        foreach (var kvp in itemCounts)
        {
            AddItem(kvp.Key, kvp.Value);
        }

        foreach (var item in unstackable)
        {
            AddItem(item);
        }
    }
    void SpawnNewItem(IInventoryItem item, InventorySlot slot, int count = 1)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        // for the new game object created, we need to get the inventory item script
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        // On the new InventoryItem script created, intialize the item
        inventoryItem.InitializeItem(item, count);
    }


    public (Dictionary<IInventoryItem, int>, List<IInventoryItem>) InventoryContents()
    {
        Dictionary<IInventoryItem, int> itemCounts = new();
        List<IInventoryItem> unstackable = new();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot == null) { continue; }
            if (!itemInSlot.isStackable())
            {
                unstackable.Add((Item) itemInSlot);
            }
            else
            {
                // is stackable
                IInventoryItem item = itemInSlot.GetItem();
                if (itemCounts.ContainsKey(item))
                {
                    itemCounts[item] += itemInSlot.count;
                }
                else
                {
                    itemCounts[item] = itemInSlot.count;
                }
            }
        }
        return (itemCounts, unstackable);
    }

    public string InventoryContentsString()
    {
        Dictionary<IInventoryItem, int> itemCounts;
        List<IInventoryItem> unstackable;
        (itemCounts, unstackable) = InventoryContents();
        string s = "{\n";
        foreach (var kvp in itemCounts)
        {
            s += $"{kvp.Value}x {kvp.Key.GetName()}\n";

        }
        s += "}\n";
        foreach (var item in unstackable)
        {
            s += $"{item.GetName()}, ";
        }
        s = s.Trim(' ').Trim(',');

        return s;
    }



    [ContextMenu("Clear Inventory()")]
    public void ClearInventoryy()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                itemInSlot.ClearItem();
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("UpdateUI()")]
    public void UpdateUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                itemInSlot.UpdateUI();
            }
        }
    }
    [ContextMenu("Print Inventory Contents")]
    public void PrintInventoryContents()
    {
        Debug.Log(InventoryContentsString());
    }
#endif
}
