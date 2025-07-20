using UnityEngine;

[CreateAssetMenu(fileName = "New Laser Weapon", menuName = "Laser Weapon")]
public class LaserWeaponData : WeaponData
{
    public float laserDPS; // Damage Per Second
    public float laserDistance;
    public float laserWidth;
    public Color laserColour = Color.blue;
    public bool autoLaser = false;
    public float autoLaserRate;
}
