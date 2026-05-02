using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject StartMenu;

    private void Start()
    {
        Time.timeScale = 1f;

        CloseSettingsMenu();
    }

    public void StartGame()
    {
        GameManager.LoadScene("FirstLevel");
    }

    public void OpenSettingsMenu()
    {
        SettingsMenu.SetActive(true);
        StartMenu.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        SettingsMenu.SetActive(false);
        StartMenu.SetActive(true);
    }
}
