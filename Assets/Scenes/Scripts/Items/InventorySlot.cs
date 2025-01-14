using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    public IChangeCrossHair changesCrossHair = null;
    private InventoryItem _item;
    public InventoryItem Item
    {
        get
        {
            if (_item == null)
            {
                _item = currentItem();
            }
            return _item;
        }
    }
    public bool HasItem{get => Item != null;}
    private void Awake()
    {
        Deselect();
    }
    private void Start()
    {

    }
    public void Select()
    {
        image.color = selectedColor;
        // check if there is a IChangeCrossHair
        if(Item == null)
        {
            return;
        }
        IChangeCrossHair changesCrossHair = Item.GetItem() as IChangeCrossHair;
        if(changesCrossHair != null)
        {
            this.changesCrossHair = changesCrossHair;
        }

    }
    public void Deselect()
    {
        image.color = notSelectedColor;
        this.changesCrossHair = null;
    }
    public void OnDrop(PointerEventData eventData)
    {
        // if nothing in the slot exist
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem draggableItem = dropped.GetComponent<InventoryItem>();
            draggableItem.parentAfterDrag = transform;
        }
        else
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem draggableItem = dropped.GetComponent<InventoryItem>();

            GameObject current = transform.GetChild(0).gameObject;
            InventoryItem currentDraggable = current.GetComponent<InventoryItem>();

            currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
            draggableItem.parentAfterDrag = transform;
        }
    }
    private InventoryItem currentItem()
    {
        if (transform.childCount > 0)
        {
            return transform.GetChild(0).GetComponent<InventoryItem>();
        }
        return null;
    }

    public bool hasRoomForItem(InventoryItem newItem, int qty, int slotMaxCapacity)
    {
        IInventoryItem newItemType = newItem.GetItem();
        return hasRoomForItem(newItemType, qty, slotMaxCapacity);
    }

    public bool hasRoomForItem(IInventoryItem newItem, int qty, int slotMaxCapacity)
    {
        // if there is no inventory item, there is room:
        if (Item == null) { return true; }
        // if there is no item in the inventory slot, there is room;
        if (Item.hasItem() == false) { return true; }
        
        IInventoryItem itemTypeInSlot = Item.GetItem();
        IInventoryItem newItemType = newItem;
        // so if there is an item in the slot,
        // check if they are stackable.
        if (itemTypeInSlot.isStackable() && newItemType.isStackable())
        {
            // if they are stackable, check if the item types are the same
            if (itemTypeInSlot.Equals(newItemType))
            {
                // if they are the same type, check if the is capacity for the new item
                int previousCount = Item.count;
                int newCount = previousCount + qty;
                if (newCount <= slotMaxCapacity)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    [ContextMenu("ClearIem()")]
    public void ClearItem()
    {
        if(Item != null)
        {
            Item.ClearItem();
        }
        _item = null;
        changesCrossHair = null;
    }
    /// <summary>
    /// Will set an item to something, overides the current item. returns false if there is no inventory item, returns true if it worked
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="count"></param>
    /// <returns> returns false if there is no inventory item, returns true if it worked</returns>
    public bool SetItem(IInventoryItem newItem, int count)
    {
        if (Item.hasItem())
        {
            Item.InitializeItem(newItem, count);
            return true;
        }
        return false;
    }
    public static bool ItemOfSameType(IInventoryItem item1, IInventoryItem item2)
    {
        // check if they are stackable.
        if (item1.GetItem().isStackable() && item2.GetItem().isStackable())
        {
            // if they are stackable, check if the item types are the same
            if (item1.GetItem().Equals(item2.GetItem()))
            {
                return true;
            }
        }
        return false;
    }
}