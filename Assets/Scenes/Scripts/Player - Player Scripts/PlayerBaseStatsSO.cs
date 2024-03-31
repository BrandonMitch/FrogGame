using UnityEngine;

[CreateAssetMenu(fileName = "New Player Base Stats", menuName = "Player Base Stats")]
public class PlayerBaseStatsSO : ScriptableObject
{
    [Header("|-----Movement Variables-----|")]
    public float playerSpeed = 1.0f;
    public float playerMaxSpeed = 0.8f;
    public float playerRestingDrag = 100f;
    public float playerRunningDrag = 5f;
    public float playerDragSlowDownTime = 0.2f;
    public float playerRunForceModifier = 40f;
    [Space]
    [Header("|-----Lunge Variables-----|")]
    public float lateralForceModifer = 150f;
    public float minimumLateralDuration = 0.3f;
    public float lateralDragCoefficient = 0f;
    public float dampingCoefficient = 0.99f;
    public float minimumDistanceToSpawnANewPoint = 0.1f;
    public float minimumTimeToSpawnANewPoint = 1f;
    public float forwardLungeCoefficient = 0.1f;
    public float forwardLungeForceModifer = 150f;
    [Space]
    public float lateralLungeEaseInFrames = 5;
    public float lateralLungeEaseOutFrames = 12;
    public float lateralLungeDesiredVEL = 3;
}
