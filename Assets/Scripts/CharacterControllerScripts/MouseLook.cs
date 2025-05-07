using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : GameBehaviour
{
    public float mouseSensitivity = 20f;
    public float controllerSensitivity = 2f;  // This will likely need tweaking

    [SerializeField] private float controllerSmoothingSpeed = 5f; // Higher = faster response

    private Vector2 currentControllerLook = Vector2.zero;

    [SerializeField]
    private Transform playerBody;

    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ProcessLook(Vector2 input)
    {
        bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;

        float mouseX, mouseY;

        if (usingMouse)
        {
            mouseX = input.x * mouseSensitivity * Time.deltaTime;
            mouseY = input.y * mouseSensitivity * Time.deltaTime;
        }
        else
        {
            // Smooth controller input over time
            currentControllerLook = Vector2.Lerp(currentControllerLook, input, controllerSmoothingSpeed * Time.deltaTime);

            mouseX = currentControllerLook.x * controllerSensitivity;
            mouseY = currentControllerLook.y * controllerSensitivity;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }


    private void Update()
    {
        // Remove this block if you're using input events instead
        Vector2 input = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

        ProcessLook(input);
    }
}
