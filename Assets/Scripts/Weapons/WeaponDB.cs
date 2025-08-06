using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Weapon Database")]
public class WeaponDB : ScriptableObject
{
    public List<WeaponData> weapons;
}
