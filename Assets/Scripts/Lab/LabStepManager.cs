using UnityEngine;
using System.Collections.Generic;

public class LabStepManager : MonoBehaviour
{
    [System.Serializable]
    public class LabStep
    {
        public string stepName;
        public string description;
        public GameObject requiredObject;
        public Transform targetSpot;
        public float requiredAmount; // optional (z.B. 432ml)
    }

    public List<LabStep> steps = new List<LabStep>();
    private int currentStepIndex = 0;

    public delegate void StepEvent(LabStep step);
    public event StepEvent OnStepStarted;
    public event StepEvent OnStepCompleted;

    void Start()
    {
        if (steps.Count > 0)
            StartStep(0);
    }

    void StartStep(int index)
    {
        if (index < steps.Count)
        {
            currentStepIndex = index;
            OnStepStarted?.Invoke(steps[currentStepIndex]);
            Debug.Log($"🔬 Step gestartet: {steps[currentStepIndex].stepName}");
        }
        else
        {
            Debug.Log("✅ Alle Schritte abgeschlossen!");
        }
    }

    public void CompleteCurrentStep()
    {
        var step = steps[currentStepIndex];
        Debug.Log($"✅ Step abgeschlossen: {step.stepName}");
        OnStepCompleted?.Invoke(step);

        if (currentStepIndex + 1 < steps.Count)
            StartStep(currentStepIndex + 1);
        else
            Debug.Log("🏁 Experiment abgeschlossen!");
    }

    public LabStep GetCurrentStep()
    {
        return steps[currentStepIndex];
    }
}
