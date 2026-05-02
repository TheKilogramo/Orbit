using System.Collections.Generic;
using UnityEngine;

public class EnemySortManager : MonoBehaviour
{
    public static EnemySortManager Instance;

    private List<BasicEnemy> activeEnemies = new();
    private int nextSpawnIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy(BasicEnemy e)
    {
        if (e == null) return;

        // Assign spawn index so we know creation order
        e.SpawnIndex = nextSpawnIndex++;
        activeEnemies.Add(e);

        UpdateSortingOrders();
    }

    public void UnregisterEnemy(BasicEnemy e)
    {
        if (e == null) return;

        activeEnemies.Remove(e);
        UpdateSortingOrders();
    }

    void UpdateSortingOrders()
    {
        if (activeEnemies.Count == 0) return;

        // Sort by spawn index so earliest spawned is first
        activeEnemies.Sort((a, b) => a.SpawnIndex.CompareTo(b.SpawnIndex));
        // Now index 0 = earliest, last = latest

        int count = activeEnemies.Count;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            BasicEnemy e = activeEnemies[i];

            int order = e.SpawnIndex * 2;

            if (e.SpriteRenderer)
            {
                e.SpriteRenderer.sortingLayerName = "Enemies";
                e.SpriteRenderer.sortingOrder = order;
            }

            if (e.HpText)
            {
                e.SpriteRenderer.sortingLayerName = "Enemies";
                e.HpText.sortingOrder = order + 1;
            }
        }
    }
}
