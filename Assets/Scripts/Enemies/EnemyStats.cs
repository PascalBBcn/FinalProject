using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public static event Action OnBossDeath;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float? overrideMaxHealth = null;
    public float currentHealth;

    public float MoveSpeed => enemyData.moveSpeed;
    public float Damage => enemyData.damage;
    public float AttackRate => enemyData.attackRate;
    public bool IsBoss => enemyData.enemyType == EnemyData.EnemyType.Boss;
    // Overriding the health for the self-multiplying boss if null, be default
    public float MaxHealth => overrideMaxHealth ?? enemyData.maxHealth;

    private void Start()
    {
        currentHealth = overrideMaxHealth ?? enemyData.maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (IsBoss) GameSession.instance?.ProcessBossDamage(currentHealth, enemyData.maxHealth);
        if (currentHealth <= 0) Die();
    }
    private void Die()
    {
        if (IsBoss)
        {
            // If it is the self-multiplying boss, split on its death
            BossSelfMultiply split = GetComponent<BossSelfMultiply>();
            if (split != null)
            {
                split.SplitOnDeath();
                if (BossSelfMultiply.activeInstances.Count == 1) OnBossDeath?.Invoke();

            }
            else
            {
                // Spawn the LevelExit after normal bosses die
                OnBossDeath?.Invoke();
            }
        }

        Destroy(gameObject);
    }
    // Used for the boss that self-multiplies on death
    public void OverrideMaxHealth(float newMax)
    {
        overrideMaxHealth = newMax;
        currentHealth = newMax;
    }
}
