using UnityEngine;

public class ProjectileWeapon : MonoBehaviour, WeaponInterface
{
    public ProjectileWeaponData weaponData;
    public Transform firePoint;
    private float timeSinceLastShot;
    private bool isFiring = false;

    void Update()
    {
        if (isFiring)
        {
            if (timeSinceLastShot > 1f / weaponData.fireRate)
            {
                Shoot();
                timeSinceLastShot = 0f;
            }
            timeSinceLastShot += Time.deltaTime;
        }
    }

    public void StartShooting()
    {
        isFiring = true;
    }
    public void StopShooting()
    {
        isFiring = false;
    }
    private void Shoot()
    {
        if (timeSinceLastShot > 1 / weaponData.fireRate)
        {
            timeSinceLastShot = 0;

            for (int i = 0; i < weaponData.bulletQuantity; i++)
            {
                GameObject bulletObject = Instantiate(weaponData.bulletPrefab, firePoint.position, firePoint.rotation);
                Physics2D.IgnoreCollision(bulletObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                if (bullet != null)
                {
                    // Apply bullet spread via rotation
                    bulletObject.transform.Rotate(0, 0, Random.Range(-weaponData.bulletSpread, weaponData.bulletSpread));
                    bullet.damage = weaponData.bulletDamage;
                    bullet.speed = weaponData.bulletSpeed;
                }
            }
        }
    }
}
