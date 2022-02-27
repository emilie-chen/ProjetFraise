using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{
    
    
    private void Start()
    {
        Misc.PutIntoOrbit(GameObject.Find("Sun"), GameObject.Find("Earth"), 5000f);
    }
    
    
    
}
