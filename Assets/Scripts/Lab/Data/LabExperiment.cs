using UnityEngine;
using System.Collections.Generic;

 public enum StepType
{
    Place,         // an Position stellen
    Interact,      // Gerät bedienen (z. B. Button drücken, Pipette einstellen)
    Pour,          // Flüssigkeit übergießen
    Transfer,      // Pipette nutzen o.ä.
    Open,          // Objekt öffnen
    Add,           // z. B. Tabletten hinzufügen
    Custom
}
[CreateAssetMenu(fileName = "LabExperiment", menuName = "Scriptable Objects/LabExperiment")]
public class LabExperiment : ScriptableObject
{

   

    [System.Serializable]
    
public class LabStep
{
    public string description;

    public StepType stepType = StepType.Place;
    public string targetTag;
    public string secondaryTargetTag; // optional (z. B. „ProbeGefäß“ → „BrauneFlasche“)
    public string toolTag;
    public string targetLocationName;
    public float tolerance = 0.15f;
    public float requiredAmount = 0;
    public bool requiresInteraction = false;

    [HideInInspector] public bool completed = false;
    [HideInInspector] public GameObject runtimeTarget;
    [HideInInspector] public Transform runtimeTargetLocation;

    // Optional: Fortschritt bei komplexeren Aufgaben (z. B. 0–1)
    [HideInInspector] public float progress = 0;
}


    public string experimentName;
    public List<LabStep> steps = new List<LabStep>();
    private int currentStep = 0;
    public int CurrentStep => currentStep;


    public void StartExperiment()
    {
        currentStep = 0;
        foreach (var s in steps)
        {
            s.completed = false;
            s.runtimeTarget = GameObject.FindGameObjectWithTag(s.targetTag);
            if (!string.IsNullOrEmpty(s.targetLocationName))
            {
                var locObj = GameObject.Find(s.targetLocationName);
                if (locObj != null)
                    s.runtimeTargetLocation = locObj.transform;
            }
        }
    }

    public bool NextTask()
    {
        steps[currentStep].completed = true;
        currentStep++;
        Debug.Log("nextTask: " + currentStep);
        UpdateHighlight();
        if (currentStep >= steps.Count)
        {
            Debug.Log($"{experimentName} abgeschlossen!");
            return true;
        }
        return false;
    }

    public string GetCurrentTaskText()
    {
        if (currentStep < steps.Count)
            return steps[currentStep].description;
        return "Experiment abgeschlossen!";
    }

    

    public void UpdateHighlight()
   {
       //setactive(false) the gameobject with the name Highlight + currentStep-1
       //setactive(true) the gameobject with the name Highlight + currentStep
   } 

   public void ReportProgress(float amount)
{
    var step = GetCurrentStep();
    if (step == null) return;

    step.progress += amount;
    if (step.progress >= 1f)
        CompleteCurrentStep();
}

public void CompleteCurrentStep()
{
    if (currentStep < steps.Count)
    {
        steps[currentStep].completed = true;
        currentStep++;
        Debug.Log("completed Step " + currentStep);
        UpdateHighlight();
    }
}

public void QuitExperiment()
{
    Debug.Log($"Experiment '{experimentName}' wurde abgebrochen.");

    // Reset all step states
    foreach (var step in steps)
    {
        step.completed = false;
        step.progress = 0;
        step.runtimeTarget = null;
        step.runtimeTargetLocation = null;
    }

    currentStep = 0;

    // Optionally disable any active highlights
    UpdateHighlight();
}

public void RestartExperiment()
{
    Debug.Log($"Experiment '{experimentName}' wird neu gestartet.");

    currentStep = 0;

    foreach (var s in steps)
    {
        s.completed = false;
        s.progress = 0;
        s.runtimeTarget = GameObject.FindGameObjectWithTag(s.targetTag);

        if (!string.IsNullOrEmpty(s.targetLocationName))
        {
            var locObj = GameObject.Find(s.targetLocationName);
            if (locObj != null)
                s.runtimeTargetLocation = locObj.transform;
        }
        else
        {
            s.runtimeTargetLocation = null;
        }
    }

    UpdateHighlight();
}



public LabStep GetCurrentStep() => 
    currentStep < steps.Count ? steps[currentStep] : null;


    
}
