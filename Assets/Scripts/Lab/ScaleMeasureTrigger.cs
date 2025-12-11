using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScaleMeasureTrigger : MonoBehaviour
{
    public ScaleDevice scale;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Item on scale");
        // Nur interaktive Laborobjekte berücksichtigen
        if (other.GetComponent<LabInteractable>() != null)
        {
            Debug.Log("labinteractable on scale");
            scale.AddItem(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Item left scale");
        if (other.GetComponent<LabInteractable>() != null)
        {
            Debug.Log("labinteractable left scale");
            scale.RemoveItem(other.gameObject);
        }
    }
}
