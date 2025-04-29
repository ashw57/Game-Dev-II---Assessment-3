using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PlayerMotor : MonoBehaviour
{
    // ------------------- //
    //      REFERENCES     //
    // ------------------- //
    private CharacterController controller;
    private Animator animator;
    private AudioSource audioSource;
    private Camera cam;
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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        //SetAnimations();
    }

    //This will receive the inputs for our Input Manager and apply them to our character controller.
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);
        Debug.Log(playerVelocity.y);
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

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(swordSwing);
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
        audioSource.pitch = 1;
        audioSource.PlayOneShot(hitSound);

        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }
}
