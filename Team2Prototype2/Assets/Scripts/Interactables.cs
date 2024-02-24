using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Interactables
{

    public enum LinkedType
    {
        NONE, MOVING_PLATFORM, DOOR
    }

    public enum LinkedState
    {
        NONE, CLOSED_DOOR, OPEN_DOOR, STOPPED_PLATFORM, MOVING_PLATFORM
    }


    [Tooltip("The type of object this button is linked to")]
    [SerializeField] private LinkedType _objectType;
    [Tooltip("The related object. Leave blank if related to buttons")]
    [SerializeField] private GameObject _linkObject;
    [Tooltip("Leave blank if related to buttons.")]
    [SerializeField] private LinkedState _defaultState;
    [Tooltip("Leave blank if related to buttons.")]
    [SerializeField] private LinkedState _pressedState;
    [Tooltip("True if it resets to its default state when the button stops being pressed.")]
    [SerializeField] private bool _resetsWhenNotPressed;

    public LinkedType ObjectType { get => _objectType; set => _objectType = value; }
    public GameObject LinkObject { get => _linkObject; set => _linkObject = value; }
    public LinkedState DefaultState { get => _defaultState; set => _defaultState = value; }
    public LinkedState PressedState { get => _pressedState; set => _pressedState = value; }
    public bool ResetsWhenNotPressed { get => _resetsWhenNotPressed; set => _resetsWhenNotPressed = value; }
}
