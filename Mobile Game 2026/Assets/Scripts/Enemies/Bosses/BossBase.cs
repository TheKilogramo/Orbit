using UnityEngine;
using System.Collections;
using System;

public abstract class BossBase : EnemyBase
{
    public static event Action OnBossDefeated;

    [Header("Boss Entry Movement")]
    private float _entryMoveDownAmount = 1.5f;
    private float _entryDelay = 1f;
    private float _entrySpeed = 5f; //speed of entry animation downward movement

    protected override void Start()
    {
        base.Start();
        StartCoroutine(BossEntrySequence());
    }

    protected override void Die()
    {
        //no base.Die() because it's different from regular enemies

        InvokeOnDiedEvent();

        OnBossDefeated?.Invoke(); //call boss defeated event
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

    public virtual void Initialize() { Initialized = true; _canBeDamaged = true; } //different from enemybase
    public virtual void BossEntryAnim() { }
}
