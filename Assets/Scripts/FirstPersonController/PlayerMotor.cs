using UnityEngine;
using System.Collections;

public enum WeaponSelection
{
    SpikedMorningStar, SpikedBaseballBat, Sledgehammer, PipeSlicer
}
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
    [SerializeField]
    private WeaponSelection weaponSelection;
    [SerializeField]
    int weaponValue = 0;
    bool weaponSelectionIsChanging;

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

    // ------------------ //
    //     ANIMATIONS     //
    // ------------------ //

    /*
    public const string IDLE = "DS_onehand_idle_A";
    public const string WALK = "DS_onehand_walk";
    */

    [Header("Sledgehammer Animations")]
    public const string SHIDLE = "Sledgehammer_Idle";
    public const string SHWALK = "Sledgehammer_Walk";
    public const string SHATTACK1 = "SledgehammerAttackA";
    public const string SHATTACK2 = "SledgehammerAttackB";

    [Header("PipeSlasher Animations")]
    public const string PSIDLE = "PipeSlasher_Idle";
    public const string PSWALK = "PipeSlasher_Walk";
    public const string PSATTACK1 = "PipeSlasherAttackA";
    public const string PSATTACK2 = "PipeSlasherAttackB";

    [Header("BaseballBat Animations")]
    public const string SBBIDLE = "SpikedBaseballBat_Idle";
    public const string SBBWALK = "SpikedBaseballBat_Walk";
    public const string SBBATTACK1 = "SpikedBaseballBatAttackA";
    public const string SBBATTACK2 = "SpikedBaseballBatAttackB";

    [Header("Morningstar Animations")]
    public const string SMSIDLE = "SpikedMorningStar_Walk";
    public const string SMSWALK = "SpikedMorningStar_Idle";
    public const string SMSATTACK1 = "SpikedMorningStarAttackA";
    public const string SMSATTACK2 = "SpikedMorningStarAttackB";
    //public const string BLOCK = "CharacterArmature|Sword_Block";

    string currentAnimationState;
    int smsAttackCount = 0;
    int sbbAttackCount = 0;
    int shAttackCount = 0;
    int psAttackCount = 0;

    // ------------------ //
    //      Weapons       //
    // ------------------ //

    [SerializeField]
    private GameObject spikedMorningStar;
    [SerializeField]
    private GameObject spikedBaseballBat;
    [SerializeField]
    private GameObject sledgehammer;
    [SerializeField]
    private GameObject pipeSlicer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = GetComponentInChildren<Camera>();
        audioSource = GetComponent<AudioSource>();
        
        playerInput = new PlayerInput();
        input = playerInput.OnFoot;

        StartCoroutine(WeaponDrawDelay());
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



    /*
    public void Jump()
    {
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }
    */

    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        StartCoroutine(SlowToAttack());

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        switch (weaponSelection)
        {
            case WeaponSelection.SpikedMorningStar:
                if (audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(swordSwing);
                }
                else Debug.LogError("AudioSourceNotFound");

                if (smsAttackCount == 0)
                {
                    ChangeAnimationState(SMSATTACK1);
                    smsAttackCount++;
                }
                else
                {
                    ChangeAnimationState(SMSATTACK2);
                    smsAttackCount = 0;
                }
                break;

            case WeaponSelection.SpikedBaseballBat:
                if (audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(swordSwing);
                }
                else Debug.LogError("AudioSourceNotFound");

                if (sbbAttackCount == 0)
                {
                    ChangeAnimationState(SBBATTACK1);
                    sbbAttackCount++;
                }
                else
                {
                    ChangeAnimationState(SBBATTACK2);
                    sbbAttackCount = 0;
                }
                break;

            case WeaponSelection.Sledgehammer:
                if (audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(swordSwing);
                }
                else Debug.LogError("AudioSourceNotFound");

                if (shAttackCount == 0)
                {
                    ChangeAnimationState(SHATTACK1);
                    shAttackCount++;
                }
                else
                {
                    ChangeAnimationState(SHATTACK2);
                    shAttackCount = 0;
                }
                break;

            case WeaponSelection.PipeSlicer:
                if (audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(swordSwing);
                }
                else Debug.LogError("AudioSourceNotFound");

                if (psAttackCount == 0)
                {
                    ChangeAnimationState(PSATTACK1);
                    psAttackCount++;
                }
                else
                {
                    ChangeAnimationState(PSATTACK2);
                    psAttackCount = 0;
                }
                break;
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
            switch(weaponSelection)
            {
                case WeaponSelection.SpikedMorningStar:
                    if (playerVelocity.x == 0 && playerVelocity.z == 0)
                    { ChangeAnimationState(SMSIDLE); }
                    else { ChangeAnimationState(SMSWALK); }
                    break;
                case WeaponSelection.SpikedBaseballBat:
                    if (playerVelocity.x == 0 && playerVelocity.z == 0)
                    { ChangeAnimationState(SBBIDLE); }
                    else { ChangeAnimationState(SBBWALK); }
                    break;
                case WeaponSelection.Sledgehammer:
                    if (playerVelocity.x == 0 && playerVelocity.z == 0)
                    { ChangeAnimationState(SHIDLE); }
                    else { ChangeAnimationState(SHWALK); }
                    break;
                case WeaponSelection.PipeSlicer:
                    if (playerVelocity.x == 0 && playerVelocity.z == 0)
                    { ChangeAnimationState(PSIDLE); }
                    else { ChangeAnimationState(PSWALK); }
                    break;
            }
            
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

    public void SelectWeapon(int value)
    {
        switch (value) 
        {
            case 0:
                weaponValue = 0;
                break;
            case 1:
                weaponValue = 1;
                break;
            case 2:
                weaponValue = 2;
                break;
            case 3:
                weaponValue = 3;
                break;
            case 4:
                if (!weaponSelectionIsChanging)
                    StartCoroutine(BumperWait(0));
                break;
            case 5:
                if (!weaponSelectionIsChanging)
                    StartCoroutine(BumperWait(1));
                break;

        }
        
        weaponSelection = (WeaponSelection)weaponValue;
        StartCoroutine(WeaponDrawDelay());
    }
    private IEnumerator WeaponDrawDelay()
    {
        yield return new WaitForSeconds(0.25f);

        spikedBaseballBat.SetActive(false);
        sledgehammer.SetActive(false);
        pipeSlicer.SetActive(false);
        spikedMorningStar.SetActive(false);

        switch (weaponSelection) 
        {
            case WeaponSelection.SpikedMorningStar:
                spikedMorningStar.SetActive(true);
                break;
            case WeaponSelection.SpikedBaseballBat:
                spikedBaseballBat.SetActive(true);
                break;
            case WeaponSelection.PipeSlicer:
                pipeSlicer.SetActive(true);
                break;
            case WeaponSelection.Sledgehammer:
                sledgehammer.SetActive(true);
                break;
        }
    }

    private IEnumerator BumperWait(int i)
    {
        weaponSelectionIsChanging = true;
        if (i == 0) 
        {
            ++weaponValue;
            if (weaponValue == 4)
            { weaponValue = 0; }
        }
        else
        {
            --weaponValue;
            if (weaponValue == -1) { weaponValue = 3; }
        }
        yield return new WaitForSeconds(0.5f);
        weaponSelectionIsChanging = false;
    }
}
