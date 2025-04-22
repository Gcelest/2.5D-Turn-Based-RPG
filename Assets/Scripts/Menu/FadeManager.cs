using System;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public Animator fadeAnimator;

    private Action onFadeComplete;

    public void PlayWithFade(Action callback)
    {
        onFadeComplete = callback;
        fadeAnimator.SetTrigger("Fade");
    }

    // This should be called at the end of the FadeOut animation via Animation Event
    public void OnFadeComplete()
    {
        onFadeComplete?.Invoke();
        onFadeComplete = null;
    }
}