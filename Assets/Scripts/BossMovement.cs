using System.Collections;
using UnityEngine;

public class BossMovement : EnemyMovement
{
    [SerializeField] private float lungeSpeed = 25f;
    [SerializeField] private float lungeDuration = 1.0f;
    [SerializeField] private float lungeCooldown = 3f;

    private bool isLunging = false;

    new void Start()  // "new" keyword handles the warning of hiding the inherited member of Start
    {
        base.Start();
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
            yield return new WaitForSeconds(lungeCooldown);

            if (currentPath != null && currentPathIndex < currentPath.Count)
            {
                isLunging = true;

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
