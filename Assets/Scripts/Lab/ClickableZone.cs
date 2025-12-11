using UnityEngine;

public class ClickableZone : MonoBehaviour
{

    public GameObject clickZoneObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
    transform.position = clickZoneObject.transform.position;
    transform.rotation = clickZoneObject.transform.rotation;
    
    }

}
