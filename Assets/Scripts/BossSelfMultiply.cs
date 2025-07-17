using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossSelfMultiply : EnemyMovement
{
    private int splitCount = 0;
    public GameObject childPrefab;
    public EnemyStats enemyStats;

    new void Start()  // "new" keyword handles the warning of hiding the inherited member of Start
    {
        base.Start();
        enemyStats = GetComponent<EnemyStats>();
    }

    protected override void MoveAlongPath()
    {
        base.MoveAlongPath(); // Regular enemy movement
    }

    public void SplitOnDeath()
    {
        if (splitCount >= 3) return;
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-0.75f, 0.75f), Random.Range(-0.75f, 0.75f), 0);
            GameObject child = Instantiate(childPrefab, transform.position + spawnOffset, Quaternion.identity);
            BossSelfMultiply childScript = child.GetComponent<BossSelfMultiply>();
            EnemyStats childStats = child.GetComponent<EnemyStats>();

            if (childScript != null) childScript.splitCount = splitCount + 1;
            child.transform.localScale *= 0.75f;

            if (childStats != null) childStats.OverrideMaxHealth(enemyStats.MaxHealth * 0.7f);
        }
    }
}
