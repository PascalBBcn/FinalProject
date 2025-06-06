using UnityEngine;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;

    void Awake()
    {
        int numOfGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numOfGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 0)
        {
            TakeLife();
        }
        else KillPlayer();

    }

    void TakeLife()
    {
        playerLives--;
    } 
    
    void KillPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }
}
