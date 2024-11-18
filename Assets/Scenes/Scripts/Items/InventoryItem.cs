using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInventoryItem
{
    
    [Header("UI")]
    public Image image;
    public Text countText;

    [SerializeField] public Item item;
    [SerializeField] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    //TODO: REMOVE THE START LINE 
    private void Start()
    {
        if (item != null)
        {
            InitializeItem(item, count);
        }
        else
        {
            ClearItem();
        }
    }
    public void InitializeItem(IInventoryItem newItem, int count = 1)
    {
        item = newItem as Item;
        if (item == null)
        {
            Debug.LogError("Cannot cast newItem: (" + newItem + ") to a IInventoryItem");
        }
        image.sprite = newItem.GetSprite();
        this.count = count;
        RefreshCount();
    }
    [ContextMenu("ClearItem()")]
    public void ClearItem()
    {
        item = null;
        image.sprite = null;
        count = 0;
        gameObject.SetActive(false);
    }
    [ContextMenu("UpdateUI()")]
    public void UpdateUI()
    {
        InitializeItem(item, count);
        RefreshCount();
        gameObject.SetActive(item != null);
    }
    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false; // Makes it so mouse doesn't interact
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true; // Make the object interactable again
    }

    public string GetName()
    {
        return item?.GetName();
    }

    public string GetDescription()
    {
        return item?.GetDescription();
    }

    public Sprite GetSprite()
    {
        return item?.GetSprite();
    }

    public bool isStackable()
    {
        if (item != null)
        {
            return item.isStackable();
        }
        return false;
    }

    public IInventoryItem GetItem()
    {
        return item;
    }

    public static explicit operator Item(InventoryItem item)
    {
        return item.item;
    }
}
