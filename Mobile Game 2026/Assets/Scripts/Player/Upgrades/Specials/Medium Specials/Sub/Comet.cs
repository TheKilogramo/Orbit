using UnityEngine;

public class Comet : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeSpan = 5f;

    private float _damage;
    private Vector2 _direction;

    private bool _initialized = false;

    public void Initialize(float damage, Vector2 direction)
    {
        Destroy(gameObject, _lifeSpan);

        _damage = damage;
        _direction = direction;

        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized) return;

        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyBase>(out var enemy))
            enemy.Damage(_damage);
    }
}
