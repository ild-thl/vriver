using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
 using UnityEngine.SceneManagement;

public class SimpleCharacterController2 : MonoBehaviour
{
    
    public float moveSpeed = 5f;         // Normal movement speed
    public float sprintMultiplier = 2f; // Sprint speed multiplier
    public float rotationSpeed = 100f; // Camera rotation sensitivity
    public float rotationSpeedTouch = 100f;
    public float arrowKeyRotationSpeed = 50f;

    
    private float verticalRotation = 0f;
    private float currentVerticalRotation = 0f;
    private float currentHorizontalRotation = 0f;
    private float rotationSmoothness = 10f; // Je höher, desto sanfter

    
    public Transform cameraTransform;   // Reference to the camera

    private CharacterController characterController;
    private Vector2 moveInput;          // Movement input
    private Vector2 lookInput;          // Mouse/touch delta
    private Vector2 lookArrowInput;     // Look arrow input
    private bool isDragging = false;    // Flag for dragging
    private bool isSprinting = false;   // Flag for sprinting
    private bool isInputEnabled = true; // Controls whether input is processed

  
    
    private PointerEventData pointerData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        pointerData = new PointerEventData(EventSystem.current);
        if(GameManager2.Instance == null)
        {
            Debug.Log("null");
        }
        
       
    }

    public void Start()
    {
        if (GameManager2.Instance.isMobile)
        {
            rotationSpeed =rotationSpeedTouch;
        }

    if (GameManager2.Instance != null)
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        
        
        if (controller != null) controller.enabled = true;
    }
 

    }

   

private void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
    SetupInputBindings();
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
    RemoveInputBindings();
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    SetupInputBindings(); // Bindings neu setzen
}
private void SetupInputBindings()
{
    var inputActions = InputManager2.Instance.GetInputActions();

    inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

    inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
    inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

    inputActions.Player.LookWithArrowKeys.performed += ctx => lookArrowInput = ctx.ReadValue<Vector2>();
    inputActions.Player.LookWithArrowKeys.canceled += ctx => lookArrowInput = Vector2.zero;

    inputActions.Player.LongPress.started += ctx => isDragging = true;
    inputActions.Player.LongPress.canceled += ctx => isDragging = false;

    inputActions.Player.Sprint.performed += ctx => isSprinting = true;
    inputActions.Player.Sprint.canceled += ctx => isSprinting = false;

    inputActions.Player.Enable();
}

private void RemoveInputBindings()
{
    var inputActions = InputManager2.Instance.GetInputActions();

    inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
    inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;

    inputActions.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
    inputActions.Player.Look.canceled -= ctx => lookInput = Vector2.zero;

    inputActions.Player.LookWithArrowKeys.performed -= ctx => lookArrowInput = ctx.ReadValue<Vector2>();
    inputActions.Player.LookWithArrowKeys.canceled -= ctx => lookArrowInput = Vector2.zero;

    inputActions.Player.LongPress.started -= ctx => isDragging = true;
    inputActions.Player.LongPress.canceled -= ctx => isDragging = false;

    inputActions.Player.Sprint.performed -= ctx => isSprinting = true;
    inputActions.Player.Sprint.canceled -= ctx => isSprinting = false;

    inputActions.Player.Disable();
}


    

    private void Update()
    {
            
            HandleMovement();
            HandleCameraRotation();
        
    }

    public void ToggleInput(bool enable)
    {
        isInputEnabled = enable;
    }

    public void SetTouchRotation()
    {
        rotationSpeed = rotationSpeedTouch;
    }

    private void HandleMovement()
    {
        
        // Determine the current speed (sprint or normal)
        float currentSpeed = isSprinting && moveInput != Vector2.zero ? moveSpeed * sprintMultiplier : moveSpeed;

        // Movement input
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        //Debug.Log("move:" + moveInput);
        // Apply gravity
        if (!characterController.isGrounded)
        {
            characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
        
    }

    private void HandleCameraRotation()
    {
         //Debug.Log("move:" + lookInput);
        if (isDragging) // Process input only when dragging
        {
            float lookX = lookInput.x * rotationSpeed * Time.deltaTime;
            float lookY = lookInput.y * rotationSpeed * Time.deltaTime;

            // **Horizontale Rotation mit Dampening**
            currentHorizontalRotation = Mathf.Lerp(currentHorizontalRotation, lookX, Time.deltaTime * rotationSmoothness);
            transform.Rotate(Vector3.up, currentHorizontalRotation);

            // **Vertikale Rotation mit Clamping & Dampening**
            verticalRotation -= lookY;
            verticalRotation = Mathf.Clamp(verticalRotation, -75f, 75f);

            currentVerticalRotation = Mathf.Lerp(currentVerticalRotation, verticalRotation, Time.deltaTime * rotationSmoothness);
            cameraTransform.localEulerAngles = new Vector3(currentVerticalRotation, 0f, 0f);
        }

        // Umschauen mit den Pfeiltasten
        if (lookArrowInput != Vector2.zero)
        {
            float arrowLookX = lookArrowInput.x * arrowKeyRotationSpeed * Time.deltaTime;
            float arrowLookY = lookArrowInput.y * arrowKeyRotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, arrowLookX);
            verticalRotation -= arrowLookY;
            verticalRotation = Mathf.Clamp(verticalRotation, -75f, 75f);
            cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
        }
    }   
    

    

    public void TeleportPlayer(Transform pos)
{
   

    if (characterController != null)
        characterController.enabled = false;

    transform.position = pos.position;

    if (characterController != null)
        characterController.enabled = true;

   
}

public void TeleportPlayer2(Vector3 pos, Vector3 eulerRot)
{
    Debug.Log("teleport player");

    if (characterController != null)
        characterController.enabled = false;

    // Set position
    transform.position = pos;

    // Convert Vector3 rotation to Quaternion
    transform.rotation = Quaternion.Euler(eulerRot);

    Debug.Log("ssc pos1: " + pos);
    Debug.Log("ssc pos2: " + transform.position);

    if (characterController != null)
        characterController.enabled = true;
}


 

}
