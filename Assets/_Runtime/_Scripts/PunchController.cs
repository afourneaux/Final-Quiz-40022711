using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class PunchController : MonoBehaviour
{
    private float lifetime = 0.1f;
    private float force = 1000f;
    private Rigidbody body;

    void Start()
    {
        // Create a very short-lived rigidbody to act as a punch force
        body = transform.GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        // Launch whatever is in front of Mario when he punches
        body.AddForce(transform.forward * Time.fixedDeltaTime * force, ForceMode.Impulse);
    }
}
