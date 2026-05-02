using TMPro;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [Header("HP")]
    public float _hp;
    public float _maxHp;
    public TextMeshPro HpText;

    protected bool _canBeDamaged = true;

    [HideInInspector] public bool Initialized = false;

    public virtual void Update()
    {
        if (!Initialized) return;

        HpText.text = ((int)_hp).ToString();

        if (_hp <= 0)
        {
            Die();
        }
    }

    public virtual void Damage(float damage)
    {
        if (!_canBeDamaged) return;
        _hp -= damage;
    }

    public virtual void Die() { }
}
