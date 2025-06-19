using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession instance { get; private set; }

    public int currentFloor = -1;
    public float difficultyMultiplier = 1.0f;
    [SerializeField] int playerHealth = 300;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        Debug.Log(currentFloor);
    }

    public void IncreaseFloor()
    {
        currentFloor++;
        difficultyMultiplier += 0.2f;
    }

    public void ProcessPlayerDeath()
    {
        if (playerHealth > 0)
        {
            TakeLife();
        }
        else KillPlayer();

    }

    void TakeLife()
    {
        playerHealth--;
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
