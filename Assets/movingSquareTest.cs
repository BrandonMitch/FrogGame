using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingSquareTest : MonoBehaviour, IRequireStickyItem, ICanMoveTheTongue
{
    Vector2 initPos;
    Rigidbody2D RB;
    [SerializeField] float posmax = 0.01f;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        initPos = transform.position;
    }

    /// <summary>
    ///  Move the box around
    /// </summary>
    private void FixedUpdate()
    {
        transform.Translate(new Vector3(posmax*Mathf.Sin(Time.time), 0));
    }

    public Rigidbody2D GetRigidbody2D()
    {
        return RB;
    }

    public Vector2 GetPosition()
    {
        return RB.position;
    }

    public Vector2 GetDiff(Vector2 tonguePos)
    {
        return GetPosition() - tonguePos;
    }

    public bool isLatchable(Player player)
    {
        return doesPlayerMeetRequirementsForSwinging(player);

    }

    public bool isPullable(Player player)
    {
        return false;
    }

    public bool isPushable(Player player)
    {
        return false;
    }

    public bool isSwingableOn(Player player)
    {
        return false;
    }

    public bool doesPlayerMeetRequirementsForSwinging(Player player)
    {
        return true;
    }

    public bool iCauseRetractOnUnSwingable(Player player)
    {
        return true;
    }

    public bool iCauseRetractOnUnLatchable(Player player)
    {
        return false;
    }
}
