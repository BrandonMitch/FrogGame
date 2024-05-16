using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairScript : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;
    public Vector3 lookat = new Vector3(1, 1, 0);
    private float maxDistance = 0.5f;
    private float maxDistanceDefault = 0.5f;
    [HideInInspector] public Vector3 difference = new Vector3(1, 1);
    [SerializeField] private Sprite swordCrossHair;
    [SerializeField] private Sprite tongueCrossHair;

    [SerializeField] private Transform parentTransform;

    private SpriteRenderer sprite;
    private void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        parentTransform = transform.parent;
        setCrossHairState(0);
        setCrossHairDistance();
        maxDistance = maxDistanceDefault;
        lookat = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }

    float joyStick2x;
    float joyStick2y;
    public bool debugLines = true;
    void Update()
    {
        switch (inputManager.controllerMode) {
            case PlayerInputManager.ControllerMode.Mouse:
                lookat = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                difference = lookat - (parentTransform.transform.position);
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
            case PlayerInputManager.ControllerMode.Controller:
                joyStick2x = inputManager.ControllerX;
                joyStick2y = -1 * inputManager.ControllerY;
                var vec = new Vector2(joyStick2x, joyStick2y);

                if (vec.magnitude > 0.5f)
                {
                    transform.localPosition = Vector3.ClampMagnitude(vec, maxDistance);
                }
                break;
        }
        if (debugLines)
        {
            Tracer.DrawCircle(parentTransform.position, 0.02f, 5, Color.white);
            Debug.DrawLine(transform.position, parentTransform.position, Color.magenta);

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
