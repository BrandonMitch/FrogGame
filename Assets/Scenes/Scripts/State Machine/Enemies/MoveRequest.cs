using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple wrapper class for IMove which can make a vector2 nullable and allows us to save some information
/// </summary>
public class MoveRequest 
{
    public readonly Vector2 pos;
    public delegate Vector2 FrameUpdatedPosition();
    public readonly FrameUpdatedPosition frameUpdatedPosition = null;
    public MoveRequest(Vector2 pos)
    {
        this.pos = pos;
    }
    public MoveRequest(FrameUpdatedPosition frameUpdatedPosition)
    {
        this.frameUpdatedPosition = frameUpdatedPosition;
    }
}
