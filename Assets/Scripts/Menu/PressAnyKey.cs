using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class PressAnyKey : MonoBehaviour
{
    [Header("Animators")]
    public Animator bookAnimator;
    public Animator bookRepeatedlyAnimator;
    public Animator pressAnyKey;
    public Animator fireAnimator;
    public Animator windAnimator;
    public Animator logoAnimator;

    [Header("UI Groups")]
    public GameObject logoScreenUI;
    public GameObject repeatUI;
    public GameObject pressAnyKeyUI;
    public GameObject mainMenuUI;
    public GameObject elementalFireUI;
    public GameObject elementalWindUI;
    public GameObject backgroundGroup;
    public GameObject book;
    public FadeManager fadeManager;

    [Header("Background")]
    public GameObject audioManager;
    public ChangeBackground changeBackground;
    private bool keyPressed = false;


    void Start()
    {
        StartCoroutine(ShowLogoThenPressAnyKey());
    }


    IEnumerator ShowLogoThenPressAnyKey()
    {
        logoScreenUI.SetActive(true);
        logoAnimator.SetTrigger("LogoShow");
        pressAnyKeyUI.SetActive(false);
        yield return new WaitForSeconds(4f); // Show logo for 2.5 seconds
        logoScreenUI.SetActive(false);

        yield return new WaitForSeconds(3f); // Short delay before showing "Press Any Key"
        book.SetActive(true);
        pressAnyKeyUI.SetActive(true);

    }

    void Update()
    {
        if (!keyPressed && pressAnyKeyUI.activeSelf && Input.anyKeyDown)
        {
            keyPressed = true;
            StartCoroutine(PlayIntro());
        }

        // Optional: animate the blinking "press any key"
        if (pressAnyKeyUI.activeSelf)
        {
            pressAnyKey.SetTrigger("PressAnyKeyTrigger");
        }
    }

    IEnumerator PlayIntro()
    {
        book.SetActive(true);
        pressAnyKeyUI.SetActive(false);
        bookAnimator.SetTrigger("OpenBookTrigger");

        audioManager.GetComponent<MusicManager>().PlayMusic(); // Optional: add fade duration
        
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

        Camera activeCam = changeBackground.GetActiveBackgroundCamera();
        if (activeCam != null)
            StartCoroutine(ChangeFieldOfView(activeCam, 179f, 60f, 2f));

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

    IEnumerator ChangeFieldOfView(Camera cam, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (cam != null)
                cam.fieldOfView = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        if (cam != null)
            cam.fieldOfView = to;
    }

}
