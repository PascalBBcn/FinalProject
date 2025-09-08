using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    public float damage;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = (Vector2)(transform.up * speed);

        // Explicitly ignore collisions between bullets
        Collider2D firstCollider = GetComponent<Collider2D>();
        BulletEnemy[] activeBullets = FindObjectsOfType<BulletEnemy>();
        for (int i = 0; i < activeBullets.Length; i++)
        {
            BulletEnemy bullet = activeBullets[i];
            if (bullet != this)
            {
                Collider2D secondCollider = bullet.GetComponent<Collider2D>();
                if (secondCollider != null) Physics2D.IgnoreCollision(firstCollider, secondCollider);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().ApplyHitVisual();
            GameSession.instance.ProcessPlayerDeath(damage);
        }
        if (collision.gameObject.layer != LayerMask.NameToLayer("Bullet") && collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }

    }
}
