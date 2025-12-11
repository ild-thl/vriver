using UnityEngine;
using TMPro;

public class OxiBottle : PourableContainer
{
    
    public bool hasCorrectFill = false;
    public bool hasDropFill = false;
    public bool hasNaOHFill = false;
    public bool hasMagnet = false; 
    public bool hasFunnel2 = false;
    public bool has10Drops = false;
    public bool has2NaOH = false;

    public bool isOnPlatform = false;
     public Collider bottleCollider;

    public int dropAmount = 0;  //0 von 10
    public int naOHAmount = 0; //0 von 2

    public int daysTested = 0;  //0 von 5
    
    

    public Transform naOHPosition;
    public Transform oxiTopPosition;


    public TextMeshProUGUI oxiBottleCheckUI;
    public GameObject oxiBottleCheckUIGameObject;





    public void Check(float fill)
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

    

    

    
}
