using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairScript : MonoBehaviour
{
    public struct CrossHairParams
    {
        public bool isNull;
        public bool hasMaxDistance;
        public bool hasMinDistance;
        public float maxDistance;
        public float minDistance;
        public Sprite spriteTexture;
        public CrossHairParams(
            bool isNull = true,
            bool hasMaxDistance = true,
            bool hasMinDistance = false,
            float maxDistance = 1f,
            float minDistance = 0f,
            Sprite spriteTexture = null
            )
        {
           this.isNull = isNull;
           this.hasMaxDistance = hasMaxDistance;
           this.hasMinDistance = hasMinDistance;
           this.maxDistance = maxDistance;
           this.minDistance = minDistance;
           this.spriteTexture = spriteTexture;
        }
    }
    #region Sword Crosshair stats
    [SerializeField] private Sprite swordCrossHairSprite;
    public CrossHairParams swordCrossHair = new CrossHairParams( 
        isNull:false,
        hasMaxDistance: true,
        hasMinDistance: false,
        maxDistance: 0.5f,
        minDistance: 0.0f,
        spriteTexture: null)
        ;
    #endregion
    #region Tongue Aim CrossHair stats
    [SerializeField] private Sprite tongueCrossHairSprite;
    public CrossHairParams tongueCrossHair = new CrossHairParams(
        isNull:false,
        hasMaxDistance: true,
        hasMinDistance: false,
        maxDistance: 1f,
        minDistance: 0.0f,
        spriteTexture: null
        );
    #endregion


    [SerializeField] private PlayerInputManager inputManager;
    public Vector3 lookat = new Vector3(1, 1, 0);
    private float maxDistance = 0.5f;
    private float maxDistanceDefault = 0.5f;
    [HideInInspector] public Vector3 difference = new Vector3(1, 1);
    [SerializeField] private Transform parentTransform;

    private SpriteRenderer sprite;
    private void Awake()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        parentTransform = transform.parent;
        // set textures
        swordCrossHair.spriteTexture = swordCrossHairSprite;
        tongueCrossHair.spriteTexture = tongueCrossHairSprite;
        // set defualts
        maxDistance = maxDistanceDefault;
        swordCrossHair.hasMaxDistance = true;
        swordCrossHair.maxDistance = maxDistance;
    }
    private void Start()
    {
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
    private void setCrossHairDistance(float distance)
    {
        maxDistance = distance;
    }

    private void setCrossHairDistance()
    {
        maxDistance = swordCrossHair.maxDistance;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="CrossHairstate">
    /// 0 swordCrossHair,
    /// 1 tongue aim,
    /// 2 spell at specific distance</param>
    public void setCrossHairState(int CrossHairstate)
    {
        switch (CrossHairstate)
        {
            case 0:
                setCrossHairState(swordCrossHair);
                break;
            case 1:
                setCrossHairState(tongueCrossHair);
                break;
            default:
                sprite.sprite = swordCrossHairSprite;
                maxDistance = maxDistanceDefault;
                break;

        }
    }
    public Vector3 getCrossHairPosition()
    {
        return transform.position;
    }

    public void setCrossHairState(CrossHairParams crossHairParams)
    {
        // Set texture
        if (crossHairParams.spriteTexture != null)
        {
            sprite.sprite = crossHairParams.spriteTexture;
        }
        // Set Maximum distance
        if (crossHairParams.hasMaxDistance)
        {
            setCrossHairDistance(crossHairParams.maxDistance);
        }
    }
}
