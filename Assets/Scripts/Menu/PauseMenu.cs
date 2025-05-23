using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public MusicManager musicManager;
    public GameObject pauseCanvas;

    private static bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        pauseCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {//when user press esc it paused the game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    //frees all the time frames from all activities into 0 for the game
    public void PauseGame()
    {
        pauseCanvas.SetActive(true);
        //musicManager.PlayPauseMenuMusic();
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("Player paused");
    }
    //Refreshed all the time frame and continue the activity.
    public void ResumeGame()
    {
        pauseCanvas.SetActive(false);
        //musicManager.PlayInGameMusic();
        Time.timeScale = 1f;   
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("Player unpaused");
    }

    //Setting it the option to change volume? still vary
    public void Options()
    {
        Debug.Log("Player click option still empty...");
    }

    //sent back to the first scene which is 0 for Menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Debug.Log("Player went back to the Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Player quit playing");
    }
}

//if using weapon or tool might add this is the future in void update() to check when pause it wouldnt create any activation tool
//if(!PauseMenu.isPaused)
//  {
//      if(usetool == UseTool.Automatic)
//  }
