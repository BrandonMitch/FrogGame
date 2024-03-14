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
    public enum ControllerMode
    {
        Mouse,
        Controller,
    }
    [SerializeField] public ControllerMode controller = ControllerMode.Mouse;
    [SerializeField] private float controllerSensitivity;
    private void Start()
    {
            sprite = this.GetComponent<SpriteRenderer>();
            setCrossHairState(0);
            setCrossHairDistance();
            maxDistance = maxDistanceDefault;
            lookat = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, 10));
    }
    float joyStick2x;
    float joyStick2y;
    void Update()
    {
        switch (controller) {
            case ControllerMode.Mouse:
                lookat = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                difference = lookat - transform.parent.transform.position;
                if (difference.magnitude < maxDistance)
                {
                    transform.position = lookat;
                }
                else
                {
                    difference.Normalize();
                    difference *= maxDistance;
                    transform.position = difference + transform.parent.transform.position;
                }
                break;
            case ControllerMode.Controller:
                joyStick2x = Input.GetAxis("stick2X");
                joyStick2y = -1*Input.GetAxis("stick2Y");

                var vec = new Vector2(joyStick2x, joyStick2y);

                if (vec.magnitude > 0.5f)
                {
                    transform.localPosition = Vector2.ClampMagnitude(vec, maxDistance);
                }
                /*
                lookat += controllerSensitivity*(new Vector3(joyStick2x, joyStick2y));
                difference = lookat - transform.parent.transform.position;
                if(difference.magnitude < maxDistance)
                {
                    transform.position = lookat;
                }
                lookat = transform.position;
                */
                break;
        }


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
