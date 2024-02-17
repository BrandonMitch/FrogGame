using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
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
    public bool AddItem(Item item)
    {
        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.item == item &&
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable == true)
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
                SpawnNewItem(item, slot);
                return true;
            }   
        }
        return false;
    }

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        // for the new game object created, we need to get the inventory item script
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        // On the new InventoryItem script created, intialize the item
        inventoryItem.IntializeItem(item);

    }
}
