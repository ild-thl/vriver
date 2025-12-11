using UnityEngine;

using TMPro;

public class PourableContainer : LabInteractable
{

    [Header("BottleData")]
    public bool isOxiBottle = false;
    
    
    public GameObject Funnel1;
    [Header("Flüssigkeitsparameter")]
    public float maxFillAmount = 400f;       // Maximale Füllmenge in ml
    public float currentFill = 0f;           // ml
    public float density = 1f;               // g/ml (Wasser = 1)
    public float pourSpeed = 50f;
    
    [Header("Pour Settings")]
    public float tiltAngle = -90f;        // Kippwinkel beim Ausgießen
    public float pourCheckDistance = 0.5f;
    public LayerMask pourableLayer; // Layer, auf die der Raycast treffen soll
    public Transform belowCheckTransform;

    [Header("Runtime")]
    
    public bool isPouring = false;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    public Transform handPosition;

    public Transform bottleneck;
   

    [Header("Effects")]
    public ParticleSystem pourParticles;
    public AudioSource pourSound;

    public TextMeshProUGUI fillUI;
    
    // Statische Referenz auf die aktuell gehaltene Flasche
    private static PourableContainer currentlyHeldBottle;

    // Der Container unter dieser Flasche (wird dynamisch ermittelt)
    public PourableContainer bottleBelow;
    public InteractableManager interactableManager;
    

    

    void Start()
{
    startRotation = transform.rotation;
    targetRotation = startRotation;
    initialPosition = transform.position;
    initialRotation = transform.rotation;

    // Setze die Anfangsfüllung auf maximal
    
    UpdateFillUI();

    if (pourParticles != null) pourParticles.Stop();
    if (pourSound != null) pourSound.Stop();
}



private void Update()
{
    if (!isHeld)
    {
        HandleEffects(false);
        return;
    }

    // Tilt nach links (Z-Rotation)
    float tiltZ = isPouring ? tiltAngle : 0f;

    // Y-Rotation zur Kamera
    Vector3 lookDir = Camera.main.transform.position - transform.position;
    lookDir.y = 0;
    float cameraY = lookDir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(lookDir).eulerAngles.y : 0f;

    // Rotation um bottleneck berechnen
    Quaternion desiredRot = Quaternion.Euler(0f, cameraY, tiltZ);

    // >>> Flasche um bottleneck drehen
    RotateAroundPoint(transform, bottleneck.position, desiredRot, Time.deltaTime * 5f);

    // Rest (Check, Pour, etc.)
    CheckContainerBelow();

    if (isPouring && currentFill > 0f)
    {
        Pour();
        HandleEffects(true);
    }
    else
    {
        HandleEffects(false);
        SwayAnima();
    }


}


private void RotateAroundPoint(Transform target, Vector3 pivot, Quaternion desiredRot, float lerpSpeed)
{   
    Debug.Log("Rotate ");
    Debug.DrawLine(pivot, target.position, Color.red);

    // Behalte aktuelle Position und Rotation
    Quaternion currentRot = target.rotation;

    // Sanft lerpen
    Quaternion newRot = Quaternion.Lerp(currentRot, desiredRot, lerpSpeed);

    // Berechne den Offset vom Pivot zur Flasche
    Vector3 offset = target.position - pivot;

    // Rotiere den Offset um den Pivot
    offset = newRot * (Quaternion.Inverse(currentRot) * offset);

    // Neue Position und Rotation setzen
    target.position = pivot + offset;
    target.rotation = newRot;
}



public void OnBottleTiltPerformed()
{
    if (!isHeld || currentFill <= 0f) return;
    Debug.Log("TILT");
    isPouring = true;
    if (bottleBelow != null)
    {

   
    if (bottleBelow.Funnel1 != null)
    {
        bottleBelow.Funnel1.SetActive(true);
    }
     }
     Debug.Log("TILT2");
     handPosition = interactableManager.handPosition;
    // Zielrotation einmal setzen
    targetRotation = Quaternion.Euler(handPosition.eulerAngles.x + tiltAngle,
                                      handPosition.eulerAngles.y,
                                      handPosition.eulerAngles.z);
}

public void OnBottleTiltCanceled()
{
    isPouring = false;
     Debug.Log("TILT STOP");
    if (bottleBelow != null)
    {
        if (bottleBelow.Funnel1 != null)
        {
            bottleBelow.Funnel1.SetActive(false);
        }
        if (bottleBelow != null && bottleBelow != this)
        {
            if (bottleBelow.isOxiBottle)
            {
                OxiBottle oxiBottle = bottleBelow.GetComponent<OxiBottle>();
                
                oxiBottle?.Check(bottleBelow.currentFill);
            }
        
        }
    }
     Debug.Log("TILT STOP2");
    // Zielrotation zurück auf Startrotation setzen
    targetRotation = startRotation;
}

