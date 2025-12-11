using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;



public class TabletUI : MonoBehaviour
{
    public static TabletUI Instance; // Singleton instance
    

     private int activeTrackTabNumber = 1;
     public int currentActiveTrack;
    [Header("Target Transforms")]
    public Transform miniTargetTransform;
    public Transform maxTargetTransform;

    [Header("Animation Settings")]
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Roboboat Tracking")]
    public GameObject robo;                     // The roboboat in world space
    public RoboManager rm;
    public RectTransform roboLocationAreaUI;    // The map area (UI RectTransform)
    public RectTransform roboLocationIndicatorUI; // The indicator icon (UI RectTransform)

    [Tooltip("World-space boundaries that correspond to the edges of the map area.")]
    public Vector2 worldMin = new Vector2(-50, -50);
    public Vector2 worldMax = new Vector2(50, 50);

    private bool isMaximized = true;
     
    private bool isTransitioning = false;
    private float transitionT = 0f;
    public GameObject maximizeButton;

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 startScale;

    private Vector3 targetPos;
    private Quaternion targetRot;
    private Vector3 targetScale;


    public bool isMapMaximized = false;
    //public GameObject graphParent;
    public GameObject[] graphParent; //hide when Map maximized
    public GameObject mapMaximizeIcon;
    public GameObject mapMinimizeIcon;

    public GameObject mapMaxMinButton;
    public Transform mapMaxMinButtonPositionWhenMin;
    public Transform mapMaxMinButtonPositionWhenMax;

    public GameObject mapDirectioIcon;
    public Transform mapDirectioIconPositionWhenMax;
    public Transform mapDirectioIconPositionWhenMin;

    public GameObject mapBottomInfo;
    public Transform mapBottomInfoPositionWhenMax;
    public Transform mapBottomInfoPositionWhenMin;

    
    public GameObject[] trackLine;
    public GameObject[] trackLineOnWater;
    public GameObject[] hiddenTrackLineOnWater;

    public GameObject[] startColliderOnWater;
    public GameObject[] endColliderOnWater;
    public GameObject[] startCircle;
    public GameObject[] endCircle;

    public GameObject[] accuracyUI;
    public float minimumAccLimit = 0.8f;

    private bool roboOnTrack;

    public GameObject onTrackUI;
    public GameObject notOnTrackUI;
    public GameObject startButton;
    public GameObject stopButton;
    public GameObject resetButton;
    public GameObject goToStartWanrning;
    public GameObject goToEndWanrning;
    public GameObject readyInfo;
    public TextMeshProUGUI goToStartWanrningText;
    public TextMeshProUGUI goToEndWanrningText;
    public TextMeshProUGUI readyInfoText;

    public Button[] trackTabButtons; //to change the activetrack tab (test1 - test4)
    public GameObject[] trackTopUI;
    public GameObject[] trackBottomUI;

    public float currentReveal;
     public float currentMaxReveal = 0;
    public GameObject currentMaesurementsOverlayTop;
    public GameObject currentMaesurementsOverlayBottom;

    public GameObject[] maesurementsOverlayTop;
    public GameObject[] maesurementsOverlayBottom;

    public GameObject[] endScaleInfoOverlay; //TabLegenden

    
    private bool isBlackingOutResults = false;

    public GameObject trailDotPrefab;
    public Transform[] trailParent;
    private Vector2 lastTrailPos;
    public float minDistance = 5f; // pixels

    private float? currentMissingStart = null;
     public bool isCorrecting = false;

    public GameObject blackOverlayPrefab;
    public GameObject blackWaterOverlayPrefab;

    public float currentCorrectionColliderExitPos = 0f;
     public float currentCorrectionColliderEnterPos = 0f;
     public float currentTrackEnterPos = 0;
     public float currentTrackExitPos = 0;

    public RectTransform[] overlayParentTop;
    public RectTransform[] overlayParentBottom;
    public Transform[] overlayWaterParentTop;

    public RectTransform mapParent;              // the map content        // content that pans
    public Color green;
    public Color red;
    public Color grey;

    public float zoomStep = 0.1f;   // how much to zoom per click
    public float minZoom = 0.5f;    // minimum scale
    public float maxZoom = 2.0f;    // maximum scale

    

