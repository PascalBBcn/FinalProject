using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    public enum EnemyType { Normal, Boss };
    public float maxHealth;
    public float moveSpeed;
    public float damage;
    public float attackRate;
    public EnemyType enemyType;    
}
