using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarsTrigger : MonoBehaviour
{
    private EnemySenses _senses;

    private void Awake()
    {
        _senses = transform.parent.GetComponent<EnemySenses>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _senses.ReceiveTriggerAdd(EnemySenses.EntityHearTypes.Player, other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _senses.ReceiveTriggerRemove(EnemySenses.EntityHearTypes.Player, other.gameObject);
        }
    }
}
