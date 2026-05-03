using UnityEngine;

public class SaturnDebri : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    private float _damage;


    public void Initialize(float damage, Color color)
    {
        _damage = damage;
        _sr.color = color;

        PlayerManager.Instance.OnPlayerDied += PlayerDied;

    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDied -= PlayerDied;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyHP>(out var enemy))
            enemy.Damage(_damage);
    }

    private void PlayerDied()
    {
        Destroy(gameObject);
    }
}