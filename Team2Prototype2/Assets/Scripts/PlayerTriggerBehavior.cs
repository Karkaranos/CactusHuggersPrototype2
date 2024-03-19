/*****************************************************************************
// File Name :         PlayerTriggerBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     March 3. 2024
//
// Brief Description : Handles the player trigger
*****************************************************************************/
using UnityEngine;

public class PlayerTriggerBehavior : MonoBehaviour
{
    /// <summary>
    /// Triggers a bool letting the player know they are triggered
    /// </summary>
    /// <param name="other">The collider entered</param>
    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<PlayerMovementBehavior>().TriggerEntered(other.gameObject);
    }
}
