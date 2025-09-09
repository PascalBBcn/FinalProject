using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;
    public GameObject pickupTextPrefab;

    public void InitializeWeapon(WeaponData data)
    {
        weaponData = data;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                AudioManager.Instance.PlaySFX("Pickup");
                player.SetWeapon(weaponData);

                // SPAWN WEAPON NAME
                if (pickupTextPrefab != null)
                {
                    Vector3 pos = transform.position + Vector3.up;
                    pos.z = 0f;
                    GameObject text = Instantiate(pickupTextPrefab, pos, Quaternion.identity);
                    PickupText pickupText = text.GetComponent<PickupText>();
                    pickupText.Setup(weaponData.weaponName);
                }

                Destroy(gameObject);
            }
        }
    }

}
