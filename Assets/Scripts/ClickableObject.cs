using UnityEngine;
using UnityEngine.UI;
public class ClickableObject : MonoBehaviour
{
    public enum ObjectType { Remote, Boje, LabObject, PlacingArea, Button } // Define object types
    public ObjectType objectType;

    // Data for the UI
    public string title;
    public string description;
    
    public Texture2D posterTexture; // Texture for the poster

    public GameObject miniModelPrefab;

    public string[] vrImagePaths; // Store file paths instead


    public Transform returnSpawnPosition; //where player spawns when returning to main scene
    public Transform snapTarget;


    // Method to get data (can be extended for complex logic)
    public object GetData()
    {
        return new { objectType, title, description, posterTexture, miniModelPrefab, vrImagePaths, returnSpawnPosition, snapTarget};
    }
}
