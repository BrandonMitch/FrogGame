using UnityEngine;
using SceneListNameSpace;
public class Portal : Colliad
{
    public Scenes scene;
    private bool firstEntry = true;
    protected override void OnCollide(Collider2D coll)
    {
        if(coll.CompareTag("Player") )
        {
            // Teleport the player
            Player playerScript = coll.GetComponent<Player>();
            if(playerScript == null)
            {
                Debug.LogError("PlayerScript is null, can't enter portal");
                Debug.Break();
                return;
            }
            if (firstEntry)
            {
                playerScript.stateMachine.ChangeState(playerScript.idleState);
                if (!playerScript.isTongueOff())
                {
                    playerScript.tongueStateMachine.ChangeState(playerScript.tongueRetractingState);
                }
                firstEntry = false;
            }
            /*string sceneName = scene.ToString();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);*/

            if (playerScript.isTongueOff())
            {
                Loader.Load(scene);
                firstEntry = true;
            }
        }
    }
}
