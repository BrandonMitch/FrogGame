using UnityEngine;

public class EndOfTongueScript : MonoBehaviour
{
    public void SetInfo(
        TongueStateMachine tongueStateMachine = null,
        ICanMoveTheTongue movingObject = null,
        TongueHitData hitData = null)
    {
        Debug.Log("Set info called" + tongueStateMachine + " " + movingObject + " " + hitData);
        if (movingObject != null) { this.movingObject = movingObject; }
        if (tongueStateMachine != null) { this.tongueStateMachine = tongueStateMachine; }
        if (hitData != null) { this.hitData = hitData; }
        if(this.movingObject != null && this.hitData != null)
        {
            diff = this.movingObject.GetDiff(this.hitData.getPos());
        }
    }
    private Vector2 diff;
    private ICanMoveTheTongue movingObject;
    private TongueStateMachine tongueStateMachine;
    private TongueHitData hitData;
 
    public void FixedUpdate()
    {
        if (!tongueStateMachine.isTongueRetracting())
        {
            UpdateEndOfTonguePosition();
        }
    }

    public void UpdateEndOfTonguePosition()
    {
        if (movingObject != null)
        {
            UpdateEndOfTonguePosition(movingObject.GetPosition() - diff);
        }
    }
    public void UpdateEndOfTonguePosition(Vector2 pos)
    {
        transform.position = pos;
    }

}
