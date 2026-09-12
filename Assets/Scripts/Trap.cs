using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{

    //[SerializeField] float activationInterval = 2f;
    [SerializeField] float moveDistance = 1f;
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] int damage = 1;


    private Vector3 startPosition;

    private Event_Manager eventManager;

    private bool isMoving = false;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;

        eventManager = FindAnyObjectByType<Event_Manager>();

        if (eventManager != null)
        {
            eventManager.OnTrapActivated += OnTrapActivated;
        }

        //StartCoroutine(TrapCycle());
    }

    //private IEnumerator TrapCycle()
    //{
    //    while (true)
    //    {
            
    //        yield return new WaitForSeconds(activationInterval);


    //        if (eventManager != null)
    //        {
    //            eventManager.HandleTramp(gameObject);
    //        }
    //    }
    //}

    private void OnTrapActivated(GameObject trap)
    {
        // Comprobar que el evento corresponde a esta trampa.
        if (trap == gameObject)
        {
            StartCoroutine(MoveTrap());
        }

    }

    private IEnumerator MoveTrap()
    {
        if (isMoving)
        yield break; 
        
        isMoving = true;

        Vector3 upperPosition = startPosition + Vector3.up * moveDistance;


        while (Vector3.Distance(transform.position,upperPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, upperPosition, moveSpeed * Time.deltaTime);
            
            yield return null;
        }


        transform.position = upperPosition;

        yield return new WaitForSeconds(0.5f);

        
        while (Vector3.Distance(transform.position, startPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = startPosition;

        isMoving = false;




    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (eventManager != null)
            {
                eventManager.TriggerDamage(damage);
            }
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnTrapActivated -= OnTrapActivated;
        }
    }
}
