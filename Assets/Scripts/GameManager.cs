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
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += onSceneLoaded;
    }
    // Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;
    
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

    [Space]
    [Header("Player Values")]
    // Logic
    public int pesos;
    public int experience;
    
    // Floating Text
    public void ShowText(string msg, int fontsSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontsSize, color, position, motion, duration);
    }

    // Upgrade Weapon
    public bool TryUpgradeWeapon()
    {
        // is the weapon max level?s
        if(weaponPrices.Count <= weapon.weaponLevel)
        {
            return false;
        }

        if(pesos >= weaponPrices[weapon.weaponLevel])
        {
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }
    
    // Hitpoint Bar
    public void OnHitpointChange() {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }
    // Experience System
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) // Max Level
            {
                return r;
            }
        }

        return r;
    }
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }
        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if(currLevel < GetCurrentLevel())
        {
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {
        Debug.Log("Level Up!");
        player.OnLevelUp();
        OnHitpointChange();
    }

    // On Scene Loaded
    public void onSceneLoaded(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;

    }

    // Save State
    /*
     * INT preferedSkin
     * INT pesos 
     * INT experience
     * INT weaponLevel
     */
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();

        PlayerPrefs.SetString("SaveState", s);
    }

    // Load State
    public void LoadState(Scene s, LoadSceneMode mode) 
    {
        SceneManager.sceneLoaded -= LoadState;
        if (PlayerPrefs.HasKey("SaveState"))
        {
            return;
        }

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        //" 0|10|15|2 " -> ["0","10","15","2"]
        if (data.Length >= 4)
        {
            // Change peso or player skin? 
            pesos = int.Parse(data[1]);

            // Experience and Levels
            experience = int.Parse(data[2]);
            if (GetCurrentLevel() != 1)
            {
                player.SetLevel(GetCurrentLevel());
            }

            // Change weapon level
            weapon.SetWeaponLevel(int.Parse(data[3]));

        }
        else
        {
            Debug.Log("SaveState/LoadStates is data is incomplete. Cannot Load State.");
        }


        //Debug.Log("we should teleport right now");
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;

    }

    // Death Menu and Respond 
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
    }
}
