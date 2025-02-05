using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float gameSpeed { get; private set; }
    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    public Button againButton;

    public AudioManager audioManager; // Reference to AudioManager
    public ParticleSystem[] stageEffects; // Effects for each stage

    private float score;
    private float stageTimer;
    private int currentStage;

    private Player player;
    private Spawner spawner;
    private Camera mainCamera; // Reference to the main camera

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();
        mainCamera = Camera.main; // Get main camera

        NewGame();
    }

    public void NewGame()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();

        foreach (Obstacle obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        score = 0f;
        gameSpeed = initialGameSpeed;
        enabled = true;

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        againButton.gameObject.SetActive(false);

        // Reset stage and timer
        currentStage = 0;
        stageTimer = 0f;
        UpdateStage();

        UpdateHiscore();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Scene1");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        // Update game speed and score
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");

        // Update stage timer
        stageTimer += Time.deltaTime;
        if (stageTimer >= 30f) // Change stage every 30 seconds
        {
            stageTimer = 0f;
            currentStage = (currentStage + 1) % 4; // Cycle through 4 stages
            UpdateStage();
        }
    }

    private void UpdateStage()
    {
        // Define colors for each stage
        Color[] stageColors = { Color.blue, Color.green, Color.yellow, Color.red};
        Color[] playerColors = { Color.red, Color.blue, Color.green, Color.yellow };

        if (audioManager != null)
        {
            switch (currentStage)
            {
                case 0:
                    audioManager.PlayMusic(audioManager.background1);
                    break;
                case 1:
                    audioManager.PlayMusic(audioManager.background2);
                    break;
                case 2:
                    audioManager.PlayMusic(audioManager.background3);
                    break;
                case 3:
                    audioManager.PlayMusic(audioManager.background4);
                    break;
            }
        }

        // Change player color
        if (player != null)
        {
            player.ChangeColor(playerColors[currentStage]);
        }

        // Change background color
        if (mainCamera != null)
        {
            StartCoroutine(FadeBackgroundColor(stageColors[currentStage]));
        }

        Debug.Log($"Stage {currentStage + 1} started! Color changed.");
    }

    private System.Collections.IEnumerator FadeBackgroundColor(Color targetColor)
    {
        float duration = 1.5f; // Fade duration
        float elapsedTime = 0;
        Color startColor = mainCamera.backgroundColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.backgroundColor = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        mainCamera.backgroundColor = targetColor;
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);

        gameOverText.gameObject.SetActive(true);
        againButton.gameObject.SetActive(true);

        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.death);
        }

        UpdateHiscore();
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }
}
