using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    private Collider ThisCollider;
    private Component isEnabled;
    public int currentCheckpoint;
    public Component CheckpointController;

    void Start()
    {
        ThisCollider = GetComponent<Collider>();
        isEnabled = GetComponent<SphereCollider>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Toggles the state (true becomes false, false becomes true)
            ThisCollider.isTrigger = !ThisCollider.isTrigger;
            gameObject.SetActive(false);
        }
    }
}
