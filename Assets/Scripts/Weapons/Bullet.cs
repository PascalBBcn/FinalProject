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
