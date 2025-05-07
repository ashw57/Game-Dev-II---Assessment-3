using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor;
    private MouseLook look;

    void Awake()
    {
        playerInput = new PlayerInput();
        motor = GetComponent<PlayerMotor>();
        look = GetComponentInChildren<MouseLook>();
        onFoot = playerInput.OnFoot;

        onFoot.WeaponSelectA.performed += ctx => motor.SelectWeapon(0);
        onFoot.WeaponSelectB.performed += ctx => motor.SelectWeapon(1);
        onFoot.WeaponSelectC.performed += ctx => motor.SelectWeapon(2);
        onFoot.WeaponSelectD.performed += ctx => motor.SelectWeapon(3);
        onFoot.WeaponSelectUP.performed += ctx => motor.SelectWeapon(4);
        onFoot.WeaponSelectDOWN.performed += ctx => motor.SelectWeapon(5);
        onFoot.Attack.performed += ctx => motor.Attack();
    }

    private void Update()
    {
        if (onFoot.Attack.IsPressed())
        { motor.Attack(); }

        if (onFoot.WeaponSelectA.IsPressed())
        { motor.SelectWeapon(0); }

        if (onFoot.WeaponSelectB.IsPressed())
        { motor.SelectWeapon(1); }

        if (onFoot.WeaponSelectC.IsPressed())
        { motor.SelectWeapon(2); }

        if (onFoot.WeaponSelectD.IsPressed())
        { motor.SelectWeapon(3); }

        if (onFoot.WeaponSelectUP.IsPressed())
        { motor.SelectWeapon(4); }

        if (onFoot.WeaponSelectDOWN.IsPressed())
        { motor.SelectWeapon(5); }

        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    void FixedUpdate()
    {
        //tell the playermotor to move using the value from our movement action.
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
