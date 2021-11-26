using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class FlagController : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && GameController.instance.raceStarted == true && GameController.instance.marioFinished == false) {
            GameController.instance.marioFinished = true;
            if (GameController.instance.booFinished == true) {
                // Mario lost - now to die!
                AudioController.instance.PlaySound("boo");
                UIController.instance.DisplayConversation(1);
                GameController.instance.marioCursed = true;
            }
        }
    }
}
