using UnityEngine;

public class AttackInputData
{ 
    private Vector2 attackDirection;
    private Vector2 movementDirection;
    private float attackChargeDuration;
    private float timeCreated;
    public AttackInputData(Vector2 attackDirection, Vector2 movementDirection, float attackChargeDuration)
    {
        this.timeCreated = Time.time;
        this.attackDirection = attackDirection;
        this.movementDirection = movementDirection;
        this.attackChargeDuration = attackChargeDuration;
    }

    public Vector2 getAttackDirection()
    {
        return attackDirection;
    }
    public Vector2 getMovementDirection()
    {
        return movementDirection;
    }
    public float getAttackChargeDuration()
    {
        return attackChargeDuration;
    }
    public float getTimeCreated()
    {
        return timeCreated;
    }
}
