using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                AudioManager.Instance.PlaySFX("Pickup");
                GameSession.instance.ProcessPlayerDeath(-25);
                Destroy(gameObject);
            }
        }
    }
}
