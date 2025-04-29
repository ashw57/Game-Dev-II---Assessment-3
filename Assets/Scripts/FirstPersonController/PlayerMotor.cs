using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class PlayerMotor : MonoBehaviour
{
    // ------------------- //
    //      REFERENCES     //
    // ------------------- //
    private CharacterController controller;
    private Animator animator;
    private AudioSource audioSource;
    private Camera cam;
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions input;
    private Vector3 playerVelocity;

    // ------------------- //
    //      VARIABLES      //
    // ------------------- //
    private bool isGrounded;

    [Header("Controller")]
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    // ------------------- //
    // ATTACKING BEHAVIOUR //
    // ------------------- //
    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    [Header("Attack References")]
    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    private bool attacking = false;
    private bool readyToAttack = true;
    int attackCount;

    // ------------------ //
    //     ANIMATIONS     //
    // ------------------ //

    public const string IDLE = "DS_onehand_idle_A";
    public const string WALK = "DS_onehand_walk";
    public const string ATTACK1 = "DS_onehand_attack_A";
    public const string ATTACK2 = "DS_onehand_attack_B";
    //public const string BLOCK = "CharacterArmature|Sword_Block";

    string currentAnimationState;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = GetComponentInChildren<Camera>();
        audioSource = GetComponent<AudioSource>();

        playerInput = new PlayerInput();
        input = playerInput.OnFoot;
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        // Continious Input
        if (input.Attack.IsPressed()) Attack();

        SetAnimations();
    }

    //This will receive the inputs for our Input Manager and apply them to our character controller.
    public void ProcessMove(Vector2 input)
    {
        if (attacking) return;

        Vector3 moveDirection = Vector3.zero;

        // Get the direction the player is facing in world space
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Apply input relative to the player's facing direction (local space)
        moveDirection += forward * input.y;  // Forward/backward
        moveDirection += right * input.x;    // Left/right

        // Normalize the direction to prevent faster diagonal movement
        moveDirection.Normalize();

        // Update player velocity based on movement direction
        playerVelocity.x = moveDirection.x * speed;
        playerVelocity.z = moveDirection.z * speed;

        // Apply movement using CharacterController (in world space)
        controller.Move(playerVelocity * Time.deltaTime);

        // Handle gravity (y velocity)
        playerVelocity.y += gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        // Apply gravity
        controller.Move(playerVelocity * Time.deltaTime);

        // Debug logging for velocity
        Debug.Log($"X Velocity: {playerVelocity.x}, Z Velocity: {playerVelocity.z}, Y Velocity: {playerVelocity.y}");
    }




    public void Jump()
    {
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        StartCoroutine(SlowToAttack());

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(swordSwing);
        }
        else Debug.LogError("AudioSourceNotFound");

        if(attackCount == 0)
        {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    private void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    private void AttackRaycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            HitTarget(hit.point);
        }
    }

    private void HitTarget(Vector3 pos)
    {
        if (audioSource != null)
        {
            audioSource.pitch = 1;
            audioSource.PlayOneShot(hitSound);
        }
        else Debug.LogError("AudioSourceNotFound");
        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }

    public void ChangeAnimationState(string newState)
    {
        // Stop the same animation from interrupting with itself
        if (currentAnimationState == newState) return;

        // Play the animation
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    private void SetAnimations()
    {
        // If player is not attacking

        if (!attacking)
        {
            if (playerVelocity.x == 0 && playerVelocity.z == 0)
            { ChangeAnimationState(IDLE); }
            else { ChangeAnimationState(WALK); }
        }
    }

    private IEnumerator SlowToAttack()
    {
        // Set a smooth deceleration speed for the velocity.
        float decelerationSpeed = 5f;  // You can adjust this to make it faster/slower

        // Continuously reduce the player's velocity
        while (Mathf.Abs(playerVelocity.x) > 0.01f || Mathf.Abs(playerVelocity.z) > 0.01f)
        {
            // Smoothly reduce the velocity
            playerVelocity.x = Mathf.Lerp(playerVelocity.x, 0f, decelerationSpeed * Time.deltaTime);
            playerVelocity.z = Mathf.Lerp(playerVelocity.z, 0f, decelerationSpeed * Time.deltaTime);

            // Apply the movement after adjusting the velocity
            controller.Move(playerVelocity * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        // After the velocity is near zero, we can explicitly set it to zero to stop any tiny remaining movement
        playerVelocity.x = 0f;
        playerVelocity.z = 0f;
    }


}
