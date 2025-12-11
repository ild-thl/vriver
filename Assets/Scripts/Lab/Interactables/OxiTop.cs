using UnityEngine;

public class OxiTop : LabInteractable
{
    public GameObject screen1;
    public GameObject screen2;

    public bool isOnOxiBottle = false;

    public int buttonPressCount = 0;

    public void PressOxiTopButton()
    {
        Debug.Log("OxiTopButton");

      
    

        if (!isOnOxiBottle || buttonPressCount >= 2)
            return;

        buttonPressCount++;

        if (buttonPressCount == 1)
        {
            screen1.SetActive(true);
            return;
        }
            
        screen1.SetActive(false);
        screen2.SetActive(true);
        var exp = LabManager.Instance?.experiments[0];
        var step = exp?.GetCurrentStep();
        if (step == null || step.targetTag != "OxiTop" || buttonPressCount < step.requiredAmount)
            return;

        Debug.Log("Pressed 2 times");
        LabManager.Instance.OnTaskCompleted();


            
    }
            
        public void CheckOxiTopAttachmentStep()
    {
        var exp = LabManager.Instance?.experiments[0];
        var step = exp?.GetCurrentStep();
        
        Debug.Log("OxiTopButton attached");
        if (isOnOxiBottle && step.targetTag == "OxiTop") LabManager.Instance.OnTaskCompleted();     
    }
    


}
