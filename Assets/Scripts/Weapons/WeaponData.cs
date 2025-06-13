using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public string weaponName;
    public float fireRate;
    public Sprite weaponSprite;
    public int rarity;
}
