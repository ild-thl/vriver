using UnityEngine;

public class MixingPlatform : MonoBehaviour
{
    public bool hasOxiBottle = false;
    public int daysTested = 0;
    public ExperimentUI expUI;

    public void AddOxiBottle(OxiBottle oxi)
    {
        
        hasOxiBottle = true;
        var exp = LabManager.Instance?.experiments[0];
        var step = exp?.GetCurrentStep();
        
        Debug.Log("OxiBottle on MixingPlatform");
        if (hasOxiBottle && step.targetTag == "OxiBottle" && step.secondaryTargetTag == "Platform")
        {
            LabManager.Instance.OnTaskCompleted(); 
            expUI.ShowWait5DaysUIButton();
        }
        else
        {
            Debug.Log("Mixing Platform Error");
        }
        
        

    }

    public void Wait5Days()
    {
        daysTested = 5;
        var exp = LabManager.Instance?.experiments[0];
        if (exp == null) return;
        var step = exp.GetCurrentStep();
        if (step == null) return;
        
            if (step.targetTag == "Platform")
            {
                
                    Debug.Log("Waited 5 days");

                    LabManager.Instance.OnTaskCompleted();
                    expUI.HideWait5DaysUIButton();
                    LabManager.Instance.Wait5Days();

                
        }
    }


}
