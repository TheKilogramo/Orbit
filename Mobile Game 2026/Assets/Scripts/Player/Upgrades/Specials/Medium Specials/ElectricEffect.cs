using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEffect : EffectBase
{
    [Header("Chain Settings")]
    [SerializeField] private int _maxChainCount = 3;
    [SerializeField] private float _chainDamage = 2f;
    [SerializeField] private float _chainRadius = 3f;
    [SerializeField] private float _chainDelay = 0.05f;

    [Header("Visuals")]
    [SerializeField] private LineRenderer _lightningLinePrefab;
    [SerializeField] private float _lineDuration = 0.15f;
    [SerializeField] private float _lineWidth = 0.05f;
    [SerializeField] private Color _lightningColor;

    private List<Orbiter> _subscribedOrbiters = new();
    private List<GameObject> _instantiatedLightings = new();

    public override void Initialize()
    {
        //subscribe to orbiter's OnHitEvent and set the electric effect on them
        foreach (var orbit in OrbitManager.Instance.GetOrbits())
            foreach (var orbiter in orbit.GetOrbiters())
            {
                SubscribeOrbiter(orbiter);
            }

        //update new orbiters
        OrbitManager.Instance.OnOrbiterAdded += SubscribeOrbiter;
    }

    public override void Disable()
    {
        //unsubscribe from all orbiter's OnHit events
        foreach (var orbiter in _subscribedOrbiters)
            orbiter.OnHit -= OnOrbiterHit;

        _subscribedOrbiters.Clear();

        OrbitManager.Instance.OnOrbiterAdded -= SubscribeOrbiter;

        //destroy all lighting objects
        foreach (GameObject go in _instantiatedLightings)
            Destroy(go);

        StopAllCoroutines();
    }

    private void SubscribeOrbiter(Orbiter orbiter)
    {
        //subscribe to orbiter's OnHit event
        orbiter.OnHit += OnOrbiterHit;
        _subscribedOrbiters.Add(orbiter);

        //show visuals for electric effect in the orbiter
        orbiter.SetEffect(SpecialEffect.Electric);
    }

    private void OnOrbiterHit(EnemyBase firstEnemy)
    {
        StartCoroutine(ChainLightning(firstEnemy));
    }

    private IEnumerator ChainLightning(EnemyBase firstEnemy)
    {
        List<EnemyBase> alreadyHit = new() { firstEnemy };
        EnemyBase current = firstEnemy;

        for (int i = 0; i < _maxChainCount; i++)
        {
            //have enemy script show electricity particles / sound
            current.SetEffect(SpecialEffect.Electric);

            EnemyBase next = GetClosestEnemy(current.transform.position, alreadyHit);
            if (next == null) break;

            //deal damage
            next.Damage(_chainDamage);
            alreadyHit.Add(next);

            //draw lightning between current and next
            yield return StartCoroutine(DrawLightning(current.transform.position, next.transform.position));
            yield return new WaitForSeconds(_chainDelay);

            current = next;
        }
    }

    private EnemyBase GetClosestEnemy(Vector3 origin, List<EnemyBase> exclude)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, _chainRadius);
        EnemyBase closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent<EnemyBase>(out var enemy)) continue;
            if (exclude.Contains(enemy)) continue;

            float dist = Vector3.Distance(origin, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    private IEnumerator DrawLightning(Vector3 from, Vector3 to)
    {
        LineRenderer line = Instantiate(_lightningLinePrefab);
        _instantiatedLightings.Add(line.gameObject);

        line.startColor = _lightningColor;
        line.endColor = _lightningColor;
        line.startWidth = _lineWidth;
        line.endWidth = 0f;
        line.useWorldSpace = true;

        // use an animation curve so tips taper to 0
        AnimationCurve widthCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.1f, _lineWidth),
            new Keyframe(0.9f, _lineWidth),
            new Keyframe(1f, 0f)
        );
        line.widthCurve = widthCurve;

        int segments = 8;
        line.positionCount = segments;

        // store base positions along the line
        Vector3[] basePositions = new Vector3[segments];
        Vector3[] offsets = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            basePositions[i] = Vector3.Lerp(from, to, t);
        }

        float elapsed = 0f;
        float wiggleSpeed = 15f;

        while (elapsed < _lineDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < segments; i++)
            {
                // tips dont wiggle, only middle points
                if (i == 0 || i == segments - 1)
                {
                    line.SetPosition(i, basePositions[i]);
                    continue;
                }

                float t = i / (float)(segments - 1);
                // taper wiggle magnitude toward tips
                float magnitude = Mathf.Sin(t * Mathf.PI) * 0.2f;

                offsets[i] = new Vector3(
                    Mathf.Sin(Time.time * wiggleSpeed + i * 1.3f) * magnitude,
                    Mathf.Cos(Time.time * wiggleSpeed + i * 0.9f) * magnitude,
                    0f
                );

                line.SetPosition(i, basePositions[i] + offsets[i]);
            }

            yield return null;
        }

        _instantiatedLightings.Remove(line.gameObject);
        Destroy(line.gameObject);
    }
}