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
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
    }
    // Update is called once per frame
    void Update()
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

        character.Move(direction *Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ChangeColor(Color newColor)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = newColor;
        }
    }
}
