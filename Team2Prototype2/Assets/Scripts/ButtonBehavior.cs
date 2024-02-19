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
    private PlayerMovementBehavior pmb;
    private MovingPlatformBehavior mpb;
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

    private void Start()
    {
        pmb = FindObjectOfType<PlayerMovementBehavior>();
        GetComponent<MeshRenderer>().material = _unpressedColor;

        InitializeLinkedState();
    }

    private void OnTriggerStay(Collider other)
    {
        print("box entered");
        if(!pressed && other.gameObject.GetComponent<PlayerMovementBehavior>()!=null && pmb.Interacting)
        {
            Interact();
            StartCoroutine(Pressed());
        }
    }

    IEnumerator Pressed()
    {
        GetComponent<MeshRenderer>().material = _pressedColor;
        pressed = true;
        yield return new WaitForSeconds(_activeTime);
        pressed = false;
        GetComponent<MeshRenderer>().material = _unpressedColor;
    }

    private void Interact()
    {
        if(_objectType == LinkedType.MOVING_PLATFORM)
        {
            MovingPlatformBehavior mpb = _linkObject.GetComponent<MovingPlatformBehavior>();
            if(mpb == null)
            {
                throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
            }
            mpb.StopMoving = !mpb.StopMoving;
        }
        else if (_objectType == LinkedType.DOOR)
        {
            /*get door behavior reference
            if (doorNameReference == null)
            {
                throw new System.Exception("Door Behavior could not be found on the linked object");
            }
            if(_pressedState == LinkedState.OPEN_DOOR)
            {
                doorNameReference.OpenDoor();
            }
            else
            {
                doorNameReference.CloseDoor();
            }*/
            print("Do door stuff");

        }
        else if (_objectType == LinkedType.BUTTONS)
        {
            //do button stuff
            print("Do button stuff");
        }


    }

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
            //do something here once doors exist
            print("initialize doors");
        }
        else if (_objectType == LinkedType.BUTTONS)
        {
            print("initialize buttons");
            //do something here once button puzzles exist
        }
    }

    private void OnDrawGizmos()
    {
        if(_objectType == LinkedType.MOVING_PLATFORM)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _linkObject.transform.position);
        }
        if (_objectType == LinkedType.DOOR)
        {
            mpb = _linkObject.GetComponentInChildren<MovingPlatformBehavior>();
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, mpb.gameObject.transform.position);
        }

    }

}
