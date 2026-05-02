using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    [Header("Damage")]
    public float _damageInterval = 0.25f; // delay between ticks
    private Dictionary<EnemyHP, float> _timers = new();

    private PlayerManager _playerData;

    private bool _initialized;

    public void Initialize(PlayerManager playerData)
    {
        _initialized = true;
        _playerData = playerData;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_initialized) return;

        if (collision.TryGetComponent<EnemyHP>(out var enemyHp))
        {
            //deal instant hit
            enemyHp.Damage(_playerData.GetDamage());

            //start timer at ZERO so next hit is in damageInterval seconds
            _timers[enemyHp] = 0f;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_initialized) return;

        if (!collision.TryGetComponent<EnemyHP>(out var enemyHp))
            return;

        //ensure the enemy exists in timer dictionary
        if (!_timers.ContainsKey(enemyHp))
            _timers[enemyHp] = 0f;

        //count time
        _timers[enemyHp] += Time.deltaTime;

        //tick damage
        if (_timers[enemyHp] >= _damageInterval)
        {
            enemyHp.Damage(_playerData.GetDamage());
            _timers[enemyHp] = 0f;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyHP>(out var enemyHp))
        {
            _timers.Remove(enemyHp);
        }
    }
}
