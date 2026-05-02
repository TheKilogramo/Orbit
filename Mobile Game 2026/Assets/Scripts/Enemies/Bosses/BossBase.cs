using UnityEngine;
using System.Collections;
using System;

public abstract class BossBase : EnemyHP
{
    public static event Action OnBossDefeated;

    [Header("Boss Entry Movement")]
    public float _entryMoveDownAmount = 1.5f;
    public float _entryDelay = 1f;
    public float _entrySpeed = 5f; //speed of entry animatino downward movement

    private void Start()
    {
        StartCoroutine(BossEntrySequence());
        HpText.text = ((int)_hp).ToString();
        _canBeDamaged = false;
    }

    public override void Die()
    {
        base.Die();
        OnBossDefeated?.Invoke();
    }

    private IEnumerator BossEntrySequence()
    {
        //determine the final position
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.down * _entryMoveDownAmount;

        //move downward until target reached
        while ((transform.position - targetPos).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _entrySpeed * Time.deltaTime);

            yield return null;
        }

        BossEntryAnim();

        //reached target, now wait
        yield return new WaitForSeconds(_entryDelay);

        //start the real boss behavior
        Initialize();
    }

    public virtual void Initialize() { Initialized = true; _canBeDamaged = true; }
    public virtual void BossEntryAnim() { }
}
