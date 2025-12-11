using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;       // Speed of pulsing
    public float pulseAmount = 0.1f;    // How much it scales

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scaleFactor;
    }
}
