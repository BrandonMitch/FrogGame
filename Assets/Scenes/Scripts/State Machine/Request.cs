using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request
{
    // Tells the state machine how to read the request
    public enum StateMachineReadMode {
        NoDelegates,
        Delegates,
        GetState,
    }
    public StateMachineReadMode readMode = StateMachineReadMode.NoDelegates;


    /// <summary>
    /// A delegate can be provided to the request to be called when the request is completed, usually a call to return back to its own state
    /// </summary>
    public delegate void OnFuffill();
    public OnFuffill onFuffill = null;

    /// <summary>
    /// A delegate can be provided with a time amount for how long to keep calling this delegate. Used in the grappler state to keep searching for a grapple point while moving
    /// </summary>
    public delegate void KeepCalling();
    public KeepCalling keepCalling = null;

    /// <summary>
    /// A delegate that will request a state from the state machine 
    /// </summary>
    /// <param name="state">The state requested is passed into this param by the state machine</param>
    public delegate void GetState(IState state);
    public GetState getState = null;
    public ScriptableObject stateTag;


    public enum SimpleRequest
    {
        None,
        MoveTo,
        RunAwayFrom,
        Grapple,
        StopCoroutines,
        Idle,
    }
    public SimpleRequest simpleVectorRequest;
    public Vector2 locationRequest;
    public object dataRequest;
    public Request(Vector2 locationRequest, SimpleRequest simpleVectorRequest = SimpleRequest.None, OnFuffill onFuffill = null, KeepCalling keepCalling = null)
    {
        // if there was provided delegates, then we will try
        if(onFuffill != null || keepCalling != null )
        {
            readMode = StateMachineReadMode.Delegates;
        }
        this.onFuffill = onFuffill;
        this.keepCalling = keepCalling;
        this.simpleVectorRequest = simpleVectorRequest;
        this.locationRequest = locationRequest;
    }

    public Request(GetState getState, ScriptableObject stateTag)
    {
        readMode = StateMachineReadMode.GetState;
        this.getState = getState;
        this.stateTag = stateTag;
    }
    public override string ToString()
    {
        string s = "";
        if (simpleVectorRequest != SimpleRequest.None)
        {
            s += "Simple Request:\n  " + simpleVectorRequest.ToString() + " | "+ locationRequest.ToString() +"\n";
        }
        if(onFuffill != null)
        {
            s += "  Onfuffil Delegate: " + onFuffill.ToString() + "\n";
        }
        if(keepCalling != null)
        {
            s += "  Keep Calling Delegate: " + keepCalling.ToString() +"\n";
        }
        if(getState != null)
        {
            s += "Get State Delegate:  " + getState.ToString() + " | StateTag: " + stateTag.name + "\n";
        }
        return s;
    }
}
