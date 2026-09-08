using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    private Collider ThisCollider;
    private Component isEnabled;
    public int currentCheckpoint;
    public Component CheckpointController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ThisCollider = GetComponent<Collider>();
        isEnabled = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
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
