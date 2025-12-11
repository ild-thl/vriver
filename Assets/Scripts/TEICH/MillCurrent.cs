using UnityEngine;

public class MillCurrent : MonoBehaviour
{
    [Header("Current Settings")]
    public Transform millCenter;      // Mittelpunkt der Mühle (Target)
    public float pullStrength = 10f;  // Stärke der Strömung
    public float maxDistance = 15f;   // Reichweite des Sog-Effekts

    private void OnTriggerStay(Collider other)
    {
        // Nur auf den Robo reagieren
        if (other.CompareTag("Robo"))
        {

            Debug.Log("inside PullArea");
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null && millCenter != null)
            {
                 Debug.Log("Pull");
                // Richtung zur Mühle
                Vector3 dir = (millCenter.position - other.transform.position);
                float distance = dir.magnitude;
                dir.Normalize();

                // Stärke abhängig von Distanz
                float strength = pullStrength * (1f - (distance / maxDistance));
                strength = Mathf.Clamp(strength, 0f, pullStrength);

                rb.AddForce(dir * strength, ForceMode.Acceleration);
            }
        }
    }
}
