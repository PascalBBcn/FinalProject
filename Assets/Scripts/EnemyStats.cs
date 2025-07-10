using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public static event Action OnBossDeath;
    [SerializeField] private EnemyData enemyData;
    public float currentHealth;

    public float MoveSpeed => enemyData.moveSpeed;
    public float Damage => enemyData.damage;
    public float AttackRate => enemyData.attackRate;
    public bool IsBoss => enemyData.enemyType == EnemyData.EnemyType.Boss;
    public float MaxHealth => enemyData.maxHealth;
    
    private void Start()
    {
        currentHealth = enemyData.maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (IsBoss) GameSession.instance?.ProcessBossDamage(currentHealth, enemyData.maxHealth);
        if (currentHealth <= 0) Die();
    }
    private void Die()
    {
        if(IsBoss) OnBossDeath?.Invoke();
        Destroy(gameObject);
    }
}
