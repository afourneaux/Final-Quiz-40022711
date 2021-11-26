using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class ThwompBottomController : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        transform.parent.GetComponent<ThwompController>().OnBottomTriggered(other);
    }
}
