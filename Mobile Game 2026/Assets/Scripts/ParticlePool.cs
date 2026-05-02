using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance;

    private Dictionary<GameObject, Queue<GameObject>> _pools = new();

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        Instance = null;
        _pools.Clear();
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!_pools.ContainsKey(prefab))
            _pools[prefab] = new Queue<GameObject>();

        GameObject obj = _pools[prefab].Count == 0
            ? Instantiate(prefab)
            : _pools[prefab].Dequeue();

        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.SetActive(true);

        var ps = obj.GetComponent<ParticleSystem>();

        if (ps != null)
            StartCoroutine(ReturnWhenDone(prefab, obj, ps));
        else
            Debug.LogWarning("Pooled particle object has no ParticleSystem.");

        return obj;
    }

    private IEnumerator ReturnWhenDone(GameObject prefab, GameObject obj, ParticleSystem ps)
    {
        float waitTime = ps.main.duration + ps.main.startLifetime.constantMax;
        yield return new WaitForSeconds(waitTime);

        obj.SetActive(false);
        _pools[prefab].Enqueue(obj);
    }
}