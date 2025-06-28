using System.Collections;
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

    private SpriteRenderer spriteRenderer;
    public Color playerColour;
    private WeaponInterface currentWeapon;

    private BSPDungeonGenerator dungeonGenerator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        dungeonGenerator = FindObjectOfType<BSPDungeonGenerator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerColour = spriteRenderer.color;
        // Needed otherwise it selects the first weaponScript (no matter if unchecked)
        var weapons = GetComponentsInChildren<WeaponInterface>();
        foreach (var weapon in weapons)
        {
            if (((MonoBehaviour)weapon).enabled)
            {
                currentWeapon = weapon;
                break;
            }
        }
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
        CheckIfEnteredRoom();

        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon?.StartShooting();
        }
        if (Input.GetMouseButtonUp(0))
        {
            currentWeapon?.StopShooting();
        }
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;
    }

    public void ApplyHitVisual()
    {
        StartCoroutine(HitVisual());
    }

    private IEnumerator HitVisual()
    {
        
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = playerColour;
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

            if (roomTriggerZone.Contains(playerPos)) // Player is in a room.
            {
                dungeonGenerator.LockRoom(dungeonGenerator.rooms[i]);
                return;
            }
        }
    }

    public void SetWeapon(WeaponData newWeapon)
    {
        // Get the weapon scripts attached to the player
        var weapons = GetComponentsInChildren<WeaponInterface>(true);
        foreach (var weapon in weapons) ((MonoBehaviour)weapon).enabled = false;
        
        if (newWeapon is ProjectileWeaponData)
        {
            ProjectileWeapon projectileWeapon = GetComponentInChildren<ProjectileWeapon>(true);
            if (projectileWeapon != null)
            {
                projectileWeapon.enabled = true;
                projectileWeapon.weaponData = newWeapon as ProjectileWeaponData;
                currentWeapon = projectileWeapon;
            }
        }
        else if (newWeapon is LaserWeaponData)
        {
            LaserWeapon laserWeapon = GetComponentInChildren<LaserWeapon>(true);
            if (laserWeapon != null)
            {
                laserWeapon.enabled = true;
                laserWeapon.weaponData = newWeapon as LaserWeaponData;
                currentWeapon = laserWeapon;
            }
        }
    }

}


