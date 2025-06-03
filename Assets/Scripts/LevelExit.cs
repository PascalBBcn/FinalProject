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
            Debug.Log("Player reached the exit!");
            dungeonGenerator?.StartGeneration();
        }
    }

}
