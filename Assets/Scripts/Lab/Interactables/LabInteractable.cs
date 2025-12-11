using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class LabInteractable : MonoBehaviour
{
    [Header("General Settings")]
    public string objectTag;                 // Used to identify in LabExperiment steps
    public bool canBePickedUp = true;
    public bool canBeUsed = false;
    public Vector3 initialPosition;
    public Quaternion initialRotation;
    public bool isBox1 = false; //Hemmstoff
    public bool isBox2 = false; //NaOH
    
    private Vector3 targetLocalPosition;
    private Quaternion targetLocalRotation;
    private float holdSwaySpeed = 5f;
    private float holdSwayAmount = 0.01f;


   [Header("Physical Properties")]
    public float baseWeight = 200f;  // Leergewicht in g
    public float weight = 200f;      // Dynamisches Gewicht in g


    public bool isHeld = false;

    protected Transform holdParent;
    public Vector3 grabOffset;
    protected Rigidbody rb;

    public GameObject toggleGameObject;
    public GameObject nextHighlightObject;
    public GameObject thisHighlightObject;

    public TextMeshProUGUI hintUI;
    public GameObject hintUIGameObject;

    public Transform targetPosition;
    public Transform targetPlatformPosition;
    //public Transform oxiTopPosition;
    public Transform bottleParent;
   
    public PourableContainer pc;
    private PlayerInputActions inputActions;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
        
    }

    private void Start()
{
    initialPosition = transform.position;
    initialRotation = transform.rotation;
}

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Tilt.performed += OnTiltPerformed;
        inputActions.Player.Tilt.canceled  += OnTiltCanceled;
        //inputActions.Player.InteractKey.performed  += OnInteractKeyPerformed;
    }

    void OnDisable()
    {
        //inputActions.Player.InteractKey.performed  -= OnInteractKeyPerformed;
        inputActions.Player.Tilt.performed -= OnTiltPerformed;
        inputActions.Player.Tilt.canceled  -= OnTiltCanceled;
        inputActions.Player.Disable();
    }

    public virtual void Interact()
    {
        if (!isHeld)
        {
            PickUp();
        }
        else if (canBeUsed)
        {
            Debug.Log($"{name} used.");
            OnUse();
            
        }
        else
        {
            Drop();
        }
    }
    public virtual float GetCurrentWeight()
    {
        return weight;
    }

    public virtual void UpdateWeight()
    {
        // Standard: bleibt gleich (keine Flüssigkeit)
        weight = baseWeight;
    }


    public virtual void PickUp()
    {
        if (!canBePickedUp) return;

        
         
   
    // Falls bereits etwas gehalten wird → loslassen
        if (LabManager.Instance.currentlyHeldObject != null)
        {
            GameObject current = LabManager.Instance.currentlyHeldObject;
            var li = current.GetComponent<LabInteractable>();
            li?.Drop();
        }
    holdParent = GameObject.FindGameObjectWithTag("HandPosition")?.transform;
        if (holdParent == null)
        {
            Debug.LogWarning("No HandPosition found!");
            return;
        }

    // Collider auslesen (egal ob Box, Capsule oder Mesh)
    Collider col = GetComponent<Collider>();
    if (col != null)
    {
        // Höhe und Mittelpunkt des Colliders
        float halfHeight = col.bounds.extents.y;
        Vector3 center = col.bounds.center;

        // Weltposition des unteren Punkts und oberen Punkts
        float bottomY = col.bounds.min.y;
        float topY = col.bounds.max.y;
        float midY = (bottomY + topY) / 2f;

        // Differenz vom Pivot zur Mitte
        float pivotToMid = midY - transform.position.y;

        // Korrektur: Objekt wird so verschoben, dass seine Mitte auf die Handposition snapt
        Vector3 offset = new Vector3(0, -pivotToMid, 0);

        transform.position = holdParent.position + offset + grabOffset;
    }
    else
    {
        // Fallback: Standardverhalten
        transform.position = holdParent.position;
    }

    transform.rotation = Quaternion.identity;
    transform.SetParent(holdParent);

    isHeld = true;

    if (rb != null)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }
     if (pc != null) pc.isPouring = false;
    
    LabManager.Instance.currentlyHeldObject = this.gameObject;

    targetLocalPosition = transform.localPosition;
    targetLocalRotation = transform.localRotation;

}

    

    public virtual void Drop()
    {
        transform.SetParent(null);
        
    if (rb != null)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }
        isHeld = false;
      
       if (pc != null) pc.isPouring = false;
    }

 public void PlaceInteractable(ClickableObject clickable, Vector3 placementPos)
{
    Debug.Log("PLACE");
    if (!isHeld) return;

    transform.SetParent(null);
    isHeld = false;

    if (rb != null)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    LabManager.Instance.currentlyHeldObject = null;
    Debug.Log(objectTag);
    Debug.Log(clickable.title);
    
    if (clickable.title == "Table")
    {
        // --- Default-Platzierung ---
        transform.position = placementPos;
        transform.rotation = Quaternion.identity;
    }
    else
    {

        // --- MAGNET ---
        if (objectTag == "Magnet" && clickable.title == "Bottleneck")
        {
            Debug.Log("magnettttt");

            if (targetPosition != null)
            {
                transform.position = targetPosition.position + Vector3.up * 0.2f; // leicht über Ziel starten
                transform.rotation = targetPosition.rotation;
            }
            else
            {
                transform.position = placementPos + Vector3.up * 0.2f;
                transform.rotation = Quaternion.identity;
            }

            // Coroutine starten, die den Magneten absenkt und danach versteckt
            StartCoroutine(AnimatePlacement());

            return;
        }

         // --- Pipette ---
        if (objectTag == "Pipette" && clickable.title == "Table")
        {
            Debug.Log("pipette on table");

             if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }   


            if (targetPosition != null)
            {
                transform.position = targetPosition.position; // leicht über Ziel starten
                 transform.rotation = targetPosition.rotation * Quaternion.Euler(0, 0, 90);
            }
            else
            {
                transform.position = placementPos;
                transform.rotation = Quaternion.identity;
            }

            // Coroutine starten, die den Magneten absenkt und danach versteckt
            StartCoroutine(AnimatePlacement());

            return;
        }



        // --- TRICHTER ---
        if (objectTag == "Funnel1" && clickable.title == "Bottleneck")
        {
            Debug.Log("Funnel 1");
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }   


            if (targetPosition != null)
            {
                // Position und Rotation exakt übernehmen
                transform.position = targetPosition.position + Vector3.up * 0.2f;
                transform.rotation = targetPosition.rotation;
            }
            else
            {
                // Fallback: Nur an der Klickposition mit neutraler Rotation
                transform.position = placementPos + Vector3.up * 0.2f;
                transform.rotation = Quaternion.identity;
            }
             StartCoroutine(AnimatePlacement());

            return;
        }


        // --- TRICHTER ---
        if (objectTag == "Funnel2" && clickable.title == "Bottleneck")
        {
            Debug.Log("Funnel 2");
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }   
            

            if (targetPosition != null)
            {
                // Position und Rotation exakt übernehmen
                transform.position = targetPosition.position + Vector3.up * 0.2f;
                transform.rotation = targetPosition.rotation;
            }
            else
            {
                // Fallback: Nur an der Klickposition mit neutraler Rotation
                transform.position = placementPos + Vector3.up * 0.2f;
                transform.rotation = Quaternion.identity;
            }
             StartCoroutine(AnimatePlacement());

            return;
        }


        


        // --- OxiBottle on Platform ---
        if (objectTag == "OxiBottle" && clickable.title == "Platform")
        {
            Debug.Log("OxiBottle on Platform");
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }   
            

            if (targetPlatformPosition != null)
            {
                Debug.Log("Position Correct");
                // Position und Rotation exakt übernehmen
                transform.position = targetPlatformPosition.position;
                transform.rotation = targetPlatformPosition.rotation;
            }
             transform.SetParent(null);
            MixingPlatform mp = clickable.GetComponent<MixingPlatform>();
            OxiBottle oxiBottle = gameObject.GetComponent<OxiBottle>();
            if (oxiBottle != null)
            {
                mp?.AddOxiBottle(oxiBottle);
                if (oxiBottle.bottleCollider != null)
                {
                    oxiBottle.bottleCollider.enabled = false;
                }

            }
            return;
        }

        // --- OxiBottle on Platform ---
        if (objectTag == "OxiTop" && clickable.title == "Bottleneck")
        {

            //OxiBottle oxiBottle = clickable.GetComponent<OxiBottle>();
            //Transform oxiTopTargetPosition = oxiBottle?.oxiTopPosition;

            //oxiBottle?.PlaceOxiTop(transform.gameObject, rb);

             Debug.Log("OxiTop");

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }   
            

            if (targetPosition != null)
            {
                // Position und Rotation exakt übernehmen
                transform.position = targetPosition.position + Vector3.up * 0.2f;
                transform.rotation = targetPosition.rotation;
            }
            OxiTop oxiTop = transform.GetComponent<OxiTop>();
            

            if (oxiTop != null)
            {
                oxiTop.isOnOxiBottle = true;
                oxiTop.CheckOxiTopAttachmentStep();
                
            }
            StartCoroutine(AnimatePlacement());

            return;
        }
    }





     if (pc != null) pc.isPouring = false;
    
}

   
    // Tilt-Eingabe an PourableContainer weitergeben
  
    private void OnTiltPerformed(InputAction.CallbackContext ctx)
    {
        if (isHeld && pc != null)
            Debug.Log("onTiltPerformed");
            pc?.OnBottleTiltPerformed();
    }

    private void OnTiltCanceled(InputAction.CallbackContext ctx)
    {
        if (pc != null)
        Debug.Log("onTiltCanceled");
            pc?.OnBottleTiltCanceled();
    }


    protected virtual void OnUse()
    {
        // Default behavior for "use" (can be overridden)
        Debug.Log($"{name} used (default behavior)");
        if (objectTag == "Pipette")
        {
            Pipette p = GetComponent<Pipette>();
            p?.UsePipette();
        }
    }

    public void OnInteractKeyPerformed()
    {
        if (objectTag == "Pipette")
        {
            Pipette p = GetComponent<Pipette>();
            p?.UsePipette();
        }
        if (objectTag == "Tweezers")
        {
            Tweezers t = GetComponent<Tweezers>();
            t?.UseTweezers();
        }
    }

    private IEnumerator AnimatePlacement()
{
    MeshCollider collider = GetComponent<MeshCollider>();
    collider.isTrigger = true;
    float duration = 0.3f;
    float elapsed = 0f;
    Vector3 startPos = transform.position;
    Vector3 endPos = targetPosition.position; // Ziel ist die finale Position

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        transform.position = Vector3.Lerp(startPos, endPos, t);

        if (objectTag == "OxiTop")
        {
            StartCoroutine(RotateSelf());
        }
        yield return null;
    }

    // Kurz warten, damit der Effekt wirken kann
    yield return new WaitForSeconds(0.1f);

        if (objectTag == "Magnet") toggleGameObject?.SetActive(true);
        if (objectTag == "Magnet") gameObject.SetActive(false);
        
        //nextHighlightObject?.SetActive(true);
        //thisHighlightObject?.SetActive(false);
        transform.SetParent(targetPosition.transform);
        
       
        
    
}

