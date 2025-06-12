using UnityEngine;

public class Bullet : MonoBehaviour
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
        Bullet[] activeBullets = FindObjectsOfType<Bullet>();
        for (int i = 0; i < activeBullets.Length; i++)
        {
            Bullet bullet = activeBullets[i];
            if (bullet != this)
            {
                Collider2D secondCollider = bullet.GetComponent<Collider2D>();
                if (secondCollider != null) Physics2D.IgnoreCollision(firstCollider, secondCollider);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.layer != LayerMask.NameToLayer("Bullet"))
        {
            Destroy(gameObject);
        }

    }
}
