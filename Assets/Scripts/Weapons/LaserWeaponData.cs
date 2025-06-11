using UnityEngine;

[CreateAssetMenu(fileName = "New Laser Weapon", menuName = "Laser Weapon")]
public class LaserWeaponData : WeaponData
{
    public GameObject bulletPrefab;
    public float laserDPS; //Damage Per Second
    public float laserDistance;
    public Color laserColour = Color.blue;
}
