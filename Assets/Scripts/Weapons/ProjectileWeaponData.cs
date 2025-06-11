using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Weapon", menuName = "Projectile Weapon")]
public class ProjectileWeaponData : WeaponData
{
    public GameObject bulletPrefab;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletSpread;
    public int bulletQuantity;
}
