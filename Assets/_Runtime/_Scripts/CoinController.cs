using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class CoinController : MonoBehaviour
{
    [SerializeField] private int value = 1;
    private float spinSpeed = 360f;
    private float angle = 0f;
    private bool toDie = false;

    void Update()
    {
        // Coins slowly spin in the air
        angle += spinSpeed * Time.deltaTime * (toDie ? 5f : 1f);    // If the coin has been collected, spin twice as fast before disappearing
        angle %= 360f;
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    void OnTriggerEnter(Collider other) {
        // Pick up the coin, then do a short flourish before deleting the coin from the world
        if (toDie == false && other.tag == "Player") {
            // Note that red coins are worth double!
            GameController.instance.GetCoin(value);
            toDie = true;
            Destroy(gameObject, 0.5f);
        }
    }
}
