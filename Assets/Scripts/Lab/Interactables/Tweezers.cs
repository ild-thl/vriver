using UnityEngine;
using TMPro;

public class Tweezers : LabInteractable
{

    private bool hasNaOH = false;

    public GameObject prefabNaOH; //NaOH-Plättchen Prefab 
    
    public float pourCheckDistance = 0.5f;
    public LayerMask pourableLayer; // Layer, auf die der Raycast treffen soll
    public TextMeshProUGUI oxiBottleCheckUI;
    public GameObject oxiBottleCheckUIGameObject;
    

    public GameObject currentPrefab;

    public Transform naOHTweezerPosition;

    public GameObject highlight;
    public GameObject naOHHighlight;


    // Statische Referenz auf die aktuell gehaltene Flasche
    private static PourableContainer currentlyHeldBottle;

    // Der Container unter dieser Flasche (wird dynamisch ermittelt)
    public PourableContainer bottleBelow;
    public OxiBottle oxiBottleBelow;

    

    // Update is called once per frame
    void Update()
    {

         if (!isHeld)
    {
        
        return;
    }
        // Prüfen, ob ein Container unter der Flasche ist
    CheckContainerBelow();
    SwayAnima();

    
    }


    public void UseTweezers()
    {
        if (bottleBelow == null)
        return;
        Debug.Log("usetweezers");
       
        if (bottleBelow.isBox2)
        {
             if (!hasNaOH)
            {
            Debug.Log("UseTweezers 1");
            TakeNaOH();
            }
        }
        else if (bottleBelow.isOxiBottle && hasNaOH)
        {
                Debug.Log("UseTweezers 2");
                AddNaOHToOxiBottle();
            
        }
    }


    private void TakeNaOH()
    {
        //Instantiates one prefab at handPosition
        GameObject prefab = Instantiate(prefabNaOH, naOHTweezerPosition);
        
        currentPrefab = prefab;
        hasNaOH = true;
        Debug.Log("NaOH in Hand");
    }


    private void AddNaOHToOxiBottle()
    {
        //Calls the function that teleports the prefab at handposition to NaOHPosition in OxiBottle
        oxiBottleBelow.AddNaOH(currentPrefab);
        hasNaOH = false;
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
            oxiBottleBelow = hit.collider.GetComponentInParent<OxiBottle>();
        }
        else
        {
            bottleBelow = null;
            oxiBottleBelow = null;
            hintUIGameObject.SetActive(false);
        }
    }
    else
    {
        bottleBelow = null;
         oxiBottleBelow = null;
        hintUIGameObject.SetActive(false);
    }

    // Ray sichtbar im Editor
    Debug.DrawRay(transform.position, Vector3.down * pourCheckDistance, Color.red);
    }

    public void HideHintUI()
    {
        bottleBelow = null;
        hintUIGameObject.SetActive(false);
    }
}
