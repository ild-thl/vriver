using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LabComputer : MonoBehaviour
{
    public Image mapImage;
    public GameObject mapImageGameObject;
    public GameObject mapButton;
    public GameObject mapMaxTask;
    public GameObject mapMinTask;
    public GameObject successUI;

    public GameObject mapMaxAnswerButton;
    public GameObject mapMinAnswerButton;
    public GameObject mapMaxAnswer;
    public GameObject mapMinAnswer;

    public GameObject computerMap;
    

    public bool mapGenerated = false;

    public void GenerateMap()
    {
        // Reset state
        mapImage.fillAmount = 0f; // radial fill
        Color c = mapImage.color;
        c.a = 0f;                 // alpha
        mapImage.color = c;

        mapImageGameObject.SetActive(true);

        // Start animation coroutine
        StartCoroutine(GenerateMapAnimation());
    }

    private IEnumerator GenerateMapAnimation()
    {
        float duration = 2f; // seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Animate fill and alpha
            mapImage.fillAmount = t;
            Color c = mapImage.color;
            c.a = t;
            mapImage.color = c;

            yield return null;
        }

        // Ensure final state
        mapImage.fillAmount = 1f;
        Color final = mapImage.color;
        final.a = 1f;
        mapImage.color = final;

        
        if (!mapGenerated)
        {
            CheckExp();
        }
        mapMinTask.SetActive(false);
        mapMaxTask.SetActive(true);
        successUI.SetActive(false);
        mapMinAnswerButton.SetActive(false);
        mapMinAnswer.SetActive(false);
        mapMaxAnswerButton.SetActive(true);
        mapMaxAnswer.SetActive(false);
        computerMap.SetActive(true);
        
    }


    private void CheckExp()
    {

        var exp = LabManager.Instance?.experiments[0];
        if (exp == null) return;
        var step = exp.GetCurrentStep();
        if (step == null) return;
        LabManager.Instance.OnTaskCompleted();
        mapGenerated = true;
        mapButton.SetActive(false);
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            gm.mapGenerated = true;
        }
        
    }
    
    //----MapQUIZ-----//

    public void ClickOnMaxDepth()
    {

        mapMaxAnswerButton.SetActive(false);
        mapMaxAnswer.SetActive(true);
        mapMinAnswerButton.SetActive(true);
        mapMinAnswer.SetActive(false);
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            gm.mapGameTask1Completed = true;
        }
        mapMaxTask.SetActive(false);
        mapMinTask.SetActive(true);

    }

    public void ClickOnMinDepth()
    {
        mapMaxAnswerButton.SetActive(false);
        mapMaxAnswer.SetActive(true);
        mapMinAnswerButton.SetActive(false);
        mapMinAnswer.SetActive(true);
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            gm.mapGameTask2Completed = true;
        }
        mapMinTask.SetActive(false);
        successUI.SetActive(true);

    }
    public void ReloadMapUI()
    {
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            if(gm.mapGenerated)
            {

                mapImage.fillAmount = 1f; // radial fill
                Color c = mapImage.color;
                c.a = 1f;                 // alpha
                mapImage.color = c;

                mapImageGameObject.SetActive(true);

                mapMinTask.SetActive(false);
                mapMaxTask.SetActive(true);
                successUI.SetActive(false);
                mapMinAnswerButton.SetActive(false);
                mapMinAnswer.SetActive(false);
                mapMaxAnswerButton.SetActive(true);
                mapMaxAnswer.SetActive(false);
                computerMap.SetActive(true);

                if(gm.mapGameTask1Completed)
                {
            
                    mapMinTask.SetActive(true);
                    mapMaxTask.SetActive(false);
                    successUI.SetActive(false);
                    mapMinAnswerButton.SetActive(true);
                    mapMinAnswer.SetActive(false);
                    mapMaxAnswerButton.SetActive(false);
                    mapMaxAnswer.SetActive(true);

                    if(gm.mapGameTask2Completed)
                    {
                        mapMinTask.SetActive(false);
                        mapMaxTask.SetActive(false);
                        successUI.SetActive(true);
                        mapMinAnswerButton.SetActive(false);
                        mapMinAnswer.SetActive(true);
                        mapMaxAnswerButton.SetActive(false);
                        mapMaxAnswer.SetActive(true);

                    }
                }
            }
            else
            {
                mapButton.SetActive(true);
                mapMinTask.SetActive(false);
                mapMaxTask.SetActive(false);
                successUI.SetActive(false);
                mapMinAnswerButton.SetActive(false);
                mapMinAnswer.SetActive(false);
                mapMaxAnswerButton.SetActive(false);
                mapMaxAnswer.SetActive(false);
                computerMap.SetActive(false);
                mapImage.fillAmount = 0f; // radial fill
                Color c = mapImage.color;
                c.a = 0f;                 // alpha
                mapImage.color = c;

                mapImageGameObject.SetActive(false);
                              

                }
            }
        }

    

}

