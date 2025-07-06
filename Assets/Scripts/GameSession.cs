using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    public static GameSession instance { get; private set; }

    public int currentFloor = -1;
    public float difficultyMultiplier = 1.0f;
    [SerializeField] float playerHealth = 100f;
    public Image playerHealthBar;
    [SerializeField] TextMeshProUGUI healthText;

    public Image bossHealthBar;
    public GameObject bossHealthBarContainer;
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
        playerHealth -= damage;
        playerHealthBar.fillAmount = playerHealth / 100f;
        if (playerHealth <= 0) KillPlayer();
        healthText.text = playerHealth.ToString();
    }

    public void ProcessBossDamage(float currentHealth, float maxHealth)
    {
        bossHealthBar.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0) bossHealthBarContainer.SetActive(false);
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
