using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Load360Image2 : MonoBehaviour
{
    private ClickableObject activeCO; // Reference to active clickable object
    private string[] imagePaths; // Array of 360 image file paths
    private int currentImageIndex = 0; // Index to track the current image
    public Material sphereMaterial; // Material applied to the 360 sphere

    private Dictionary<int, Texture2D> textureCache = new Dictionary<int, Texture2D>(); // Cache for preloaded textures

    void Start()
    {

        activeCO = GameManager2.Instance.activeClickableObject;
        imagePaths = activeCO.vrImagePaths;
        Debug.Log("co load " + activeCO);

        if (imagePaths != null && imagePaths.Length > 0)
        {
            StartCoroutine(LoadTextureFromPath(currentImageIndex, true));
            
        }
    }

    public void Next_Change360MaterialTexture()
    {
        if (imagePaths != null && imagePaths.Length > 0)
        {
            currentImageIndex = (currentImageIndex + 1) % imagePaths.Length;
            ApplyTextureFromCache(currentImageIndex);
            StartCoroutine(PreloadAdjacentTextures()); // Preload next and previous
        }
    }

    public void Last_Change360MaterialTexture()
    {
        if (imagePaths != null && imagePaths.Length > 0)
        {
            currentImageIndex = (currentImageIndex - 1 + imagePaths.Length) % imagePaths.Length;
            ApplyTextureFromCache(currentImageIndex);
            StartCoroutine(PreloadAdjacentTextures()); // Preload next and previous
        }
    }

    private IEnumerator LoadTextureFromPath(int index, bool applyImmediately = false)
    {
        if (textureCache.ContainsKey(index))
        {
            if (applyImmediately) ApplyTextureFromCache(index);
            yield break;
        }

        string relativePath = imagePaths[index];
        string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + fullPath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                textureCache[index] = texture;

                if (applyImmediately)
                    ApplyTextureFromCache(index);
            }
            else
            {
                Debug.LogError("Failed to load texture from: " + fullPath + " | Error: " + uwr.error);
            }
        }
    }

    private void ApplyTextureFromCache(int index)
    {
        if (textureCache.ContainsKey(index) && sphereMaterial != null)
        {
            sphereMaterial.mainTexture = textureCache[index];
        }
    }

    private IEnumerator PreloadAdjacentTextures()
    {
        int nextIndex = (currentImageIndex + 1) % imagePaths.Length;
        int prevIndex = (currentImageIndex - 1 + imagePaths.Length) % imagePaths.Length;

        if (!textureCache.ContainsKey(nextIndex))
            StartCoroutine(LoadTextureFromPath(nextIndex));

        if (!textureCache.ContainsKey(prevIndex))
            StartCoroutine(LoadTextureFromPath(prevIndex));

        yield return null;
    }
}
