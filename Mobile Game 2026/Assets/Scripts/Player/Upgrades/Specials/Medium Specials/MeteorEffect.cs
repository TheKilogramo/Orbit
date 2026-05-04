using System.Collections.Generic;
using UnityEngine;

public class MeteorEffect : EffectBase
{
    [Header("Meteor Settings")]
    [SerializeField] private float _fireDamagePerSecond;
    [SerializeField] private float _fireDuration;

    private List<Orbiter> _subscribedOrbiters = new();
    private Dictionary<EnemyBase, float> _enemiesOnFire = new();

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
        //clear fire effects
        foreach (var enemy in _enemiesOnFire.Keys)
        {
            enemy.ClearEffect(SpecialEffect.Meteor);
            enemy.OnDied -= OnTrackedEnemyDied;
        }
        _enemiesOnFire.Clear();

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
        orbiter.SetEffect(SpecialEffect.Meteor);
    }

    private void OnOrbiterHit(EnemyBase enemy)
    {
        if (!_enemiesOnFire.ContainsKey(enemy))
            enemy.OnDied += OnTrackedEnemyDied; // only subscribe once

        //reset timer if already on fire, otherwise add
        _enemiesOnFire[enemy] = _fireDuration;
        enemy.SetEffect(SpecialEffect.Meteor);
    }

    private void OnTrackedEnemyDied(EnemyBase enemy)
    {
        enemy.OnDied -= OnTrackedEnemyDied;
        _enemiesOnFire.Remove(enemy);
        // no need to ClearEffect since it's dead
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        List<EnemyBase> toRemove = null;
        List<(EnemyBase, float)> toUpdate = new();

        foreach (var kvp in _enemiesOnFire)
        {
            kvp.Key.Damage(_fireDamagePerSecond * Time.deltaTime, false);
            float newTime = kvp.Value - Time.deltaTime;

            if (newTime <= 0f)
            {
                kvp.Key.ClearEffect(SpecialEffect.Meteor);
                (toRemove ??= new()).Add(kvp.Key);
            }
            else
            {
                toUpdate.Add((kvp.Key, newTime));
            }
        }

        foreach (var (enemy, time) in toUpdate)
            _enemiesOnFire[enemy] = time;

        if (toRemove != null)
            foreach (var e in toRemove)
                _enemiesOnFire.Remove(e);
    }
}