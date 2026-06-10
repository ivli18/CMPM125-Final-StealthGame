using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject settingsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1"); 
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void HideCredits()
    {
        creditsPanel.SetActive(false);
    }
    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }
    public void HideSettings()
    {
        settingsPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}