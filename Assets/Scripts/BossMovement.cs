using System.Collections;
using UnityEngine;

public class BossMovement : EnemyMovement
{
    [SerializeField] private float lungeSpeed = 20f;
    [SerializeField] private float lungeDuration = 1.0f;
    [SerializeField] private float chargeDelay = 0.5f;
    private bool isLunging = false;

    protected override void MoveAlongPath()
    {
        if (isLunging || currentPath == null || currentPathIndex >= currentPath.Count)
            return;

        Vector3 targetPos = new Vector3(
            currentPath[currentPathIndex].x,
            currentPath[currentPathIndex].y,
            transform.position.z);

        StartCoroutine(Lunge(targetPos));
    }

    IEnumerator Lunge(Vector3 targetPos)
    {
        isLunging = true;
        float timeElapsed = 0f;
        yield return new WaitForSeconds(chargeDelay);

        Vector3 direction = (targetPos - transform.position).normalized;
        while (timeElapsed < lungeDuration)
        {
            rb.MovePosition(transform.position + direction * lungeSpeed * Time.fixedDeltaTime);
            timeElapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        currentPathIndex++;
        isLunging = false;
    }
}
