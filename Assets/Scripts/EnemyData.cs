using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;
    public float damage;
    public float attackRate;
}
