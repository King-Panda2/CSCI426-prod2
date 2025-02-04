using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float leftEdge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x -2f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * GameManager.Instance.gameSpeed * Time.deltaTime;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
