using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CharacterController controller;
    [SerializeField, Min(0)] private float speed = 5f;
    [SerializeField, Min(0)] private float rotationSpeed = 10f;

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField, Min(0)] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField, Min(0)] private float jumpHeight = 2f;
    [SerializeField, Min(0)] private float doubleJumpMultiplier = 1.5f;

    [SerializeField, Min(0)] private float groundTimeForDoubleJump = 0.2f;
    [SerializeField, Min(0)] private float speedBoostPerCoin = 0.01f; 
    [SerializeField, Min(0)] private float comboDelay = 0.5f;

    [SerializeField] private GameObject punchForcePrefab;

    private Vector3 movement;
    private Vector3 gravitationalForce;
    private bool isGrounded;
    private float timeGrounded;
    private float timeInCombo;
    int combo;
    private bool jumpMomentumCheck;
    private bool isDoubleJumping;
    private Animator animator;

    private void Start() {
        timeGrounded = 0.0f;
        isGrounded = true;
        isDoubleJumping = false;
        animator = transform.Find("Model/mario64_animated").GetComponent<Animator>();
        combo = 0;
    }

    private void Update()
    {
        if (UIController.instance.dialogueOpen == true) {
            return;
        }

        // calculate movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        movement = right * horizontal + forward * vertical;

        // check if player is trying to move
        if (movement != Vector3.zero)
        {
            // look in the direction of the movement
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotationSpeed * Time.deltaTime);
            animator.SetBool("Run", true);
        } else {
            animator.SetBool("Run", false);
        }

        bool wasGrounded = isGrounded;

        // check if mario is grounded
        isGrounded = false;
        Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, whatIsGround);
        for (int i = 0; i < hitColliders.Length; ++i)
        {
            if (hitColliders[i].gameObject == this.gameObject)
                continue;

            if (!hitColliders[i].isTrigger)
            {
                isGrounded = true;
                if (wasGrounded == false) {
                    if (gravitationalForce.y < -20f) {
                        // If Mario lands too hard, take damage
                        GameController.instance.TakeDamage();
                    } else {
                        // Track the time Mario lands so that a quick new jump will double jump
                        // If Mario just landed a double jump, prevent the next double jump entirely
                        timeGrounded = isDoubleJumping ? 0f : Time.time;
                    }
                    isDoubleJumping = false;
                    animator.SetBool("Jump", false);
                }
                break;
            }
        }

        jumpMomentumCheck = jumpMomentumCheck && Input.GetButton("Jump") && !isGrounded;


        // simulate gravity
        if (isGrounded)
        {
            // mario is standing on ground
            gravitationalForce.y = gravity * Time.deltaTime;
            jumpMomentumCheck = true;
        }
        else
        {
            // mario is in the air
            if (!jumpMomentumCheck && gravitationalForce.y > 0)
                gravitationalForce.y = 0;
            else
            {
                gravitationalForce.y += gravity * Time.deltaTime;
            }
        }

        // Track timer for punch combos
        if (combo > 0) {
            timeInCombo -= Time.deltaTime;
            if (timeInCombo < 0) {
                combo = 0;
            }
        }

        // jump
        if (Input.GetButton("Jump") && isGrounded) {
            float actualJumpHeight = jumpHeight;
            string jumpSound = "mario_ya";
            float cooldown = Time.time - timeGrounded;
            if (cooldown < groundTimeForDoubleJump) {
                isDoubleJumping = true;
                jumpSound = "mario_wah";
                actualJumpHeight *= doubleJumpMultiplier;
            }
            gravitationalForce.y = Mathf.Sqrt(-2 * actualJumpHeight * gravity);
            AudioController.instance.PlaySound(jumpSound);
            animator.SetBool("Jump", true);
        } else {
            // Punch in a 3-move combo
            if (Input.GetButtonDown("Fire1") && isGrounded) {
                switch(combo) {
                    case 0:
                        AudioController.instance.PlaySound("mario_ya");
                        break;
                    case 1:
                        AudioController.instance.PlaySound("mario_wah");
                        break;
                    case 2:
                        AudioController.instance.PlaySound("mario_hoo");
                        break;
                    default:
                        Debug.LogError("Invalid combo number: " + combo);
                        break;
                }

                timeInCombo = comboDelay;
                combo++;
                if (combo >= 3) {
                    combo = 0;
                }
                animator.SetTrigger("Punch");
                Instantiate(punchForcePrefab, transform.position, transform.rotation);
            }
        }
        // Bonus movement speed from coin collection
        float coinMovementBoost = 1 + GameController.instance.coins * speedBoostPerCoin;

        // move mario
        controller.Move((movement * speed * Time.deltaTime * coinMovementBoost) + (gravitationalForce * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.gameObject.name);
    }
}
