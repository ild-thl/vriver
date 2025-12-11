using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager2 : MonoBehaviour
{
    public static InputManager2 Instance;
    public PlayerInputActions inputActions;

    public GameObject touchUIMove;
    public GameObject touchUILook;

    public GameObject touchUI;
    
   

    private void Awake()
    {
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        inputActions = new PlayerInputActions();
    }
    else
    {
        //Debug.LogWarning("Duplicate InputManager detected and destroyed.");
        Destroy(gameObject);
    }
    }


    private void OnEnable()
    {
        EnablePlayerInput();
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            DisableAllInputs();
        }
    }

    private void Update()
    {

    /*
        if (EventSystem.current != null)
        {
       
            // Check if pointer is over UI but not on touch sticks
            if ((EventSystem.current.IsPointerOverGameObject() && !AreTouchesOnTouchUI() && !IsPointerOverWorldSpaceUI() ))
            {
                EnableUIInput();
                Debug.Log("UI");
            }
            else
            {
                EnablePlayerInput();
                Debug.Log("Player");
        }
       
    }
     */
    
    


    }

    private bool AreTouchesOnTouchUI()
    {
        foreach (Touch touch in Input.touches)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == touchUIMove || result.gameObject == touchUILook)
            {
                return true; // Touch is over one of the UI elements
            }
        }
    }

    return false;
    }


    private bool IsPointerOverWorldSpaceUI()
    {
    PointerEventData pointerData = new PointerEventData(EventSystem.current)
    {
        position = Input.mousePosition
    };

    var results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(pointerData, results);

    foreach (var result in results)
    {
        Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            return true; // Mauszeiger ist über einer World Space UI
        }
    }
    return false;
    }

    public void EnablePlayerInput()
    {
        if (inputActions == null) return;

        if (!inputActions.Player.enabled)
        {
        //Debug.Log("Enabling P Input");
            inputActions.Player.Enable();
            inputActions.UI.Disable();
        }
    }

    public void EnableUIInput()
    {
        if (inputActions == null) return;

        if (!inputActions.UI.enabled)
        {
            //Debug.Log("Enabling UI Input");
            inputActions.UI.Enable();
            inputActions.Player.Disable();
        }
    }

    public void DisableAllInputs()
    {
        if (inputActions == null) return;

       //Debug.Log("Disabling All Input");
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }
    
    public void ShowMobileUI()
{
    GameObject touch_UI = GameObject.FindGameObjectWithTag("MobileNav");
    if (touch_UI != null)
    {
        Debug.Log("show2");
        touchUI = touch_UI;

        // Enable all child objects
        foreach (Transform child in touchUI.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    else
    {
        Debug.Log("null");
    }
}

public void HideMobileUI()
{
    GameObject touch_UI = GameObject.FindGameObjectWithTag("MobileNav");
    if (touch_UI != null)
    {
        touchUI = touch_UI;

        // Disable all child objects
        foreach (Transform child in touchUI.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}

    public PlayerInputActions GetInputActions()
    {
        return inputActions;
    }

    
}
