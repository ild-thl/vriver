using UnityEngine;
using System.Collections;

public class DuckMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;                // Geschwindigkeit
    public float rotationChange = 90f;      // Rotationsänderung bei Kollision (in Grad)
    public float randomTurnFactor = 30f;    // Zufällige Variation zusätzlich

    [Header("Audio Settings")]
    public AudioClip[] duckSounds;          // Array mit Enten-Sounds
    public Vector2 quackInterval = new Vector2(3f, 10f); // Zufallsintervall in Sekunden

    private Rigidbody rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Startrichtung zufällig
        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, randomAngle, 0);

        // Coroutine für zufällige Sounds starten
        StartCoroutine(PlayRandomQuacks());
    }

    void FixedUpdate()
    {
        // Bewegung immer in Blickrichtung
        Vector3 forward = transform.forward * speed;
        rb.MovePosition(rb.position + forward * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Nur reagieren, wenn nicht auf andere Enten trifft
        if (collision.gameObject.CompareTag("Duck")) return;

        // Neue Rotation: bestehende Richtung + Abprallwinkel + Zufall
        float randomExtra = Random.Range(-randomTurnFactor, randomTurnFactor);
        float newY = transform.eulerAngles.y + rotationChange + randomExtra;

        transform.rotation = Quaternion.Euler(0, newY, 0);
    }

    private IEnumerator PlayRandomQuacks()
    {
        while (true)
        {
            // Zufällige Zeit warten
            float waitTime = Random.Range(quackInterval.x, quackInterval.y);
            yield return new WaitForSeconds(waitTime);

            // Sound nur abspielen, wenn Array nicht leer ist
            if (duckSounds.Length > 0)
            {
                AudioClip clip = duckSounds[Random.Range(0, duckSounds.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
