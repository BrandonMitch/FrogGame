using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Static Fields
    public static GameManager instance;

    public static StateTag moveTag;
    public static StateTag grappleTag;
    public static StateTag idleTag;
    #endregion

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject); 

            return;
        }
        moveTag = _moveTag;
        grappleTag = _grappleTag;
        idleTag = _idleTag;
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
    public FloatingTextManager floatingTextManager;
    public Animator deathMenuAnim;

    [Header("--References: State Tags")]
    [SerializeField] private StateTag _moveTag;
    [SerializeField] private StateTag _grappleTag;
    [SerializeField] private StateTag _idleTag;
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
