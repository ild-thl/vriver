using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Canvas richtet sich zur Kamera aus, bleibt aber am Parent
        Vector3 lookDir = transform.position - cam.transform.position;

        // Optional: nur Y-Achse (z. B. für Labels über Objekten)
        // lookDir.y = 0f;

        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
