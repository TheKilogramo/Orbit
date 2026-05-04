using System.Collections.Generic;
using UnityEngine;

public class IceGiantEffect : EffectBase
{
    [Header("Ice Giant Settings")]
    [SerializeField] private float _slowMultiplier;
    [SerializeField] private float _slowDuration;

    private List<Orbiter> _subscribedOrbiters = new();
    private Dictionary<EnemyBase, float> _slowedEnemies = new();

    public override void Initialize()
    {
        // initialize existing orbiters
        foreach (var orbit in OrbitManager.Instance.GetOrbits())
            foreach (var orbiter in orbit.GetOrbiters())
                SubscribeOrbiter(orbiter);

        // subscribe for future orbiters
        OrbitManager.Instance.OnOrbiterAdded += SubscribeOrbiter;
    }

    public override void Disable()
    {
        //clear ice effects
        foreach (var enemy in _slowedEnemies.Keys)
        {
            enemy.ClearEffect(SpecialEffect.IceGiant);
            enemy.OnDied -= OnTrackedEnemyDied;
        }
        _slowedEnemies.Clear();

        //unsubscribe from all orbiter's OnHit events
        foreach (var orbiter in _subscribedOrbiters)
            orbiter.OnHit -= OnOrbiterHit;

        _subscribedOrbiters.Clear();

        OrbitManager.Instance.OnOrbiterAdded -= SubscribeOrbiter;
    }

    private void SubscribeOrbiter(Orbiter orbiter)
    {
        //subscribe to orbiter's OnHit event
        orbiter.OnHit += OnOrbiterHit;
        _subscribedOrbiters.Add(orbiter);

        //show visuals for meteor effect in the orbiter
        orbiter.SetEffect(SpecialEffect.IceGiant);
    }

    private void OnOrbiterHit(EnemyBase enemy)
    {
        if (!_slowedEnemies.ContainsKey(enemy))
            enemy.OnDied += OnTrackedEnemyDied; // only subscribe once

        //reset timer if already on fire, otherwise add
        _slowedEnemies[enemy] = _slowDuration;
        enemy.SetEffect(SpecialEffect.IceGiant);
        enemy.SetExternalSpeedMultiplier(_slowMultiplier);
    }

    private void OnTrackedEnemyDied(EnemyBase enemy)
    {
        enemy.OnDied -= OnTrackedEnemyDied;
        _slowedEnemies.Remove(enemy);
        // no need to ClearEffect since it's dead
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        List<EnemyBase> toRemove = null;
        List<(EnemyBase, float)> toUpdate = new();

        foreach (var kvp in _slowedEnemies)
        {
            float newTime = kvp.Value - Time.deltaTime;

            if (newTime <= 0f)
            {
                kvp.Key.ClearEffect(SpecialEffect.IceGiant);
                kvp.Key.ClearExternalSpeedMultiplier();
                (toRemove ??= new()).Add(kvp.Key);
            }
            else
            {
                toUpdate.Add((kvp.Key, newTime));
            }
        }

        foreach (var (enemy, time) in toUpdate)
            _slowedEnemies[enemy] = time;

        if (toRemove != null)
            foreach (var e in toRemove)
                _slowedEnemies.Remove(e);
    }
}