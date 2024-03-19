using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponComponentDisplay : MonoBehaviour
{
    public WeaponComponent weaponComponent;
    public Text nameText;
    public Text descriptionText;
    public Image weaponComponentSprite;

    private void Start()
    {
        nameText.text = weaponComponent.name;
        descriptionText.text = weaponComponent.description;

        weaponComponentSprite.sprite = weaponComponent.image;
    }

}
