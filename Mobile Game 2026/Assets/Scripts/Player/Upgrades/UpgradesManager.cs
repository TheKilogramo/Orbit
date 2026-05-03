using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Upgrade;

public class UpgradesManager : MonoBehaviour
{
    public static UpgradesManager Instance;

    [Header("Upgrades")]
    [SerializeField] private List<Upgrade> _possibleUpgrades;
    [SerializeField] private List<SpecialEffect> _ownedSpecials = new();

    private Dictionary<BuffTag, int> _ownedTags = new();
    private Dictionary<Upgrade, int> _upgradeShowCounts = new();

    [Header("UI / Audio")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private CanvasGroup _panelGroup;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI NameText;
    [SerializeField] private Transform _upgradeButtonsParent;
    [SerializeField] private GameObject _upgradeButtonPrefab;
    [SerializeField] private int _choices = 3;
    [Space]
    [SerializeField] private AudioClip _upgradesShowClip;
    [SerializeField] private AudioClip _upgradeChosenClip;
    [SerializeField] private float _fadeDuration = 0.1f;

    [HideInInspector] public Upgrade CurrentSelection;
    private Coroutine _fadeRoutine;


    private void Awake()
    {
        Instance = this;

        if (_panel)
            _panel.SetActive(false);
    }

    private void Start()
    {
        PlayerManager.Instance.OnPlayerDied += PlayerDied;
        PlayerManager.Instance.OnPlayerLevelUp += PlayerLeveledUp;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= PlayerDied;
        PlayerManager.Instance.OnPlayerLevelUp -= PlayerLeveledUp;
    }

    private void Update()
    {
/*        var log = "Owned Tags:\n";
        foreach (var kvp in _ownedTags)
        {
            log += $"{kvp.Key}: {kvp.Value}\n";
        }
        Debug.Log(log);*/
    }

    private void PlayerDied()
    {
        _panel.SetActive(false);
        Destroy(this);
    }

    public void PlayerLeveledUp()
    {
        StartCoroutine(ShowUpgradesPanel());
    }

    #region UI

    private IEnumerator ShowUpgradesPanel()
    {
        yield return new WaitForSecondsRealtime(.55f);

        //build choices first, before touching time scale or UI
        int choiceCount = CreateUpgradeChoices();
        if (choiceCount == 0)
            yield break;

        PauseMenuManager.Instance.PauseButton.SetActive(false);
        Time.timeScale = 0f;

        _panel.SetActive(true);
        _panelGroup.alpha = 0f;
        _panelGroup.interactable = false;
        _panelGroup.blocksRaycasts = false;

        AudioPlayer.PlayOneShot(_upgradesShowClip);

        yield return StartCoroutine(FadeCanvas(0f, 1f));
    }

    private int CreateUpgradeChoices()
    {
        foreach (Transform child in _upgradeButtonsParent)
            Destroy(child.gameObject);

        List<Upgrade> pool = new();
        foreach (var upg in _possibleUpgrades)
        {
            // check max show times
            _upgradeShowCounts.TryGetValue(upg, out int timesShown);
            if (timesShown >= upg.MaxShowTimes) continue;

            // check required stat tags
            bool eligible = true;
            foreach (var tag in upg.RequiredTags)
            {
                int timesRequired = upg.RequiredTags.Count(t => t == tag);
                _ownedTags.TryGetValue(tag, out int owned);
                if (owned < timesRequired)
                {
                    eligible = false;
                    break;
                }
            }

            if (!eligible) continue;

            // check required specials
            foreach (var special in upg.RequiredSpecials)
            {
                int timesRequired = upg.RequiredSpecials.Count(s => s == special);
                int owned = _ownedSpecials.Count(s => s == special);
                if (owned < timesRequired)
                {
                    eligible = false;
                    break;
                }
            }

            if (eligible) pool.Add(upg);
        }

        // pick without replacement
        List<Upgrade> selected = new();
        while (selected.Count < _choices && pool.Count > 0)
        {
            int idx = Random.Range(0, pool.Count);
            selected.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        if (selected.Count == 0)
            return 0;

        foreach (var upg in selected)
        {
            var obj = Instantiate(_upgradeButtonPrefab, _upgradeButtonsParent);
            obj.GetComponent<UpgradeButton>().Setup(upg);
        }

        CurrentSelection = null;
        NameText.text = "";
        DescriptionText.text = "";

        StartCoroutine(SelectMiddleUpgrade());

        return selected.Count;
    }


    IEnumerator SelectMiddleUpgrade()
    {
        yield return null; //skip 1 frame

        //auto select the center button
        if (_upgradeButtonsParent.childCount > 0)
        {
            int mid = _upgradeButtonsParent.childCount / 2;
            var btn = _upgradeButtonsParent.GetChild(mid).GetComponent<UpgradeButton>();
            btn.Select();
        }
    }

    public void OnSelectPressed()
    {
        if (CurrentSelection == null)
            return; // nothing picked yet

        ApplyUpgrade(CurrentSelection);
    }


    public void ApplyUpgrade(Upgrade upg)
    {
        _upgradeShowCounts.TryGetValue(upg, out int count);
        _upgradeShowCounts[upg] = count + 1;

        // track stat tags
        foreach (var tag in upg.Tags)
        {
            _ownedTags.TryGetValue(tag, out int owned);
            _ownedTags[tag] = owned + 1;
        }

        // track owned specials
        if (upg.IsSpecial)
            _ownedSpecials.Add(upg.EffectID);

        PlayerManager.Instance.ApplyUpgrade(upg);
        CloseMenu();
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeOutAndClose());

        AudioPlayer.PlayOneShot(_upgradeChosenClip, .5f);
    }

    private IEnumerator FadeOutAndClose()
    {
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        _panel.SetActive(false);
        _fadeRoutine = null;

        PauseMenuManager.Instance.PauseButton.SetActive(true);
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        float t = 0f;

        _panelGroup.alpha = from;

        while (t < _fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            _panelGroup.alpha = Mathf.Lerp(from, to, t / _fadeDuration);
            yield return null;
        }

        _panelGroup.alpha = to;

        _panelGroup.interactable = (to == 1f);
        _panelGroup.blocksRaycasts = (to == 1f);
    }

    #endregion
}
