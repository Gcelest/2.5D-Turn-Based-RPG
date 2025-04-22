using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimelineTriggerEnding : MonoBehaviour
{
    public PlayableDirector playableDirector;

    void Start()
    {
        // Subscribe to the timeline's finished event
        playableDirector.stopped += OnTimelineFinished;
    }

    void OnTimelineFinished(PlayableDirector aDirector)
    {
        // Trigger the scene transition when the timeline ends
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
}