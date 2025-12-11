using UnityEngine;

public class RemoteCollider : MonoBehaviour
{
    [Header("UI Element das angezeigt werden soll")]
    public GameObject remoteInteractionUI; // Hier dein Canvas-Panel oder ähnliches reinziehen

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Wichtig: Player muss den Tag "Player" haben
        {
            if (remoteInteractionUI != null)
                remoteInteractionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (remoteInteractionUI != null)
                remoteInteractionUI.SetActive(false);
        }
    }
}
