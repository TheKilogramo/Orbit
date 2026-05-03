using System.Collections.Generic;
using UnityEngine;

public class SaturnEffect : EffectBase
{
    [SerializeField] private bool _debug = true;

    [Header("Debris Settings")]
    [SerializeField] private GameObject _debrisPrefab;
    [SerializeField] private int _debrisPerOrbit = 8;
    [SerializeField] private Vector2 _debrisSizeRange = new Vector2(0.1f, 0.3f);
    [SerializeField] private Vector2 _offsetRange = new Vector2(-0.2f, 0.2f);
    [SerializeField] private float _debrisDamage = 1f;

    [Header("Debris Colors")]
    [SerializeField] private Color[] _debrisColors;

    private List<Vector3> _debrisOffsets = new();
    private List<GameObject> _spawnedDebris = new();
    private int _lastDebrisPerOrbit = -1;
    private int _lastOrbitCount = -1;

    public override void Initialize()
    {
        if (_debug) Debug.Log("Initialized Saturn");
        RefreshDebris();
    }

    public override void OnUpdate()
    {
        if (_debug) Debug.Log("Updated Saturn");

        int currentOrbitCount = OrbitManager.Instance.GetOrbits().Count;

        // respawn if counts changed
        if (_debrisPerOrbit != _lastDebrisPerOrbit || currentOrbitCount != _lastOrbitCount)
            RefreshDebris();

        UpdateDebrisPositions();
    }

    public override void Disable()
    {
        if (_debug) Debug.Log("Disabled Saturn");
        ClearAllDebris();
    }

    private void RefreshDebris()
    {
        ClearAllDebris();
        _debrisOffsets.Clear();

        int orbitCount = OrbitManager.Instance.GetOrbits().Count;

        foreach (var orbit in OrbitManager.Instance.GetOrbits())
        {
            for (int i = 0; i < _debrisPerOrbit; i++)
            {
                float angle = (Mathf.PI * 2f / _debrisPerOrbit) * i;
                Vector3 pos = orbit.GetPointOnOrbit(angle);

                Vector3 offset = new Vector3(
                    Random.Range(_offsetRange.x, _offsetRange.y),
                    Random.Range(_offsetRange.x, _offsetRange.y),
                    0f);

                _debrisOffsets.Add(offset);

                pos += offset;

                GameObject debris = Instantiate(_debrisPrefab, pos, Quaternion.identity);

                float size = Random.Range(_debrisSizeRange.x, _debrisSizeRange.y);
                debris.transform.localScale = Vector3.one * size;
                Color randomColor = _debrisColors.Length > 0 ? _debrisColors[Random.Range(0, _debrisColors.Length)] : Color.white;
                debris.GetComponent<SaturnDebri>().Initialize(_debrisDamage, randomColor);
                _spawnedDebris.Add(debris);


            }
        }

        _lastDebrisPerOrbit = _debrisPerOrbit;
        _lastOrbitCount = orbitCount;
    }

    private void ClearAllDebris()
    {
        foreach (var d in _spawnedDebris)
            Destroy(d);
        _spawnedDebris.Clear();
    }

    private void UpdateDebrisPositions()
    {
        int orbitCount = OrbitManager.Instance.GetOrbits().Count;

        for (int orbitIndex = 0; orbitIndex < orbitCount; orbitIndex++)
        {
            var orbit = OrbitManager.Instance.GetOrbits()[orbitIndex];

            for (int i = 0; i < _debrisPerOrbit; i++)
            {
                int debrisIndex = orbitIndex * _debrisPerOrbit + i;
                if (debrisIndex >= _spawnedDebris.Count) break;

                float angle = (Mathf.PI * 2f / _debrisPerOrbit) * i;
                float finalAngle = angle + OrbitManager.Instance.GetOrbitAngle();
                _spawnedDebris[debrisIndex].transform.position = orbit.GetPointOnOrbit(finalAngle) + _debrisOffsets[debrisIndex];
            }
        }
    }
}