    private float trackStartTime;
    private float trackDuration;

    public GameObject durationLiveUI; // assign in Inspector
    public TextMeshProUGUI durationLiveText;


   
    public GameObject[] trackTabUI; //windows with all the track data
    public List<MissingInterval> missingIntervals = new List<MissingInterval>();

    public GameObject buoyButtonUI;
    public GameObject buoyLocationUI;
    public GameObject buoyDataTakenAnimatedUI;


    private void Start() {
        gameObject.SetActive(false);
        
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        
    }

    private void Update()
    {
        HandleTransition();
        UpdateRoboPositionOnMap();

        // Live timer update
        if (rm != null && rm.isMeasuring && durationLiveText != null)
        {
            trackDuration = Time.time - trackStartTime;
            if (durationLiveText != null)
            {
                durationLiveText.text = trackDuration.ToString("F1") + "s"; // e.g. "12.3s"
            }
           
            

            if (isCorrecting)
            {
                Debug.Log("is Correcting");
            }
    }

       //UI MainLogic 
    }

    private void HandleTransition()
    {
        if (!isTransitioning) return;

        transitionT += Time.deltaTime * transitionSpeed;
        float curvedT = transitionCurve.Evaluate(transitionT);

        transform.position = Vector3.Lerp(startPos, targetPos, curvedT);
        transform.rotation = Quaternion.Lerp(startRot, targetRot, curvedT);
        transform.localScale = Vector3.Lerp(startScale, targetScale, curvedT);

        if (transitionT >= 1f)
        {
            isTransitioning = false;
            transitionT = 0f;
        }
    }

    public void ToggleMapUISize()
    {
        // Flip the state
        isMapMaximized = !isMapMaximized;

       
       

        // Toggle icons
        mapMaximizeIcon.SetActive(!isMapMaximized);
        mapMinimizeIcon.SetActive(isMapMaximized);

        

         // Show/hide graph depending on state
        if (isMapMaximized)
        {
             HideGraphParentsUI();
             mapBottomInfo.transform.position = mapBottomInfoPositionWhenMin.position;
             mapDirectioIcon.transform.position = mapDirectioIconPositionWhenMin.position;
             mapMaxMinButton.transform.position = mapMaxMinButtonPositionWhenMin.position;
        }
        else
        {
            mapBottomInfo.transform.position = mapBottomInfoPositionWhenMax.position;
             mapDirectioIcon.transform.position = mapDirectioIconPositionWhenMax.position;
             mapMaxMinButton.transform.position = mapMaxMinButtonPositionWhenMax.position;
             ShowGraphParentsUI();
            
        }
    }
    
    public void ChangeTab(int tabNumber)
    {
        Debug.Log("Change Tab to: " + tabNumber);
        currentReveal = 0f;
        currentMaxReveal = 0f;
        activeTrackTabNumber = tabNumber;
        currentActiveTrack = activeTrackTabNumber;
        currentMaesurementsOverlayBottom = maesurementsOverlayBottom[tabNumber - 1];
        currentMaesurementsOverlayTop = maesurementsOverlayTop[tabNumber - 1];
        HideTrackTabs();
        ShowActiveTrackTab();
        ShowActiveTrackWaterColliderOverlays();
        ShowResetButton();
        LeavingStartPoint(tabNumber);
        ChangeActiveTrackTabButtonsColor(tabNumber);
    
    }

    public void ToggleTabUI(bool show)
    {
        gameObject.SetActive(show);
        ChangeTab(currentActiveTrack);
        if (!show)
        {
            HideTabAllInfoHints();
        }

        Debug.Log("tabUI " + show);
        Debug.Log("tabUI " + gameObject.activeSelf);
    }

    private void SetTabSize(bool maximize)
    {
        Transform target = maximize ? maxTargetTransform : miniTargetTransform;
        if (target == null) return;

        StartTransition(target);
        maximizeButton.SetActive(!maximize);
        isMaximized = maximize;
    }
        
    public void MinimizeTab() => SetTabSize(false);
    public void MaximizeTab() => SetTabSize(true);
        
