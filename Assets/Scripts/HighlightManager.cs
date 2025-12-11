using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    public GameObject[] highlightArray_BOD5;
    public GameObject[] secondaryHighlightArray_BOD5; 
    public GameObject[] toolHighlightArray_BOD5;


    public GameObject[] highlightArray_RQPOD;
    public GameObject[] secondaryHighlightArray_RQPOD;
    public GameObject[] toolHighlightArray_RQPOD;
    //public GameObject[] highlightArray_Computer;

    public void UpdateHighlights(LabExperiment currentExp)
    {
        HideAll();

        int cs = currentExp.CurrentStep;

        if (cs < highlightArray_BOD5.Length && highlightArray_BOD5[cs] != null)
            highlightArray_BOD5[cs].SetActive(true);

        if (cs < secondaryHighlightArray_BOD5.Length && secondaryHighlightArray_BOD5[cs] != null)
            secondaryHighlightArray_BOD5[cs].SetActive(true);
        
         if (cs < toolHighlightArray_BOD5.Length && toolHighlightArray_BOD5[cs] != null)
            toolHighlightArray_BOD5[cs].SetActive(true);

        
    }

    public void HideAll()
    {
        Debug.Log("HideAll");
        DisableArray(highlightArray_BOD5);
        DisableArray(secondaryHighlightArray_BOD5);
        DisableArray(toolHighlightArray_BOD5);
        
    }

    private void DisableArray(GameObject[] array)
    {
        foreach (var go in array)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
}

