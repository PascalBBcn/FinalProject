using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 20;
    public Rigidbody2D rb;
    public InputAction playerControls;
    Vector2 direction = Vector2.zero;
    BoxCollider2D playerCollider;

    Vector2 mousePosition;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    private BSPDungeonGenerator dungeonGenerator;


    public StatModifier currentWeapon;
    private bool isFiring = false;
    // To control the fire rate (otherwise will shoot every frame)
    private float timeSinceLastShot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        dungeonGenerator = FindObjectOfType<BSPDungeonGenerator>();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        direction = playerControls.ReadValue<Vector2>();
        rb.velocity = new Vector2(direction.x * movementSpeed, direction.y * movementSpeed);
        Die();
        CheckIfEnteredRoom();

        if (Input.GetMouseButtonDown(0))
        {
            isFiring = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isFiring = false;
        }
        if (isFiring)
        {
            if (timeSinceLastShot > 1 / currentWeapon.fireRateMultiplier)
            {
                timeSinceLastShot = 0;
                Shoot();
            }
            timeSinceLastShot += Time.deltaTime;
        }
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;
    }
    public void Shoot()
    {
        for (int i = 0; i < currentWeapon.bulletQuantityModifier; i++)
        {
            GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Physics2D.IgnoreCollision(bulletObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            Bullet bullet = bulletObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bulletObject.transform.Rotate(0, 0, Random.Range(-currentWeapon.bulletSpreadModifier, currentWeapon.bulletSpreadModifier));
                bullet.damage = currentWeapon.damageMultiplier;
                bullet.speed = currentWeapon.bulletSpeedMultiplier;
            }
        }
    }


    void Die()
    {
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
    
    void CheckIfEnteredRoom()
    {
        if (dungeonGenerator == null) return;
        Vector2Int playerPos = Vector2Int.FloorToInt(transform.position);
        for (int i = 1; i < dungeonGenerator.rooms.Count - 1; i++)
        {
            RectInt roomTriggerZone = new RectInt(
                dungeonGenerator.rooms[i].x + 1,
                dungeonGenerator.rooms[i].y + 1,
                dungeonGenerator.rooms[i].width - 2,
                dungeonGenerator.rooms[i].height - 2
            );

            if (roomTriggerZone.Contains(playerPos))
            {
                dungeonGenerator.LockRoom(dungeonGenerator.rooms[i]);
                // Player is in a room.
                Debug.Log("IN ROOM");
                return;
            }
        }
    }
}


