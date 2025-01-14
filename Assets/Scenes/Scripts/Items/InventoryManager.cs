using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IInventory
{
    // TODO: There is an error when trying to set active
    // NOTE: May be useful to make a hotbar slot array and a full inventory slot arrary and then combine them
    public int maxStackedItems = 64;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public GameEvent OnSelectedSlotChanged;
    // used if a spell/item changes the max distance of the crosshair
    public IChangeCrossHair CrossHairChangeParams {
        get{
            if(SelectedSlot != null)
            {
                return SelectedSlot.changesCrossHair;
            }
            return null;
        }

    }
    [SerializeField] PlayerInventory inventory;
    int selectedSlot = -1;

    public InventorySlot SelectedSlot
    {
        get
        {
            if (selectedSlot < 0 || selectedSlot > inventorySlots.Length - 1)
            {
                return null;
            }
            return inventorySlots[selectedSlot];
        }
    }
    private void Start()
    {
        ChangeSelectedSlot(0);
        if(inventory != null)
        {
            inventory.inventory = this;
        }
    }
    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        inventorySlots[newValue].Select();
        selectedSlot = newValue;

        OnSelectedSlotChanged?.Raise();
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
    public bool AddItem(IInventoryItem item, int qty)
    {
        for(int i = 0; i <inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
           
            if (slot.hasRoomForItem(item, qty, maxStackedItems))
            {
                InventoryItem itemInSlot = slot.Item;
                if(itemInSlot == null)
                {
                    // we neeed to create a new item
                    SpawnNewItem(item, slot, qty);
                    slot.Item.UpdateUI();

                    // Make sure to update selected slot if is selected
                    if (SlotIsSelected(i)) { ChangeSelectedSlot(i); }
                    return true;
                }
                else if (!itemInSlot.hasItem()) /// empty InventoryItem just add the item
                {
                    itemInSlot.InitializeItem(item, qty);
                    itemInSlot.UpdateUI();

                    // Make sure to update selected slot if is selected
                    if (SlotIsSelected(i)) { ChangeSelectedSlot(i); }
                    return true;
                }
                else if(InventorySlot.ItemOfSameType(itemInSlot, item))
                {
                    itemInSlot.count += qty;
                    itemInSlot.UpdateUI();

                    // Make sure to update selected slot if is selected
                    if (SlotIsSelected(i)) { ChangeSelectedSlot(i); }
                    return true;
                }
                else
                {
                    continue;
                }
            }
        }
        return false;
    }

    public bool SlotIsSelected(InventorySlot slot)
    {
        return SelectedSlot.Equals(slot);
    }
    public bool SlotIsSelected(int slotN)
    {
        return slotN == selectedSlot;
    }
    [ContextMenu("ReorganizeInventory()")]
    public void ReorganizeInventory()
    {
        Dictionary<IInventoryItem, int> itemCounts;
        List<IInventoryItem> unstackable;
        (itemCounts, unstackable) = InventoryContents();

        ClearItems();

        // add all stackable items first
        foreach (var kvp in itemCounts)
        {
            AddItem(kvp.Key, kvp.Value);
        }

        foreach (var item in unstackable)
        {
            AddItem(item, 1);
        }
        UpdateUI();
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
    public void ClearItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            slot.ClearItem();
        }
    }

    public bool hasRoomForItem(IInventoryItem newItem, int qty, int maxItemsPerStack)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot.hasRoomForItem(newItem, qty, maxStackedItems))
            {
                InventoryItem itemInSlot = slot.Item;
                if (itemInSlot == null)
                {
                    itemInSlot.UpdateUI();
                    return true;
                }
                else if (!itemInSlot.hasItem())
                {
                    return true;
                }
                else if (InventorySlot.ItemOfSameType(itemInSlot, newItem))
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }
        }
        return false;
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
    [SerializeField] Item testItem;
    [SerializeField] int qty;
    [ContextMenu("TestAddItem()")]
    public void TestAddItem()
    {
        Debug.Log($"Adding item: {qty}x: {testItem}\n" +
            $"Worked?: {AddItem(testItem, qty)}");
    }


    [ContextMenu("Check if current slot changes CrossHair")]
    public void PrintSlotInfo()
    {
        string s = "";
        s += $"Selected Slot #: {selectedSlot}\n";
        s += $"Inventory Slot: {SelectedSlot}\n";
        s += $"Inventory Item: {SelectedSlot.Item}\n";

        if (SelectedSlot.Item != null && SelectedSlot.Item.GetItem() != null)
        {
            s += $" Item: {inventorySlots[selectedSlot].Item.GetItem()}\n";
            var x = SelectedSlot.Item.GetItem() as IChangeCrossHair;

            s += $"IChangeCrossHair: {x}\n";
            if (x != null)
            {
                var y = x.GetCrossHairParams();
                s += $"hasMax:{y.hasMaxDistance}\n";
                s += $"hasMin:{y.hasMinDistance}\n";
                s += $"max:{y.maxDistance}\n";
                s += $"min:{y.minDistance}\n";
                s += $"Sprite:{y.spriteTexture}\n";
            }
        }
        Debug.Log(s);
    }
    [ContextMenu("Check If Changes Selected Slot")]
    public void PrintChangesSelectedSlot()
    {
        string s = "";
        s += $"Inventory Slot: {SelectedSlot}\n";
        s += $"Inventory Item: {SelectedSlot.Item}\n";
        s += $"CrossHairChangeParams: {CrossHairChangeParams}\n";
        
        var x = CrossHairChangeParams;

        if (x != null)
        {
            var y = x.GetCrossHairParams();
            s += $"hasMax:{y.hasMaxDistance}\n";
            s += $"hasMin:{y.hasMinDistance}\n";
            s += $"max:{y.maxDistance}\n";
            s += $"min:{y.minDistance}\n";
            s += $"Sprite:{y.spriteTexture}\n";
        }

        Debug.Log(s);
    }
#endif
}
