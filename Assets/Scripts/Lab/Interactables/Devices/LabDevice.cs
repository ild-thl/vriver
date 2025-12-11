using UnityEngine;

public abstract class LabDevice : MonoBehaviour, ILabDevice
{
    public string deviceName;

    public abstract void Interact();

    public string GetDeviceName() => deviceName;
}