    public void ToggleTabUISize()
    {
        if (isTransitioning) return;
            SetTabSize(!isMaximized);
    }

    private void StartTransition(Transform target)
    {
        isTransitioning = true;
        transitionT = 0f;

        startPos = transform.position;
        startRot = transform.rotation;
        startScale = transform.localScale;

        targetPos = target.position;
        targetRot = target.rotation;
        targetScale = target.localScale;
    }

    public void ZoomIn_Map()
    {
        if (mapParent == null) return;

        Vector3 scale = mapParent.localScale;
        scale += Vector3.one * zoomStep;
        scale = ClampScale(scale);
        mapParent.localScale = scale;
    }

    public void ZoomOut_Map()
    {
        if (mapParent == null) return;

        Vector3 scale = mapParent.localScale;
        scale -= Vector3.one * zoomStep;
        scale = ClampScale(scale);
        mapParent.localScale = scale;
    }

    private Vector3 ClampScale(Vector3 scale)
    {
        float clamped = Mathf.Clamp(scale.x, minZoom, maxZoom);
        return new Vector3(clamped, clamped, 1f); // keep uniform scaling
    }

    //TrackRevealUILogic

    public void ToggleOnTrackUI(bool onTrack)
    {
        Debug.Log("on trackUI " + onTrack);
        roboOnTrack = onTrack;
        onTrackUI.SetActive(onTrack);
        notOnTrackUI.SetActive(!onTrack);
    }

    public void ToggleTrackTab(int trackTabNumber)
    {
        Debug.Log(trackTabNumber);

    }

    private void UpdateRoboPositionOnMap()
    {
        if (robo == null || roboLocationAreaUI == null || roboLocationIndicatorUI == null)
            return;

        Vector3 roboPos = robo.transform.position;
        float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, roboPos.x);
        float normalizedY = Mathf.InverseLerp(worldMin.y, worldMax.y, roboPos.z);

        Vector2 areaSize = roboLocationAreaUI.rect.size;
        Vector2 localPos = new Vector2(
            (normalizedX - 0.5f) * areaSize.x,
            (normalizedY - 0.5f) * areaSize.y
        );

        roboLocationIndicatorUI.anchoredPosition = localPos;
        roboLocationIndicatorUI.localRotation = Quaternion.Euler(0, 0, -robo.transform.eulerAngles.y);


    
    // Only spawn if moved far enough

