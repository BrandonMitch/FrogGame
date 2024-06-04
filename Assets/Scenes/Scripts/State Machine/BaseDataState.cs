using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A state that is capable of sending and recieving data between states
/// </summary>
/// <typeparam name="RefType">The type of parent object the statemachine belongs to</typeparam>
/// <typeparam name="DataType">The type of data you want to send between states</typeparam>
public abstract class BaseDataState<RefType, DataType> : BaseState<RefType>
    where RefType : class
{
    protected DataType stateData;

    public virtual void RecieveData(DataType data)
    {
        stateData = data;
    }
    public virtual DataType SendData()
    {
        return stateData;
    }
}
