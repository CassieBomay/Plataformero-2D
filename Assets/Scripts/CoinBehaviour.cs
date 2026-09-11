using System.Security.Cryptography;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float rot_Speed = 150f;
    [SerializeField] private float move_Speed = 1f;

    [SerializeField] private float height = 0.3f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        float heightOffset = Mathf.Abs(Mathf.Sin(Time.time * move_Speed) * height);
        transform.localPosition = new Vector3(startPosition.x, startPosition.y + heightOffset, startPosition.z);

        transform.Rotate(0, -rot_Speed * Time.fixedDeltaTime, 0, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
