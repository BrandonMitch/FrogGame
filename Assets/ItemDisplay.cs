using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : Colliad
{
    [SerializeField] private Transform itemTransform;
    [SerializeField] private SpriteRenderer itemRenderer;
    public ItemModifier itemType;
    private bool isItemCollected = false;

    protected override void Start()
    {
        base.Start();
        if (itemType != null && itemRenderer != null)
        {
            itemRenderer.enabled = true;
            itemRenderer.sprite = itemType.image;
        }
        else
        {
            Debug.Log("Item Display Parameters are null");
        }
    }

    protected override void Update()
    {
        if (!isItemCollected)
        {
            itemTransform.localPosition = (1 + Mathf.Sin(Time.time)) * 0.2f * Vector3.up;
        }
    }
    protected void FixedUpdate()
    {
        if (!isItemCollected)
        {
            base.Update();
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            Debug.Log("Collided with player");
            PlayerStatManager stats = coll.GetComponent<PlayerStatManager>();
            if(stats != null)
            {
                GiveItem(stats);
            }
        }
    }
    public void GiveItem(PlayerStatManager stats)
    {
        stats.RegisterItem(itemType, true);
        isItemCollected = true;
        itemRenderer.enabled = false;
    }
    public override void AttackTypeBehavior(Collider2D coll)
    {
        base.AttackTypeBehavior(coll);
    }
}