    void Pour()
    {
        float pourAmount = pourSpeed * Time.deltaTime; // ml pro Sekunde
       
        Debug.Log("Pour");
            
            if (bottleBelow != null && bottleBelow != this)
            {
                Debug.Log("Pour2");
                float space = bottleBelow.maxFillAmount - bottleBelow.currentFill;
                float amountToPour = Mathf.Min(pourAmount, space, currentFill);

                bottleBelow.Fill(amountToPour);
                currentFill -= amountToPour;
                currentFill = Mathf.Clamp(currentFill, 0f, maxFillAmount);
                
                UpdateFillUI();
                UpdateWeight();
                return;
            }
        

        // Kein anderer Container drunter → einfach reduzieren
        currentFill -= pourAmount;
        currentFill = Mathf.Clamp(currentFill, 0f, maxFillAmount);
        UpdateFillUI();
    }

    public void Fill(float amount)
    {
        currentFill += amount;
        currentFill = Mathf.Clamp(currentFill, 0f, maxFillAmount);
       
        UpdateFillUI();
        UpdateWeight();
    }

    void UpdateFillUI()
    {
        Debug.Log("UpdateFillUI");
        if (fillUI != null)
            fillUI.text = Mathf.RoundToInt(currentFill) + " ml";
    }

    
    public override void UpdateWeight()
    {
        // 1 ml ≈ 1 g für Wasser; kann für andere Flüssigkeiten angepasst werden
        weight = baseWeight + (currentFill * density);
    }

    public override float GetCurrentWeight()
    {
        return baseWeight + (currentFill * density);
    }

    private void HandleEffects(bool active)
{
    if (active)
    {
        if (pourParticles != null && !pourParticles.isPlaying)
            pourParticles.Play();
        if (pourSound != null && !pourSound.isPlaying)
            pourSound.Play();
    }
    else
    {
        if (pourParticles != null && pourParticles.isPlaying)
        {
            pourParticles.Stop();
            pourParticles.Clear();
        }
        if (pourSound != null && pourSound.isPlaying)
            pourSound.Stop();
    }
}


    

private void CheckContainerBelow()
{
    Ray ray = new Ray(bottleneck.position, Vector3.down);
    if (Physics.Raycast(ray, out RaycastHit hit, pourCheckDistance, pourableLayer))
    {
        // Logge den Namen des getroffenen Colliders
        Debug.Log($"Raycast hit collider: {hit.collider.name}");

        // Suche die PourableContainer-Komponente im Parent
        var target = hit.collider.GetComponentInParent<PourableContainer>();
        if (target != null && target != this)
        {
            bottleBelow = target;
            hintUIGameObject.SetActive(true);
            hintUI.text = bottleBelow.name;
            Debug.Log($"PourableContainer below: {bottleBelow.name}");
        }
        else
        {
            bottleBelow = null;
            hintUIGameObject.SetActive(false);
        }
    }
    else
    {
        bottleBelow = null;
        hintUIGameObject.SetActive(false);
    }

    // Ray sichtbar im Editor
    Debug.DrawRay(bottleneck.position, Vector3.down * pourCheckDistance, Color.red);
    
}


/*
private void Check(float fill)
    {

        if (Mathf.RoundToInt(fill) == 432)
        {
            var exp = LabManager.Instance?.experiments[0];
            if (exp == null) return;
            var step = exp.GetCurrentStep();
            if (step == null) return;
        
            if (step.targetTag == "SampleBottle" && !hasCorrectFill)
            {
                hasCorrectFill = true;
                Debug.Log("Oxibottle has correct fill");
                oxiBottleCheckUIGameObject.SetActive(true);
                
                Debug.Log($"✅ Correct Fill");

                LabManager.Instance.OnTaskCompleted();
            }
        }
        else
        {
            hasCorrectFill = false;
            oxiBottleCheckUIGameObject.SetActive(false);
            Debug.Log("no correct fill");
        }

        
        
    }

    private void CheckDropAmount(float fill)
    {

        if (dropAmount == 10)
        {
            hasDropFill = true;
            Debug.Log("Oxibottle has correct Drop fill");

            var exp = LabManager.Instance?.experiments[0];
            if (exp == null) return;
            var step = exp.GetCurrentStep();
            if (step == null) return;
        
            if (step.targetTag == "SampleBottle")
            {
                oxiBottleCheckUIGameObject.SetActive(true);
                
                
                Debug.Log($"✅ Correct Fill");

                LabManager.Instance.OnTaskCompleted();
            }
        }
        else
        {
            hasCorrectFill = false;
            oxiBottleCheckUIGameObject.SetActive(false);
            Debug.Log("no correct fill");
        }

        
        
    }

    public void AddNaOH(GameObject currentPrefab)
    {
        Debug.Log("Add NaOH");
      
        currentPrefab.transform.SetParent(naOHPosition);
        currentPrefab.transform.position = naOHPosition.position;

        naOHAmount++;
        CheckNaOH();
    }

    private void CheckNaOH()
    {
        var exp = LabManager.Instance?.experiments[0];
            if (exp == null) return;
            var step = exp.GetCurrentStep();
            if (step == null) return;
        
            if (step.targetTag == "Box2")
            {
                if (naOHAmount < step.requiredAmount)
                return;

                if (naOHAmount == step.requiredAmount)
                {
                    Debug.Log($"✅ Correct NaOH Amount");

                    LabManager.Instance.OnTaskCompleted();
                }
                else
                {
                    LabManager.Instance.FailedExp();
                }
                
                
            }
    }

*/
}
