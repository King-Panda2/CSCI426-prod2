using UnityEngine;
using TMPro;

public class Ground : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private float distanceTraveled = 0f;
    [SerializeField] private TMP_Text scoreText; // Assign in Inspector Score Text
    
    void Start()
    {
        UpdateScoreText();
    }
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        float speed = GameManager.Instance.gameSpeed / transform.localScale.x;
        meshRenderer.material.mainTextureOffset += Vector2.right * speed * Time.deltaTime;

        distanceTraveled += speed * Time.deltaTime; //calc distance of player
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = distanceTraveled.ToString("F3");
        }
        else
        {
            Debug.LogWarning("Score Text is not assigned in ScoreManager! Assign it in the Inspector.");
        }
    }
}

