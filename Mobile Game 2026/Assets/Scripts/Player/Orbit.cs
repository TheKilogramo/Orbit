using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    private OrbitManager _orbitData;
    private PlayerManager _playerData;
    private float _radius;
    private bool _initialized = false;

    [Header("Orbiters")]
    [SerializeField] private GameObject _orbiterPrefab;
    [SerializeField] private List<Orbiter> _orbiters = new List<Orbiter>();

    [Header("Visuals")]
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private int _resolution = 150;

    public float GetRadius() => _radius;
    public List<Orbiter> GetOrbiters() => _orbiters;

    public void Initialize(OrbitManager data, PlayerManager playerData, float radius)
    {
        _orbitData = data;
        _playerData = playerData;
        _radius = radius;

        _lr.enabled = true;
        _lr.loop = true;
        _lr.useWorldSpace = true;
        _lr.textureMode = LineTextureMode.Tile;

        _initialized = true;

        UpdateOrbiters();
    }
    public Vector3 GetPointOnOrbit(float angle)
    {
        return transform.position + OrbitPoint(angle, _radius);
    }

    private void Update()
    {
        if (!_initialized) return;

        DrawOrbit();
        ArrangeOrbiters();
    }

    #region Orbit Visuals
    private void DrawOrbit()
    {
        //set all positions
        _lr.positionCount = _resolution;

        for (int i = 0; i < _resolution; i++)
        {
            float t = (i / (float)_resolution) * Mathf.PI * 2f;
            Vector3 pos = transform.position + OrbitPoint(t, _radius);
            _lr.SetPosition(i, pos);
        }
    }

    Vector3 OrbitPoint(float rad, float radius)
    {
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
    }

    #endregion

    #region Orbiter 
    public void UpdateOrbiters()
    {
        int targetCount = _playerData.GetOrbitersPerOrbit();
        if (_orbiters.Count < targetCount)
        {
            int orbitersNeeded = targetCount - _orbiters.Count;
            for (int i = 0; i < orbitersNeeded; i++)
                AddOrbiter();
        }

        // Move this OUTSIDE the spawn block so it always runs
        Vector3 size = _playerData.GetOrbiterSize();
        for (int i = 0; i < _orbiters.Count; i++)
            _orbiters[i].transform.localScale = size;

        ArrangeOrbiters();
    }

    private void AddOrbiter()
    {
        GameObject obj = Instantiate(_orbiterPrefab, transform);
        Orbiter orbiter = obj.GetComponent<Orbiter>();

        _orbiters.Add(orbiter);
        orbiter.Initialize(_playerData);

        _orbitData.OrbiterAdded(orbiter);
    }

    private void ArrangeOrbiters()
    {
        int count = _orbiters.Count;
        if (count == 0) return;

        float angleStep = Mathf.PI * 2f / count;

        float baseAngle = _orbitData.GetOrbitAngle();

        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle + (i * angleStep);

            Vector3 localPos = OrbitPoint(angle, _radius);
            _orbiters[i].transform.localPosition = localPos;
        }

        #endregion
    }
}
