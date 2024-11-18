using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bouncy Spell Behaviour", menuName = "Item/Spell Behavior/BouncySpell")]
public class BouncySpellBehavior : SpellBehavior
{
    [SerializeField] private GameObject bouncySpellPrefab;
    public override void OnCast(Player player = null, float xPos = 0, float yPos = 0, float xDirection = 0, float yDirection = 0, float Accuracy = 0)
    {
        // Define the target vector
        Vector2 targetVector = new Vector2(1, 1);

        // Calculate the angle of rotation
        float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;

        // Construct the quaternion using the angle
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject.Instantiate(bouncySpellPrefab, new Vector3(xPos, yPos), rotation);
    }

    [ContextMenu("Test Spell")]
    public void TestCast()
    {
        OnCast(player: null, 0, 0, 0, 0, 0);
    }
}