private IEnumerator RotateSelf()
{
    Debug.Log("rotate OxiTop");
    float duration = 0.5f;
    float elapsed = 0f;
    Quaternion startRot = transform.rotation;
    Quaternion endRot = startRot * Quaternion.Euler(0f, 360f, 0f); // 360° um Y-Achse

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        transform.rotation = Quaternion.Slerp(startRot, endRot, t);
        yield return null;
    }

}

public void ResetInteractable()
{
    transform.SetParent(null); // Optional: detach from any parent
    transform.position = initialPosition;
    transform.rotation = initialRotation;

    Debug.Log(objectTag + " " + initialPosition);

    if (rb != null)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    isHeld = false;
    if (pc != null) pc.isPouring = false;
}

public void SwayAnima()
{
    if (isHeld && holdParent != null)
    {
        // Simuliere leichtes Schwanken basierend auf Zeit
        float swayX = Mathf.Sin(Time.time * holdSwaySpeed) * holdSwayAmount;
        float swayY = Mathf.Cos(Time.time * holdSwaySpeed * 0.5f) * holdSwayAmount;

        Vector3 swayOffset = new Vector3(swayX, swayY, 0);
        Vector3 desiredPosition = targetLocalPosition + swayOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, Time.deltaTime * 5f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetLocalRotation, Time.deltaTime * 5f);
    }
}

 

}
