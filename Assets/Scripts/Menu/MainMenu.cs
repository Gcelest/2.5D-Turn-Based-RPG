using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Animator bookAnimator;
    public GameObject mainMenuUI;
    public GameObject saveSlotUI;
    public FadeManager fadeManager; // Reference to the FadeManager

    public void Play()
    {
        Debug.Log("Opening book for save slots...");
        bookAnimator.SetTrigger("OpenBookTrigger");
        StartCoroutine(ShowSaveSlots());
    }

    public void Back()
    {
        bookAnimator.SetTrigger("CloseBookTrigger");
        StartCoroutine(ShowMainMenu());
    }

    IEnumerator ShowSaveSlots()
    {
        yield return new WaitForSeconds(2f); // Match your book open anim
        saveSlotUI.SetActive(true);
    }

    IEnumerator ShowMainMenu()
    {
        yield return new WaitForSeconds(2f); // Match your book close anim
        mainMenuUI.SetActive(true);
    }

    // public void StartGameAfterSlot()
    // {
    //     Debug.Log("Fading and loading game scene...");
    //     fadeManager.PlayWithFade(); // 👈 this handles the fade+scene
    // }

    public void Quit()
    {
        Debug.Log("Player has quit the game.");
        Application.Quit();
    }
}
