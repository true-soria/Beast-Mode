using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEnder : MonoBehaviour
{
    [SerializeField] private Event currentEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (currentEvent.GetEventState == Event.EventState.Active)
            {
                currentEvent.EndEvent();
            }
        }
    }
}
