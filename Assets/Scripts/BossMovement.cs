using System.Collections;
using UnityEngine;

public class BossMovement : EnemyMovement
{
    [SerializeField] private float lungeSpeed = 25f;
    [SerializeField] private float lungeDuration = 1.0f;
    [SerializeField] private float lungeCooldown = 2f;

    private bool isLunging = false;

    private EnemyStats enemyStats;

    new void Start()  // "new" keyword handles the warning of hiding the inherited member of Start
    {
        base.Start();
        enemyStats = GetComponent<EnemyStats>();
        StartCoroutine(Lunge());
    }

    // The boss can move like regular enemies, but also has a lunge movement
    protected override void MoveAlongPath()
    {
        if (isLunging) return;
        base.MoveAlongPath(); // When not lunging, use regular enemy movement
    }

    IEnumerator Lunge()
    {
        while (true)
        {
            float maxHealth = enemyStats.MaxHealth;
            float currentHealth = enemyStats.currentHealth;

            if (currentHealth / maxHealth <= 0.3f)
            {
                lungeDuration = Random.Range(0.4f, 0.9f);
                lungeCooldown = 0.1f;
                lungeSpeed = 40f;
            }
            else if (currentHealth / maxHealth <= 0.5f)
            {
                lungeCooldown = 1f;
                lungeSpeed = 35f;
            }
            else if (currentHealth / maxHealth <= 0.75f)
            {
                lungeCooldown = 1.5f;
                lungeSpeed = 30f;
            }

            yield return new WaitForSeconds(lungeCooldown);

            if (currentPath != null && currentPathIndex < currentPath.Count)
            {
                isLunging = true;

                // Enlarge sprite
                if(lungeCooldown >= 1.5f) yield return StartCoroutine(ScaleSprite(Vector3.one * 1.5f, 0.4f));
                else if(lungeCooldown >= 1f) yield return StartCoroutine(ScaleSprite(Vector3.one * 1.4f, 0.25f));
                else yield return StartCoroutine(ScaleSprite(Vector3.one * 1.3f, 0.1f));

                Vector3 targetPos = playerTransform.position;
                Vector3 direction = (targetPos - transform.position).normalized;

                float timeElapsed = 0f;
                while (timeElapsed < lungeDuration)
                {
                    rb.MovePosition(transform.position + direction * lungeSpeed * Time.fixedDeltaTime);
                    timeElapsed += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }

                // Shrink back sprite
                if(lungeCooldown >= 1.5f) yield return StartCoroutine(ScaleSprite(Vector3.one, 0.2f));
                else if(lungeCooldown >= 1f) yield return StartCoroutine(ScaleSprite(Vector3.one, 0.1f));
                else yield return StartCoroutine(ScaleSprite(Vector3.one, 0.05f));
                isLunging = false;
            }

        }
    }

    private IEnumerator ScaleSprite(Vector3 targetSize, float duration)
    {
        Vector3 initialSize = transform.localScale;
        float time = 0f;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(initialSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
