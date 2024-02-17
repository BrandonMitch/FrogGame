using UnityEngine;

public class Portal : Colliad
{
    //public variables let you insert data in editor
    public string[] sceneNames;

    protected override void OnCollide(Collider2D coll)
    {
        if(coll.name == "Player")
        {
            // Teleport the player
            string sceneName = sceneNames[Random.Range(0, sceneNames.Length)];
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
