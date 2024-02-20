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
    #region Variables
    private PlayerMovementBehavior pmb;
    private MovingPlatformBehavior mpb;
    private DoorBehavior db;
    private bool pressed;

    private enum LinkedType
    {
        NONE, MOVING_PLATFORM, DOOR, BUTTONS
    }

    private enum LinkedState
    {
        NONE, CLOSED_DOOR, OPEN_DOOR, STOPPED_PLATFORM, MOVING_PLATFORM
    }

    [Header("Button Information")]
    [Tooltip("How long the button's effect lasts for")]
    [SerializeField] private float _activeTime;
    [Tooltip("The default color of this button")]
    [SerializeField] private Material _unpressedColor;
    [Tooltip("The pressed color of this button")]
    [SerializeField] private Material _pressedColor;


    [Header("Linked Objects"), Tooltip("The type of object this button is linked to")]
    [SerializeField] private LinkedType _objectType; 
    [Tooltip("The related object. Leave blank if related to buttons")]
    [SerializeField] private GameObject _linkObject;
    [Tooltip("Leave blank if related to buttons.")]
    [SerializeField] private LinkedState _defaultState;
    [Tooltip("Leave blank if related to buttons.")]
    [SerializeField] private LinkedState _pressedState;
    [Tooltip("All other related buttons. Leave blank if related to platforms or doors")]
    [SerializeField] private GameObject[] _buttons;
    [Tooltip("The order this button should be pressed in. Leave blank if related to platforms or doors")]
    [SerializeField] private int _order;

    public int ButtonOrder { get => _order;}

    #endregion

    #region Functions

    /// <summary>
    /// Start is called on the first frame update. Initializes color and gets reference to player
    /// </summary>
    private void Start()
    {
        pmb = FindObjectOfType<PlayerMovementBehavior>();
        GetComponent<MeshRenderer>().material = _unpressedColor;

        InitializeLinkedState();
    }

    /// <summary>
    /// Occurs when a trigger stays in contact with this object. Checks if button was pressed
    /// </summary>
    /// <param name="other">The object collided with</param>
    private void OnTriggerStay(Collider other)
    {
        //If not pressed and the player is interacting
        if(!pressed && other.gameObject.GetComponent<PlayerMovementBehavior>()!=null && pmb.Interacting)
        {
            Interact();
            StartCoroutine(Pressed());
        }
    }

    /// <summary>
    /// Called when the button is pressed. Sets color and pressed bool, waits, then resets them
    /// </summary>
    /// <returns>The time before the button resets</returns>
    IEnumerator Pressed()
    {
        GetComponent<MeshRenderer>().material = _pressedColor;
        pressed = true;
        yield return new WaitForSeconds(_activeTime);
        pressed = false;
        GetComponent<MeshRenderer>().material = _unpressedColor;
    }

    /// <summary>
    /// Handles button interactions. Takes the related type and the pressed action, then calls needed functions
    /// Throws exceptions if related object does not have required behavior
    /// </summary>
    private void Interact()
    {
        //Handles button interactions for moving platforms
        if(_objectType == LinkedType.MOVING_PLATFORM)
        {
            MovingPlatformBehavior mpb = _linkObject.GetComponent<MovingPlatformBehavior>();
            if(mpb == null)
            {
                throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
            }
            mpb.StopMoving = !mpb.StopMoving;
        }
        //Handles button interactions for doors
        else if (_objectType == LinkedType.DOOR)
        {
            DoorBehavior db = _linkObject.GetComponent<DoorBehavior>();
            if (db == null)
            {
                throw new System.Exception("Door Behavior could not be found on the linked object");
            }
            if(_pressedState == LinkedState.OPEN_DOOR)
            {
                db.OpenDoor();
            }
            else
            {
                db.CloseDoor();
            }

        }
        //Handles button interactions for buttons
        else if (_objectType == LinkedType.BUTTONS)
        {
            //do button stuff
            print("Do button stuff");
        }


    }

    /// <summary>
    /// Initializes the state of the linked object
    /// </summary>
    private void InitializeLinkedState()
    {
        if (_objectType == LinkedType.MOVING_PLATFORM)
        {
            mpb = _linkObject.GetComponentInChildren<MovingPlatformBehavior>();
            if (mpb == null)
            {
                throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
            }
            if(_defaultState == LinkedState.STOPPED_PLATFORM)
            {
                mpb.StopMoving = true;
            }
            else
            {
                mpb.StopMoving = false;
            }
        }
        else if (_objectType == LinkedType.DOOR)
        {
            DoorBehavior db = _linkObject.GetComponent<DoorBehavior>();
            if (db == null)
            {
                throw new System.Exception("Door Behavior could not be found on the linked object");
            }
            if (_defaultState == LinkedState.OPEN_DOOR)
            {
                db.OpenInitialDoor();
            }
        }
        else if (_objectType == LinkedType.BUTTONS)
        {
            print("initialize buttons");
            //do something here once button puzzles exist
        }
    }

    /// <summary>
    /// Draws a gizmo to visually link buttons with their associated object in scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if(_objectType == LinkedType.MOVING_PLATFORM)
        {

            mpb = _linkObject.GetComponentInChildren<MovingPlatformBehavior>();
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _linkObject.transform.position);
        }
        if (_objectType == LinkedType.DOOR)
        {
            db = _linkObject.GetComponent<DoorBehavior>();
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, db.gameObject.transform.GetChild(0).position);
        }

    }

    #endregion
}
