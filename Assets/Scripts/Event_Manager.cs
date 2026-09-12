using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Manager : MonoBehaviour
{
    public delegate void TrapEvent(GameObject obj);
    public delegate void SimpleEvent();
    public delegate void DamageEvent(int amount);

    public event TrapEvent OnTrapActivated;
    public event SimpleEvent OnVictory;
    public event SimpleEvent OnPlayerDeath;
    public event DamageEvent OnPlayerDamaged;

    private Queue<IEnumerator> eventQueue = new Queue<IEnumerator>();
    private bool processingQueue = false;

    [SerializeField] float activationInterval = 2f;
    [SerializeField] Trap[] traps;

    private Dictionary<string, Action<GameObject>> tagActions;

    private void Awake()
    {
        tagActions = new Dictionary<string, Action<GameObject>>();
        tagActions.Add("Trap", HandleTramp);
        tagActions.Add("Victory", HandleVictory);

    }

    private void Start()
    {
        StartCoroutine(TrapCycle());
    }

    public void ProcessObject(GameObject obj)
    {
        string objectTag = obj.tag;

        if (tagActions.ContainsKey(objectTag))
        {
            tagActions[objectTag](obj);
        }

    }

    public void HandleTramp(GameObject trap)
    {
        eventQueue.Enqueue(ActivateTrap(trap));

        if (!processingQueue)
        {
            StartCoroutine(ProcessQueue());
        }

    }

    private IEnumerator ActivateTrap(GameObject trap)
    {
        // La Queue controla el tiempo de activación.
        yield return new WaitForSeconds(2f);

        if (trap != null)
        {
            OnTrapActivated?.Invoke(trap);

            //if (OnTrapActivated != null)
            //{
            //    OnTrapActivated(trap);
            //}
        }
    }


    public void TriggerDamage(int damage)
    {
        
        if (OnPlayerDamaged != null)
        {
            OnPlayerDamaged(damage);
        }
    }


    private void HandleVictory(GameObject obj)
    {
        
        if (OnVictory != null)
        {
            OnVictory();
        }
    }

    public void TriggerDeath()
    {
        
        if (OnPlayerDeath != null)
        {
            OnPlayerDeath();
        }
    }

    private void EnqueueEvent(IEnumerator eventAction)
    {
        eventQueue.Enqueue(eventAction);
        
        if (!processingQueue)
        {
            StartCoroutine(ProcessQueue());
        }
    }


    private IEnumerator ProcessQueue()
    {
        processingQueue = true;

        while (eventQueue.Count > 0)
        {
            IEnumerator currentEvent = eventQueue.Dequeue();

            yield return StartCoroutine(currentEvent);
        }

        processingQueue = false;
    }

    private IEnumerator TrapCycle()
    {
        int currentTrap = 0;

        while (true)
        {
            if (traps.Length == 0)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(2f);

            HandleTramp(traps[currentTrap].gameObject);

            currentTrap++;

            if (currentTrap >= traps.Length)
            {
                currentTrap = 0;
            }
        }
    }


}
