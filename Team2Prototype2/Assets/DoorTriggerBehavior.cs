using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponentInParent<DoorBehavior>().CloseDoor();
    }
}
