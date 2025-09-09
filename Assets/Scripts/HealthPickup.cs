using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public GameObject pickupTextPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                AudioManager.Instance.PlaySFX("Pickup");
                GameSession.instance.ProcessPlayerDamage(-25);

                // SPAWN TEXT
                if (pickupTextPrefab != null)
                {
                    Vector3 pos = transform.position + Vector3.up;
                    pos.z = 0f;
                    GameObject text = Instantiate(pickupTextPrefab, pos, Quaternion.identity);
                    PickupText pickupText = text.GetComponent<PickupText>();
                    pickupText.Setup("+25 health");
                }
                Destroy(gameObject);
            }
        }
    }
}
