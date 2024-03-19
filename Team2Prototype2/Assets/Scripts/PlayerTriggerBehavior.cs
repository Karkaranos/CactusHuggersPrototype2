using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<PlayerMovementBehavior>().TriggerEntered(other.gameObject);
    }
}
