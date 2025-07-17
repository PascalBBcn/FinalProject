using System;
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
    public GameObject playerHealthBarContainer;

    public Image bossHealthBar;
    public GameObject bossHealthBarContainer;
    public GameObject gameOverText;

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

    public void ProcessPlayerDeath(float damage)
    {
        playerHealth -= damage;
        playerHealthBar.fillAmount = playerHealth / 100f;
        if (playerHealth <= 0) KillPlayer();
    }

    public void ProcessBossDamage(float currentHealth, float maxHealth)
    {
        bossHealthBar.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            bossHealthBarContainer.SetActive(false);
        }
    }

    void KillPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            gameOverText.SetActive(true);
        }
    }

    public void ResetGame()
    {
        gameOverText.SetActive(false);
        currentFloor = 0;
        difficultyMultiplier = 1.0f;
        playerHealth = 100f;
        playerHealthBar.fillAmount = 1f;
    }
}
