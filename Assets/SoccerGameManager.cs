using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoccerGameManager : MonoBehaviour
{
    public SoccerGameEvent OnGoal;
    public SoccerGameEvent.GoalSide scoreSide;

    public SoccerGameEvent OnScoreChange;


    public FloatVariable P1ScoreFV;
    public FloatVariable P2ScoreFV;

    public GridSOValues gridSOValues;
    [SerializeField] private SoccerNetScript Lnet;
    [SerializeField] private SoccerNetScript Rnet;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private GameObject soccerBall;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Transform player1Transform;
    [SerializeField] private Transform player2Transform;
    [SerializeField] private Transform soccerBallTransform;

    Rigidbody2D p1RB;
    Rigidbody2D p2RB;
    Rigidbody2D soccerBallRB;

    Player p1script;
    Player p2script;
    Vector2 p1Starting;
    Vector2 p2Starting;
    Vector2 soccerBallStarting;

    void Start()
    {
        grid = new GridLogical(26, 16, 0.24f, new Vector3(-3.12f,-1.92f));
        P1ScoreFV.value = 0;
        P2ScoreFV.value = 0;
        player1Transform = player1.transform;
        player2Transform = player2.transform;
        soccerBallTransform = soccerBall.transform;
        p1Starting = player1Transform.position;
        p2Starting = player2Transform.position;
        soccerBallStarting = soccerBallTransform.position;

        p1RB = player1.GetComponent<Rigidbody2D>();
        p2RB = player2.GetComponent<Rigidbody2D>();
        soccerBallRB = soccerBall.GetComponent<Rigidbody2D>();

        p1script = player1.GetComponent<Player>();
        p2script = player2.GetComponent<Player>();
    }
    private GridLogical grid;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(Tracer.GetMouseWorldPosition(), 1);
        }
        Lnet.DidBallEnter();
        Rnet.DidBallEnter();
    }
    public void Reset()
    {

        player1Transform.position = p1Starting;
        player2Transform.position = p2Starting;
        soccerBallTransform.position = soccerBallStarting;
        soccerBallRB.velocity = Vector2.zero;
        p1RB.velocity = Vector2.zero;
        p2RB.velocity = Vector2.zero;
        soccerBallTransform.rotation = Quaternion.identity;
        p1script.tongueStateMachine.EnterOffStateImmediately(p1script);
        p1script.stateMachine.EnterOffStateImmediately(p1script);

        p2script.tongueStateMachine.EnterOffStateImmediately(p2script);
        p2script.stateMachine.EnterOffStateImmediately(p2script);
    }
    public void OnGoalFunction()
    {
        scoreSide = OnGoal.goalSide;
        switch (scoreSide)
        {
            case SoccerGameEvent.GoalSide.Left:
                P2ScoreFV.value++;
                OnScoreChange.Raise(scoreSide);
                break;
            case SoccerGameEvent.GoalSide.Right:
                P1ScoreFV.value++;
                OnScoreChange.Raise(scoreSide);
                break;
            default:
                return;
        }
        Reset();
    }

    [ContextMenu("Save Values to Scriptable Object")]
    private void SaveValuesToSO()
    {
        gridSOValues.FromGridLogicalToSO(grid);
    }
    [ContextMenu("Print SO")]
    private void PrintSO()
    {
        gridSOValues.printArray();
    }


}
