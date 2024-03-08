
using UnityEngine;
using System;

public abstract class BaseState<EState> where EState : Enum
{
    public BaseState(EState key) {
        stateKey = key;
    }
      
        public EState stateKey { get; private set; }
    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void UpdateState();

    public abstract EState GetNextState();

    public abstract void OnTriggerEnter2D(Collider other);

    public abstract void OnTriggerStay2D(Collider other);
    public abstract void OnTriggerExit2D(Collider other);

    

}
