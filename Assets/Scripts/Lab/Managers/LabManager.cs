using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LabManager : MonoBehaviour
{
    public static LabManager Instance;  // Singleton für globalen Zugriff

    [Header("Experimente")]
    public List<LabExperiment> experiments = new List<LabExperiment>();

    [Header("UI")]
    public TextMeshProUGUI currentTaskText;
    public TextMeshProUGUI currentExperimentText;
    public ExperimentUI experimentUI;

    private LabExperiment currentExperiment;
    public LabExperiment CurrentExperiment
{
    get { return currentExperiment; }
    set { currentExperiment = value; } // optional setter
}
    public GameObject currentlyHeldObject;
    public HighlightManager hm;
   
    public InteractableManager lim;
    public delegate void StepEvent(LabExperiment.LabStep step, float progressValue);
    public static event StepEvent OnProgressReported;

    void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {
        hm = FindFirstObjectByType<HighlightManager>();
        lim = FindFirstObjectByType<InteractableManager>();
        hm.HideAll();
        
    }
    
    public void StartExperiment(int expIndex)
    {
        
        if (experiments.Count < expIndex)
        return;

        QuitActiveExp();
        LabExperiment exp = experiments[expIndex];   
        currentExperiment = exp;
        Debug.Log("Start Experiment" + exp.experimentName);
        
        exp.StartExperiment();

        experimentUI.ShowExperiment(exp);
        experimentUI.UpdateProgress(exp);
        hm.UpdateHighlights(currentExperiment);
        UpdateUI();
        
    }

    public void OnTaskCompleted()
    {
        if (currentExperiment != null)
        {
            bool done = currentExperiment.NextTask();
            UpdateUI();
             experimentUI.UpdateProgress(currentExperiment);
             hm.UpdateHighlights(currentExperiment);
            if (done)
            {
                Debug.Log("Experiment abgeschlossen!");
                experimentUI.DisplaySuccessUI();
                
                // Optional: nächstes Experiment starten
            }
        }
    }

    private void UpdateUI()
    {
        Debug.Log("updateUI");
        if (currentExperiment == null) return;
        currentTaskText.text = currentExperiment.GetCurrentTaskText();
        
        currentExperimentText.text = currentExperiment.experimentName;

        
    }

    
private void CompleteStep(LabExperiment.LabStep step)
{
    Debug.Log($"✅ Schritt '{step.description}' erfolgreich abgeschlossen.");
    currentExperiment.CompleteCurrentStep();
    UpdateUI();
    experimentUI.UpdateProgress(currentExperiment);
}

public void QuitActiveExp()
{
    if (currentExperiment != null)
    {
        Debug.Log($"Experiment '{currentExperiment.experimentName}' wurde beendet.");
        currentExperiment.QuitExperiment(); // Optional: implement this in LabExperiment if needed
        currentExperiment = null;

        currentTaskText.text = "";
        currentExperimentText.text = "";
         lim.Reset();
        experimentUI.ShowExperiment(null);
    }
}

public void RestartActiveExp()
{
    if (currentExperiment != null)
    {
        Debug.Log($"Experiment '{currentExperiment.experimentName}' wird neu gestartet.");
        currentExperiment.RestartExperiment(); // Optional: implement this in LabExperiment if needed
        lim.Reset();
        experimentUI.ShowExperiment(currentExperiment);
        experimentUI.UpdateProgress(currentExperiment);

        UpdateUI();
    }
}

    public void FailedExp()
    {
       
        Debug.Log("Failed EXP");
        experimentUI.DisplayFailUI(currentExperiment);
        
    }

    public void Wait5Days()
    {
        Debug.Log("LabManager Wait 5 Days");
        experimentUI.Wait5DaysUIAnimation();
    }
}
