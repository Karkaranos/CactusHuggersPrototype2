/*****************************************************************************
// File Name :         DoorTriggerBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     February 20. 2024
//
// Brief Description : Sends a pulse when the door trigger is hit
*****************************************************************************/
using UnityEngine;

public class DoorTriggerBehavior : MonoBehaviour
{
    /// <summary>
    /// Closes the door on contact with an object
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponentInParent<DoorBehavior>().CloseDoor();
    }
}
