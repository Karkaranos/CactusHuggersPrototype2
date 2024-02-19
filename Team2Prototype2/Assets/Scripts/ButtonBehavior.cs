/*****************************************************************************
// File Name :         ButtonBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     February 18. 2024
//
// Brief Description : Handles basic player movement and player controls
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    private enum LinkedType
    {
        MOVING_PLATFORM, DOOR, BUTTONS
    }

    [Header("Linked Object")]
    [Tooltip("The type of object this button is linked to")]
    [SerializeField] private LinkedType objectType; 
    [SerializeField] private GameObject linkObject;
    [Tooltip("Choose from ClosedDoor, OpenDoor, StoppedPlatform, MovingPlatform")]
    [SerializeField] private string defaultState;
    [Tooltip("Choose from ClosedDoor, OpenDoor, StoppedPlatform, MovingPlatform")]
    [SerializeField] private string pressedState;

}
