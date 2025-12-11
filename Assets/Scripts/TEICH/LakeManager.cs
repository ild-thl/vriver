using UnityEngine;

public class LakeManager : MonoBehaviour
{
    


    public GameObject robo;                     // The roboboat in world space
    public RoboManager rm;
    public GameObject rQPODRoboInWater;
    public GameObject rQPODRoboOnGras;
    public GameObject rQPODRoboOnGrasInCase;
    

    public GameObject sampleBottle;       
    public GameObject noteBook;       
    public GameObject tabletOnTable; 
    public GameObject remoteOnTable;
    public GameObject leaveRoboModeButtonOnTable;        


    public GameObject[] trackLineOnWater;
    public GameObject[] startColliderOnWater;
    public GameObject[] endColliderOnWater;

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("LAKEMANAGER");
    }

    public void RemoveRoboCase()
    {
        Debug.Log("remove case");
        rQPODRoboOnGrasInCase.SetActive(false);
        rQPODRoboOnGras.SetActive(true);
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            GameManager2.Instance.rQPODRoboIsInCase = false;
        }
        
        
    }

    public void LeaveRoboMode()
    {
        rm.LeaveRoboMode();
        
    }

    
}
