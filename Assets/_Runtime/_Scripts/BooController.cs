using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class BooController : MonoBehaviour
{
    bool racing = false;
    bool fadeAway = false;
    float fadeY = 3f;
    float fadeSpeed = 0.5f;
    Vector3 nextNode;
    float speed = 7f;
    float rotationSpeed = 10f;
    float distanceBuffer = 0.1f;
    [SerializeField] private List<GameObject> nodes;
    [SerializeField] private GameObject StarPrefab;
    int soundIndex;

    void Update()
    {
        if (racing) {
            Vector3 destinationDirection = nextNode - transform.position;
            if (destinationDirection.magnitude <= distanceBuffer) {
                if (nodes.Count <= 0) {
                    AudioController.instance.StopLoopingSound(soundIndex);
                    racing = false;
                    if (GameController.instance.marioFinished == true) {
                        // Mario won!
                        AudioController.instance.PlaySound("boo_lose");
                        UIController.instance.DisplayConversation(2);
                        fadeAway = true;
                        Instantiate(StarPrefab, transform.position, Quaternion.identity);
                        racing = false;
                    } else {
                        GameController.instance.booFinished = true;
                    }
                    return;
                }
                nextNode = nodes[0].transform.position;
                nodes.RemoveAt(0);
                destinationDirection = nextNode - transform.position;
            }
            destinationDirection.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(destinationDirection), rotationSpeed * Time.deltaTime); // Look at Mario
            transform.position += destinationDirection * speed * Time.deltaTime;  // Move towards the next node
        }
        // After losing the race, Boo descends into the ground and vanishes
        if (fadeAway) {
            fadeY -= Time.deltaTime * fadeSpeed;
            transform.position += Vector3.down * Time.deltaTime * fadeSpeed;
            if (fadeY <= 0) {
                Destroy(gameObject);
            }
        }
    }

    public void OnAwarenessTriggered(Collider other) {
        // Mario has entered the awareness radius. Begin the first challenge conversation
        AudioController.instance.PlaySound("boo");
        UIController.instance.DisplayConversation(0);
        racing = true;
        GameController.instance.raceStarted = true;
        nextNode = nodes[0].transform.position;
        nodes.RemoveAt(0);
        soundIndex = AudioController.instance.PlayLoopingSound("boo_move");
    }
}
