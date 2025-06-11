using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public StatModifier weaponModifier;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetWeapon(weaponModifier);
            Destroy(gameObject); 
        }
    }
}
