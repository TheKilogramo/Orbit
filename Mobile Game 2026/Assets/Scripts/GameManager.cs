using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 120;
    }

    [Header("GLOBAL SFX")]
    public AudioClip ButtonPressSound;

    [Header("SCENE MANAGER")]
    [SerializeField] private Animator _sceneAnim;
    [SerializeField] private float _transitionTime = 1f;

    private string _loadSceneName;
    private bool _changingScenes = false;

    //SCENE NAMES
    public const string SCENE_NAME_MAIN_MENU = "MainMenu";
    public const string SCENE_NAME_FIRST_LEVEL = "FirstLevel";


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void LoadScene(string sceneName)
    {
        if (Instance._changingScenes == true)
            return;

        Instance.StartCoroutine(Instance.LoadSceneCoroutine(sceneName));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName)
    {

        _changingScenes = true;

        _sceneAnim.SetTrigger("start");
        _loadSceneName = sceneName;

        yield return new WaitForSecondsRealtime(_transitionTime);

        SceneManager.LoadScene(sceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_loadSceneName == scene.name)
            StartCoroutine(PlayEnd());
    }

    IEnumerator PlayEnd()
    {
        yield return null; // wait one frame for Animator initialization
        _sceneAnim.SetTrigger("end");
        _loadSceneName = "";

        yield return new WaitForSeconds(_transitionTime);
        _changingScenes = false;
    }

}

