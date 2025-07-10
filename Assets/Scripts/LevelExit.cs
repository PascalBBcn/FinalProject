using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public BSPDungeonGenerator dungeonGenerator;

    public void SetGenerator(BSPDungeonGenerator generator)
    {
        this.dungeonGenerator = generator;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // To remove any persisting visuals of the laser
            LaserWeapon laserBeam = collision.GetComponentInChildren<LaserWeapon>();
            if (laserBeam != null) laserBeam.StopShooting();

            Debug.Log("Player reached the exit!");
            dungeonGenerator?.StartGeneration();
        }
    }
}
