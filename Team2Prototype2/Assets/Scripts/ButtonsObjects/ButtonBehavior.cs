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
    private bool interactedYet;


    [Header("Button Information")]
    [Tooltip("How long the button's effect lasts for")]
    [SerializeField] private float _activeTime;

    [Header("Linked Objects")]
    [SerializeField] private Interactables[] allInteractables;

    [Header("Button Press Animation")]
    private float _initialButtonXPos;
    [Tooltip("How far the button moves in when pressed"), Range(0, 4)]
    [SerializeField] private float _buttonPressDistance;
    [Tooltip("The amount of time the total animation takes")]
    [SerializeField] private float _buttonAnimationTime;

    #endregion

    #region Functions

    /// <summary>
    /// Start is called on the first frame update. Initializes color and gets reference to player
    /// </summary>
    private void Start()
    {
        pmb = FindObjectOfType<PlayerMovementBehavior>();

        InitializeLinkedState();
    }

    /// <summary>
    /// Occurs when a trigger stays in contact with this object. Checks if button was pressed
    /// </summary>
    /// <param name="other">The object collided with</param>
    private void OnTriggerStay(Collider other)
    {
        //If the player isn't interacting, allow this button to be interacted with
        if(!pmb.Interacting)
        {
            interactedYet = false;
        }
        //If not pressed and the player is interacting and has not interacted with the button this time
        if(!pressed && other.gameObject.GetComponent<PlayerMovementBehavior>()!=null && pmb.Interacting && !interactedYet)
        {
            Interact();
            StartCoroutine(Pressed());
            interactedYet = true;
        }
    }

    /// <summary>
    /// Called when the button is pressed. Sets color and pressed bool, waits, then resets them
    /// </summary>
    /// <returns>The time before the button resets</returns>
    IEnumerator Pressed()
    {
        print("started pressed");
        Vector3 pos = transform.localPosition;
        pressed = true;

        float buttonAnimCounter = _buttonAnimationTime / 2;

        while(buttonAnimCounter > 0)
        {
            buttonAnimCounter -= Time.deltaTime;
            pos.x -= _buttonPressDistance /(Screen.width/_buttonPressDistance);
            transform.localPosition = pos;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        print("Switched button direction");
        buttonAnimCounter = _buttonAnimationTime / 2;

        while(buttonAnimCounter > 0)
        {
            buttonAnimCounter -= Time.deltaTime;
            pos.x += _buttonPressDistance / (Screen.width / _buttonPressDistance);
            transform.localPosition = pos;
            yield return new WaitForSeconds(Time.deltaTime);
        }



        pressed = false;

        StopInteraction();

    }

    /// <summary>
    /// Handles stopping interactions on buttons
    /// </summary>
    private void StopInteraction()
    {
        foreach (Interactables i in allInteractables)
        {
            if (i.ResetsWhenNotPressed)
            {
                if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
                {
                    mpb = i.LinkObject.GetComponentInChildren<MovingPlatformBehavior>();
                    if (mpb == null)
                    {
                        throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
                    }
                    if (i.DefaultState == Interactables.LinkedState.STOPPED_PLATFORM)
                    {
                        mpb.StopMoving = true;
                    }
                    else
                    {
                        mpb.StopMoving = false;
                    }
                }
                else if (i.ObjectType == Interactables.LinkedType.DOOR)
                {
                    DoorBehavior db = i.LinkObject.GetComponent<DoorBehavior>();
                    if (db == null)
                    {
                        throw new System.Exception("Door Behavior could not be found on the linked object");
                    }
                    if (i.DefaultState == Interactables.LinkedState.OPEN_DOOR)
                    {
                        db.OpenDoor();
                    }
                    else
                    {
                        db.CloseDoor();
                    }
                }
            }

        }
    }

    /// <summary>
    /// Handles button interactions. Takes the related type and the pressed action, then calls needed functions
    /// Throws exceptions if related object does not have required behavior
    /// </summary>
    private void Interact()
    {
        foreach(Interactables i in allInteractables)
        {
            //Handles button interactions for moving platforms
            if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
            {
                MovingPlatformBehavior mpb = i.LinkObject.GetComponent<MovingPlatformBehavior>();
                if (mpb == null)
                {
                    throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
                }
                mpb.StopMoving = !mpb.StopMoving;
            }
            //Handles button interactions for doors
            else if (i.ObjectType == Interactables.LinkedType.DOOR)
            {
                DoorBehavior db = i.LinkObject.GetComponent<DoorBehavior>();
                if (db == null)
                {
                    throw new System.Exception("Door Behavior could not be found on the linked object");
                }
                if ((i.PressedState == Interactables.LinkedState.OPEN_DOOR && !i.TogglesWhenPressed)|| (i.TogglesWhenPressed && !db.IsOpen))
                {
                    db.OpenDoor();
                }
                else 
                {
                    db.CloseDoor();
                }

            }
        }
        


    }

    /// <summary>
    /// Initializes the state of the linked object
    /// </summary>
    private void InitializeLinkedState()
    {
        foreach(Interactables i in allInteractables)
        {
            if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
            {
                mpb = i.LinkObject.GetComponentInChildren<MovingPlatformBehavior>();
                if (mpb == null)
                {
                    throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
                }
                if (i.DefaultState == Interactables.LinkedState.STOPPED_PLATFORM)
                {
                    mpb.StopMoving = true;
                }
                else
                {
                    mpb.StopMoving = false;
                }
            }
            else if (i.ObjectType == Interactables.LinkedType.DOOR)
            {
                DoorBehavior db = i.LinkObject.GetComponent<DoorBehavior>();
                if (db == null)
                {
                    throw new System.Exception("Door Behavior could not be found on the linked object");
                }
                if (i.DefaultState == Interactables.LinkedState.OPEN_DOOR)
                {
                    db.OpenInitialDoor();
                }
            }
        }
    }

    /// <summary>
    /// Draws a gizmo to visually link buttons with their associated object in scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        foreach(Interactables i in allInteractables)
        {
            if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
            {

                mpb = i.LinkObject.GetComponentInChildren<MovingPlatformBehavior>();
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, i.LinkObject.transform.position);
            }
            if (i.ObjectType == Interactables.LinkedType.DOOR)
            {
                db = i.LinkObject.GetComponent<DoorBehavior>();
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, db.gameObject.transform.GetChild(0).position);
            }
        }

    }

    #endregion
}
