using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCustomizable : Weapon
{

    [Header("Player/Refernces")]
    //public GameObject player;
    public GameObject CharacterObject; // This is so that everything can be centered around the spirte center
    [Header("Weapon Component List")]
    public List<WeaponComponent> weaponcomponents;
    [Header("Sword")]
    public GameObject SwordSlashPrefab;
    private List<GameObject> SlashList = new List<GameObject>();

    // Start is called before the first frame update
    protected override void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    { 
    }

    protected override void OnCollide(Collider2D coll)
    {
        base.OnCollide(coll);
    }

    public void BackSwordSlash() {
        GameObject newSlash = Instantiate(SwordSlashPrefab,CharacterObject.transform.position,Quaternion.identity);
        
        //player.GetComponent<Player>().animator.SetTrigger("QuickBackSlash");

        newSlash.GetComponent<Animator>().SetTrigger("PlaySlashAnimation");
        SlashList.Add(newSlash);
        Destroy(newSlash, 3f);
    }


}
