using System.Collections;
using UnityEngine;

public class LevelTeleport : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;

    public void SetGenerator(DungeonGenerator generator)
    {
        this.dungeonGenerator = generator;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            FindObjectOfType<Transitions>().StartFadeTransition();
            // To remove any persisting visuals of the laser
            LaserWeapon laserBeam = collision.GetComponentInChildren<LaserWeapon>();
            if (laserBeam != null) laserBeam.StopShooting();

            dungeonGenerator?.TeleportToBossRoom(collision.gameObject);
            StartCoroutine(DelayBossMusic(2f));
        }
    }
    private IEnumerator DelayBossMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.PlayMusic("BossMusic");
    }
}
