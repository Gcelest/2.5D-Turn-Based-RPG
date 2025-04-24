using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public FadeManager fadeManager; // Reference to the FadeManager
    public GameObject mainMenuUI;
    public GameObject saveSlotUI; // Reference to the FadeManager
    public GameObject repeatedlyBookObject;
    public GameObject elementalUI; // Reference to the FadeManager
    public Animator bookRepeatedlyAnimator; // Reference to the book animator
    public Animator bookAnimator;
    public Animator fireAnimator; // Reference to the fire animator
    public GameObject quitUI; // Reference to the FadeManager
    public void Play()
    {
        Debug.Log("Opening book for save slots...");
        StartCoroutine(ShowSaveSlots());
    }

    public void Back()
    {
        StartCoroutine(ShowMainMenu());
    }

    public IEnumerator ShowSaveSlots()
    {
        elementalUI.SetActive(true);
        fireAnimator.SetTrigger("FireUp");
        yield return new WaitForSeconds(.4f);
        mainMenuUI.SetActive(false);
        saveSlotUI.SetActive(true);
        yield return new WaitForSeconds(.6f);// Match your book open anim
        elementalUI.SetActive(false);
    }

    public IEnumerator ShowMainMenu()
    {
        elementalUI.SetActive(true);
        fireAnimator.SetTrigger("FireUp"); // Match your book close anim
        yield return new WaitForSeconds(.4f);
        saveSlotUI.SetActive(false);
        mainMenuUI.SetActive(true);
        yield return new WaitForSeconds(.6f);
        elementalUI.SetActive(false);
    }

    // public void StartGameAfterSlot()
    // {
    //     Debug.Log("Fading and loading game scene...");
    //     fadeManager.PlayWithFade(); // 👈 this handles the fade+scene
    // }

    public void Quit()
    {
        StartCoroutine(QuitGame());
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
