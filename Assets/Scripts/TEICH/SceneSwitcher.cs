using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher Instance;
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WebGLReady();
    [DllImport("__Internal")]
    private static extern void ShowVRButton(bool show);
#endif

    private string targetScene = "";




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

    }
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLReady(); // Unity meldet sich bei JS
#endif
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        NotifyJSBasedOnScene(scene.name);
        #endif
        // Prüfen, wohin gewechselt wurde
        Debug.Log("onsceneLoadedMAIN");

        if (scene.name == "LAB")
        {
            Debug.Log("sceneloaded Lab");
            GameManager2 gm = GameManager2.Instance;
            gm.SetNavModeUI();
            gm.SwitchPositionToLabPosition();
            gm.isInInteractionCollider = false;
            gm.isInLab = true;
            gm.isAtLake = false;
            gm.SetUpLabUI();
            gm.SetupLabScene();
            UIManager uiM = UIManager.Instance;
            if (uiM != null)
            {
                uiM.ToggleTitleUI();
                uiM.HideInteractionInfo();
                uiM.HideWaterSwitchButton();
            } 
        }
        else if (scene.name == "LAKE")
        {
            GameManager2 gm = GameManager2.Instance;
            gm.SetNavModeUI();
            TabletUI tabletUI = TabletUI.Instance;
            if (tabletUI != null)
            {
                tabletUI.GetLakeObjectsFromLakeManager();
            }
            gm.SetupWaterScene();
            gm.SwitchPositionToTeichPosition();
            Debug.Log("sceneloaded Teich");
            GameObject robo = GameObject.FindGameObjectWithTag("Robo");
            if (robo != null)
            {
                RoboManager roboManager = robo.GetComponent<RoboManager>();
                GameObject player_ = GameObject.FindGameObjectWithTag("Player");
                if (robo != null)
            {
                roboManager.player = player_.transform;
            }
            }
            gm.isInLab = false;
            gm.isAtLake = true;
            
            
            UIManager uiM = UIManager.Instance;
            if (uiM != null)
            {
                uiM.ToggleTitleUI();
                uiM.HideInteractionInfo();
                uiM.HideWaterSwitchButton();
            } 
        }
        else if (scene.name == "LAKE_VR")
        {
            GameManager2 gm = GameManager2.Instance;
            gm.isXR = true;
            gm.isMobile = false;
            
        }

        // Reset
        //targetScene = "";
    }

    public void SwitchToLab()
    {
        GameManager2 gm = GameManager2.Instance;
        targetScene = "LAB";
        Debug.Log("Switch to LabScene");
        if (gm != null)
        {
            if (gm.rQPODRoboIsInWater)
            {
                GameObject robo = GameObject.FindGameObjectWithTag("Robo");
                if (robo != null)
                {
                    RoboManager roboManager = robo.GetComponent<RoboManager>();
                    roboManager.LeaveRoboMode();
                    SceneManager.LoadScene(targetScene);
                }
            }
            else
            {
                SceneManager.LoadScene(targetScene);
            }
        }
        
        
    }

    public void SwitchToWater()
    {
        targetScene = "LAKE";
        GameObject FishGameGameObject = GameObject.FindGameObjectWithTag("FishGame");
        if (FishGameGameObject != null)
        {
            FishGame fg = FishGameGameObject.GetComponent<FishGame>();
            if (fg != null)
            {
                fg.SaveGameState();
            }
        
        
    }
    SceneManager.LoadScene(targetScene);
    }

    public void SwitchToStart()
    {
        targetScene = "START";
        
        
        SceneManager.LoadScene(targetScene);
    }

    public void SwitchToLakeVR()
    {
        targetScene = "LAKE_VR";
        
        
        SceneManager.LoadScene(targetScene);
    }
    
    public void NotifyJSBasedOnScene(string sceneName)
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
    if (sceneName == "LAKE_VR")
    {
        ShowVRButton(true);
        
        
    }
    else
    {
        ShowVRButton(false);
      
      
    }
    #endif
}
}
