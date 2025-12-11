using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackCollision : MonoBehaviour
{
    public bool isStartPoint;
    public bool isEndPoint;
    public bool isTrack;
    public bool isHiddenTrack; //Appears when missing interval spawned
    public bool isBuoy;
    public int trackNumber;

    public RoboManager robo;
    private BoxCollider trackCol;
    public TabletUI tabletUI;
    
    public bool isOnTrack = false;

    private void Awake()
    {
        trackCol = GetComponent<BoxCollider>();
        GameObject tabletUIGameObject = GameObject.FindGameObjectWithTag("TabletUI");
    }

private void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    tabletUI = TabletUI.Instance;
}
    void Update()
    { 
        if (isEndPoint || isStartPoint) return;

        if (isHiddenTrack && isOnTrack)
        {
            tabletUI.isCorrecting = true;
            // Continuously calculate t based on robo's position
            
        }
        if (isTrack)
        {
            if (robo.isOnTrack && robo.isMeasuring)
            {
                // Continuously calculate t based on robo's position
                float t = GetNormalizedTrackPosition(robo.transform.position);

                // Send current t to RoboManager
                robo.DrawTrack(t);

                //Debug.Log($"Update Track pos: {t}");
            }
        }

        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tabletUI == null)
        {
            tabletUI = TabletUI.Instance;
        }
        
        if (isHiddenTrack)
        {
            Debug.Log("isHiddenTrackCollisionENTER");
            isOnTrack = true;
            float t = GetNormalizedTrackPosition(other.transform.position);
            

            tabletUI.StartCorrection(t);
            
        }
    if (!other.TryGetComponent(out RoboManager rm)) return;

        if(isTrack)
        {
        float t = GetNormalizedTrackPosition(other.transform.position);
        robo.isOnTrack = true;
        robo.DrawTrack(t);
        tabletUI.StopBlackingOutResult(t);
        tabletUI.ToggleOnTrackUI(true);
        Debug.Log($"Enter Track pos: {t}");
        }
        
        else if (isStartPoint)
        {
            tabletUI.EnteringStartPoint(trackNumber);
        }
        else if(isEndPoint)
        {
            tabletUI.EnteringEndPoint(trackNumber);
        }

        if (isBuoy)
        {
            tabletUI.ToggleBuoyUI(true);
        }
        
        
    }

    private void OnTriggerExit(Collider other)
    {

        if (tabletUI == null)
        {
            tabletUI = TabletUI.Instance;
        }
        if (isHiddenTrack)
        {
            Debug.Log("isHiddenTrackCollisionEXIT");
             isOnTrack = false;
             float t = GetNormalizedTrackPosition(other.transform.position);
             tabletUI.StopCorrection(t, gameObject);
        }
        
        if (!other.TryGetComponent(out RoboManager rm)) return;

        if(isTrack)
        {
            float t = GetNormalizedTrackPosition(other.transform.position);
            robo.isOnTrack = false;
            
            tabletUI.ToggleOnTrackUI(false);
            robo.StopTrack(t);
            Debug.Log($"Exit Track pos: {t}");
        }
        else if (isStartPoint)
        {
            tabletUI.LeavingStartPoint(trackNumber);
        }
        else if(isEndPoint)
        {
            tabletUI.LeavingEndPoint(trackNumber);
        }

         if (isBuoy)
        {
            tabletUI.ToggleBuoyUI(false);
        }
        
        
    }

    private float GetNormalizedTrackPosition(Vector3 worldPos)
    {
        // Lokale Position im Collider
        Vector3 local = trackCol.transform.InverseTransformPoint(worldPos);

        // Länge entlang der lokalen Z-Achse
        float halfLength = trackCol.size.z * 0.5f;
        float clamped = Mathf.Clamp(local.z, -halfLength, halfLength);

        // Normalisieren von [-L/2, L/2] → [0, 1]
        float normalized = (clamped + halfLength) / trackCol.size.z;
        return normalized;
    }

    
   
}

