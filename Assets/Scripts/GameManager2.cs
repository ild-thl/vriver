using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance; // Singleton instance


    // Game Data
    public bool isMobile;
    public bool isXR;

      [Header("Scene Positions (Vector3)")]
    public Vector3 teichPosition;
    public Vector3 labPosition;

    public Vector3 teichRotationEuler = Vector3.zero; // Optional rotation
    public Vector3 labRotationEuler = Vector3.zero;

    

    public InteractionCollision currentCollision;
    public ClickableObject activeClickableObject;
    public Vector3 activeSpawnPosition;
    public Quaternion activeSpawnRotation;

    private bool audioInitialized = false;

    public bool isInInteractionCollider = false;
    public bool firstLoaded = false;

    public bool firstTimeAtLake = true;

    public bool isInRoboMode = false;
    public bool isInLab = false;
    

    

    //-----LAKE-----

    public bool roboActive = false;
    public bool isAtLake = true;
    public int pickedUpTrashItems = 0;

    public bool[] trackFinished = new bool[4];
    public bool track1Finished = false;
    public bool track1FinishedSuccess = false;
    public float resultTrack1;

    public bool track2Finished = false;
    public bool track2FinishedSuccess = false;
    public float resultTrack2;

    public bool track3Finished = false;
    public bool track3FinishedSuccess = false;
    public float resultTrack3;

    public bool track4Finished = false;
    public bool track4FinishedSuccess = false;
    public float resultTrack4;

    public bool buoyDataTaken;
    public bool noteBookInLab = true;
    public bool sampleBottleInLab = false;
    public bool rQPODInLab = true;
    public bool rQPODRoboIsInWater = false;
    public bool rQPODRoboIsInCase = true;
    


    //-----LAB FISH GAME-----
    [Header("Fish Game")]

    public int fishGameScore = 0;
    public bool[] fishGameResults = new bool[5];

     // Which card is on which slot (-1 = empty)
    public int[] savedCardAssignments;

    //------MAP-----//
    public bool mapGenerated = false;
    public bool mapGameTask1Completed = false;
    public bool mapGameTask2Completed = false;


    //----UI-----

    
   

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateTrackFinishedArray();
        
    }

    // Helper to set active spawn for later scene loads
    public void SetActiveSpawn(Vector3 position, Quaternion rotation)
    {
        activeSpawnPosition = position;
        activeSpawnRotation = rotation;
    }

    public void SwitchPositionToTeichPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SimpleCharacterController2 scc2 = player.GetComponent<SimpleCharacterController2>();
        if (player != null)
        {
            scc2.TeleportPlayer2(teichPosition, teichRotationEuler);
            Debug.Log("pos: " + teichPosition);
            
            //SetActiveSpawn(teichPosition, Quaternion.Euler(teichRotationEuler));
        }
        else
        {
            Debug.LogWarning("Player not found in scene!");
        }
    }

    public void SwitchPositionToLabPosition()
    {
         GameObject player = GameObject.FindGameObjectWithTag("Player");
        SimpleCharacterController2 scc2 = player.GetComponent<SimpleCharacterController2>();
        if (player != null)
        {
             scc2.TeleportPlayer2(labPosition, labRotationEuler);
           
            //SetActiveSpawn(labPosition, Quaternion.Euler(labRotationEuler));
        }
        else
        {
            Debug.LogWarning("Player not found in scene!");
        }
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        /*if (player != null)
        {
           
            player.transform.position = activeSpawnPosition;
            player.transform.rotation = activeSpawnRotation;
        }*/
        Debug.Log("Scene Loaded: " + scene.name);
    }


    public void AddTrashItem(int trashValue)
    {
        GameObject robo = GameObject.FindGameObjectWithTag("Robo");
        if (robo != null)
        {
            RoboManager roboManager = robo.GetComponent<RoboManager>();
             pickedUpTrashItems++;
            roboManager.trashScoreUI.text = "" + pickedUpTrashItems;
           

        }
    }

    public void SetupWaterScene(){
        Debug.Log("SetUpWaterScene");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameManager2 gm = GameManager2.Instance;
        GameObject lakeManagerGameObject = GameObject.FindGameObjectWithTag("LakeManager");
        if (lakeManagerGameObject != null)
        {
            LakeManager lakeManager = lakeManagerGameObject.GetComponent<LakeManager>();
        
        if (lakeManager != null)
        {
            RoboManager roboManager = lakeManager.rm;
            if (noteBookInLab)
            {
                lakeManager.noteBook.SetActive(false);
            }
            else
            {
                lakeManager.noteBook.SetActive(true);
            }

            if (sampleBottleInLab)
            {
                lakeManager.sampleBottle.SetActive(false);
            }
            else
            {
                lakeManager.sampleBottle.SetActive(true);
            }
            lakeManager.leaveRoboModeButtonOnTable.SetActive(false);
            if (rQPODInLab)
            {
                lakeManager.rQPODRoboOnGras.SetActive(false);
                lakeManager.rQPODRoboInWater.SetActive(false);
                lakeManager.rQPODRoboOnGrasInCase.SetActive(false);
                lakeManager.tabletOnTable.SetActive(false);
                lakeManager.remoteOnTable.SetActive(false);
                
                
                
            }
            else
            {
                if (!rQPODRoboIsInWater)
                {
                    Debug.Log("not rQPODRoboIsInWater");
                    //roboManager.LeaveRoboMode();
                    lakeManager.tabletOnTable.SetActive(false);
                    lakeManager.remoteOnTable.SetActive(false);



                    
                    if (!rQPODRoboIsInCase)
                    {
                        Debug.Log("not rQPODRoboIsInCase");
                        lakeManager.rQPODRoboOnGras.SetActive(true);
                        lakeManager.rQPODRoboOnGrasInCase.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("rQPODRoboIsInCase");
                        //roboManager.ToggleRoboMode();
                        lakeManager.rQPODRoboOnGras.SetActive(false);
                        lakeManager.rQPODRoboOnGrasInCase.SetActive(true);
                    }
                    
                }
                
                if (rQPODRoboIsInWater)
                {
                    Debug.Log("rQPODRoboIsInWater");
                    lakeManager.rQPODRoboInWater.SetActive(true);
                    lakeManager.remoteOnTable.SetActive(true);
                    lakeManager.tabletOnTable.SetActive(true);
                    lakeManager.rQPODRoboOnGras.SetActive(false);
                    lakeManager.rQPODRoboOnGrasInCase.SetActive(false);
                }
                
            }
            if (gm.isInRoboMode)
            {
                roboManager.EnterRoboMode();
            }

            if (roboManager != null)
            {
            
                roboManager.SetRemote();
            }
        }

        }
        
        

        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            UIManager.Instance.ToggleTitleUI();
        }
    }
     public void SetupLabScene()
        {
            
            GameObject interactableManagerGameObject = GameObject.FindGameObjectWithTag("InteractableManager");
            

        if (interactableManagerGameObject != null)
        {
            InteractableManager interactableManager = interactableManagerGameObject.GetComponent<InteractableManager>();
        
            if (interactableManager != null)
            {
                interactableManager.SetHandPosition();
                interactableManager.ReloadMapUI();
                if (noteBookInLab)
                {
                    interactableManager.noteBookInLab.SetActive(true);
                }
                else
                {
                    interactableManager.noteBookInLab.SetActive(false);
                }

                if (sampleBottleInLab)
                {
                    interactableManager.sampleBottleInLab.SetActive(true);
                }
                else
                {
                    interactableManager.sampleBottleInLab.SetActive(false);
                }

                if (!rQPODInLab)
                {
                    interactableManager.rQPODInLab.SetActive(false);
                    
                    interactableManager.rQPODCaseInLab.SetActive(false);
                
                }
                else
                {
                    interactableManager.rQPODInLab.SetActive(true);
                    interactableManager.rQPODCaseInLab.SetActive(true);
                }
            
            } 
            
            
        }

        GameObject FishGameGameObject = GameObject.FindGameObjectWithTag("FishGame");
        if (FishGameGameObject != null)
        {
            FishGame fg = FishGameGameObject.GetComponent<FishGame>();
            if (fg != null)
            {
                fg.RestoreGameState();
            }
        }
       
    }
    public void SetNavModeUI()
    {
        if (isMobile)
        {
             Debug.Log("set mobile nav");
            InputManager2.Instance.ShowMobileUI();
            
        }
        else
        {
            InputManager2.Instance.HideMobileUI();
        }
    }
    public void SetMoblieNavMode()
    {
       
        isMobile = true;
         Debug.Log("mobile");
        
    }

    public void SetDesktopNavMode()
    {
        isMobile = false;
        
    }

    public void UpdateTrackFinishedArray()
    {
        trackFinished[0] = track1Finished;
        trackFinished[1] = track2Finished;
        trackFinished[2] = track3Finished;
        trackFinished[3] = track4Finished;
    }

    public void SetUpLabUI()
    {
        Debug.Log("SetUpLabUI");
        ExperimentUI expUI = FindFirstObjectByType<ExperimentUI>();
        if (expUI != null)
        {
            expUI.SetUpLabComputerUI();
        }
        else
        {
            Debug.Log("LabUI missing");
        }
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            UIManager.Instance.ToggleTitleUI();
        }

        
    }
   
}
