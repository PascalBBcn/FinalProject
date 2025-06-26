using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    private float currentHealth;

    public float MoveSpeed => enemyData.moveSpeed;
    public float Damage => enemyData.damage;
    public float AttackRate => enemyData.attackRate;

    private void Start()
    {
        currentHealth = enemyData.maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Die();
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
