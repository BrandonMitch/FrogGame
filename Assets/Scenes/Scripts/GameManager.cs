using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject); 
            Destroy(HUD);
            Destroy(Menu);
            return;
        }

        instance = this;
        /*
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += onSceneLoaded;
        */
    }
    // Resources
    
    [Space]
    [Header("References")]
    // References. Important to note that most of these will be lost when we change scenes. 
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitpointBar; // This lets us change the scale of bar when we take damage
    public Animator deathMenuAnim;
    public GameObject HUD;
    public GameObject Menu;

    
    // Floating Text
    public void ShowText(string msg, int fontsSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontsSize, color, position, motion, duration);
    }

    



    // On Scene Loaded
    /*
    public void onSceneLoaded(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }
    */


    // Death Menu and Respond 
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
