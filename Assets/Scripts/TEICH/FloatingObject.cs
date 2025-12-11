using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    
    
    [Header("Floating Motion")]
    public float floatAmplitude = 0.2f; // wie stark es auf/ab geht
    public float floatFrequency = 1f;   // wie schnell es schwingt
    public float rotationSpeed = 15f;   // optionale langsame Drehung

    private float startY;
    private float phaseOffset;

    private void Start()
    {
        // Start-Position merken
        startY = transform.position.y;

        // zufällige Phase, damit nicht alle gleich wippen
        phaseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        // Floaten auf/ab
        float newY = startY + Mathf.Sin(Time.time * floatFrequency + phaseOffset) * floatAmplitude;
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;

        // optionale leichte Drehung
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    
}

