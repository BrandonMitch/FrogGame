using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoccerNetScript : MonoBehaviour
{
    public SoccerGameEvent.GoalSide goalSide;
    public SoccerGameEvent OnGoal;
    public ContactFilter2D filter;
    BoxCollider2D netCollider2D;
    void Start()
    {
        netCollider2D = gameObject.GetComponent<BoxCollider2D>();
    }

    public bool DidBallEnter()
    {
        Collider2D[] hits = new Collider2D[5];
        netCollider2D.OverlapCollider(filter, hits);
        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            if (hit.CompareTag("SoccerBall"))
            {
                OnGoal.Raise(goalSide);
                return true;
            }
        }
        return false;
    }

    public void Update()
    {
    }
    public void Test()
    {
        Tracer.CreateWorldText("soccer net", transform, Vector3.zero, 12, Color.white, default, default, default);
    }


}
