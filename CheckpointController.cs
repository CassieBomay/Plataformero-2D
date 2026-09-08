using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public GameObject player;
    public GameObject checkPoint;
    public GameObject Notification;
    public Vector3 playerPosition;
    public List<Transform> checkpoints = new List<Transform>();
    public int currentChck = 0;
    public int lives = 5;
    public bool alive = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (alive == false)
        {
            player.transform.position = checkpoints[currentChck].position;
            lives -= 1;
            alive = true;
        }

    }

    public void TurnOff()
    {
        Notification.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("checkpoint"))
        {
            Debug.Log("You hit a checkpoint!");
            currentChck += 1;
            Notification.SetActive(true);
            Invoke("TurnOff", 1f);
            // hacer un monto de vidas concreto, y solo tomar en cuenta el ultimo checkpoint que tocaste
        }
    }
}
