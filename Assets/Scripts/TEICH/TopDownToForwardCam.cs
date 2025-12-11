using UnityEngine;

public class TopDownToForwardCam : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;          // The object or point the camera moves towards
    public Vector3 targetOffset;      // Offset from the target position
    public float moveSpeed = 2f;      // Speed of movement
    public float rotationSpeed = 2f;  // Speed of rotation adjustment
    private Vector3 velocity = Vector3.zero;

    [Header("Rotation Settings")]
    public float startXRotation = 90f;   // Starting X rotation (top-down)
    public float endXRotation = 0f;      // Final X rotation (forward-facing)
    public float rotationEndDistance = 2f; // When to fully reset to 0 rotation

    [Header("Player Activation")]
    public GameObject player;           // The player to activate after movement
    public float finishThreshold = 0.05f; // Distance threshold for "done"
    public GameObject labButton;        // Optional button to activate
     public GameObject sceneTitleText;    

    private Vector3 startPos;

    void Start()
    {
        if (GameManager2.Instance.firstTimeAtLake)
        {

       
        if (target != null)
        {
            startPos = transform.position;
            transform.rotation = Quaternion.Euler(startXRotation, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        if (player != null)
            player.SetActive(false); // ensure player starts inactive
        if (labButton != null && GameManager2.Instance.firstTimeAtLake)
            labButton.SetActive(false);
         }else
         {
             labButton.SetActive(true);
             Destroy(this.gameObject); // destroy camera object
         }
    }

    void Update()
    {
        if (target == null) return;

        // Target position with offset
        Vector3 desiredPos = target.position + targetOffset;

        // Smoothly move toward target, speed controlled by moveSpeed
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            1f / Mathf.Max(moveSpeed, 0.01f) // smaller smoothTime = faster movement
        );

        // Calculate distance to target
        float dist = Vector3.Distance(transform.position, desiredPos);

        // Lerp X rotation back to 0 when close enough
        float t = Mathf.InverseLerp(rotationEndDistance * 2f, 0f, dist);
        float newX = Mathf.Lerp(startXRotation, endXRotation, t);

        Quaternion targetRot = Quaternion.Euler(newX, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        // Check if we are finished
        if (dist <= finishThreshold)
        {
            if (player != null)
                player.SetActive(true); // activate player
            if (labButton != null)
                labButton.SetActive(true);
                sceneTitleText.SetActive(true);

            GameManager2.Instance.firstTimeAtLake = false;
            Destroy(this.gameObject); // destroy camera object
        }
    }
}
