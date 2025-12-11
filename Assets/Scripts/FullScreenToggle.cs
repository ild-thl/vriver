using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ToggleFullscreenJS();

    private bool isFullscreen = false;

    public Texture2D enterFullScreenIcon;
    public Texture2D exitFullScreenIcon;
    public RawImage buttonIcon;

    public void ToggleFullscreen()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            ToggleFullscreenJS(); // WebGL: Nutze JavaScript für Fullscreen
            isFullscreen = !isFullscreen;
        }
        else
        {
            isFullscreen = !isFullscreen;
            Screen.fullScreen = isFullscreen; // Standard für andere Plattformen
        }

        if (buttonIcon != null)
        {
            buttonIcon.texture = isFullscreen ? exitFullScreenIcon : enterFullScreenIcon;
        }

        //Debug.Log(isFullscreen ? "Entered Fullscreen" : "Exited Fullscreen");
    }
}