    if (Vector2.Distance(lastTrailPos, localPos) > minDistance)
    {
        if (rm.isMeasuring)
        {
            GameObject dot = Instantiate(trailDotPrefab, trailParent[currentActiveTrack - 1]);
            dot.GetComponent<RectTransform>().anchoredPosition = localPos;
            
            lastTrailPos = localPos;
            Image dotImage = dot.GetComponent<Image>();

            if (dotImage != null)
            {
               
                dotImage.color = red;
            }
            

            
        
        }
    }
    }
    
    public void RevealPart(float t)
{

    if (currentMaesurementsOverlayTop == null || currentMaesurementsOverlayBottom == null) return;

    if (t > currentMaxReveal)
        currentMaxReveal = t;

    float revealValue = currentMaxReveal;

    Vector3 scale = currentMaesurementsOverlayTop.transform.localScale;
    scale.x = 1 - revealValue;

    currentMaesurementsOverlayTop.transform.localScale = scale;
    currentMaesurementsOverlayBottom.transform.localScale = scale;

    currentReveal = revealValue;

    
}
    private void ResetReveal()
    {
        currentMaxReveal = 0;
        currentReveal = 0;
        RevealPart(currentReveal);
        Transform p = trailParent[activeTrackTabNumber - 1];

        // Loop backwards to avoid index issues while destroying
        for (int i = p.childCount - 1; i >= 0; i--)
        {
            Destroy(p.GetChild(i).gameObject);
        }
        endScaleInfoOverlay[activeTrackTabNumber - 1].SetActive(false);
        
    }

    public void StartBlackingOutResult(float t1) 
    {
        Debug.Log("Start Blackout at " + t1);
        if (currentMaxReveal < t1)
        {
            currentMissingStart = t1;
        }
        
        
    }

    public void StopBlackingOutResult(float t2) 
    {
        Debug.Log("Stop Blackout at " + t2);

        if (currentMissingStart.HasValue && currentMissingStart < t2)
        {
            float startT = currentMissingStart.Value;

            // Create overlay for this interval
            (GameObject top, GameObject bottom, GameObject water) = CreateBlackOverlay(startT, t2);

            // Save interval with overlay references
            missingIntervals.Add(new MissingInterval(currentActiveTrack, startT, t2, top, bottom, water));
            Debug.Log("interval added at " + currentActiveTrack + " missingIntervals: " + missingIntervals.Count );

            currentMissingStart = null;
        }
    }


    public void StartTrackMeasurement()
    {
        if (currentActiveTrack != null)
        {
            Debug.Log("StartTrackMeasurement");
            startButton.SetActive(false);
            trackLine[currentActiveTrack - 1].SetActive(true);
            trackLineOnWater[currentActiveTrack - 1].SetActive(true);
            endColliderOnWater[currentActiveTrack - 1].SetActive(true);
            endCircle[currentActiveTrack - 1].SetActive(true);
            startCircle[currentActiveTrack - 1].SetActive(false);
            startColliderOnWater[currentActiveTrack - 1].SetActive(false);
           
            goToStartWanrning.SetActive(false);
            readyInfo.SetActive(false);
             goToStartWanrningText.text = "Fahre zu Endpunkt " + currentActiveTrack;
             
            goToEndWanrning.SetActive(true);
            resetButton.SetActive(true);
            rm.isMeasuring = true;
            HideEndScaleInfo();
             // Start timer
             
            trackStartTime = Time.time;
            DisableOtherTrackTabButtons(currentActiveTrack);
            UIManager.Instance.ToggleLabTeleportButton(false);
        }

    }

    public void EndTrackMeasurement()
    {
        if (currentActiveTrack != null)
        {
            Debug.Log("StopTrackMeasurement");
            stopButton.SetActive(false);
            Debug.Log(currentActiveTrack);
            trackLine[currentActiveTrack - 1].SetActive(false);
            trackLineOnWater[currentActiveTrack - 1].SetActive(false);
            endColliderOnWater[currentActiveTrack - 1].SetActive(false);
            endCircle[currentActiveTrack - 1].SetActive(false);
            float acc = GetAccuracy();
            Debug.Log("Robo accuracy: " + acc);
            onTrackUI.SetActive(false);
            notOnTrackUI.SetActive(false);
            goToEndWanrning.SetActive(false);
            ShowEndScaleInfo();
            rm.isMeasuring = false;

            // Stop timer
            trackDuration = Time.time - trackStartTime;
            DisplayAccuracyAndDuration(acc, trackDuration);
            
            Debug.Log("Track duration: " + trackDuration + " seconds");

            GameManager2 gm = GameManager2.Instance;
            switch (currentActiveTrack)
        {
            case 1:
                gm.resultTrack1 = acc;
                
                    gm.track1Finished = true;
                if (acc >= minimumAccLimit)
                {
                    gm.trackFinished[activeTrackTabNumber - 1] = true;
                    if (acc >= minimumAccLimit)
                {
                    
                    gm.track1FinishedSuccess = true;
                }
                else
                {
                    gm.track1FinishedSuccess = false;
                }
                    Debug.Log("Track 1 finished.");
                }
                
                break;
            case 2:
                gm.resultTrack2 = acc;
                gm.track2Finished = true;
                
                if (acc >= minimumAccLimit)
                {
                    gm.trackFinished[activeTrackTabNumber - 1] = true;
                    gm.track2FinishedSuccess = true;
                }
                else
                {
                    gm.track2FinishedSuccess = false;
                }
                break;
            case 3:
                gm.resultTrack2 = acc;
                gm.track3Finished = true;
                
                if (acc >= minimumAccLimit)
                {
                    gm.trackFinished[activeTrackTabNumber - 1] = true;
                    gm.track3FinishedSuccess = true;
                }
                else
                {
                    gm.track3FinishedSuccess = false;
                }
                break;
            case 4:
                gm.resultTrack4 = acc;
                gm.track4Finished = true;
                
                if (acc >= minimumAccLimit)
                {
                    gm.trackFinished[activeTrackTabNumber - 1] = true;
                    gm.track4FinishedSuccess = true;
                }
                else
                {
                    gm.track4FinishedSuccess = false;
                }
                break;
        }
        EnableAllTrackTabButtons();
        ShowResetButton();
        UIManager.Instance.ToggleLabTeleportButton(true);
        durationLiveText.text = "0s"; // e.g. "12.3s"
        }

    }
    public void EnteringEndPoint(int trackNum)
    {

        
        Debug.Log("EnteringEndPoint: " + trackNum);
        stopButton.SetActive(true);
        
    }
    public void LeavingEndPoint(int trackNum)
    {
        Debug.Log("EnteringEndPoint: " + trackNum);
        stopButton.SetActive(false);
        
    }
    public void EnteringStartPoint(int trackNum)
    {
      
            Debug.Log("EnteringStartPoint: " + trackNum);
            startButton.SetActive(true);
            goToStartWanrning.SetActive(false);
            readyInfoText.text = "Starte Test " + trackNum;
            readyInfo.SetActive(true);
            currentActiveTrack = trackNum; 
        
        
         
    }
    public void LeavingStartPoint(int trackNum)
    {
        Debug.Log("LeavingStartPoint: " + trackNum);
        
        startButton.SetActive(false);
        readyInfo.SetActive(false);   
        if (!rm.isMeasuring)
        {
            goToStartWanrning.SetActive(true);
            goToStartWanrningText.text = "Fahre zu Startpunkt " + trackNum;
        }
        
    }

