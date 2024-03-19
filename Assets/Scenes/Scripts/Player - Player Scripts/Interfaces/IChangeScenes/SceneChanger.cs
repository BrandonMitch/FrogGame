using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour, IChangeScenes
{

    public void ChangeScenes()
    {
        OnSceneExit();
        throw new System.NotImplementedException();
    }

    public bool CheckPlayerTags(Object obj)
    {
        throw new System.NotImplementedException();
    }

    public Player getPlayerScript(GameObject obj)
    {
        return obj.GetComponent<Player>();
    }

    public void OnSceneExit()
    {
        throw new System.NotImplementedException();
    }

    public void RunShutOff()
    {
        throw new System.NotImplementedException();
    }
}
