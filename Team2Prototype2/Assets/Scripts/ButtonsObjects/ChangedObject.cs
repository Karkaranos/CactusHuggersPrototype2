using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangedObject
{
    [SerializeField] private GameObject _linkedObject;
    [Tooltip("The waypoint to go to, starting at 1")]
    [SerializeField] private int _waypointToGoTo;

    public GameObject LinkedObject { get => _linkedObject; set => _linkedObject = value; }
    public int WaypointToGoTo { get => _waypointToGoTo; set => _waypointToGoTo = value; }
}
