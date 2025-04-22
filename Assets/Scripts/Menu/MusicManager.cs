using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource pauseMenuMusic;
    public AudioSource inGameMusic;

    void Start()
    {
        pauseMenuMusic = GameObject.Find("PauseMusic").GetComponent<AudioSource>();
        inGameMusic = GameObject.Find("GameMusic").GetComponent<AudioSource>();

        // Start with in-game music playing
        inGameMusic.Play();
    }

    // Add methods to switch between Pause Menu and in-game music
    public void PlayPauseMenuMusic()
    {
        inGameMusic.Stop();
        pauseMenuMusic.Play();
    }

    public void PlayInGameMusic()
    {
        pauseMenuMusic.Stop();
        inGameMusic.Play();
    }
}
