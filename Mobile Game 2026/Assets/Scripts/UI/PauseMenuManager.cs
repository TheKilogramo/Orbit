using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class PauseMenuManager : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject PauseMenu;
    public GameObject PauseButton;

    public static PauseMenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= PlayerDied;
    }

    private void Start()
    {
        PlayerManager.Instance.OnPlayerDied += PlayerDied;

        Time.timeScale = 1f;
    }

    private void PlayerDied()
    {
        PauseButton.SetActive(false);
    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        PauseButton.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        PauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Resume();
        PlayerManager.Instance.Die();
    }

    public void MainMenuButton()
    {
        GameManager.LoadScene(GameManager.SCENE_NAME_MAIN_MENU);
    }

    public void OpenSettingsMenu()
    {
        SettingsMenu.SetActive(true);
        PauseMenu.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        SettingsMenu.SetActive(false);
        PauseMenu.SetActive(true);
    }
}
