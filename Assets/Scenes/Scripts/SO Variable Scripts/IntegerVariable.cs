using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Integer Variable", menuName ="Variable/Integer Variable"  )]
public class IntegerVariable : ScriptableObject
{
    [SerializeField] public int value;
}
