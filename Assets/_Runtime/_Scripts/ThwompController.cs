using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class ThwompController : MonoBehaviour
{
    bool thwomping = false;
    bool rising = false;
    float resting;
    float restDelay = 1f;
    float riseSpeed = 1f;
    float rotationSpeed = 5f;
    [SerializeField] private float height = 1.5f;

    Vector3 lookDirection;
    float restingY;

    void Start()
    {
        // Track the position in the air for the Thwomp to return to after thwomping
        restingY = transform.position.y;
    }

    void Update()
    {
        // Thwomps have four states:
        // Idle: Waiting for the player to enter their awareness hitbox
        // Thwomping: On the way down, will deal damage. Will stop after the player is hit, or if they reach the ground
        // Resting: After impact, wait for a few seconds
        // Rising: Slowly rise up to the initial position
        if (thwomping) {
            transform.position += Vector3.down * Time.deltaTime * height;
            // Rotate to face Mario
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), rotationSpeed * Time.deltaTime);
            // When the ground is hit, impact
            if (transform.position.y <= restingY - height) {
                Impact();
            }
        }
        // Rest or, if done resting, begin rising
        if (resting > 0) {
            resting -= Time.deltaTime;
        } else {
            if (rising) {
                transform.position += Vector3.up * Time.deltaTime * riseSpeed;
                if (transform.position.y >= restingY) {
                    rising = false;
                }
            }
        }
    }

    public void OnBottomTriggered(Collider other) {
        // Something hit the bottom of the thwomp. Check if this matters
        if (thwomping == true) {
            if (other.tag == "Player") {
                GameController.instance.TakeDamage();
            }
            // Regardless of what was hit, stop thwomping
            Impact();
        }
    }

    private void Impact() {
        thwomping = false;
        AudioController.instance.PlaySound("thwomp");
        resting = restDelay;
        rising = true;
    }

    public void OnAwarenessTriggered(Collider other) {
        // Mario came too close to the Thwomp, begin thwomping
        if (thwomping == false && rising == false) {
            thwomping = true;
            lookDirection = other.transform.position - transform.position;
            lookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z);
        }
    }
}
