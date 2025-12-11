using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RoboManager : MonoBehaviour
{
    public static RoboManager Instance; 
    [Header("Movement")]
    public float baseSpeed = 5f;          // Standardgeschwindigkeit
    public float maxSpeed = 12f;          // obere Grenze
    public float minSpeed = 2f;           // untere Grenze
    public float speedStep = 1f;          // Schrittweite pro Button
    private float currentSpeed;

    public float sprintMultiplier = 2f;
    public float rotationSmoothness = 10f;
    private float rotationVelocity;

    [Header("Remote Joysticks")]
    public Transform forwardStick;
    public Transform turnStick;
    public float stickMaxAngle = 25f;
    public float stickReturnSpeed = 8f;

    private Vector2 moveInput;
    private bool isInputEnabled = true;
    private bool isInRoboMode = false;
    private Rigidbody rb;

    public GameObject remoteOnPlayer;
    public GameObject remoteOnTable;
    public GameObject tabletOnTable;
    public GameObject leaveRoboModeButtonOnTable;

    [Header("UI Fade")]
    public CanvasGroup roboCanvas;
    public Transform player;
    public float maxDistance = 50f;
    public float minDistance = 5f;

    [Header("UI")]
    public TextMeshProUGUI trashScoreUI;
    public TextMeshProUGUI speedUI;  // <-- Text für aktuelle Geschwindigkeit

    public bool isOnTrack = false;
    //public TabletUI tabletUI;

    public float lastPointOnTrack = 0;
    public bool isMeasuring = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationZ;
        }

        currentSpeed = baseSpeed;
        UpdateSpeedUI();
        
        if (Instance == null)
            Instance = this;

        
    
    }

    private void OnEnable()
    {
        var inputActions = InputManager2.Instance.GetInputActions();

        inputActions.Player.MoveRobo.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.MoveRobo.canceled += ctx => moveInput = Vector2.zero;

        // neue Buttons
        inputActions.Player.SpeedUp.performed += ctx => ChangeSpeed(+speedStep);
        inputActions.Player.SpeedDown.performed += ctx => ChangeSpeed(-speedStep);

        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        var inputActions = InputManager2.Instance.GetInputActions();

        inputActions.Player.MoveRobo.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.MoveRobo.canceled -= ctx => moveInput = Vector2.zero;

        inputActions.Player.SpeedUp.performed -= ctx => ChangeSpeed(+speedStep);
        inputActions.Player.SpeedDown.performed -= ctx => ChangeSpeed(-speedStep);

        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (!isInputEnabled || !GameManager2.Instance.isInRoboMode) return;

        HandleMovement();
        AnimateRemoteSticks();
        UpdateCanvasOpacity();

        
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        rb.MovePosition(rb.position + moveDir * currentSpeed * Time.deltaTime);

        if (moveInput.sqrMagnitude > 0.01f && moveInput.y >= 0f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.y;

            float smoothedAngle = Mathf.SmoothDampAngle(
                currentAngle,
                targetAngle,
                ref rotationVelocity,
                0.25f
            );

            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
        }
    }

    private void AnimateRemoteSticks()
    {
        if (forwardStick != null)
        {
            float targetX = moveInput.y * stickMaxAngle;
            Quaternion targetRot = Quaternion.Euler(targetX, 0f, 0f);
            forwardStick.localRotation = Quaternion.Slerp(forwardStick.localRotation, targetRot, Time.deltaTime * stickReturnSpeed);
        }

        if (turnStick != null)
        {
            float targetY = moveInput.x * stickMaxAngle;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, -targetY);
            turnStick.localRotation = Quaternion.Slerp(turnStick.localRotation, targetRot, Time.deltaTime * stickReturnSpeed);
        }
    }

    private void UpdateCanvasOpacity()
    {
        if (roboCanvas == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float alpha = Mathf.SmoothStep(0f, 1f, t);
        roboCanvas.alpha = alpha;
    }

    private void ChangeSpeed(float delta)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + delta, minSpeed, maxSpeed);
        UpdateSpeedUI();
    }

    private void UpdateSpeedUI()
    {
        if (speedUI != null)
        {
            speedUI.text = currentSpeed + "";
        }
    }
    public void EnterRoboMode()
    {
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            gm.isInRoboMode = true;
        }
        
        SetRemote();
        isInRoboMode = true;
        remoteOnTable.SetActive(false);
        tabletOnTable.SetActive(false);
        ToggleRemote(true);
        
        TabletUI tabletUI = TabletUI.Instance;
        if (tabletUI != null)
        {
            tabletUI.ToggleTabUI(true);
        }
    }

    public void ToggleRoboMode()
    {
         GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            gm.isInRoboMode = !gm.isInRoboMode;
            bool inRoboMode = gm.isInRoboMode;
       
        
        SetRemote();
        
        remoteOnTable.SetActive(!inRoboMode);
        tabletOnTable.SetActive(!inRoboMode);
        ToggleRemote(inRoboMode);
        Debug.Log(inRoboMode);
        TabletUI tabletUI = TabletUI.Instance;
        if (tabletUI != null)
        {
            tabletUI.ToggleTabUI(inRoboMode);
        }
        }
    }

    public void LeaveRoboMode()
    {
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            gm.isInRoboMode = false;
            
       
        isInRoboMode = false;
        Debug.Log("leave robo mode");
        SetRemote();
        
        //tablet.SetActive(false);

        remoteOnTable.SetActive(true);
        tabletOnTable.SetActive(true);
        
        TabletUI tabletUI = TabletUI.Instance;
        if (tabletUI != null)
        {
        tabletUI.ToggleTabUI(false);
        }
        ToggleRemote(false);
        }
    }

    public void SetRemote()
    {
        GameObject remote = GameObject.FindGameObjectWithTag("RoboRemote");
        if (remote != null)
        {
            remoteOnPlayer = remote;
        }
    }
    
    public void ToggleRemote(bool show)
    {
        
        foreach (Transform child in remoteOnPlayer.transform)
        {
            child.gameObject.SetActive(show);
            
        }

        if (show == true)
        {
            GameObject stick1 = GameObject.FindGameObjectWithTag("RemoteStick1");
            GameObject stick2 = GameObject.FindGameObjectWithTag("RemoteStick2");
            forwardStick = stick1.transform;
            turnStick = stick2.transform;
            leaveRoboModeButtonOnTable.SetActive(true);
        }
        else
        {
            leaveRoboModeButtonOnTable.SetActive(false);
        }
        
        
        
    }

    public void DrawTrack(float t)
    {
        Debug.Log("start revealing track ui");
        
        isMeasuring = true;
        TabletUI tabletUI = TabletUI.Instance;
        if (tabletUI != null)
        {
        tabletUI.RevealPart(t);
        }
        
        
    }

    public void StopTrack(float t)
    {
        //isMeasuring = false;
        lastPointOnTrack = t;
        Debug.Log("stop revealing track ui");
        TabletUI tabletUI = TabletUI.Instance;
        if (tabletUI != null)
        {
        tabletUI.ToggleOnTrackUI(false);
        
        tabletUI.StartBlackingOutResult(t);
        }
        
    }

}
