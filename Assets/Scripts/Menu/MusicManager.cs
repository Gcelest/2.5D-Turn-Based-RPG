using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource inGameMusic;

    void Awake()
    {
        // Ensure music continues across scenes if needed
        DontDestroyOnLoad(gameObject);

        // Optionally preload the music clip
        if (inGameMusic != null && inGameMusic.clip != null)
        {
            inGameMusic.playOnAwake = false;
            inGameMusic.loop = true;
        }
    }

    public void PlayMusic(float fadeInTime = 1f)
    {
        if (inGameMusic == null || inGameMusic.isPlaying) return;
        StartCoroutine(FadeInMusic(fadeInTime));
    }

    private IEnumerator FadeInMusic(float duration)
    {
        float volume = 0f;
        inGameMusic.volume = 0f;
        inGameMusic.Play();

        while (volume < 1f)
        {
            volume += Time.deltaTime / duration;
            inGameMusic.volume = Mathf.Clamp01(volume);
            yield return null;
        }
    }
}


// using UnityEngine;

// public class MusicManager : MonoBehaviour
// {
//     public AudioSource pauseMenuMusic;
//     public AudioSource inGameMusic;

//     void Start()
//     {
//         //pauseMenuMusic = GameObject.Find("PauseMusic").GetComponent<AudioSource>();
//         inGameMusic = GameObject.Find("GameMusic").GetComponent<AudioSource>();

//         // Start with in-game music playing
//         inGameMusic.Play();
//     }

//     // Add methods to switch between Pause Menu and in-game music
//     public void PlayPauseMenuMusic()
//     {
//         inGameMusic.Stop();
//         pauseMenuMusic.Play();
//     }

//     public void PlayInGameMusic()
//     {
//         pauseMenuMusic.Stop();
//         inGameMusic.Play();
//     }
// }
