using TMPro;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession instance { get; private set; }

    public int currentFloor = -1;
    public float difficultyMultiplier = 1.0f;
    [SerializeField] int playerHealth = 1000;
    [SerializeField] TextMeshProUGUI healthText;


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

    void Start()
    {
        healthText.text = playerHealth.ToString();
    }

    public void IncreaseFloor()
    {
        currentFloor++;
        difficultyMultiplier += 0.2f;
    }

    public void ProcessPlayerDeath(float damage)
    {
        playerHealth -= Mathf.RoundToInt(damage);
        if (playerHealth <= 0) KillPlayer();
        healthText.text = playerHealth.ToString();
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
