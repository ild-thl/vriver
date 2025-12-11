using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ExperimentUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform stepListParent;          // Panel mit VerticalLayoutGroup
    public GameObject stepEntryPrefab;        // Prefab für jeden Schritt

    private List<GameObject> stepEntries = new();
    private LabExperiment currentExp;
    public GameObject expUIFull;
    public bool expListHidden = true;
    public GameObject expStepListBig;
    public GameObject expStepListSmall;

    public TextMeshProUGUI currentExpTextSmall;

    public TextMeshProUGUI mixingPlatformDaysTestedText;
    public GameObject wait5DaysUIButton;
    public GameObject resultUI;

    public CanvasGroup wait5DaysCanvasGroup;
    public GameObject wait5DaysUI;
    public GameObject clockObject;
    public float fadeDuration = 1f;
    public float waitDuration = 5f;
    public float rotationSpeed = 720f; // degrees per second
    

    public GameObject failUI;
    public TextMeshProUGUI failUIText;
    public float failDisplayDuration = 5f;
    public float successDisplayDuration = 5f;
    private Coroutine failCoroutine;
    private Coroutine successCoroutine;

    public GameObject computerUI;


    public GameObject successUI;

    public Color defaultColor;
    public Color completedColor;
    public Color activeColor;

    public GameObject[] rQPODComputerResultGraph;
    public GameObject[] rQPODComputerResultGraphInRoom;
    public TextMeshProUGUI[] rQPODComputerResultText;

    public GameObject bouyComputerUI;
    public GameObject noBouyDataComputerUI;
    public GameObject successBouyDataComputerUI;
    public GameObject noBouyDataComputerInRoom;
    public GameObject successBouyDataComputerInRoom;

   
    public GameObject bouyData_O2Big;
    
    public GameObject bouyData_TempBig;

    public GameObject oxiTopDataUI;
    public GameObject oxiTopDataUI_1Big;
    public GameObject oxiTopDataUI_2Big;

    public GameObject startExp1UI; //Oxitop
    public GameObject quitExp1UI;

    private void Start() {
        
        
    }
    public void ShowExperiment(LabExperiment exp)
    {
        ClearStepList();
        currentExp = exp;
        if (exp == null)
        {
            currentExpTextSmall.text = "";
            expUIFull.SetActive(false);
            return;
        }
        expUIFull.SetActive(true);
        foreach (var step in exp.steps)
        {
            var entry = Instantiate(stepEntryPrefab, stepListParent);
            var text = entry.GetComponentInChildren<TextMeshProUGUI>();
            text.text = step.description;

            var img = entry.GetComponentInChildren<Image>();
            img.color = step.completed ? completedColor : defaultColor;

            stepEntries.Add(entry);
        }

         currentExpTextSmall.text = exp.steps[exp.CurrentStep].description;

    }

    public void UpdateProgress(LabExperiment exp)
    {
        if (exp == null || exp != currentExp) return;

        for (int i = 0; i < exp.steps.Count; i++)
        {
            var step = exp.steps[i];
            var entry = stepEntries[i];
            var img = entry.GetComponentInChildren<Image>();

            if (step.completed)
                img.color = completedColor; // grün
            else if (i == exp.CurrentStep)
            {
                img.color = activeColor; 
                currentExpTextSmall.text = exp.steps[exp.CurrentStep].description;
                var imgSmall = expStepListSmall.GetComponentInChildren<Image>();
                imgSmall.color = activeColor; 
            }
            else
            {
                img.color = Color.white;
            }
        }
        

       
    }

    private void ClearStepList()
    {
        foreach (Transform child in stepListParent)
            Destroy(child.gameObject);
        stepEntries.Clear();
    }

    public void DisplayFailUI(LabExperiment exp)
    {
        if (exp == null) return;

        string expName = exp.experimentName;
        int stepNumber = exp.CurrentStep + 1;

        failUIText.text = $"{expName} – Schritt {stepNumber} fehlgeschlagen";

        failUI.SetActive(true);

        if (failCoroutine != null)
            StopCoroutine(failCoroutine);

        failCoroutine = StartCoroutine(HideFailUIAfterDelay());
        LabManager.Instance.QuitActiveExp();
    }

    private IEnumerator HideFailUIAfterDelay()
    {   
        yield return new WaitForSeconds(failDisplayDuration);
        failUI.SetActive(false);
        failCoroutine = null;
    }
    public void ToggleExpUI()
    {
        expStepListBig.SetActive(expListHidden);
        expStepListSmall.SetActive(!expListHidden);
        expListHidden = !expListHidden;

    }

    public void DisplaySuccessUI()
    {
  

    successUI.SetActive(true);
    if (successCoroutine != null)
            StopCoroutine(successCoroutine);

        successCoroutine = StartCoroutine(HideSuccessUIAfterDelay());
        LabManager.Instance.QuitActiveExp();
    
    }

    private IEnumerator HideSuccessUIAfterDelay()
    {   
        yield return new WaitForSeconds(successDisplayDuration);
        successUI.SetActive(false);
        successCoroutine = null;
    }

    public void Wait5DaysUIAnimation()
{
    StartCoroutine(AnimateWait5DaysUI());
}

