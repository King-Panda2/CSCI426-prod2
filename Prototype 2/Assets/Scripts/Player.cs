using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;
    private Renderer playerRenderer; // Reference to Renderer

    public float gravity = 9.81f * 2;
    public float jumpForce = 8f;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        playerRenderer = GetComponent<Renderer>();

        // Create a new material instance to avoid shared material issues
        if (playerRenderer != null)
        {
            playerRenderer.material = new Material(playerRenderer.material);
        }
        else
        {
            Debug.LogError("Player Renderer component not found!");
        }
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
    }

    private void Update()
    {
        direction += Vector3.down * gravity * Time.deltaTime;

        if (character.isGrounded)
        {
            direction = Vector3.down;

            if (Input.GetButton("Jump"))
            {
                direction = Vector3.up * jumpForce;
                AudioManager.Instance.PlaySFX(AudioManager.Instance.jump);
            }
        }

        character.Move(direction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ChangeColor(Color backgroundColor)
    {
        if (playerRenderer == null)
        {
            Debug.LogError("Player Renderer is not assigned!");
            return;
        }

        // Calculate complementary color
        Color complementaryColor = new Color(1f - backgroundColor.r, 1f - backgroundColor.g, 1f - backgroundColor.b);

        // Apply the complementary color to the player's material
        playerRenderer.material.color = complementaryColor;
        Debug.Log("Player color changed to: " + complementaryColor);
    }
}