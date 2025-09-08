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
        float randomValue = Random.value;
        switch (floor)
        {
            case 1:
                if (randomValue <= 0.7f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 1).ToList(); // 70%
                else if (randomValue <= 0.95f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 2).ToList(); // 20%
                else possibleWeapons = weaponDB.weapons.Where(w => w.rarity >= 3 && w.rarity <= 4).ToList(); // 5%
                break;
            case 2:
                if (randomValue <= 0.7f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 2).ToList();
                else if (randomValue <= 0.92f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 3).ToList();
                else possibleWeapons = weaponDB.weapons.Where(w => w.rarity >= 4).ToList();
                break;
            default:
                if (randomValue <= 0.7f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 3).ToList();
                else if (randomValue <= 0.9f) possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 4).ToList();
                else possibleWeapons = weaponDB.weapons.Where(w => w.rarity == 4).ToList();
                break;

        }
        return possibleWeapons[Random.Range(0, possibleWeapons.Count())];
    }
}
