using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Button endlessButton;
    private bool isEndlessMode = false; // Track if endless mode is enabled

    public float gameSpeed { get; private set; }
    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI youWinText;
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
        endlessButton.gameObject.SetActive(false); // Hide the button at the start
        endlessButton.onClick.AddListener(StartEndlessMode); // Assign function to button

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
        youWinText.gameObject.SetActive(false);
        againButton.gameObject.SetActive(false);

        // Reset stage and timer
        currentStage = 0;
        stageTimer = 0f;
        UpdateStage();

        for (int i = 1; i < stageEffects.Length; i++)
        {
            if (stageEffects[i] != null)
            {

                stageEffects[i].Stop(); // Stop effects from other stages

            }
        }
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
        if (stageTimer >= 20f) // Change stage every 30 seconds
        {
            stageTimer = 0f;
            currentStage = (currentStage + 1) % 4; // Cycle through 4 stages
            UpdateStage();
        }

        if (score > 1000 && !isEndlessMode)
        {
            Win();
        }
    }

    private void UpdateStage()
    {
        // Define colors for each stage
        Color[] stageColors = { Color.blue, Color.green, Color.yellow, Color.red };

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


        // Update the background color and player color
        if (mainCamera != null)
        {
            StartCoroutine(FadeBackgroundColor(stageColors[currentStage]));

            // Change player color based on complementary background color
            if (player != null)
            {
                player.ChangeColor(stageColors[currentStage]);
            }
        }

        // Activate the corresponding Particle System and disable others
        for (int i = 0; i < stageEffects.Length; i++)
        {
            if (stageEffects[i] != null)
            {
                if (i == currentStage)
                {
                    stageEffects[i].Play(); // Start effect for this stage
                }
                else
                {
                    //stageEffects[i].Stop(); // Stop effects from other stages
                }
            }
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
            audioManager.PlayMusic(audioManager.winTrack);

        }

        UpdateHiscore();
    }

    public void Win()
    {
        gameSpeed = 0f;
        enabled = false;

        //player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);

        youWinText.gameObject.SetActive(true);
        againButton.gameObject.SetActive(true);
        endlessButton.gameObject.SetActive(true); // Show Endless Mode button

        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.trumpets);
            audioManager.PlaySFX(audioManager.clapping);
            audioManager.PlayMusic(audioManager.winTrack);

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

    public void StartEndlessMode()
    {
        isEndlessMode = true;
        youWinText.gameObject.SetActive(false);
        againButton.gameObject.SetActive(false);
        endlessButton.gameObject.SetActive(false); // Hide the button after starting endless mode
                                                   // Remove obstacles
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>(); // Find all active obstacles
        foreach (Obstacle obstacle in obstacles)
        {
            Destroy(obstacle.gameObject); // Destroy each obstacle
        }

        // Restart the game but keep the player moving
        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameSpeed = initialGameSpeed;
        enabled = true;
    }

}
