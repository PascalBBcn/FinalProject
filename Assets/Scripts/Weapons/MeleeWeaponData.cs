using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Melee Weapon")]
public class MeleeWeaponData : WeaponData
{
    public GameObject bulletPrefab;
    public float damage; 
    public float range;
    public float arc;
}