private IEnumerator AnimateWait5DaysUI()
{
    wait5DaysUI.SetActive(true);

    // Fade in
    yield return StartCoroutine(FadeCanvasGroup(wait5DaysCanvasGroup, 0f, 1f, fadeDuration));

    float elapsed = 0f;

    // Rotate clock while waiting
    while (elapsed < waitDuration)
    {
        elapsed += Time.deltaTime;
        clockObject.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        yield return null;
    }

    // Fade out
    yield return StartCoroutine(FadeCanvasGroup(wait5DaysCanvasGroup, 1f, 0f, fadeDuration));

    wait5DaysUI.SetActive(false);
}

private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
{
    float elapsed = 0f;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
        yield return null;
    }
    cg.alpha = end;
    mixingPlatformDaysTestedText.text = "Tage: " + 5;
    resultUI.SetActive(true);

}

    public void ShowWait5DaysUIButton()
    {
        wait5DaysUIButton.SetActive(true);
    }

    public void HideWait5DaysUIButton()
    {
        wait5DaysUIButton.SetActive(false);
    }

    public void OpenComputerUI()
    {
        computerUI.SetActive(true);
    }

    public void CloseComputerUI()
    {
        computerUI.SetActive(false);
    }


    public void OpenBuoyComputerUI()
    {
        bouyComputerUI.SetActive(true);
    }

    public void CloseBuoyComputerUI()
    {
        bouyComputerUI.SetActive(false);
    }
    public void SetUpLabComputerUI()
{
    GameManager2 gm = GameManager2.Instance;

    // Alle Graphen erstmal deaktivieren
    for (int i = 0; i < rQPODComputerResultGraph.Length; i++)
    {
        if (rQPODComputerResultGraph[i] != null)
            rQPODComputerResultGraph[i].SetActive(false);

        if (rQPODComputerResultText[i] != null)
            rQPODComputerResultText[i].text = "";
    }

    // Jetzt nur die fertigen anzeigen
    for (int i = 0; i < gm.trackFinished.Length; i++)
    {
        if (gm.trackFinished[i])
        {
            if (rQPODComputerResultGraph[i] != null)
                rQPODComputerResultGraph[i].SetActive(true);
            
            if (rQPODComputerResultGraphInRoom[i] != null)
                rQPODComputerResultGraphInRoom[i].SetActive(true);

            if (rQPODComputerResultText[i] != null)
            {
                float resultValue = 0f;
                switch (i)
                {
                    case 0: resultValue = gm.resultTrack1; break;
                    case 1: resultValue = gm.resultTrack2; break;
                    case 2: resultValue = gm.resultTrack3; break;
                    case 3: resultValue = gm.resultTrack4; break;
                }

                rQPODComputerResultText[i].text = "Genauigkeit: " + (resultValue * 100f).ToString("F1") + "%";
               
            }
        }
    }
    if (gm.buoyDataTaken)
    {
        DisplayBuoyData();
    }
    
}

public void DisplayBuoyData()
{
    GameManager2 gm = GameManager2.Instance;

    if (gm.buoyDataTaken)
    {
        noBouyDataComputerUI.SetActive(false);
        successBouyDataComputerUI.SetActive(true);
        noBouyDataComputerInRoom.SetActive(false);
        successBouyDataComputerInRoom.SetActive(true);
    }
    else
    {
        noBouyDataComputerUI.SetActive(true);
        successBouyDataComputerUI.SetActive(false);
        noBouyDataComputerInRoom.SetActive(true);
        successBouyDataComputerInRoom.SetActive(false);

    }
    
}

public void ShowExp1UIPopUP()
{
    LabManager lm = LabManager.Instance;
        if (lm != null)
        {
            if (lm.CurrentExperiment != null)
            {
                quitExp1UI.SetActive(true);
            }
            else
            {
                startExp1UI.SetActive(true);
            }
        }
    
}
public void HideExp1UIPopUP()
{
    startExp1UI.SetActive(false);
    quitExp1UI.SetActive(false);
}

public void MaximizeBuoyDataUI(int dataNumber)
{
    if (dataNumber == 1)
    {
        bouyData_O2Big.SetActive(true);
        bouyData_TempBig.SetActive(false);
    }
    if (dataNumber == 2)
    {
         bouyData_O2Big.SetActive(false);
        bouyData_TempBig.SetActive(true);
    }
    
}


public void MinimizeBuoyDataUI()
{
    bouyData_O2Big.SetActive(false);
    bouyData_TempBig.SetActive(false);
}
public void ToggleOxiTopDataUI()
{
    oxiTopDataUI.SetActive(!oxiTopDataUI.activeSelf);
}
public void MinimizeOxiTopDataUI()
{
    oxiTopDataUI_1Big.SetActive(false);
    oxiTopDataUI_2Big.SetActive(false);
}

public void MaximizeOxiTopDataUI(int dataNumber)
{
    if (dataNumber == 1)
    {
        oxiTopDataUI_1Big.SetActive(true);
        oxiTopDataUI_2Big.SetActive(false);
    }
    if (dataNumber == 2)
    {
         oxiTopDataUI_1Big.SetActive(false);
        oxiTopDataUI_2Big.SetActive(true);
    }
    
}

public void StartExp1()
{
    LabManager lm = LabManager.Instance;
        if (lm != null)
        {
            lm.StartExperiment(0);
        }
        HideExp1UIPopUP();
        
}
public void QuitActiveExp()
{
    LabManager lm = LabManager.Instance;
        if (lm != null)
        {
            lm.QuitActiveExp();
        }
        HideExp1UIPopUP();
}
}