private (GameObject, GameObject, GameObject) CreateBlackOverlay(float startT, float endT)
{
    GameObject overlayTop = Instantiate(blackOverlayPrefab, overlayParentTop[currentActiveTrack-1]);
    GameObject overlayBottom = Instantiate(blackOverlayPrefab, overlayParentBottom[currentActiveTrack-1]);
    GameObject overlayWater = Instantiate(blackWaterOverlayPrefab, trackLineOnWater[currentActiveTrack-1].transform);
    // Collider passt sich automatisch an
    BoxCollider col = overlayWater.GetComponent<BoxCollider>();
    if (col != null)
    {
        StartCoroutine(EnableColliderDelayed(col));
        col.center = Vector3.zero;
        col.size   = new Vector3(1f, 1f, 1f); // Basisgröße, wird durch localScale multipliziert
    }

    RectTransform rt = overlayTop.GetComponent<RectTransform>();
    RectTransform rt2 = overlayBottom.GetComponent<RectTransform>();

    // === UI Overlays (Top/Bottom) ===
    rt.anchorMin = new Vector2(startT, rt.anchorMin.y);
    rt.anchorMax = new Vector2(endT, rt.anchorMax.y);
    rt.offsetMin = new Vector2(0f, rt.offsetMin.y);
    rt.offsetMax = new Vector2(0f, rt.offsetMax.y);

    rt2.anchorMin = new Vector2(startT, rt2.anchorMin.y);
    rt2.anchorMax = new Vector2(endT, rt2.anchorMax.y);
    rt2.offsetMin = new Vector2(0f, rt2.offsetMin.y);
    rt2.offsetMax = new Vector2(0f, rt2.offsetMax.y);

    // === Wasser-Overlay (3D) ===
    float lengthNorm = endT - startT;          // Länge im normierten Bereich (0–1)
    float midNorm    = (startT + endT) * 0.5f; // Mittelpunkt im normierten Bereich

    // Umrechnung: 0–1 → -0.5 bis +0.5
    float midZ    = midNorm - 0.5f;
    float lengthZ = lengthNorm; // da 1 → volle Breite von -0.5 bis +0.5

    Transform tW = overlayWater.transform;
    tW.localPosition = new Vector3(0f, 0f, midZ);   // Mitte des Intervalls auf Z-Achse
    tW.localScale    = new Vector3(1f, 1f, lengthZ); // Länge = Intervall auf Z-Achse

    
    Debug.Log($"Overlay + WaterObject created for interval {startT} → {endT} (Z: {midZ}, length: {lengthZ})");

    return (overlayTop, overlayBottom, overlayWater);
}




   

