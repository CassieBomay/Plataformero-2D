using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    [SerializeField] private Collider2D ThisCollider;
    [SerializeField] private Component isEnabled;
    public int currentCheckpoint;
    public Component CheckpointController;

    void Start()
    {
        ThisCollider = GetComponent<Collider2D>();
        isEnabled = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Toggles the state (true becomes false, false becomes true)
            ThisCollider.isTrigger = !ThisCollider.isTrigger;
            gameObject.SetActive(false);
        }
    }
}
