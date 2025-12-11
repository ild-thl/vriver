using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject creditsUI;
    public GameObject labButton;
    public GameObject helpUI;
    public GameObject interactionTipUI; // UI Element to show interaction hint

    public GameObject lakeTitleUI;
    public GameObject labTitleUI;

    public GameObject waterSwitchButton;
    public GameObject labSwitchButton;

    public GameObject interactionInfoUI;
    public GameObject sampleBottlePopUpUI;
    public GameObject notesPopUpUI;
    public GameObject rQPODTakeToLakePopUpUI;
    public GameObject rQPODPutInWaterPopUpUI;
    
    
    private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
}



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    
    
    
    public void ToggleCredits()
{
    if (creditsUI != null)
        creditsUI.SetActive(!creditsUI.activeSelf);
}

    public void ToggleHelp()
    {
        helpUI.SetActive(!helpUI.activeSelf);
    }


    public void ToggleLabTeleportButton(bool show)
    {
        labButton.SetActive(show);
    }

    public void ToggleTitleUI()
    {
        GameManager2 gm = GameManager2.Instance;

        labTitleUI.SetActive(gm.isInLab);
        lakeTitleUI.SetActive(gm.isAtLake);
    }


    public void HideInteractionInfo()
    {
        interactionInfoUI.SetActive(false);
    }

    public void SwitchToLabButtonPress()
    {
        SceneSwitcher sceneswitcher = SceneSwitcher.Instance;
        if (sceneswitcher != null)
        {
            sceneswitcher.SwitchToLab();
        }
    }

    public void SwitchToWaterButtonPress()
    {
        SceneSwitcher sceneswitcher = SceneSwitcher.Instance;
        if (sceneswitcher != null)
        {
            sceneswitcher.SwitchToWater();
        }
    }

    public void SwitchToStartButtonPress()
    {
        SceneSwitcher sceneswitcher = SceneSwitcher.Instance;
        if (sceneswitcher != null)
        {
            sceneswitcher.SwitchToStart();
        }
    }

    public void HideWaterSwitchButton()
    {
        waterSwitchButton.SetActive(false);
        labSwitchButton.SetActive(true);
    }
    public void HideLabSwitchButton()
    {
        waterSwitchButton.SetActive(true);
        labSwitchButton.SetActive(false);
    }

    public void ShowNotesPopUpUI()
    {
        notesPopUpUI.SetActive(true);
    }
    
    public void ShowSampleBottlePopUpUI()
    {
        sampleBottlePopUpUI.SetActive(true);
    }
    public void ShowRQPODLabPopUpUI()
    {
        rQPODTakeToLakePopUpUI.SetActive(true);
    }


    public void ShowRQPODPutInWaterPopUpUI()
    {
        rQPODPutInWaterPopUpUI.SetActive(true);
    }

    public void HidePopUps()
    {
        notesPopUpUI.SetActive(false);
        sampleBottlePopUpUI.SetActive(false);
        rQPODPutInWaterPopUpUI.SetActive(false);
        rQPODTakeToLakePopUpUI.SetActive(false);
    }

    public void TakeNotebookButton()
    {
        Debug.Log("takeNotebook");
        GameManager2 gM = GameManager2.Instance;

        if (gM != null)
        {
            if (gM.isAtLake)
            {
                GameObject lakeManagerGameObject = GameObject.FindGameObjectWithTag("LakeManager");
                if (lakeManagerGameObject != null)
                {
                    LakeManager lakeManager = lakeManagerGameObject.GetComponent<LakeManager>();
        
                    if (lakeManager != null)
                    {
            
                        gM.noteBookInLab = true;
                        lakeManager.noteBook.SetActive(false);
                        
                    }
                }
            }
            if (!gM.isAtLake)
              {
                  GameObject interactableManagerGameObject = GameObject.FindGameObjectWithTag("InteractableManager");
                if (interactableManagerGameObject != null)
                {
                    InteractableManager interactableManager = interactableManagerGameObject.GetComponent<InteractableManager>();
        
                    if (interactableManager != null)
                    {
            
                        gM.noteBookInLab = false;
                        interactableManager.noteBookInLab.SetActive(false);
                        
                    }
                }
              }
        }
        HidePopUps();
    }


    public void TakeRQPODToLakeButton()
    {
        Debug.Log("take rQPOD to lake");
        GameManager2 gM = GameManager2.Instance;

        if (!gM.isAtLake && gM.isInLab) //if in Lab
              {
                  GameObject interactableManagerGameObject = GameObject.FindGameObjectWithTag("InteractableManager");
                if (interactableManagerGameObject != null)
                {
                    InteractableManager interactableManager = interactableManagerGameObject.GetComponent<InteractableManager>();
        
                    if (interactableManager != null)
                    {
            
                        
                        interactableManager.rQPODInLab.SetActive(false);
                        interactableManager.rQPODCaseInLab.SetActive(false);
                        gM.rQPODInLab = false;
                        gM.rQPODRoboIsInCase = true;
                        
                        
                    }
                }
              }
        
        HidePopUps();
    }

    public void TakeSampleBottleButton()
    {
        Debug.Log("takeSampleBottleToLab");
        GameManager2 gM = GameManager2.Instance;

        if (gM != null)
        {
            if (gM.isAtLake)
            {
                GameObject lakeManagerGameObject = GameObject.FindGameObjectWithTag("LakeManager");
                if (lakeManagerGameObject != null)
                {
                    LakeManager lakeManager = lakeManagerGameObject.GetComponent<LakeManager>();
        
                    if (lakeManager != null)
                    {
            
                        gM.sampleBottleInLab = true;
                        lakeManager.sampleBottle.SetActive(false);
                        
                    }
                }
            }
              if (!gM.isAtLake)
              {

              }
        }
        HidePopUps();
    }

    public void PutRQPODinWaterButton()
    {
        Debug.Log("put rQPOD in water");
         GameManager2 gM = GameManager2.Instance;

        if (gM != null)
        {
            if (gM.isAtLake)
            {
                GameObject lakeManagerGameObject = GameObject.FindGameObjectWithTag("LakeManager");
                if (lakeManagerGameObject != null)
                {
                    LakeManager lakeManager = lakeManagerGameObject.GetComponent<LakeManager>();
        
                    if (lakeManager != null)
                    {
            
                        
                        lakeManager.rQPODRoboOnGras.SetActive(false);
                        lakeManager.rQPODRoboInWater.SetActive(true);
                        lakeManager.remoteOnTable.SetActive(true);
                        lakeManager.tabletOnTable.SetActive(true);
                        gM.rQPODRoboIsInWater = true;
                       
                        gM.rQPODRoboIsInCase = false;
                        
                    }
                }
            }
        }
        
        HidePopUps();
    }
    
}
