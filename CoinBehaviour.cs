using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    private Collider ThisCollider;
    private Component isEnabled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ThisCollider = GetComponent<Collider>();
        isEnabled = GetComponent<CapsuleCollider>();
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
