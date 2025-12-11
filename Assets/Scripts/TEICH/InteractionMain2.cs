using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InteractionMain2 : MonoBehaviour
{
    public Camera mainCamera;
    private PlayerInputActions inputActions;
    public SimpleCharacterController2 scc;

    private float clickStartTime;
    private bool isHolding = false;
    private float clickThreshold = 0.25f; // max Dauer für kurzen Klick

    private void Awake()
    {
        inputActions = InputManager2.Instance.inputActions;
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.started += OnInteractStarted;
        inputActions.Player.Interact.canceled += OnInteractCanceled;

        inputActions.Player.InteractKey.performed += OnInteractKey;
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.started -= OnInteractStarted;
        inputActions.Player.Interact.canceled -= OnInteractCanceled;
        inputActions.Player.InteractKey.performed -= OnInteractKey;
        inputActions.Player.Disable();
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        clickStartTime = Time.time;
        isHolding = true;
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        float heldTime = Time.time - clickStartTime;
        isHolding = false;

        // nur bei kurzem Klick reagieren
        if (heldTime <= clickThreshold)
        {
            HandleClick();
        }
        else
        {
            // war ein Drag oder langer Hold → nichts tun
            Debug.Log("Hold detected, no interaction");
        }
    }

    private void HandleClick()
    {
        Debug.Log("Short click detected");

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        int layerMask = LayerMask.GetMask("ClickableObject");

        if (Physics.Raycast(ray, out RaycastHit hit, 500, layerMask))
        {
            var clickable = hit.transform.GetComponent<ClickableObject>();
            if (clickable != null)
            {
                Debug.Log("HandleInteraction");
                HandleInteraction(clickable);
            }
        }
    }

    private void OnInteractKey(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (GameManager2.Instance.isInInteractionCollider)
        {
            var clickable = GameManager2.Instance.currentCollision.GetComponent<ClickableObject>();
            if (clickable != null)
                HandleInteraction(clickable);
        }
        
        if (LabManager.Instance != null)
        {
            if (LabManager.Instance.currentlyHeldObject != null)
        {
            LabInteractable li = LabManager.Instance.currentlyHeldObject.GetComponent<LabInteractable>();
            li?.OnInteractKeyPerformed();
        }

        }
        

    }

    private void HandleInteraction(ClickableObject clickable)
    {
        Debug.Log($"Interaction: {clickable.objectType}");

        switch (clickable.objectType)
        {
            case ClickableObject.ObjectType.Remote:
                ToggleRoboMode();
                break;
            case ClickableObject.ObjectType.LabObject:
                InteractLabObject(clickable);
                break;
            case ClickableObject.ObjectType.PlacingArea:
                PlaceObject(clickable);
                break;
            case ClickableObject.ObjectType.Button:
                InteractButton(clickable);
                break;
        }
    }

    public void ToggleRoboMode()
    {
        GameManager2 gM = GameManager2.Instance;
        
        if (gM != null)
        {
            if (gM.isAtLake)
            {
                //gM.isInRoboMode = !gM.isInRoboMode;
                var rM = FindFirstObjectByType<RoboManager>();
                if (rM) rM.ToggleRoboMode();
            }
            
        }
        
    }

    public void InteractLabObject(ClickableObject clickable)
    {
        if (clickable.title == "Bottle")
        {
            Debug.Log("HandleInteraction_Bottle");
            var pc = clickable.GetComponent<PourableContainer>();

            if (pc == null && clickable.transform.parent != null)
            pc = clickable.transform.parent.GetComponent<PourableContainer>();

            if (pc == null && clickable.transform.parent?.parent != null)
            pc = clickable.transform.parent.parent.GetComponent<PourableContainer>();
            Debug.Log("interactLabObject_PC");
            Debug.Log(pc);
            if (pc != null) pc.Interact();
        }
        else if (clickable.title == "Magnet")
        {
            var m = clickable.GetComponent<Magnet>();
            if (m != null) m.Interact();
            
        }
        else
        {
            var li = clickable.GetComponent<LabInteractable>();
             Debug.Log("interactLabObject_LI");
            if (li != null) li.Interact();
        
        }
       
        
        
    }

   public void InteractButton(ClickableObject clickable)
   {
       Debug.Log("click on Button " + clickable.title);
       var uiM = UIManager.Instance;
       
       switch (clickable.title)
        {
            case "ZeroScale":
                var scale = FindFirstObjectByType<ScaleDevice>();
                if (scale) scale.ZeroScale();
                break;
            case "PipetteTop":
                var pipette = FindFirstObjectByType<Pipette>();
                if (pipette) pipette.SetDropSize();
                break;
            case "OxiTopButton":
                var oxiTop = FindFirstObjectByType<OxiTop>();
                if (oxiTop) oxiTop.PressOxiTopButton();
                break;
            case "NotesOnTable":
                
                if (uiM != null) {
                    uiM.ShowNotesPopUpUI();
                }
                break;
            case "TabletOnTable":
                var gM = GameManager2.Instance;
                if (gM != null) {
                    if (gM.isAtLake)
                    {

                    }
   
                }
                break;
            case "SampleOnTable":
                
                if (uiM != null) {
                    uiM.ShowSampleBottlePopUpUI();
                }
                break;
            case "rQPODInLab":
                
                if (uiM != null) {
                    uiM.ShowRQPODLabPopUpUI();
                }
                break;
            case "rQPODInCase":
                GameObject lakeManagerGameObject = GameObject.FindGameObjectWithTag("LakeManager");
                if (lakeManagerGameObject != null)
                {
                    LakeManager lakeManager = lakeManagerGameObject.GetComponent<LakeManager>();
        
                    if (lakeManager != null)
                    {
                        lakeManager.RemoveRoboCase();
                    }
                }
                break;
            case "rQPODOnGras":
                if (uiM != null) {
                    uiM.ShowRQPODPutInWaterPopUpUI();
                }
                break;
            case "LeaveRoboMode":
                GameObject lakeManagerGameObject2 = GameObject.FindGameObjectWithTag("LakeManager");
                if (lakeManagerGameObject2 != null)
                {
                    LakeManager lakeManager2 = lakeManagerGameObject2.GetComponent<LakeManager>();
        
                    if (lakeManager2 != null)
                    {
                        lakeManager2.LeaveRoboMode();
                    }
                }
                break;
            
        }

   }

    public void PlaceObject(ClickableObject clickable)
    {
        Debug.Log("PlaceObject");
        var lm = LabManager.Instance;
        if (lm.currentlyHeldObject == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 placePosition = hit.point;
            var gm = lm.currentlyHeldObject;                //Object in Hand
            var lm_co = gm.GetComponent<ClickableObject>(); //ClickableObject of the Object in Hand
           
               
                
                    var li = gm.GetComponent<LabInteractable>();
                    if (li != null) li.PlaceInteractable(clickable, placePosition);
                
            
        }
    }
    
}
