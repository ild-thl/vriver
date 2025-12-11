using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageSequencePlayer : MonoBehaviour
{
    public RawImage displayImage; // Assign this in the Inspector
    public string folderPath = "ImageSequence"; // Folder path in Resources
    public int totalFrames = 100; // Total number of frames
    public float frameRate = 30f; // Playback speed in frames per second

    private int currentFrame = 0; // Track the current frame
    private bool isPlaying = false; // Playback state

    void Start()
    {
        if (displayImage == null)
        {
            //Debug.LogError("No RawImage assigned to displayImage.");
            return;
        }
        PlayImageSequence();
        
    }

    public void PlayImageSequence()
    {
        StartCoroutine(PlaySequence());
    }
    // Coroutine to play the image sequence
    IEnumerator PlaySequence()
    {
        isPlaying = true;

        while (isPlaying)
        {
            // Construct the file name (e.g., frame1, frame2, ...)
            string fileName = $"{folderPath}/frame{currentFrame + 1}";

            // Load the texture from Resources
            Texture2D texture = Resources.Load<Texture2D>(fileName);

            if (texture != null)
            {
                displayImage.texture = texture;
            }
            else
            {
                //Debug.LogError($"Failed to load texture: {fileName}");
            }

            // Increment the frame counter
            currentFrame = (currentFrame + 1) % totalFrames;

            // Wait for the next frame
            yield return new WaitForSeconds(1f / frameRate);
        }
    }

    // Stop the sequence playback
    public void StopSequence()
    {
        isPlaying = false;
    }
}
