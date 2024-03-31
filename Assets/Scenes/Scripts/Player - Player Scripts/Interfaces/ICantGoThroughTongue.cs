using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICantGoThroughTongue : ICantBeSwungOn
{ 
    public void OnTongueCollide(RaycastHit2D hit);
}
