using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangedObject
{
    [SerializeField] private GameObject _linkedObject;
    //[SerializeField] private Interactables.LinkedState _stateToGoTo;

    public GameObject LinkedObject { get => _linkedObject; set => _linkedObject = value; }
    //public Interactables.LinkedState StateToGoTo { get => _stateToGoTo; set => _stateToGoTo = value; }
}
