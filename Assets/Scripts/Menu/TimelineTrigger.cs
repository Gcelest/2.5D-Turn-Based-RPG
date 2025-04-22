using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineTrigger : MonoBehaviour
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
