using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitManager : MonoBehaviour
{
    public static OrbitManager Instance;

    public event Action<Orbiter> OnOrbiterAdded;
    public void OrbiterAdded(Orbiter o) { OnOrbiterAdded?.Invoke(o); }

    [Header("Orbits")]
    [SerializeField] private GameObject _orbitPrefab;
    [SerializeField] private List<Orbit> _orbits = new List<Orbit>();
    [Space]
    [SerializeField] private float _distanceBetweenOrbits;
    [SerializeField] private bool _rotateClockwise = true;

    private float _globalOrbitAngle;

    //references
    private PlayerManager _playerManager;

    //getters
    public float GetOrbitAngle() => _globalOrbitAngle;
    public List<Orbit> GetOrbits() => _orbits;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _playerManager.OnPlayerDied += PlayerDied;
        AddOrbit(false);
    }

    private void OnDisable()
    {
        _playerManager.OnPlayerDied -= PlayerDied;
    }

    private void Update()
    {
        float direction = _rotateClockwise ? -1f : 1f;
        _globalOrbitAngle += direction * _playerManager.GetSpeed() * Time.deltaTime;
    }

    public void RefreshOrbiters()
    {
        foreach (Orbit orbit in _orbits)
            orbit.UpdateOrbiters();
    }


    private void PlayerDied()
    {
        foreach(Orbit orbit in _orbits)
        {
            Destroy(orbit.gameObject);
        }

        enabled = false;
    }


    #region Orbit Handling

    public void AddOrbit(bool cameraEffect = true)
    {
        GameObject orbitObj = Instantiate(_orbitPrefab, transform);
        Orbit orbit = orbitObj.GetComponent<Orbit>();

        int index = _orbits.Count;

        float radius = _distanceBetweenOrbits + (_distanceBetweenOrbits * index);

        _orbits.Add(orbit);

        orbit.Initialize(this, _playerManager, radius);

        if(cameraEffect)
            CameraEffectsManager.Instance.OnOrbitAdded();
    }

    public void AddOrbiter()
    {
        foreach(Orbit orbit in _orbits)
        {
            orbit.UpdateOrbiters();
        }
    }

    #endregion
}
