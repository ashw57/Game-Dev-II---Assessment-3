using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    [Header("CharacterMovement")]
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private float speed = 12f;
    [SerializeField]
    private float gravity = -9.81f;

    private Vector3 velocity;

    [Header("SprintReferences")]
    [SerializeField]
    private bool isSprinting;

    [Header("JumpReferences")]
    [SerializeField]
    private float jumpHeight = 3f;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundDistance = 0.4f;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private bool isGrounded;

    private void Update()
    {

        float currentSpeed = speed;

        if(Input.GetButton("Sprint") && isGrounded)
        {
            isSprinting = true;
            currentSpeed = Sprinting();
        }
        else
        {
            isSprinting = false;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0 )
        {
            velocity.y = -2f;
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded )
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private float Sprinting()
    {
        float sprintspeed = speed * 2;
        return sprintspeed;
    }
}
