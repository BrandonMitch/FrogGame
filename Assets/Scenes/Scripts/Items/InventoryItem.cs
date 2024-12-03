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
    private IInventoryItem _item;
    [SerializeField] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    //TODO: REMOVE THE START LINE 
    private void Awake()
    {
        if(_item == null)
        {
            _item = item;
        }

        if (hasItem())
        {
            InitializeItem(GetItem(), count);
        }
        else
        {
            ClearItem();
        }
    }
    public void InitializeItem(IInventoryItem newItem, int count = 1)
    {
        _item = newItem;
        item = newItem as Item;

        if (item == null)
        {
            Debug.LogWarning("Cannot cast newItem: (" + newItem + ") to a Item");
        }
        image.sprite = newItem.GetSprite();
        this.count = count;
        RefreshCount();
    }
private void InitializeItem(Item newItem, int count = 1)
    {
        item = newItem;
        image.sprite = newItem.GetSprite();
        this.count = count;
        RefreshCount();
    }


    [ContextMenu("ClearItem()")]
    public void ClearItem()
    {
        _item = null;
        item = null;
        image.sprite = null;
        count = 0;
       // gameObject.SetActive(false);
    }
    [ContextMenu("UpdateUI()")]
    public void UpdateUI()
    {
        image.sprite = GetSprite();
        RefreshCount();
        if (hasItem())
        {
            image.enabled = true;
        }
        else { 
            Debug.Log("Disabling");
            image.enabled = false;
        }
    }
    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false; // Makes it so mouse doesn't interact
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true; // Make the object interactable again
    }

    public string GetName()
    {
        return GetItem()?.GetName();
    }

    public string GetDescription()
    {
        return GetItem()?.GetDescription();
    }

    public Sprite GetSprite()
    {
        return GetItem()?.GetSprite();
    }

    public bool isStackable()
    {
        if (hasItem())
        {
            return GetItem().isStackable();
        }
        return false;
    }

    public IInventoryItem GetItem()
    {
        if( item != null)
        {
            return item;
        }
        return _item;
    }

    public static explicit operator Item(InventoryItem item)
    {
        return item.item;
    }

    public bool hasItem()
    {
        if (GetItem() == null)
        {
            return false;
        }
        return true;
    }

    [ContextMenu("PRINT()")]
    public void print()
    {
        string s = 
            $"item:{item}\n" +
            $"_item:{item}\n" +
            $"GetItem():{GetItem()}\n" +
            $"count:{count}" +
            $"hasItem():{hasItem()}\n" +
            $"isStackable():{isStackable()}\n";
        Debug.Log(s);
    }
}
