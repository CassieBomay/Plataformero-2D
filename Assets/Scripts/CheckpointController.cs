using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public GameObject player;
    public GameObject Notification;
    public Vector3 playerPosition;
    public List<Transform> checkpoints = new List<Transform>();
    public int currentChck = 0;
    public bool alive = true;

    void Start()
    {

    }

    void Update()
    {
        if (alive == false)
        {
            player.transform.position = checkpoints[currentChck].position;
            alive = true;
        }

    }

    public void TurnOff()
    {
        Notification.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("checkpoint"))
        {
            Debug.Log("You hit a checkpoint!");
            currentChck += 1;
            Notification.SetActive(true);
            Invoke("TurnOff", 1f);
            // hacer un monto de vidas concreto, y solo tomar en cuenta el ultimo checkpoint que tocaste
        }
    }
}
