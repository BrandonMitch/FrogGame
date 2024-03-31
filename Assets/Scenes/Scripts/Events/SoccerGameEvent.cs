using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Score Game Event", menuName = "Game Event/Soccer Game Event")]
public class SoccerGameEvent : GameEvent
{
    public enum GoalSide
    {
        Left,
        Right
    }
    public GoalSide goalSide;

    /// <summary>
    ///  Overload of the raise method that takes in a goal side
    /// </summary>
    /// <param name="goalSide"></param>
    public void Raise(GoalSide goalSide)
    {
        // set the SO goal side to one side
        this.goalSide = goalSide;
        Raise();
    }
}
