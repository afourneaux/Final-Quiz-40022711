using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class StarController : MonoBehaviour
{
    private float spinSpeed = 100f;
    private float angle = 0f;
    private float yAmplitude = 0.005f;

    void Start()
    {
        // TODO: Play a sound when the star appears
        transform.position = new Vector3(transform.position.x, transform.position.y - yAmplitude, transform.position.z);
    }

    void Update()
    {
        // Slowly spin and bob the star
        angle += spinSpeed * Time.deltaTime;
        angle %= 360f;
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        transform.position = new Vector3(transform.position.x, transform.position.y + (Mathf.Sin(Time.time) * yAmplitude), transform.position.z);
    }

    void OnTriggerEnter(Collider other) {
        // Collecting the star awards a congratulations and ends the game
        if (other.tag == "Player") {
            // TODO Play a victory jingle
            UIController.instance.DisplayConversation(3);
            GameController.instance.gameOver = true;
        }
    }
}
