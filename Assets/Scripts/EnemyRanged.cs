using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float fireRate = 3f;
    public float bulletSpeed = 20f;
    public float bulletDamage = 10f;
    public int bulletQuantity = 1;
    public float bulletSpread = 0f;

    private float timeSinceLastShot;
    private Transform player;
    public RectInt room;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        room = GetComponent<EnemyMovement>().roomBounds;

    }

    void Update()
    {
        if(player != null)
        {
            Vector2 playerDirection = player.position - firePoint.position;
            float aimAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, aimAngle - 90f);

            Vector2Int playerPos = new Vector2Int(
                Mathf.FloorToInt(player.position.x),
                Mathf.FloorToInt(player.position.y)
            );

            if(room.Contains(playerPos))
            {
                if (timeSinceLastShot > 1f / fireRate)
                {
                    Shoot();
                    timeSinceLastShot = 0f;
                }
            }
            timeSinceLastShot += Time.deltaTime;
        }
    }   



    private void Shoot()
    {    

        for (int i = 0; i < bulletQuantity; i++)
        {
            GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            Physics2D.IgnoreCollision(bulletObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            BulletEnemy bullet = bulletObject.GetComponent<BulletEnemy>();
            if (bullet != null)
            {
                // Apply bullet spread via rotation
                bulletObject.transform.Rotate(0, 0, Random.Range(-bulletSpread, bulletSpread));
                bullet.damage = bulletDamage;
                bullet.speed = bulletSpeed;
            }
        }
        
    }
}
