using UnityEngine;
using TMPro;
using System.Collections;

public class Pipette : LabInteractable
{
    [Header("Pipette")]
   
    public int dropletCount = 0;
    public int targetDrops = 10;

    public bool task1Completed = false; //Richtige DropSize
    public bool task2Completed = false; //Hemmstoff aufgenommen
    public bool task3Completed = false; //10 Tropfen hinzugefügt

    public bool hasCorrectSetting = false;

    
    public float maxFillAmount = 10f; //ml
    public float currentFillAmount = 0f;  //ml
    public float dropSize = 0.5f;
    public float density = 1f;
    public Transform pipetteTip;
    public Transform dropTarget;
    public AudioSource dropSound;
    public TextMeshProUGUI dropSizeTextUI;
    public TextMeshProUGUI currentFillAmountTextUI;

    public GameObject dropSizeCheckUIGameObject;
    
    public OxiBottle oxiBottleBelow;
    
    
    public PourableContainer bottleBelow;
    public float pourCheckDistance = 0.5f;
    public LayerMask pourableLayer; // Layer, auf die der Raycast treffen soll
    


    private void Update() {
        if (!isHeld) return;
        CheckContainerBelow();

    }
    public override void UpdateWeight()
    {
        weight = baseWeight + (currentFillAmount * density);
    }

    public override float GetCurrentWeight()
    {
        return baseWeight + (currentFillAmount * density);
    }


    public void UsePipette()
    {
        if (bottleBelow == null)
        return;

        if (bottleBelow.isBox1)
        {
            Debug.Log("UsePipette 1");
            FillPipette();
        }
        else if (bottleBelow.isOxiBottle)
        {
             Debug.Log("UsePipette 2");
            DropADrop();
        }
    }
    public void DropADrop() // Taste/Button über OxiBottle
    {
       
        if (currentFillAmount > 0 && hasCorrectSetting)
        {
             Debug.Log("dropadrop");
            currentFillAmount -= dropSize;
            currentFillAmount = Mathf.Max(0f, currentFillAmount);
            UpdateWeight();
            UpdatePipetteUI();
            dropletCount++;
            Debug.Log("droplet");
            Debug.Log(dropletCount);
            oxiBottleBelow.dropAmount++;
            Debug.Log("droplet2");
              Debug.Log(oxiBottleBelow.dropAmount);
            if (dropSound) dropSound.Play();
        }
        if (dropletCount == targetDrops)
        {
            Debug.Log("10 Tropfen abgegeben!");
            task3Completed = true;
            LabManager.Instance.OnTaskCompleted();
        }

        if(oxiBottleBelow.dropAmount > targetDrops)
        {
            LabManager.Instance.FailedExp();
        }
    }

    public void FillPipette() // Taste/Button über Hemmstoff Dose
    {
        currentFillAmount = maxFillAmount;
        if (task1Completed)
        {
            LabManager.Instance.OnTaskCompleted();
            task2Completed = true;
        }
        UpdateWeight();
        UpdatePipetteUI();
        
        StartCoroutine(AnimateFillMotion());
    }

    public void SetDropSize() // klicken
{
    if (!task1Completed)
    {
        Debug.Log("SetDropSize");
        dropSize += 0.1f;

        if (dropSize > 1.0f)
            dropSize = 0.1f;

        dropSize = Mathf.Round(dropSize * 10f) / 10f; // Rundet auf eine Nachkommastelle
        UpdatePipetteUI();
    }
    
}



    public void UpdatePipetteUI()
    {

        if (dropSize == 0.5f && !task1Completed)
        {
            hasCorrectSetting = true;
            Debug.Log("Pipette has correct setting");
           
            dropSizeCheckUIGameObject.SetActive(true);

            if (!task1Completed)
            {
                 LabManager.Instance.OnTaskCompleted();
                 task1Completed = true;
                 Debug.Log("task1Completed");
            }
        }
        else
        {
             hasCorrectSetting = true;
            dropSizeCheckUIGameObject.SetActive(false);
        }
        currentFillAmountTextUI.text = Mathf.Round(currentFillAmount) + " ml";
        dropSizeTextUI.text = dropSize + " ml";


    }

    private void CheckContainerBelow()
{
    Ray ray = new Ray(transform.position, Vector3.down);
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
            if (bottleBelow.isBox1)
            {
                dropTarget = hit.collider.gameObject.transform;
            }
            oxiBottleBelow = hit.collider.GetComponentInParent<OxiBottle>();
        }
        else
        {
            bottleBelow = null;
            oxiBottleBelow = null;
            dropTarget = null;
            hintUIGameObject.SetActive(false);
        }
    }
    else
    {
        bottleBelow = null;
         oxiBottleBelow = null;
        dropTarget = null;
        hintUIGameObject.SetActive(false);
    }

    // Ray sichtbar im Editor
    Debug.DrawRay(transform.position, Vector3.down * pourCheckDistance, Color.red);
}
    private IEnumerator AnimateFillMotion()
{
    if (dropTarget == null) yield break;

    Vector3 startPos = transform.position;
    Vector3 targetPos = dropTarget.position;

    float duration = 0.5f;
    float elapsed = 0f;

    // Move down
    while (elapsed < duration)
    {
        transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }
    transform.position = targetPos;

    yield return new WaitForSeconds(0.2f);

    // Move back up
    elapsed = 0f;
    while (elapsed < duration)
    {
        transform.position = Vector3.Lerp(targetPos, startPos, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }
    transform.position = startPos;
}

    
}
