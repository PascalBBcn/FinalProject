using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyStats enemyStats;
    private float cooldown;

    private void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        cooldown = 0f;
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && cooldown <= 0f)
        {
            GameSession.instance.ProcessPlayerDeath(enemyStats.Damage);
            cooldown = 1f / enemyStats.AttackRate;
        }
    }

}
