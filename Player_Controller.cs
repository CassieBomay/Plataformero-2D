using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public GameObject player;
    public int Coins = 0;
    public GameObject Notification;
    public float Timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log("You got a coin!");
            Coins += 1;
            // hacer un monto de vidas concreto, y solo tomar en cuenta el ultimo checkpoint que tocaste
        }
    }
}
