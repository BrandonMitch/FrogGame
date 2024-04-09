using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class MethodVariableBase<TSender, TReceiver, TMessage, TCallBack> : ScriptableObject
{
    [SerializeField] public TSender Sender;
    [SerializeField] public TReceiver Receiver;
    [SerializeField] public TMessage Message;

    #region is Valid Sender?, 3 overloads
    public bool isValidSender() // 
    {
        return isValidSender(Sender);
    }
    public static bool isValidSender(TSender sender) //casts to the interface
    {
        ISendMethodVariables<TCallBack> s = sender as ISendMethodVariables<TCallBack>;
        return isValidSender(s);
    }
    private static bool isValidSender(ISendMethodVariables<TCallBack> s)
    {
        return s != null;
    }
    #endregion
    #region is Valid Reciever?, 3 overloads
    public bool isValidReciever()
    {
        return isValidReciever(Receiver);
    }
    public static bool isValidReciever(TReceiver receiver) // casts to the interface
    {
        IReceiveMethodVariables<TMessage, TCallBack> r = receiver as IReceiveMethodVariables<TMessage, TCallBack>;
        return isValidReciever(r);
    }
    private static bool isValidReciever(IReceiveMethodVariables<TMessage, TCallBack> r)
    {
        return r != null; 
    }
    #endregion

    /// <summary>
    /// Sends an object to the reciever<br></br> 
    /// -Recievers must Implment the IRecieveMethodVariable interface, when Raise() is called, Recieve() is called on the reciever<br></br>
    /// -If the sender needs some information back from the reciever, the RecieveCallBack() function of the sender will be called. 
    /// </summary>
    /// <param name="Reciever">Object to Recieve message</param>
    /// <param name="Sender">  Optional Sender who can recieve a call back</param>
    /// <param name="Message"> Optional Message to Send</param>
    public static void Raise(TReceiver receiver, TSender sender = default, TMessage message = default)
    {
        ISendMethodVariables<TCallBack> s = sender as ISendMethodVariables<TCallBack>;
        IReceiveMethodVariables<TMessage, TCallBack> r = receiver as IReceiveMethodVariables<TMessage, TCallBack>;

        if (isValidReciever(r))
        {
            r.Receive(message);
            if (isValidSender(s))
            {
                TCallBack callback = r.GetReceiverCallBack();
                s.ReceiveCallBack(callback);
            }
        }
    }

    public void Raise()
    {
        Raise(Receiver, Sender, Message);
    }
}

public interface IReceiveMethodVariables<TMessage, TCallBack>
{
    void Receive(TMessage message = default);
    TCallBack GetReceiverCallBack();
}

public interface ISendMethodVariables<TCallBack>
{
    void ReceiveCallBack(TCallBack callBack = default);
}

public class MethodWrapper : ScriptableObject /* */
{
    private Action method;

/*    public MethodWrapper(Action method)
    {
        this.method = method;
    }*/

    public void Invoke()
    {
        method?.Invoke();
    }
}
