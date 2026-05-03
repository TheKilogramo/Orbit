using System.Collections;
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

    [Header("Effects")]
    [SerializeField] private ParticleSystem _fireParticles;
    private float _fireDamage;
    private bool _onFire = false;
    private Coroutine _fireRoutine;

    public virtual void Update()
    {
        if (!Initialized) return;
        HpText.text = ((int)_hp).ToString();
        if (_hp <= 0)
            Die();

        if (_onFire)
        {
            Damage(_fireDamage * Time.deltaTime, false);
        }
    }

    public virtual void Damage(float damage, bool showDamagedParticles = true)
    {
        if (!_canBeDamaged) return;
        _hp -= damage;
    }

    public void SetOnFire(float duration, float damagePerSecond)
    {
        // refresh duration if already on fire
        if (_fireRoutine != null)
            StopCoroutine(_fireRoutine);

        _fireDamage = damagePerSecond;
        _fireRoutine = StartCoroutine(FireRoutine(duration));
    }

    private IEnumerator FireRoutine(float duration)
    {
        _onFire = true;

        if (_fireParticles != null)
            _fireParticles.Play();

        yield return new WaitForSeconds(duration);

        _onFire = false;

        if (_fireParticles != null)
            _fireParticles.Stop();

        _fireRoutine = null;
    }

    public virtual void Die() { }
}