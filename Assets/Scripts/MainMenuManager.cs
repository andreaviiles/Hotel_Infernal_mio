using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject optionsCanvas;
    public GameObject controlesCanvas;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        optionsCanvas.SetActive(false);
        controlesCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    public void PlayGame()
    {
        Time.timeScale = 1f; 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("Hotel");
    } 


    public void OpenOptions()
    {
        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void OpenControles()
    {
        mainMenuCanvas.SetActive(false);
        controlesCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        optionsCanvas.SetActive(false);
        controlesCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void SetSensibilidad(float value)
    {
        SettingsManager.Instance.SetSensibilidad(value);
    }

    public void SetVolumen(float value)
    {
        SettingsManager.Instance.SetVolumen(value);
    }

}