public void StartCorrection(float enterPos)
{
     //Debug.Log("STARTCORRECTION");
    isCorrecting = true;
    currentCorrectionColliderEnterPos = enterPos;
    currentTrackEnterPos = currentReveal;
    //Debug.Log("EnterPos: " + enterPos);
}

public void StopCorrection(float exitPos, GameObject waterBlackoutOverlay)
{
     //Debug.Log("STOPCORRECTION");
    isCorrecting = false;
    currentCorrectionColliderExitPos = exitPos;
    currentTrackExitPos = currentReveal;
    //Debug.Log("ExitPos: " + exitPos);
    UpdateBlackout(waterBlackoutOverlay);
}

    private void DeleteBlackOverlay()
    {
        for (int i = missingIntervals.Count - 1; i >= 0; i--)
{
    var interval = missingIntervals[i];
    if (interval.trackNumber == currentActiveTrack)
    {
        if (interval.overlayTop != null) Destroy(interval.overlayTop);
        if (interval.overlayBottom != null) Destroy(interval.overlayBottom);
        if (interval.overlayWater != null) Destroy(interval.overlayWater);

        missingIntervals.RemoveAt(i);
        Debug.Log("destroyed");
    }
}

        
    }

    private void UpdateBlackout(GameObject waterBlackoutOverlay)
{
    for (int i = 0; i < missingIntervals.Count; i++)
    {
        var interval = missingIntervals[i];
        
    if (interval.overlayWater == waterBlackoutOverlay)
        {
            
            float waterEnter = currentCorrectionColliderEnterPos;
            float waterExit  = currentCorrectionColliderExitPos;

            float trackEnter = currentTrackEnterPos;
            float trackExit  = currentTrackExitPos;

            // Fall 1: komplette Korrektur
            if ((waterEnter == 1 && waterExit == 0) || (waterEnter == 0 && waterExit == 1) )
            {
                Destroy(interval.overlayTop);
                Destroy(interval.overlayBottom);
                Destroy(interval.overlayWater);
                missingIntervals.RemoveAt(i);
                Debug.Log("Interval fully corrected and removed.");
            }
            

            return; // Fertig, nur ein Intervall bearbeiten
        }
    }
}
private void UpdateOverlay(MissingInterval interval)
{
    // UI Overlays neu setzen
    RectTransform rt = interval.overlayTop.GetComponent<RectTransform>();
    RectTransform rt2 = interval.overlayBottom.GetComponent<RectTransform>();

    rt.anchorMin = new Vector2(interval.startT, rt.anchorMin.y);
    rt.anchorMax = new Vector2(interval.endT, rt.anchorMax.y);
    rt.offsetMin = new Vector2(0f, rt.offsetMin.y);
    rt.offsetMax = new Vector2(0f, rt.offsetMax.y);

    rt2.anchorMin = new Vector2(interval.startT, rt2.anchorMin.y);
    rt2.anchorMax = new Vector2(interval.endT, rt2.anchorMax.y);
    rt2.offsetMin = new Vector2(0f, rt2.offsetMin.y);
    rt2.offsetMax = new Vector2(0f, rt2.offsetMax.y);

    // Wasser‑Overlay neu setzen
    float lengthNorm = interval.endT - interval.startT;
    float midNorm    = (interval.startT + interval.endT) * 0.5f;
    float midZ       = midNorm - 0.5f;

    Transform tW = interval.overlayWater.transform;
    tW.localPosition = new Vector3(0f, 0f, midZ);
    tW.localScale    = new Vector3(1f, 1f, lengthNorm);
}

 IEnumerator EnableColliderDelayed(BoxCollider col)
{
    col.enabled = false;
    yield return new WaitForSeconds(0.2f); // kurz warten
    col.enabled = true;
}


    private void HideGraphParentsUI()
    {
        foreach (GameObject g in graphParent)
        {
            if (g != null)
                g.SetActive(false);
        }
    }

    private void ShowGraphParentsUI()
    {
        foreach (GameObject g in graphParent)
        {
            if (g != null)
                g.SetActive(true);
        }
    }


    private void ShowActiveTrackWaterColliderOverlays()
    {
        foreach (GameObject s in startColliderOnWater)
        {
            if (s != null)
                s.SetActive(false);
        }
        GameManager2 gm = GameManager2.Instance;
        bool trackFinishedBool = gm.trackFinished[activeTrackTabNumber - 1];
        if (trackFinishedBool != null)
        {
            if (!trackFinishedBool)
            {
                startColliderOnWater[activeTrackTabNumber -1].SetActive(true);
            }
        }
        
        
    }

    private void HideTrackTabs()
    {
        
        foreach (GameObject g in trackTabUI)
        {
            if (g != null)
                g.SetActive(false);
        }
    }
    private void ShowActiveTrackTab()
    {
        trackTabUI[activeTrackTabNumber-1].SetActive(true);
    }
    // Disable all buttons except the one at trackTabNumber
    public void DisableOtherTrackTabButtons(int trackTabNumber)
    {
        for (int i = 0; i < trackTabButtons.Length; i++)
        {
            if (trackTabButtons[i] != null)
            {
                trackTabButtons[i].interactable = (i == trackTabNumber - 1);
            }
        }
    }

  
