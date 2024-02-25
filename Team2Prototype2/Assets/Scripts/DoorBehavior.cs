/*****************************************************************************
// File Name :         DoorBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     February 20. 2024
//
// Brief Description : Handles basic door functionality
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    private GameObject doorObj;
    [SerializeField] private float doorOpenTime;
    [SerializeField] private float doorCloseTime = .2f;
    [SerializeField] private float doorOpenHeight;
    private bool isOpen;

    public bool IsOpen { get => isOpen; set => isOpen = value; }

    /// <summary>
    /// Gets a reference to the door's object
    /// </summary>
    private void Start()
    {
        doorObj = transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// Public facing function to open the door
    /// </summary>
    public void OpenDoor()
    {
        if(!IsOpen)
        {
            StartCoroutine(DoorSlideOpen());
            IsOpen = true;
        }
    }

    /// <summary>
    /// Sets the door's initial state to be open
    /// </summary>
    public void OpenInitialDoor()
    {
        if(!isOpen)
        {
            doorObj = transform.GetChild(0).gameObject;
            Vector3 doorPos = doorObj.transform.position;
            doorPos.y += doorOpenHeight;
            doorObj.transform.position = doorPos;
            IsOpen = true;
        }
    }

    /// <summary>
    /// Opens the door over a set period of time
    /// </summary>
    /// <returns>The time the door opens in</returns>
    private IEnumerator DoorSlideOpen()
    {
        float timer = doorOpenTime;
        Vector3 doorPos = doorObj.transform.position;
        while(timer > 0)
        {
            doorPos = doorObj.transform.position;
            doorPos.y += doorOpenHeight * (Time.deltaTime / doorOpenTime);
            doorObj.transform.position = doorPos;
            timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
    }

    /// <summary>
    /// Public facing function to close the door
    /// </summary>
    public void CloseDoor()
    {
        if(IsOpen)
        {
            StartCoroutine(DoorShut());
            IsOpen = false;
        }
    }

    /// <summary>
    /// Closes the door over a short time period
    /// </summary>
    /// <returns>The time the door closes in</returns>
    private IEnumerator DoorShut()
    {
        float timer = doorCloseTime;
        Vector3 doorPos = doorObj.transform.position;
        while (timer > 0)
        {
            doorPos = doorObj.transform.position;
            doorPos.y -= doorOpenHeight * (Time.deltaTime / doorCloseTime);
            timer -= Time.deltaTime;
            doorObj.transform.position = doorPos;
            yield return Time.deltaTime;
        }
    }
}
