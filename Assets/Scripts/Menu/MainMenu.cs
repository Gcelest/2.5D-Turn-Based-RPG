using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public FadeManager fadeManager; // Reference to the FadeManager
    public GameObject mainMenuUI;
    public GameObject saveSlotUI; // Reference to the FadeManager
    public GameObject repeatedlyBookObject;
    public GameObject elementalFireUI; // Reference to the FadeManager
    public GameObject styleMenuUI;
    public GameObject backgroundGroup;
    public ChangeBackground changeBackground;

    public Animator bookRepeatedlyAnimator; // Reference to the book animator
    public Animator bookAnimator;
    public Animator fireAnimator; // Reference to the fire animator
    public GameObject quitUI; // Reference to the FadeManager
    public void Play()
    {
        Debug.Log("Opening book for save slots...");
        StartCoroutine(ShowSaveSlots());
    }

    public void BackSaveSlots()
    {
        StartCoroutine(ShowSaveSlotReturnMainMenu());
    }

    public IEnumerator ShowSaveSlots()
    {
        elementalFireUI.SetActive(true);
        fireAnimator.SetTrigger("FireUp");
        yield return new WaitForSeconds(.4f);
        mainMenuUI.SetActive(false);
        saveSlotUI.SetActive(true);
        yield return new WaitForSeconds(.6f);// Match your book open anim
        elementalFireUI.SetActive(false);
    }

    public IEnumerator ShowSaveSlotReturnMainMenu()
    {
        elementalFireUI.SetActive(true);
        fireAnimator.SetTrigger("FireUp"); // Match your book close anim
        yield return new WaitForSeconds(.4f);
        saveSlotUI.SetActive(false);
        mainMenuUI.SetActive(true);
        yield return new WaitForSeconds(.6f);
        elementalFireUI.SetActive(false);
    }

    // might change OpenStyleReturnMainMenu to IEnumetator
    // public void BackOpenStyle()
    // {
    //     StartCoroutine(OpenStyleReturnMainMenu());
    // }

    public void OpenStyleMenu()
    {
        if (!backgroundGroup.activeSelf)
        {
            backgroundGroup.SetActive(true);
            int savedIndex = PlayerPrefs.GetInt("BackgroundIndex", 0);
            changeBackground.SetBackground(savedIndex);
        }

        mainMenuUI.SetActive(false); // Optional: Hide main menu if needed
        styleMenuUI.SetActive(true);
    }

    public void OpenStyleReturnMainMenu()
    {
        styleMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }


    // public void StartGameAfterSlot()
    // {
    //     Debug.Log("Fading and loading game scene...");
    //     fadeManager.PlayWithFade(); // 👈 this handles the fade+scene
    // }

    public void Quit()
    {
        quitUI.SetActive(true); // Show quit confirmation UI
    }

    public void QuitNo()
    {
        quitUI.SetActive(false); // Hide quit confirmation UI
        mainMenuUI.SetActive(true); // Go back to the main menu
    }

    public void QuitYes()
    {
        StartCoroutine(QuitGame()); // Proceed to quit game
    }

    private IEnumerator QuitGame()
    {
        quitUI.SetActive(false); // Hide the quit confirmation UI
        bookRepeatedlyAnimator.SetTrigger("CloseRepeatedly");
        yield return new WaitForSeconds(0.5f); // Wait for animation
        repeatedlyBookObject.SetActive(false);

        bookAnimator.SetTrigger("CloseBookTrigger");

        fadeManager.gameObject.SetActive(true); // Ensure fade is active

        fadeManager.PlayWithFade(() =>
        {
            Application.Quit();
        });
    }

}
