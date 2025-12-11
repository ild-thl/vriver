using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    [Header("Alle Interactables in der Szene (automatisch gefüllt)")]
    public LabInteractable[] Interactables;
    public GameObject noteBookInLab;
    public GameObject rQPODInLab;
    public GameObject rQPODCaseInLab;
    public GameObject sampleBottleInLab;
    public Transform handPosition;
    public LabComputer labComputer;
    

    void Awake()
    {
        // Sucht alle aktiven Interactables in der Szene
        Interactables = FindObjectsOfType<LabInteractable>(true); // true = auch inaktiven Objekte
        //Debug.Log($"[InteractableManager] {Interactables.Length} Interactables gefunden.");
    }

    void Start()
    {
        // Optional: Beispielhafte Ausgabe
        foreach (var interactable in Interactables)
        {
            //Debug.Log($" - {interactable.name} ({interactable.GetType().Name})");
        }
    }

    public void RefreshList()
{
    Interactables = FindObjectsOfType<LabInteractable>(true);
    //Debug.Log($"[InteractableManager] Liste aktualisiert: {Interactables.Length} Interactables.");
}

 public void Reset()
{
    if (Interactables == null || Interactables.Length == 0)
    {
        //Debug.LogWarning("[InteractableManager] Keine Interactables zum Zurücksetzen gefunden.");
        return;
    }

    foreach (var interactable in Interactables)
    {
        if (interactable != null)
        {
            interactable.ResetInteractable();
            //Debug.Log($"[InteractableManager] {interactable.name} wurde zurückgesetzt.");
        }
    }
}

 public void SetHandPosition()
 {
      GameObject h = GameObject.FindGameObjectWithTag("HandPosition");
      if (h != null)
      {
          handPosition = h.transform;
      }
 }

 
public void ReloadMapUI()
 {
      labComputer.ReloadMapUI();
 }
}
