using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class TongueHitData 
{
    public GameObject pointObject;
    private Transform transform;
    private RaycastHit2D collisionInstance;
    public TonguePointType type;

    /// <summary>
    /// This constructor is used for creating new tongue hitpoints. 
    /// </summary>
    /// <param transform="transform">The transform of the tongue point object</param>
    /// <param type="type">Type of tongue point (baseOfTongue, tongueHitPoint)</param>
    public TongueHitData(RaycastHit2D collisionInstance, TonguePointType type, GameObject pointObject)
    {
        if(type != TonguePointType.tongueHitPoint)
        {
            Debug.LogError("Incorrect use of the constructor for TongueHitData. This constructor is only used for the hit points");
            return;
        }

        //Debug.Log("New tongue hit point created");
        this.collisionInstance = collisionInstance;
        this.pointObject = pointObject;
        this.transform = pointObject.transform;
        this.type = type;
    }
    /// <summary>
    /// This constructor is used for creating new tongueEndPoints!
    /// </summary>
    /// <param transform="transform">The transform of the tongue point object</param>
    /// <param type="type">Type of tongue point (baseOfTongue, tongueHitPoint)</param>
    public TongueHitData(Transform transform, TonguePointType type, GameObject pointObject)
    {
        if(type != TonguePointType.endOfTongue)
        {
            Debug.LogError("Incorrect use of constructor for TongueHitData, this method is only used for the end of the tongue"); 
            return;
        }
        this.type = type;
        this.transform = transform;
        this.pointObject = pointObject;
    }

    /// <summary>
    /// This constructor is used for the base of the tongue only! The game object wil be set to null.
    /// </summary>
    /// <param transform="transform">The transform of the tongue point object</param>
    /// <param type="type">Type of tongue point (baseOfTongue, tongueHitPoint)</param>
    public TongueHitData(Transform transform, TonguePointType type)
    {
        if(type != TonguePointType.baseOfTongue)
        {
            Debug.LogError("Incorrect use of constructor for TongueHitData, this method is used only for the base of the tongue");
            return;
        }
        this.type = type;
        this.transform = transform;
        this.pointObject = null;
    }

    public bool DestroyPoint()
    {
        switch (type)
        {
            case TonguePointType.baseOfTongue:
                Debug.LogError("CANNOT DESTROY BASE OF TONGUE");
                break;
            case TonguePointType.tongueHitPoint:
                if (pointObject == null)
                {
                    Debug.Log("Point object is null");
                    return false;
                }
                break;
            case TonguePointType.endOfTongue:
                //Debug.Log("Destroying End of tongue");
                break;
        }
        return true;
    }

    public GameObject getGameObject()
    {
        return pointObject;
    }
    public override string ToString()
    {
        string s =
            "Type: " + type.ToString() + "\n" +
            "isGameObjectNull?: " + (pointObject == null ? true : false) + "\n";
        return s;
    }
    public Vector3 getPos()
    {
        return (Vector2)transform.position;
    }
}
