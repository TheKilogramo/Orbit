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
    public SpriteRenderer SpriteRendr;
    [SerializeField] private ParticleSystem _fireParticles;
    [SerializeField] private ParticleSystem _coldParticles;

    private float _fireDamage;
    private bool _onFire = false;
    private Coroutine _fireRoutine;

    protected bool _slowed = false;
    protected float _slowMultiplier;
    private Coroutine _slowRoutine;

    public virtual void Initialize(Vector3 playerPosition, int newHp)
    {
        Initialized = true;

        _slowed = false;
        _onFire = false;

        if (_fireRoutine != null) StopCoroutine(_fireRoutine);
        if(_slowRoutine != null) StopCoroutine(_slowRoutine);

        _fireParticles.Stop();
        _coldParticles.Stop();

        _canBeDamaged = true;
    }

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

    public virtual void Slow(float duration, float slowMultiplier)
    {
        if (_slowRoutine != null) StopCoroutine(_slowRoutine);
        _slowMultiplier = slowMultiplier;
        _slowRoutine = StartCoroutine(SlowRoutine(duration));
    }

    private IEnumerator SlowRoutine(float duration)
    {
        _slowed = true;

        _coldParticles.Play();


        yield return new WaitForSeconds(duration);

        _slowed = false;
        _coldParticles.Stop();

        _slowRoutine = null;
        _slowMultiplier = 0f;
    }


    public virtual void Die() { }
}