using System;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public string weaponName;
    public float fireRate;
    public Sprite weaponSprite;
    [Range(0, 5)] public int rarity;
}
