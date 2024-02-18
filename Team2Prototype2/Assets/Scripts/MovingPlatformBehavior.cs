/*****************************************************************************
// File Name :         MovingPlatformBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     February 18, 2024
//
// Brief Description : Handles the moving platform's behavior
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MovingPlatformBehavior : MonoBehaviour
{

    [Header("Platform Waypoints"), Tooltip("Put the transforms you want the platform to go to here, in the order you want the platform to use")]
    //list of all the places the moving platform will go to
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [Header("This slider manipulates the speed of the platform")]
    [SerializeField, Range(3f, 20f)] private float speed;

    [Header("Check this bool if you want the platform to move in a loop")]
    //changes if the next waypoint targeting loops or snakes
    [SerializeField] private bool loops;

    //refrences
    private Rigidbody rb;

    [Header("Debug Yay!!!!")]
    //can stop the platform in the inspector to check if something is working/not
    [SerializeField] private bool stopMoving = false;

    //next location for the moving platform to go to - formatted as waypoints[nextWaypoint]
    private int nextWaypoint;

    /// <summary>
    /// set refrences
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        nextWaypoint = 0;
    }

    /// <summary>
    /// handles movement
    /// </summary>
    void FixedUpdate()
    {
        if (!stopMoving)
        {
            Move();
        }
    }

    /// <summary>
    /// moves the platform to the next location according to speed
    /// </summary>
    private void Move()
    {
        //calcs how far to move
        float temp = speed * Time.deltaTime;

        //moves
        transform.position = Vector3.MoveTowards(transform.position,
            waypoints[nextWaypoint].position, temp);

        //if it hits the current target, it changes to the next target
        if (transform.position == waypoints[nextWaypoint].position)
        {
            GetNextWaypoint();
        }
    }

    /// <summary>
    /// finds the next waypoint in the list, loops back to the start if it reaches the end
    /// </summary>
    private void GetNextWaypoint()
    {
        //checks to see if its reached the end of the list
        if (nextWaypoint + 1 >= waypoints.Count)
        {
            //if the designers dont want it to loop, it reverses the order of the list
            if (!loops)
            {
                //saves all the waypoints in reverse order to a temp list, clears waypoints
                //then resets waypoints to the reversed temp list
                List<Transform> tempWaypointList = new List<Transform>();
                for (int i = waypoints.Count - 1; i >= 0; i--)
                {
                    tempWaypointList.Add(waypoints[i]);
                }
                waypoints.Clear();
                foreach (Transform t in tempWaypointList)
                {
                    waypoints.Add(t);
                }
                
            }
            //sets the platform back at the beginning
            nextWaypoint = 0;

        }
        else
        {
            //just goes up the list if you havent reached the end
            nextWaypoint++;
        }
    }

    /// <summary>
    /// sets the parent of anything that lands on top of the platform to the platform
    /// </summary>
    /// <param name="collision"> whatever the player collides with </param>
    private void OnCollisionEnter(Collision collision)
    {
        //check to see if the object landed on top of the platform vs landing on the bottom or side
        if (collision.transform.position.y > transform.position.y)
        {
            collision.transform.SetParent(transform);
        }
    }

    /// <summary>
    /// when the object jumps/falls off the platform, resets the parent to null
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        collision.transform.SetParent(null);
    }
}
