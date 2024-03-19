using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCustomizable : Weapon
{

    [Header("Player/Refernces")]
    public Player playerScript;
    public GameObject CharacterObject; // This is so that everything can be centered around the spirte center
    public GameObject CrossHair;
    [Header("Weapon Component List")]
    public List<WeaponComponent> weaponcomponents;
    [Header("Sword")]
    public GameObject SwordSlashPrefab;
    //TODO: unused
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
        // Step 1: Instinate new slash
        GameObject newSlash = Instantiate(SwordSlashPrefab,CharacterObject.transform.position,Quaternion.identity);
        // Step 2: Enable the Slash
        newSlash.SetActive(true);
        /*DEPRECATED Step 3: Rotate the slash towards the last looking direction
        //Vector3 lastLookDirection = playerScript.lastDirection;
        */
        // Step 3: Rotate the slash towards the cursor
        Vector3 lastLookDirection = CrossHair.GetComponent<CrossHairScript>().difference;
        float rotationAngle = Vector3.SignedAngle(new Vector3(0, 1, 0), lastLookDirection,new Vector3(0,0,1));
        newSlash.GetComponent<Transform>().Rotate(0.0f, 0.0f, rotationAngle, Space.Self);
        /**
         * Debug.Log(rotationAngle);
         player.GetComponent<Player>().animator.SetTrigger("QuickBackSlash");
        **/
        // Step 4: Give the new slash the information about the weapon
        newSlash.GetComponent<ArcAttack>().setWeapon(this);

        newSlash.GetComponent<Animator>().SetTrigger("PlaySlashAnimation");
        SlashList.Add(newSlash);
        Destroy(newSlash, 3f);
    }


}
