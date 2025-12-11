using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // Singleton instance
    private AudioSource audioSource; // Store reference to AudioSource

    public AudioClip menuMusic; // Menu background music
    public AudioClip gameMusic; // Game background music

    public Image volumeButton;
    public Sprite volumeStop;
    public Sprite volumePlay;

    private bool isMuted = true; // Track whether music is playing
    private AudioClip currentTrack; // Store current playing track


    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
            return; // Exit to prevent further execution
        }

        // Get or Add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        
    }

    // Function to play background music
    public void PlayBackgroundMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = true; // Loop background music
            audioSource.Play();
            isMuted = false;
            UpdateVolumeIcon(); // Update UI
        }
    }

    // Function to stop background music
    public void StopBackgroundMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            isMuted = true;
            UpdateVolumeIcon(); // Update UI
        }
    }

    // Function to toggle between playing and stopping the music
    public void ToggleMusic()
    {
        if (isMuted)
        {
            PlayBackgroundMusic();
        }
        else
        {
            StopBackgroundMusic();
        }
    }

    // Update the volume button sprite
    private void UpdateVolumeIcon()
    {
        if (volumeButton != null)
        {
            volumeButton.sprite = isMuted ? volumeStop : volumePlay;
        }
    }

    public void SwitchAudioFile(bool isGame)
    {
          AudioClip newTrack = isGame ? gameMusic : menuMusic;

           if (newTrack != null && newTrack != currentTrack)
        {
            currentTrack = newTrack;
            audioSource.clip = currentTrack;

            if (!isMuted)
            {
                audioSource.Play();
            }
        }
    }
}
