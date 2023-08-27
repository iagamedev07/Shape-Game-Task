using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private Camera cam;

    private GameInputActions inputActions;

    private CharacterController characterController;

    //Rotation Variables
    private float xRotation = 0f;

    //Movement Variables
    private Vector3 velocity;
    [SerializeField] private float gravity = -9.81f;
    private bool grounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = InputManager.Instance.gameInput;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Jump.performed -= OnJump;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        grounded = characterController.isGrounded;
        if(grounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        Vector2 moveInput = GetMovement();
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        Vector2 lookInput = GetLook();
        float lookX = lookInput.x * lookSpeed * Time.deltaTime;
        float lookY = lookInput.y * lookSpeed * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookX);
        
    }

    private Vector2 GetMovement()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }

    private Vector2 GetLook()
    {
        return inputActions.Player.Look.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if(grounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }


}
