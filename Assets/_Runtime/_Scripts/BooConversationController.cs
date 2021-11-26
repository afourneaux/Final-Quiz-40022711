using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class BooConversationController : MonoBehaviour
{
    // When Mario approaches Boo for the first time, begin the race
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            transform.parent.GetComponent<BooController>().OnAwarenessTriggered(other);
            Destroy(this);  // Don't ever trigger this again
        }
    }
}
