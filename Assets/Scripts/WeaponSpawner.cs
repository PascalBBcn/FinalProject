using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private WeaponDB weaponDB;

    public WeaponData GetWeapon()
    {
        int floor = GameSession.instance.currentFloor;
        List<WeaponData> possibleWeapons = new List<WeaponData>();
        switch (floor)
        {
            case 1:
                if (Random.value <= 0.7f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 1).ToList(); // 70%
                else if (Random.value <= 0.95f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 2).ToList(); // 20%
                else possibleWeapons = weaponDB.weapons.Where(w => w.rarity >= 3 && w.rarity <= 5).ToList(); // 5%
                break;
            case 2:
                if (Random.value <= 0.7f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 2).ToList(); 
                else if (Random.value <= 0.92f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 3).ToList(); 
                else possibleWeapons = weaponDB.weapons.Where(w => w.rarity >= 4 && w.rarity <= 5).ToList(); 
                break;
            case 3:
                if (Random.value <= 0.7f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 3).ToList(); 
                else if (Random.value <= 0.9f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 4).ToList(); 
                else possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 5).ToList(); 
                break;
        }
        return possibleWeapons[Random.Range(0, possibleWeapons.Count())];
    }
}
