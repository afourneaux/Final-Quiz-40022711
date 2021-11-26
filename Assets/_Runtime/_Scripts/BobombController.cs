using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class BobombController : MonoBehaviour
{
    CharacterController controller;
    Transform target;
    bool inPursuit;
    float radius = 2f;
    float moveSpeed = 4f;
    float rotationSpeed = 10f;
    const float gravity = 5f;        // Gravity is a constant velocity for bob-ombs due to their spite
    float fuseTime = 5f;
    Coroutine fuse;


    void Start()
    {
        inPursuit = false;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (inPursuit) {
            // If the bob omb gets close to Mario, detonate
            if ((target.position - transform.position).magnitude < radius / 2) {
                Explode();
                return;
            }

            Vector3 direction = target.position - transform.position;   // Naively walk towards Mario
            direction.Normalize();
            direction.y = 0;    // Do not angle up or down to look at Mario
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-direction), rotationSpeed * Time.deltaTime); // Look at Mario
            direction.y = -gravity; // Fall with gravity if in the air
            controller.Move((direction * moveSpeed * Time.deltaTime));  // Move towards Mario
        }
    }

    public void OnAwarenessTriggered(Collider other) {
        // Mario has entered the awareness radius. Light the bob omb's fuse and have it chase Mario
        if (inPursuit == false) {
            AudioController.instance.PlaySound("bobomb_lit");
            inPursuit = true;
            target = other.transform;
            fuse = StartCoroutine(LightFuse());
        }
    }

    void Explode() {
        // Damage the player if nearby and delete the bob omb
        StopCoroutine(fuse);
        if ((target.position - transform.position).magnitude < radius) {
            GameController.instance.TakeDamage();
        }
        AudioController.instance.PlaySound("bobomb_blast");
        Destroy(gameObject);
    }

    IEnumerator LightFuse() {
        // Automatically detonate after a set amount of seconds
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }
}
