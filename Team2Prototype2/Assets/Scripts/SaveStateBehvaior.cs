/*****************************************************************************
// File Name :         SaveStateBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     February 17, 2024
//
// Brief Description : Handles stave stating and reverting back
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class SaveStateBehvaior : MonoBehaviour
{
    public const int NumberOfSaveStates = 3;

    [Header("Save State Cooldown: "), Tooltip("Edit this to change the max cooldown time")]
    [SerializeField] private float maxCooldownTime;
    

    [Header("Save State Waypoint Colors"), Tooltip("Change these to change the color of each"
        + " save state's waypoint color")]
    [SerializeField] private Color[] waypointColors = new Color[NumberOfSaveStates];

    [Header("Variables In Save States: "), Tooltip("DO NOT TOUCH THESE VARIABLES")]
    [SerializeField] private SaveStateVariables[] saveStates = new SaveStateVariables[NumberOfSaveStates];
    [SerializeField] private int currentStateSelected;

    [Header("Debug Variables: "), Tooltip("dont touch these pls")]
    [SerializeField] private float currentCooldownTime;
    private bool onCooldown;
    [SerializeField] public static int selectedSaveState;
    [SerializeField] private GameObject saveStateWaypoint;
    [SerializeField] private GameObject[] waypoints = new GameObject[NumberOfSaveStates];
    [SerializeField] private TMP_Text stateText;
    private bool noText;
    

    /// <summary>
    /// set refrences
    /// </summary>
    void Start()
    {
        currentCooldownTime = maxCooldownTime;
        onCooldown = false;
        if (stateText == null)
        {
            noText = true;
            Debug.Log("No Save State Text Refrenced");
        }
        SwitchSelectedState(1);
    }

    /// <summary>
    /// This handles the cooldown of save states
    /// </summary>
    void FixedUpdate()
    {
        //only runs if its on cooldown
        if (onCooldown)
        {
            //as long as its not done with the cooldown it will make it go down
            if (currentCooldownTime > 0)
            {
                //subtracts the time passed
                currentCooldownTime -= Time.deltaTime;
            }
            else
            {
                //allows the player to use the ability again
                currentCooldownTime = maxCooldownTime;
                onCooldown = false;
            }
        }
    }

    /// <summary>
    /// Switches the selected save state to the input number
    /// </summary>
    /// <param name="switchTo"> number of the state to switch to </param>
    public void SwitchSelectedState(int switchTo)
    {
        //-1 because selected save state is the index of the save state
        selectedSaveState = switchTo - 1;
        if (!noText)
        {

            print("Ran switch state");
            stateText.text = "Selected State: " + switchTo;
        }
        
    }

    /// <summary>
    /// Loads the selected save state if the state has a state in it
    /// </summary>
    public void LoadState()
    {
        print("Load triggered");
        //only works if its not on cooldown
        if (!onCooldown)
        {
            //puts it on cooldown
            onCooldown = true;

            //checks to see if the save state has anything in it
            if (saveStates[selectedSaveState].hasBeenSaved)
            {
                //sets all the variables in the saveStateVariables attached to it
                transform.position = saveStates[selectedSaveState].pPos;
                print("should have set player position");
                transform.rotation = saveStates[selectedSaveState].pRot;
                transform.localScale = saveStates[selectedSaveState].pScale;

                //makes it so you have to save again to load the state
                saveStates[selectedSaveState].hasBeenSaved = false;
                Destroy(waypoints[selectedSaveState].gameObject);
            }
            else
            {
                Debug.Log("Nothing Saved In Slot" + (selectedSaveState + 1));
            }
        }
    }

    /// <summary>
    /// handles saving the save state variables
    /// </summary>
    public void SetSaveState()
    {
        print("save triggered");
        //checks to see if its on cooldown already
        if (!onCooldown)
        {
            //puts it on cooldown
            onCooldown = true;

            //sets all the variables
            saveStates[selectedSaveState].pPos = transform.position;
            saveStates[selectedSaveState].pRot = transform.rotation;
            saveStates[selectedSaveState].pScale = transform.localScale;

            //allows the player to load it now
            saveStates[selectedSaveState].hasBeenSaved = true;

            GameObject go = Instantiate(saveStateWaypoint, transform.position, Quaternion.identity);
            go.GetComponent<Renderer>().material.color = waypointColors[selectedSaveState];

            if (waypoints[selectedSaveState] != null)
            {
                Destroy(waypoints[selectedSaveState].gameObject);
            }
            waypoints[selectedSaveState] = go;
        }
    }
}