public void ChangeActiveTrackTabButtonsColor(int trackTabNumber)
{
    // Reset all buttons to default color first
    foreach (Button btn in trackTabButtons)
    {
        if (btn != null)
            btn.image.color = Color.white; // default color
    }

    // Highlight the active one
    int index = trackTabNumber - 1;
    if (index >= 0 && index < trackTabButtons.Length && trackTabButtons[index] != null)
    {
        trackTabButtons[index].image.color = green;; // active color
    }
}


    // Enable all buttons
    public void EnableAllTrackTabButtons()
    {
        foreach (Button btn in trackTabButtons)
        {
            if (btn != null)
                btn.interactable = true;
        }
    }

    private void ShowEndScaleInfo()
    {
        endScaleInfoOverlay[activeTrackTabNumber - 1].SetActive(true);
    }

    private void HideEndScaleInfo()
    {
        endScaleInfoOverlay[activeTrackTabNumber - 1].SetActive(false);
    }

    public void ResetTrackResult()
    {
        if (currentActiveTrack != null)
        {
            Debug.Log("StopAndResetTrackMeasurement: " + currentActiveTrack);
            stopButton.SetActive(false);
            startButton.SetActive(false);
            resetButton.SetActive(false);
            HideAccuracyAndDuration();
            Debug.Log(currentActiveTrack);
            trackLine[currentActiveTrack - 1].SetActive(false);
            trackLineOnWater[currentActiveTrack - 1].SetActive(false);
            endColliderOnWater[currentActiveTrack - 1].SetActive(false);
            endCircle[currentActiveTrack - 1].SetActive(false);
            startCircle[currentActiveTrack - 1].SetActive(true);
            startColliderOnWater[currentActiveTrack - 1].SetActive(true);
            onTrackUI.SetActive(false);
            notOnTrackUI.SetActive(false);
            goToEndWanrning.SetActive(false);
            
            HideEndScaleInfo();
            rm.isMeasuring = false;
            DeleteBlackOverlay();
            ResetReveal();
            // Stop timer
            trackDuration = Time.time - trackStartTime;
            

            GameManager2 gm = GameManager2.Instance;
            switch (currentActiveTrack)
        {
            case 1:
                gm.resultTrack1 = 0f;
                gm.trackFinished[activeTrackTabNumber - 1] = false;
                gm.track1Finished = false;
                gm.track1FinishedSuccess = false;

                Debug.Log("Track 1 finished.");
                break;
            case 2:
                gm.resultTrack2 = 0f;
                gm.trackFinished[activeTrackTabNumber - 1] = false;
                gm.track2Finished = false;
                gm.track2FinishedSuccess = false;
                break;
            case 3:
                gm.resultTrack3 = 0f;
                gm.trackFinished[activeTrackTabNumber - 1] = false;
                gm.track3Finished = false;
                gm.track3FinishedSuccess = false;
                break;
            case 4:
                gm.resultTrack4 = 0f;
                gm.trackFinished[activeTrackTabNumber - 1] = false;
                gm.track4Finished = false;
                gm.track4FinishedSuccess = false;
                break;
        }
        EnableAllTrackTabButtons();
        UIManager.Instance.ToggleLabTeleportButton(true);
        durationLiveText.text = "0s"; // e.g. "12.3s"
        }
    }

    public void HideTabAllInfoHints()
    {
        goToStartWanrning.SetActive(false);
        goToEndWanrning.SetActive(false);
        readyInfo.SetActive(false);
    }
