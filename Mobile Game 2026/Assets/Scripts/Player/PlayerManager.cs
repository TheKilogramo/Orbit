using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Action OnPlayerDied;
    public Action OnPlayerLevelUp;

    //references
    private OrbitManager _orbitManager;

    public static PlayerManager Instance;

    [Header("Player Stats")]
    [SerializeField] private float _baseDamage = 3;
    [SerializeField] private float _baseSpeed = 3;
    [SerializeField] private Vector3 _baseSize;
    [SerializeField] private int _orbitersPerOrbit = 3;

    private float _damageMultiplier = 1f;
    private float _speedMultiplier = 1f;
    private float _sizeMultiplier = 1f;

    //getters
    public float GetDamage() => _baseDamage * _damageMultiplier;
    public float GetSpeed() => _baseSpeed * _speedMultiplier;
    public Vector3 GetOrbiterSize() => _baseSize * _sizeMultiplier;
    public int GetOrbitersPerOrbit() => _orbitersPerOrbit;

    [Header("HP")]
    [SerializeField] private int _lives = 3;
    [HideInInspector] public bool IsDead = false;

    [Header("HP Visuals")]
    [SerializeField] private GameObject _heartPrefab; 
    [SerializeField] private Transform _heartsContainer;
    [SerializeField] private SpriteRenderer[] _srs;
    [Space]
    [SerializeField] private GameObject _deathParticlesPrefab;
    [SerializeField] private AudioClip _damageClip;
    [SerializeField] private AudioClip _deathClip;
    [Space]
    [SerializeField] private float _invulnerabilityTime = 1.0f;
    [SerializeField] private float _flickerInterval = 0.1f;

    private List<GameObject> _hearts = new List<GameObject>();
    private bool _canTakeDamage = true;

    [Header("XP")]
    [SerializeField] private float _xpNeeded = 20;
    [SerializeField] float _xpRequirementMultiplier = 1.25f;

    private int _level = 1;
    private float _xp = 0;
    private bool _canGetXp = true;

    [Header("XP Visuals")]
    [SerializeField] private Image _xpBar;
    [SerializeField] private TextMeshProUGUI _lvlText;


    private void Awake()
    {
        _orbitManager = GetComponent<OrbitManager>();
        Instance = this;

        //spawn hearts based on lives
        for (int i = 0; i < _lives; i++)
        {
            GameObject h = Instantiate(_heartPrefab, _heartsContainer);
            _hearts.Add(h);
        }

        LevelManager.OnBossSpawned += StopGettingXP;
    }

    private void OnDisable()
    {
        LevelManager.OnBossSpawned -= StopGettingXP;
    }

    void Update()
    {
        if (IsDead || Time.timeScale == 0f) return;

        Move();
        ClampToScreen();
        UpdateXPVisuals();

        if (Keyboard.current.eKey.isPressed) AddXP(20 * _level);
        if (Keyboard.current.rKey.isPressed) AddXP(200 * _level);
    }

    #region Movement

    private void Move()
    {
        if (!InputManager.TouchIsHeld) return;

        // Convert screen-space delta to world-space delta
        Vector3 currentScreenPos = new Vector3(
            InputManager.TouchPosition.x,
            InputManager.TouchPosition.y, 0f);
        Vector3 previousScreenPos = new Vector3(
            InputManager.TouchPosition.x - InputManager.TouchDelta.x,
            InputManager.TouchPosition.y - InputManager.TouchDelta.y, 0f);

        Vector3 worldCurrent = Camera.main.ScreenToWorldPoint(currentScreenPos);
        Vector3 worldPrevious = Camera.main.ScreenToWorldPoint(previousScreenPos);

        worldCurrent.z = 0f;
        worldPrevious.z = 0f;

        Vector3 worldDelta = worldCurrent - worldPrevious;
        transform.position += worldDelta;
    }

    private void ClampToScreen()
    {
        float halfW = WorldAreaManager.Instance.playWidth * 0.5f;
        float halfH = WorldAreaManager.Instance.playHeight * 0.5f;

        Vector3 camPos = WorldAreaManager.Instance.center;

        float minX = camPos.x - halfW;
        float maxX = camPos.x + halfW;
        float minY = camPos.y - halfH;
        float maxY = camPos.y + halfH;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    #endregion

    #region Upgrades

    public void ApplyUpgrade(Upgrade upgrade)
    {
        _damageMultiplier += upgrade.DamageMultiplier;
        _speedMultiplier += upgrade.SpeedMultiplier;
        _sizeMultiplier += upgrade.SizeMultiplier;

        if (upgrade.ExtraOrbiters > 0)
        {
            for (int i = 0; i < upgrade.ExtraOrbiters; i++)
            {
                AddOrbiter();
            }
        }

        if (upgrade.ExtraOrbits > 0)
        {
            for (int i = 0; i < upgrade.ExtraOrbits; i++)
            {
                AddOrbit();
            }
        }

        _orbitManager.RefreshOrbiters();

        if (upgrade.IsSpecial)
            EffectsManager.Instance.ApplyEffect(upgrade.EffectID);
    }

    public void AddOrbit()
    {
        _orbitManager.AddOrbit();
    }

    public void AddOrbiter()
    {
        _orbitersPerOrbit++;
        _orbitManager.AddOrbiter();
    }

    #endregion

    #region HP

    public void Damage()
    {
        if (!_canTakeDamage || IsDead)
            return;

        _lives--;

        //remove heart from UI
        if (_lives >= 0 && _lives < _hearts.Count)
        {
            Destroy(_hearts[_lives]);
            _hearts.RemoveAt(_lives);
        }

        Handheld.Vibrate();
        CameraEffectsManager.Instance.ShakeCamera(.15f, .2f);

        if (_lives <= 0)
        {
            Die();
            return;
        }

        AudioPlayer.PlayOneShot(_damageClip);
        StartCoroutine(InvulnerabilityRoutine());
    }

    public void Heal()
    {
        if (IsDead) return;

        _lives++;

        //add heart to UI
        GameObject h = Instantiate(_heartPrefab, _heartsContainer);
        _hearts.Add(h);
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        _canTakeDamage = false;

        // Flicker 3 times
        for (int i = 0; i < 3; i++)
        {
            foreach (SpriteRenderer sr in _srs)
            {
                sr.enabled = false;
            }

            yield return new WaitForSeconds(_flickerInterval);

            foreach (SpriteRenderer sr in _srs)
            {
                sr.enabled = true;
            }

            yield return new WaitForSeconds(_flickerInterval);
        }

        //continue invulnerability for the remaining time
        float remaining = _invulnerabilityTime - (_flickerInterval * 6);
        if (remaining > 0)
            yield return new WaitForSeconds(remaining);

        _canTakeDamage = true;
    }

    public void Die()
    {
        if (IsDead) return;

        OnPlayerDied?.Invoke();

        IsDead = true;
        StopAllCoroutines();
        _canTakeDamage = false;
        AudioPlayer.PlayOneShot(_deathClip);
        Handheld.Vibrate();
        Handheld.Vibrate();

        foreach(SpriteRenderer sr in _srs)
        {
            sr.enabled = false;
        }

        Instantiate(_deathParticlesPrefab, transform.position, transform.rotation);

        StartCoroutine(RestartSceneRoutine());

    }

    IEnumerator RestartSceneRoutine()
    {
        yield return new WaitForSeconds(3f);
        GameManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region XP

    private void StopGettingXP()
    {
        _canGetXp = false;
    }

    private void UpdateXPVisuals()
    {
        _xpBar.fillAmount = _xp / _xpNeeded;
        _lvlText.text = "LVL " + _level;
    }

    public void AddXP(float amount)
    {
        if (!_canGetXp) return;

        _xp += amount;

        //level up when threshold reached
        while (_xp >= _xpNeeded)
        {
            _xp -= _xpNeeded;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _level++;

        //increase XP requirement each level
        _xpNeeded *= _xpRequirementMultiplier;

        OnPlayerLevelUp?.Invoke();
    }

    #endregion
}
