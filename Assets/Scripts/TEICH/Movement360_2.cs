using UnityEngine;

public class Movement360_2 : MonoBehaviour
{
    public float rotationSpeed = 10f;  // Camera rotation sensitivity
     public float arrowKeyRotationSpeed = 50f;
    public float rotationSpeedTouch = 75f;
    public Transform cameraTransform;   // Reference to the camera

    // Movement input
    private Vector2 lookInput;          // Mouse/touch delta
    private Vector2 lookArrowInput;         // Arrow key input
    private bool isDragging = false;    // Flag for dragging

    [Header("Zoom Controls")]
    public Camera cam360;
    public float zoomMin = 20f; // Minimum FoV for zoom
    public float zoomMax = 60f; // Maximum FoV for zoom
    public float zoomSpeedButton = 10f; // Speed of zooming
    public float zoomSpeed = 1f; // Speed of zooming touch

     private float currentVerticalRotation = 0f;
    private float currentHorizontalRotation = 0f;
    private float rotationSmoothness = 10f; // Je höher, desto sanfter

    private float verticalRotation = 0f;

    private void OnEnable()
    {
        var inputActions = InputManager2.Instance.GetInputActions();
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.LongPress.started += ctx => isDragging = true;
        inputActions.Player.LongPress.canceled += ctx => isDragging = false;

        inputActions.Player.LookWithArrowKeys.performed += ctx => lookArrowInput = ctx.ReadValue<Vector2>();
        inputActions.Player.LookWithArrowKeys.canceled += ctx => lookArrowInput = Vector2.zero;

        


        inputActions.Player.Enable(); // Ensure input is enabled
    }

    void Start()
    {
        Debug.Log("move");
        if (GameManager2.Instance.isMobile)
        {
            rotationSpeed =rotationSpeedTouch;
        }

        // Set the initial FoV to the current camera's FoV
        if (cam360 != null)
        {
            cam360.fieldOfView = Mathf.Clamp(cam360.fieldOfView, zoomMin, zoomMax);
        }
    }

    private void OnDisable()
    {
        var inputActions = InputManager2.Instance.GetInputActions();

        inputActions.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled -= ctx => lookInput = Vector2.zero;

        inputActions.Player.LongPress.started -= ctx => isDragging = true;
        inputActions.Player.LongPress.canceled -= ctx => isDragging = false;

        inputActions.Player.LookWithArrowKeys.performed -= ctx => lookArrowInput = ctx.ReadValue<Vector2>();
        inputActions.Player.LookWithArrowKeys.canceled -= ctx => lookArrowInput = Vector2.zero;


        inputActions.Player.Disable();
    }

    private void Update()
    {
        
        HandleCameraRotation();
        HandleMouseScrollZoom();
        HandleTouchZoom();
    }

   private void HandleCameraRotation()
    {
        // Entscheide, welche Eingabe genutzt wird
        Vector2 input = isDragging ? lookInput : lookArrowInput;

        // Wähle die entsprechende Rotationsgeschwindigkeit
        float rotationSpeedToUse = (lookArrowInput != Vector2.zero) ? arrowKeyRotationSpeed : rotationSpeed;

        // Berechne Drehung
        float targetHorizontalRotation = input.x * rotationSpeedToUse * Time.deltaTime;
        float targetVerticalRotation = input.y * rotationSpeedToUse * Time.deltaTime;

        // **Horizontale Rotation mit weichem Übergang**
        currentHorizontalRotation = Mathf.Lerp(currentHorizontalRotation, targetHorizontalRotation, Time.deltaTime * rotationSmoothness);
        transform.Rotate(Vector3.up, currentHorizontalRotation);

        // **Vertikale Rotation mit Clamping & SmoothDamp**
        verticalRotation = Mathf.Clamp(verticalRotation - targetVerticalRotation, -90f, 90f);

        float smoothTime = 0.1f; // Anpassbar für weichere Bewegungen
        float velocity = 0f;
        currentVerticalRotation = Mathf.SmoothDamp(currentVerticalRotation, verticalRotation, ref velocity, smoothTime);

        cameraTransform.localEulerAngles = new Vector3(currentVerticalRotation, 0f, 0f);
    }


    // Handles zoom in and out using mouse scroll (desktop)
    private void HandleMouseScrollZoom()
    {
        if (cam360 != null)
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollDelta) > 0.01f) // Ignore tiny movements
            {
                cam360.fieldOfView -= scrollDelta * zoomSpeed;
                cam360.fieldOfView = Mathf.Clamp(cam360.fieldOfView, zoomMin, zoomMax);
            }
        }
    }

    // Handles zoom in and out using touch pinch (mobile)
    private void HandleTouchZoom()
    {
        if (Input.touchCount == 2 && cam360 != null)
        {
        // Get the touches
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Calculate the distance between the touches in the previous and current frames
            float prevTouchDeltaMag = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float currentTouchDeltaMag = (touch0.position - touch1.position).magnitude;

            // Calculate the zoom change
            float deltaMagnitudeDiff = prevTouchDeltaMag - currentTouchDeltaMag;

            // Adjust for WebGL platform
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                deltaMagnitudeDiff = -deltaMagnitudeDiff; // Invert the zoom change for WebGL
            }

            // Apply zoom
            cam360.fieldOfView += deltaMagnitudeDiff * zoomSpeed * 0.1f;
            cam360.fieldOfView = Mathf.Clamp(cam360.fieldOfView, zoomMin, zoomMax);
        }
    }

    public void Update360Zoom(float deltaZoom)
    {
         Camera cam360 = Camera.main;
        if (cam360 != null)
        {
            cam360.fieldOfView += deltaZoom * zoomSpeedButton;
            cam360.fieldOfView = Mathf.Clamp(cam360.fieldOfView, zoomMin, zoomMax);
        }
    }
 
}
