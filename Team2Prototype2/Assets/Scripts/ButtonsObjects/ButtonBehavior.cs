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
    LineRenderer l = null;
    GameObject[] walls;


    [Header("Button Information")]
    [Tooltip("How long the button's effect lasts for")]
    [SerializeField] private float _activeTime;
    [Tooltip("The wall this button is on")]
    [SerializeField] private GameObject _wallReference;

    [Header("Linked Objects")]
    [SerializeField] private Interactables[] allInteractables;

    [Header("Button Press Animation")]
    private float _initialButtonXPos;
    [Tooltip("How far the button moves in when pressed"), Range(0, 3.8f)]
    [SerializeField] private float _buttonPressDistance = 3.8f;
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


        walls = GameObject.FindGameObjectsWithTag("Wall");

        InitializeLinkedState();

        GenerateWirePositions();
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


    private void GenerateWirePositions()
    {
        Vector3[] wirePositions = new Vector3[10];
        foreach (Interactables i in allInteractables)
        {
            Transform parent = transform.parent;
            Vector3 objPos = i.WireBox.transform.position;
            wirePositions[0] = transform.position;
            wirePositions[1] = new Vector3(wirePositions[0].x, .005f, wirePositions[0].z);
            int valueCount = 2;
            Vector3 buildMe = wirePositions[1];

            if (Mathf.Abs(90 - (parent.eulerAngles.y % 180)) < 1f)
            {
                if (parent.position.x - objPos.x < 0)
                {
                    print("needs to move right");
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount-1], -1, 'z');
                    valueCount++;
                    print(wirePositions[valueCount - 1]);
                }
                else
                {
                    print("Needs to move left");
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount-1], 1, 'z');
                    valueCount++;

                    print(wirePositions[valueCount - 1]);
                }
            }
            else if (Mathf.Abs(parent.eulerAngles.y % 180) < 1f)
            {
                if (parent.position.x - objPos.x < 0)
                {
                    print("needs to move forward");
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], 1, 'x');
                    valueCount++;
                    print(wirePositions[valueCount - 1]);
                }
                else
                {
                    print("Needs to move back");
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], -1, 'x');
                    valueCount++;

                    print(wirePositions[valueCount - 1]);
                }
            }

            /*
            while ((Mathf.Abs(buildMe.x - objPos.x) > .3f && (Mathf.Abs(buildMe.z - objPos.z) > .3f)))
            {

            }*/


            


            //foreach () ;

            wirePositions[valueCount] = new Vector3(objPos.x, .005f, objPos.z);
            valueCount++;
            wirePositions[valueCount] = objPos;
            valueCount++;

            DrawWires(wirePositions, valueCount);

            /*
            if(transform.parent.transform.rotation.y != 0)
            {
                wirePositions[2] = new Vector3(wirePositions[1].x + (objPos.x - transform.parent.transform.position.x), wirePositions[1].y, wirePositions[1].z);
                wirePositions[3] = new Vector3(objPos.x, wirePositions[1].y, objPos.z);
                wirePositions[4] = objPos;
                print("changed x");

                DrawWires(wirePositions, 5);
            }
            else if (transform.parent.transform.rotation.y == 0)
            {
                wirePositions[2] = new Vector3(wirePositions[1].x, wirePositions[1].y, wirePositions[1].z + (objPos.z - transform.parent.transform.position.z));
                wirePositions[3] = new Vector3(objPos.x, wirePositions[1].y, objPos.z);
                wirePositions[4] = objPos;

                print("changed z");
                DrawWires(wirePositions, 5);
            }
            else
            {
                print("no change in x or z");
                wirePositions[2] = new Vector3(objPos.x, wirePositions[1].y, objPos.z);
                wirePositions[3] = objPos;
                DrawWires(wirePositions, 4);
            }*/

            
        }
    }


    private Vector3 GeneratePoint(Vector3 lastPoint, int direction, char axis)
    {
        GameObject g = null;
        float difference;

        if (direction == 1)
        {
            difference = float.MaxValue;
        }
        else
        {
            difference = float.MinValue;
        }
        
        foreach(GameObject w in walls)
        {
            if(axis == 'x' && direction == 1)
            {
                if((w.transform.position.x > lastPoint.x) && difference > (w.transform.position.x - lastPoint.x))
                {
                    difference = w.transform.position.x - lastPoint.x;
                    g = w;
                }
            }
            else if (axis == 'x' && direction == -1)
            {
                if ((w.transform.position.x < lastPoint.x) && difference < (w.transform.position.x - lastPoint.x))
                {
                    difference = w.transform.position.x - lastPoint.x;
                    g = w;
                }
            }
            else if (axis == 'z' && direction == 1)
            {
                if ((w.transform.position.z > lastPoint.z) && difference > (w.transform.position.z - lastPoint.z))
                {
                    difference = w.transform.position.z - lastPoint.z;
                    g = w;
                }
            }
            else if (axis == 'z' && direction == -1)
            {
                if ((w.transform.position.z < lastPoint.z) && difference < (w.transform.position.z - lastPoint.z))
                {
                    difference = w.transform.position.z - lastPoint.z;
                    g = w;
                }
            }
        }

        print(g.name);

        if(axis == 'z')
        {
            return new Vector3(g.transform.position.x + (.6f * -direction) -.25f, .005f, lastPoint.z);
        }
        else
        {
            return new Vector3(lastPoint.x, .005f, g.transform.position.z + (.6f * -direction));
        }

        /*(lastPoint.x + difference) + (3 * -direction)*/
    }
    private void DrawWires(Vector3[] points, int size)
    {
        print("length of points: " + size);
        if (l == null)
        {
            l = transform.GetChild(0).gameObject.AddComponent<LineRenderer>();
        }

        l.startWidth = .5f;
        l.endWidth = .5f;
        l.alignment = LineAlignment.View;
        l.positionCount = size;
        l.SetPositions(points);
        l.useWorldSpace = true;

    }

    #endregion
}
