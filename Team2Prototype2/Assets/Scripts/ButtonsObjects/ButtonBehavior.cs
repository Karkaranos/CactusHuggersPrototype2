/*****************************************************************************
// File Name :         ButtonBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     February 18. 2024
//
// Brief Description : Handles basic player movement and player controls
*****************************************************************************/
using System.Collections;
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

    [Header("Wire Controls")]
    [SerializeField] private Material _inactiveWire;
    [SerializeField] private Material _activeWire;
    [SerializeField] private Material _inactiveWireBox;
    [SerializeField] private Material _activeWireBox;

    #endregion

    #region Functions

    /// <summary>
    /// Start is called on the first frame update. Initializes color and gets reference to player
    /// </summary>
    private void Start()
    {
        pmb = FindObjectOfType<PlayerMovementBehavior>();


        walls = GameObject.FindGameObjectsWithTag("Wall");

        //Generates wires only in puzzle 1
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("1"))
        {
            GenerateWirePositions();
        }

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
        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.ButtonSound();
        }
        Vector3 pos = transform.localPosition;
        pressed = true;

        float buttonAnimCounter = _buttonAnimationTime / 2;

        //Handles button being pressed in
        while(buttonAnimCounter > 0)
        {
            buttonAnimCounter -= Time.deltaTime;
            pos.x -= _buttonPressDistance /(Screen.width/_buttonPressDistance);
            transform.localPosition = pos;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        buttonAnimCounter = _buttonAnimationTime / 2;

        //Handles button being moved out
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
        //Iterates over each interactable to reset it and set it to its default state
        foreach (Interactables i in allInteractables)
        {
            //If I resets when it isn't pressed
            if (i.ResetsWhenNotPressed)
            {
                //Change the moving platform status
                if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
                {
                    mpb = i.LinkObject.GetComponentInChildren<MovingPlatformBehavior>();
                    if (mpb == null)
                    {
                        throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
                    }
                    if (i.DefaultState == Interactables.LinkedState.STOPPED_PLATFORM)
                    {
                        mpb.ButtonSaysStopMoving = true;
                    }
                    else
                    {
                        mpb.ButtonSaysStopMoving = false;
                    }
                }
                //Change the door status
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
                if(i.PressedState == Interactables.LinkedState.MOVING_PLATFORM && !i.TogglesWhenPressed)
                {
                    mpb.ButtonSaysStopMoving = false;
                }
                else if (i.PressedState == Interactables.LinkedState.STOPPED_PLATFORM && !i.TogglesWhenPressed)
                {
                    mpb.ButtonSaysStopMoving = true;
                }
                else
                {
                    mpb.ButtonSaysStopMoving = !mpb.ButtonSaysStopMoving;
                }

                //Change the state of wires
                if(mpb.ButtonSaysStopMoving)
                {
                    DeactivateWires(i);
                }
                else
                {
                    ActivateWires(i);
                }
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
                    ActivateWires(i);
                }
                else 
                {
                    db.CloseDoor();
                    DeactivateWires(i);
                }

            }

            //Handle other objects this button changes
            if(i.ResetsThese.Length != 0)
            {
                foreach(ChangedObject c in i.ResetsThese)
                {
                    MovingPlatformBehavior mpb = c.LinkedObject.GetComponentInChildren<MovingPlatformBehavior>();
                    mpb.ButtonSaysStopMoving = true;
                    c.LinkedObject.transform.position = mpb.Waypoints[c.WaypointToGoTo-1].transform.position;
                }
            }
        }
        


    }

    /// <summary>
    /// Sets the default state of the given interactable
    /// </summary>
    /// <param name="i">The interactable to set</param>
    public void SetDefaultState(Interactables i)
    {
        //Sets the default state of a moving platform
        if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
        {
            mpb = i.LinkObject.GetComponentInChildren<MovingPlatformBehavior>();
            mpb.RelatedInteractable = i;
            if (mpb == null)
            {
                throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
            }
            //Sets the default state
            if (i.DefaultState == Interactables.LinkedState.STOPPED_PLATFORM)
            {
                mpb.ButtonSaysStopMoving = true;
            }
            else
            {
                mpb.ButtonSaysStopMoving = false;
                ActivateWires(i);
            }
        }
        //Sets the default state of a door
        else if (i.ObjectType == Interactables.LinkedType.DOOR)
        {
            DoorBehavior db = i.LinkObject.GetComponent<DoorBehavior>();
            db.RelatedInteractable = i;
            if (db == null)
            {
                throw new System.Exception("Door Behavior could not be found on the linked object");
            }
            //Open the door
            if (i.DefaultState == Interactables.LinkedState.OPEN_DOOR)
            {
                db.OpenInitialDoor();
                ActivateWires(i);
            }
        }
    }

    /// <summary>
    /// Initializes the state of the linked object
    /// </summary>
    public void InitializeLinkedState()
    {
        //Initializes the linked state by iterating over each related interactable
        foreach(Interactables i in allInteractables)
        {
            //Sets the linked state of a moving platform
            if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
            {
                mpb = i.LinkObject.GetComponentInChildren<MovingPlatformBehavior>();
                mpb.RelatedInteractable = i;
                if (mpb == null)
                {
                    throw new System.Exception("Moving Platform Behavior could not be found on the linked object");
                }
                if (i.DefaultState == Interactables.LinkedState.STOPPED_PLATFORM)
                {
                    mpb.ButtonSaysStopMoving = true;
                }
                else
                {
                    mpb.ButtonSaysStopMoving = false;
                    ActivateWires(i);
                }
            }
            //Sets the initial state of a door
            else if (i.ObjectType == Interactables.LinkedType.DOOR)
            {
                DoorBehavior db = i.LinkObject.GetComponent<DoorBehavior>();
                db.RelatedInteractable = i;
                if (db == null)
                {
                    throw new System.Exception("Door Behavior could not be found on the linked object");
                }
                if (i.DefaultState == Interactables.LinkedState.OPEN_DOOR)
                {
                    db.OpenInitialDoor();
                    ActivateWires(i);
                }
            }
        }
    }

    /// <summary>
    /// Draws a gizmo to visually link buttons with their associated object in scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        //Draws a line to each related object
        foreach(Interactables i in allInteractables)
        {
            //Draws a red line to each moving platform
            if (i.ObjectType == Interactables.LinkedType.MOVING_PLATFORM)
            {

                mpb = i.LinkObject.GetComponentInChildren<MovingPlatformBehavior>();
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, i.LinkObject.transform.position);
            }
            //Draws a green line to each moving platform
            if (i.ObjectType == Interactables.LinkedType.DOOR)
            {
                db = i.LinkObject.GetComponent<DoorBehavior>();
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, db.gameObject.transform.GetChild(0).position);
            }

            //Draws a blue line to each object this button resets
            if(i.ResetsThese.Length!=0)
            {
                foreach(ChangedObject c in i.ResetsThese)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, c.LinkedObject.transform.position);
                }
            }
        }

    }

    #region Wires

    /// <summary>
    /// Generates the wires for this button
    /// Hella inefficient code, but it works
    /// </summary>
    private void GenerateWirePositions()
    {
        Vector3[] wirePositions = new Vector3[10];

        //Iterate over each interactables saved to this button
        foreach (Interactables i in allInteractables)
        {
            
            Transform parent = transform.parent;
            Vector3 objPos = i.WireBox.transform.position;
            wirePositions[0] = transform.position;
            wirePositions[1] = new Vector3(wirePositions[0].x, .005f, wirePositions[0].z);
            wirePositions[9] = i.WireBox.transform.position;
            wirePositions[8] = new Vector3(wirePositions[9].x, .005f, wirePositions[9].z);
            wirePositions[7] = Vector3.zero;
            int valueCount = 2;
            
            //Any time you see something comparing the angles, it is checking what axis the button is on
            //In this case, the button is on the x axis
            if (Mathf.Abs(90 - (parent.eulerAngles.y % 180)) < 1f)
            {
                //Gets the direction the button is facing. Sets a point a distance away on the ground
                //Then calls generation of the next point
                if (parent.position.x - objPos.x < 0)
                {
                    //Generates a point on the ground a set distance away from the wall
                    wirePositions[valueCount] = new Vector3(wirePositions[1].x, wirePositions[1].y, wirePositions[1].z - .2f);
                    valueCount++;
                    //Generates the next point
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount-1], 1, 'x');
                    valueCount++;
                }
                else
                {
                    //Generates a point on the ground a set distance away from the wall
                    wirePositions[valueCount] = new Vector3(wirePositions[1].x, wirePositions[1].y, wirePositions[1].z + .2f);
                    valueCount++;
                    //Generates the next point
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount-1], -1, 'x');
                    valueCount++;
                }
            }
            //If the button is on the z axis
            else if (Mathf.Abs(parent.eulerAngles.y % 180) < 1f)
            {
                //Gets the direction the button is facing. Sets a point a distance away on the ground
                //Then calls generation of the next point
                if (parent.position.z - objPos.z < 0)
                {
                    //Generates a point on the ground a set distance away from the wall
                    wirePositions[valueCount] = new Vector3(wirePositions[1].x + .2f, wirePositions[1].y, wirePositions[1].z);
                    valueCount++;
                    //Generates the next point
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], 1, 'z');
                    valueCount++;
                }
                else
                {
                    //Generates a point on the ground a set distance away from the wall
                    wirePositions[valueCount] = new Vector3(wirePositions[1].x - .2f, wirePositions[1].y, wirePositions[1].z);
                    valueCount++;
                    //Generates the next point
                    wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], -1, 'z');
                    valueCount++;
                }
            }

            //Here's where the code gets messy. This could probably be done with a for loop thst runs twice and a direction variable

            //Checks if the previous point was close to the second-to-last point
            if(Mathf.Abs(wirePositions[valueCount-1].x - wirePositions[8].x) > .5f || Mathf.Abs(wirePositions[valueCount - 1].z - wirePositions[8].z) > .5f)
            {
                //If the button is on the z axis, change the next point's X
                if (Mathf.Abs(parent.eulerAngles.y % 180) < 1f)
                {
                    if (parent.position.x - objPos.x < 0)
                    {
                        wirePositions[valueCount] = new Vector3(objPos.x, .005f, wirePositions[valueCount-1].z);
                        valueCount++;
                    }
                    else
                    {
                        wirePositions[valueCount] = new Vector3(objPos.x, .005f, wirePositions[valueCount - 1].z);
                        valueCount++;
                    }
                }
                //Otherwise if the button is on the x axis, change the next point's Z
                else if (Mathf.Abs(90 - (parent.eulerAngles.y % 180)) < 1f)
                {
                    if (parent.position.z - objPos.z < 0)
                    {
                        wirePositions[valueCount] = new Vector3(wirePositions[valueCount - 1].x, .005f, objPos.z);
                        valueCount++;
                    }
                    else
                    {
                        wirePositions[valueCount] = new Vector3(wirePositions[valueCount - 1].x , .005f, objPos.z);
                        valueCount++;
                    }
                }
            }
            //If the previous point is close enough to the end point, set the value to something that exists
            else
            {
                if (Mathf.Abs(90 - (parent.eulerAngles.y % 180)) < 1f)
                {
                    wirePositions[7] = new Vector3(wirePositions[2].x, wirePositions[8].y, wirePositions[8].z);
                }
                else
                {
                    wirePositions[7] = new Vector3(wirePositions[8].x, wirePositions[8].y, wirePositions[2].z);
                }
            }

            //Checks if the previous point was close to the second-to-last point
            if (Mathf.Abs(wirePositions[valueCount - 1].x - wirePositions[8].x) > .5f || Mathf.Abs(wirePositions[valueCount - 1].z - wirePositions[8].z) > .5f)
            {
                //If the button is on the X axis, change the X value
                if (Mathf.Abs(90 - (parent.eulerAngles.y % 180)) < 1f)
                {
                    if (parent.position.x - objPos.x < 0)
                    {
                        wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], 1, 'x');
                        valueCount++;
                    }
                    else
                    {
                        wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], -1, 'x');
                        valueCount++;
                    }
                }
                //If the button is on the Z axis, change the Z value
                else if (Mathf.Abs(parent.eulerAngles.y % 180) < 1f)
                {
                    if (parent.position.z - objPos.z < 0)
                    {
                        wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], 1, 'z');
                        valueCount++;
                    }
                    else
                    {
                        wirePositions[valueCount] = GeneratePoint(wirePositions[valueCount - 1], -1, 'z');
                        valueCount++;
                    }
                }
            }
            //Else if the previous point is close enough to the last point, set the value to an existing value
            else
            {
                if (Mathf.Abs(parent.eulerAngles.y % 180) < 1f)
                {
                    wirePositions[7] = new Vector3(wirePositions[2].x, wirePositions[8].y, wirePositions[8].z);
                }
                else
                {
                    wirePositions[7] = new Vector3(wirePositions[8].x, wirePositions[8].y, wirePositions[2].z);
                }
            }

            //If the third-to-last position is empty, set it to am adjusted version of the second-to-last position
            if(wirePositions[7] == Vector3.zero)
            {
                //Adjusts the x-coordinate
                if (Mathf.Abs(90 - (parent.eulerAngles.y % 180)) < 1f)
                {
                    wirePositions[7] = new Vector3(wirePositions[8].x + (wirePositions[2].x - wirePositions[1].x), wirePositions[8].y, wirePositions[8].z);
                }
                //Adjusts the z-coordinate 
                else
                {
                    wirePositions[7] = new Vector3(wirePositions[8].x, wirePositions[8].y, wirePositions[8].z + (wirePositions[2].z - wirePositions[1].z));
                }
            }

            //shift the array so no positions point to 0,0
            for(int j=7; j < wirePositions.Length; j++)
            {
                wirePositions[valueCount] = wirePositions[j];
                valueCount++;
            }

            DrawWires(wirePositions, valueCount, i);

            
        }
    }

    /// <summary>
    /// Generates wire points following a set algorithim
    /// </summary>
    /// <param name="lastPoint">The last point the wire went to</param>
    /// <param name="direction">The positive/negative direction the wire should go</param>
    /// <param name="axis">The axis the wire should generate a new point on</param>
    /// <returns>The new point as a Vector3</returns>
    private Vector3 GeneratePoint(Vector3 lastPoint, int direction, char axis)
    {
        GameObject g = null;
        float difference;

        //Set the value to check the difference against
        if (direction == 1)
        {
            difference = float.MaxValue;
        }
        else
        {
            difference = float.MinValue;
        }
        
        //Iterate over each wall and find the nearest wall in the given direction
        foreach(GameObject w in walls)
        {
            //If looking in the x axis and towards -x
            if(axis == 'x' && direction == 1)
            {
                //If the x position of the wall is greater than the last point and the difference between this wall and the last point is greater than the difference and the absolute value is within a set range
                if((w.transform.position.x > lastPoint.x) && difference > (w.transform.position.x - lastPoint.x) && Mathf.Abs(w.transform.position.z - lastPoint.z) < 5f)
                {
                    difference = w.transform.position.x - lastPoint.x;
                    g = w;
                }
            }
            //If looking in the x axis and towards x
            else if (axis == 'x' && direction == -1)
            {
                //If the x position of the wall is less than the last point and the difference between this wall and the last point is less than the difference and the absolute value is within a set range
                if ((w.transform.position.x < lastPoint.x) && difference < (w.transform.position.x - lastPoint.x) && Mathf.Abs(w.transform.position.z - lastPoint.z) < 5f)
                {
                    difference = w.transform.position.x - lastPoint.x;
                    g = w;
                }
            }

            else if (axis == 'z' && direction == 1)
            {
                //If the z position of the wall is greater than the last point and the difference between this wall and the last point is greater than the difference and the absolute value is within a set range
                if ((w.transform.position.z > lastPoint.z) && difference > (w.transform.position.z - lastPoint.z) && Mathf.Abs(w.transform.position.x - lastPoint.x) < 5f)
                {
                    difference = w.transform.position.z - lastPoint.z;
                    g = w;
                }
            }
            else if (axis == 'z' && direction == -1)
            {
                //If the z position of the wall is greater than the last point and the difference between this wall and the last point is less than the difference and the absolute value is within a set range
                if ((w.transform.position.z < lastPoint.z) && difference < (w.transform.position.z - lastPoint.z) && Mathf.Abs(w.transform.position.x - lastPoint.x) < 5f)
                {
                    difference = w.transform.position.z - lastPoint.z;
                    g = w;
                }
            }
        }


        //Double check the value multiplier and return the vector with the new point
        //Changes on the X axis
        if(axis == 'x')
        {
            if (transform.position.x < g.transform.position.x)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
            return new Vector3(g.transform.position.x + (1f * direction), .005f, lastPoint.z);
        }
        //Changes on the Z axis
        else
        {
            if (transform.position.z < g.transform.position.z)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
            return new Vector3(lastPoint.x, .005f, g.transform.position.z + (1f * direction));
        }

    }

    /// <summary>
    /// Draws the wires for the given interactable
    /// </summary>
    /// <param name="points">The points the wire is drawn between</param>
    /// <param name="size">Number of points</param>
    /// <param name="i">Interactable to draw wires for</param>
    private void DrawWires(Vector3[] points, int size, Interactables i)
    {
        //Create a new game object and give it a line renderer
        GameObject newChild = new GameObject(i.LinkObject.name + " Line Renderer");
        newChild.transform.parent = transform;
        i.LineRendererObject = newChild;
        l = newChild.AddComponent<LineRenderer>();

        //update the line renderer's values
        l.startWidth = .3f;
        l.endWidth = .3f;
        l.material = _inactiveWire;
        l.alignment = LineAlignment.View;
        l.positionCount = size;
        l.SetPositions(points);
        l.useWorldSpace = true;

    }
    /// <summary>
    /// Activates the wire for the given interactable
    /// </summary>
    /// <param name="i">The interactable to activate wire states on</param>
    public void ActivateWires(Interactables i)
    {
        //If the current scene is 1, activate the wires
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("1"))
        {
            LineRenderer l = i.LineRendererObject.GetComponent<LineRenderer>();
            l.material = _activeWire;
            i.WireBox.GetComponent<Renderer>().material = _activeWireBox;
        }
    }

    /// <summary>
    /// Deactivates wthe wire for the given interactable
    /// </summary>
    /// <param name="i">The interactable to deactivate wire states on</param>
    public void DeactivateWires(Interactables i)
    {
        //If the current scene is Room 1, deactivate the wires
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("1"))
        {
            LineRenderer l = i.LineRendererObject.GetComponent<LineRenderer>();
            l.material = _inactiveWire;
            i.WireBox.GetComponent<Renderer>().material = _inactiveWireBox;
        }
    }

    #endregion

#endregion
}
