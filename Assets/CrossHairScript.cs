using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairScript : MonoBehaviour
{
    public Vector3 lookat = new Vector3(1, 1, 0);
    private float maxDistance = 0.5f;
    private float maxDistanceDefault = 0.5f;
    [HideInInspector] public Vector3 difference = new Vector3(1, 1);
    [SerializeField] private Sprite swordCrossHair;
    [SerializeField] private Sprite tongueCrossHair;
    private SpriteRenderer sprite;
    private void Start()
    {
            sprite = this.GetComponent<SpriteRenderer>();
            setCrossHairState(0);
            setCrossHairDistance();
            maxDistance = maxDistanceDefault;
            lookat = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, 10));
    }
    void Update()
    {
        lookat = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        difference = lookat - transform.parent.transform.position;
        if(difference.magnitude < maxDistance)
        {
            transform.position = lookat;
        }
        else
        {
            difference.Normalize();
            difference *= maxDistance;
            transform.position = difference + transform.parent.transform.position;
        }
        //transform.position = lookat;

        //Vector3 difference = lookat - transform.position;
        //difference.Normalize();
        //float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, rotation_z);
    }
    public void setCrossHairDistance(float distance)
    {
        maxDistance = distance;
    }
    public void setCrossHairDistance()
    {
        maxDistance = maxDistanceDefault;
    }
    public void setCrossHairState(int CrossHairstate)
    {
        switch (CrossHairstate)
        {
            case 0:
                sprite.sprite = swordCrossHair;
                break;
            case 1:
                sprite.sprite = tongueCrossHair;
                break;
            default:
                sprite.sprite = swordCrossHair;
                break;

        }
    }
    public Vector3 getCrossHairPosition()
    {
        return transform.position;
    }
}
