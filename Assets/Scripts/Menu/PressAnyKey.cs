using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class PressAnyKey : MonoBehaviour
{
    public Animator bookAnimator;
    public Animator bookRepeatedlyAnimator;
    public Animator pressAnyKey;
    public Animator fireAnimator;
    public Animator windAnimator;
    public GameObject repeatUI;
    public GameObject pressAnyKeyUI;
    public GameObject mainMenuUI;
    public GameObject elementalFireUI;
    public GameObject elementalWindUI;
    public ChangeBackground changeBackground;
    public GameObject backgroundGroup;

    private bool keyPressed = false;

    void Update()
    {
        pressAnyKey.SetTrigger("PressAnyKeyTrigger");
        if (!keyPressed && Input.anyKeyDown)
        {
            keyPressed = true;
            StartCoroutine(PlayIntro());
        }
    }

    IEnumerator PlayIntro()
    {
        pressAnyKeyUI.SetActive(false);
        bookAnimator.SetTrigger("OpenBookTrigger");
        yield return new WaitForSeconds(.1f);
        elementalWindUI.SetActive(true);
        windAnimator.SetTrigger("WindBurst");
        yield return new WaitForSeconds(.4f);
        repeatUI.SetActive(true);

        yield return new WaitForSeconds(1f);


        bookRepeatedlyAnimator.SetTrigger("OpenRepeatedly");
        if (backgroundGroup != null)
            backgroundGroup.SetActive(true);

        int saved = PlayerPrefs.GetInt("BackgroundIndex", 0);
        changeBackground.SetBackground(saved);

        yield return new WaitForSeconds(.1f);
        elementalWindUI.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        // match animation duration
        elementalFireUI.SetActive(true);
        fireAnimator.SetTrigger("FireUp");
        yield return new WaitForSeconds(.4f);
        mainMenuUI.SetActive(true);
        yield return new WaitForSeconds(.6f);
        elementalFireUI.SetActive(false);
    }
}
