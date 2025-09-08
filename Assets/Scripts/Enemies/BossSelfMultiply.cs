using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSelfMultiply : EnemyMovement
{
    [SerializeField] private float lungeSpeed = 10f;
    [SerializeField] private float lungeDuration = 0.05f;
    [SerializeField] private float lungeCooldown = 1f;

    private bool isLunging = false;
    public static List<GameObject> activeInstances = new List<GameObject>();
    public int splitCount = 0;
    public GameObject childPrefab;
    public EnemyStats enemyStats;


    private void OnEnable()
    {
        activeInstances.Add(gameObject);
    }
    private void OnDisable()
    {
        activeInstances.Remove(gameObject);
    }

    new void Start()  // "new" keyword handles the warning of hiding the inherited member of Start
    {
        base.Start();
        enemyStats = GetComponent<EnemyStats>();
        StartCoroutine(Lunge());

    }

    protected override void MoveAlongPath()
    {
        if (isLunging) return;
        base.MoveAlongPath(); // Regular enemy movement
    }

    public void SplitOnDeath()
    {
        if (splitCount >= 3) return;
        for (int i = 0; i < 2; i++)
        {
            GameObject child = Instantiate(childPrefab, transform.position, Quaternion.identity);
            BossSelfMultiply childScript = child.GetComponent<BossSelfMultiply>();
            EnemyStats childStats = child.GetComponent<EnemyStats>();

            if (childScript != null) childScript.splitCount = splitCount + 1;
            child.transform.localScale *= 0.8f;

            if (childStats != null)
            {
                childStats.OverrideMaxHealth(enemyStats.MaxHealth * 0.7f);
                childStats.OverrideDamage(enemyStats.Damage * 0.65f);
            }
        }
    }

    IEnumerator Lunge()
    {
        while (true)
        {
            yield return new WaitForSeconds(lungeCooldown);

            if (currentPath != null && currentPathIndex < currentPath.Count)
            {
                isLunging = true;

                if (playerTransform == null)
                {
                    isLunging = false;
                    continue;
                }
                Vector3 targetPos = playerTransform.position;
                Vector3 direction = (targetPos - transform.position).normalized;

                float timeElapsed = 0f;
                while (timeElapsed < lungeDuration)
                {
                    rb.MovePosition(transform.position + direction * lungeSpeed * Time.fixedDeltaTime);
                    timeElapsed += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }

                isLunging = false;
            }

        }
    }
}