public float GetAccuracy()
{
    Debug.Log("acc: " + missingIntervals.Count );
    if (missingIntervals.Count == 0)
        return 1f; // perfect accuracy

    float totalMissing = 0f;
    foreach (var interval in missingIntervals)
    {
        if (interval.trackNumber == activeTrackTabNumber)
        {
            totalMissing += (interval.endT - interval.startT);
        }
        
    }

    // assume full reveal length is 1.0 (normalized track)
    float totalLength = 1f;

    float accuracy = Mathf.Clamp01(1f - (totalMissing / totalLength));
    return accuracy;
}



    private void ShowResetButton()
{
    int index = currentActiveTrack - 1;

    GameManager2 gm = GameManager2.Instance;

    bool hasFinishedResult =
        index >= 0 &&
        index < gm.trackFinished.Length &&
        gm.trackFinished[index];

    
    bool hasReveal =
        currentReveal > 0f || currentMaxReveal > 0f;

    
    bool shouldShow =
        hasFinishedResult ||
        hasReveal;
        

    resetButton.SetActive(shouldShow);
}


public void DisplayAccuracyAndDuration(float acc, float duration)
{
    // Get the correct accuracyUI for the active track
    GameObject ui = accuracyUI[currentActiveTrack - 1];

    Debug.Log("Accuracy: " + acc);

    // Find children by name
    TextMeshProUGUI accText = ui.transform.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
    TextMeshProUGUI durText = ui.transform.Find("DurationText").GetComponent<TextMeshProUGUI>();
    Image background = ui.GetComponent<Image>();

    if (acc < minimumAccLimit)
    {
        background.color = red;
    }
    else
    {
        background.color = green;
    }
    

    // Update both
    accText.text = (acc * 100f).ToString("F1") + "%";
    durText.text = duration.ToString("F1") + "s";

    ui.SetActive(true);
}

    public void HideAccuracyAndDuration()
    {
        GameObject ui = accuracyUI[currentActiveTrack - 1];
        ui.SetActive(false);
    }
    public void ToggleBuoyUI(bool show)
    {
        buoyButtonUI.SetActive(show);
    }

    public void TakeBuoyData()
    {
        GameManager2 gm = GameManager2.Instance;
        if (gm != null)
        {
            gm.buoyDataTaken = true;
            buoyButtonUI.SetActive(false);
            buoyLocationUI.SetActive(false);
            
            buoyDataTakenAnimatedUI.SetActive(true);
            StartCoroutine(HideBuoyUIAfterDelay());
        }
        
    }
    private IEnumerator HideBuoyUIAfterDelay()
    {   
        yield return new WaitForSeconds(5f);
        buoyDataTakenAnimatedUI.SetActive(false);
        
    }

    public void GetLakeObjectsFromLakeManager()
{
    GameObject lmGameObject = GameObject.FindGameObjectWithTag("LakeManager");
    if (lmGameObject == null)
    {
        Debug.LogWarning("LakeManager not found in scene!");
        return;
    }

    LakeManager lm = lmGameObject.GetComponent<LakeManager>();
    if (lm == null)
    {
        Debug.LogWarning("LakeManager component missing on object with tag 'LakeManager'!");
        return;
    }

    // Assign references
    trackLineOnWater = lm.trackLineOnWater;
    //hiddenTrackLineOnWater = lm.hiddenTrackLineOnWater;
    startColliderOnWater = lm.startColliderOnWater;
    endColliderOnWater = lm.endColliderOnWater;
    robo = lm.robo;
    rm = lm.rm;
    Debug.Log("Lake objects successfully retrieved from LakeManager.");
}

}
