using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    public static GameSession instance { get; private set; }

    public int currentFloor = -1;
    [SerializeField] float playerHealth = 100f;
    public Image playerHealthBar;
    public GameObject playerHealthBarContainer;

    [SerializeField] private WeaponData startingWeapon;


    public Image bossHealthBar;
    public GameObject bossHealthBarContainer;
    public GameObject gameOverMenu;
    public GameObject winMenu;

    public TextMeshProUGUI floorReachedText;

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
    }

    public void ProcessPlayerDamage(float damage)
    {
        playerHealth -= damage;
        AudioManager.Instance.PlaySFX("PlayerHit");
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
            floorReachedText.text = $"You reached floor {currentFloor}";
            AudioManager.Instance.PlaySFX("PlayerDeath");
            Destroy(player);
            gameOverMenu.SetActive(true);
        }
    }

    public void ResetGame()
    {
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);
        currentFloor = 0;
        playerHealth = 100f;
        playerHealthBar.fillAmount = 1f;

        PlayerController p = FindObjectOfType<PlayerController>();
        if (p != null && startingWeapon != null) p.SetWeapon(startingWeapon);
    }
}
