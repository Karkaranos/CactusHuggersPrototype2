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
    [SerializeField] private float currentSaveCooldownTime;
    [SerializeField] private float currentLoadCooldownTime;
    private bool onSaveCooldown;
    private bool onLoadCooldown;
    [SerializeField] public static int selectedSaveState;
    [SerializeField] private GameObject saveStateWaypoint;
    [SerializeField] private GameObject[] waypoints = new GameObject[NumberOfSaveStates];
    [SerializeField] private TMP_Text stateText;
    private bool noText;
    private bool pauseMovement = false;

    [Header("Canvas References")]
    [SerializeField] private GameObject _saveStateTextFeedback;
    [SerializeField] private float _textFadeOutTime;
    private Coroutine stopMe;

    public int CurrentStateSelected { get => currentStateSelected;}


    /// <summary>
    /// set refrences
    /// </summary>
    void Start()
    {
        _saveStateTextFeedback.SetActive(false);
        currentSaveCooldownTime = maxCooldownTime;
        onSaveCooldown = false;
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
        if (onSaveCooldown)
        {
            //as long as its not done with the cooldown it will make it go down
            if (currentSaveCooldownTime > 0)
            {
                //subtracts the time passed
                currentSaveCooldownTime -= Time.deltaTime;
            }
            else
            {
                //allows the player to use the ability again
                currentSaveCooldownTime = maxCooldownTime;
                onSaveCooldown = false;
            }
        }
        if(onLoadCooldown)
        {
            //as long as its not done with the cooldown it will make it go down
            if (currentLoadCooldownTime > 0)
            {
                //subtracts the time passed
                currentLoadCooldownTime -= Time.deltaTime;
            }
            else
            {
                //allows the player to use the ability again
                currentLoadCooldownTime = maxCooldownTime;
                onLoadCooldown = false;
            }
        }
    }

    /// <summary>
    /// Switches the selected save state to the input number
    /// </summary>
    /// <param name="switchTo"> number of the state to switch to </param>
    public void SwitchSelectedState(int switchTo)
    {
        UIManager uim = FindObjectOfType<UIManager>();
        //-1 because selected save state is the index of the save state
        selectedSaveState = switchTo - 1;
        if(uim!=null)
        {
            uim.SelectSaveState(selectedSaveState);
        }
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
        //only works if its not on cooldown
        if (!onLoadCooldown)
        {
            //puts it on cooldown
            onLoadCooldown = true;

            //checks to see if the save state has anything in it
            if (saveStates[selectedSaveState].hasBeenSaved)
            {
                UIManager uim = FindObjectOfType<UIManager>();
                if (uim != null)
                {
                    uim.EmptySaveState(selectedSaveState);
                }
                //pauseMovement = true;
                TeleportPlayer();
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
        //checks to see if its on cooldown already
        if (!onSaveCooldown)
        {
            //puts it on cooldown
            onSaveCooldown = true;

            saveStates[selectedSaveState].pPos = transform.position;
            saveStates[selectedSaveState].pRot = transform.rotation;
            saveStates[selectedSaveState].pScale = transform.localScale;

            //allows the player to load it now
            saveStates[selectedSaveState].hasBeenSaved = true;
            UIManager uim = FindObjectOfType<UIManager>();


            GameObject go = Instantiate(saveStateWaypoint, transform.position, Quaternion.identity);
            go.GetComponent<Renderer>().material.color = waypointColors[selectedSaveState];
            go.GetComponentInChildren<SpriteRenderer>().material.color = waypointColors[selectedSaveState];

            if (waypoints[selectedSaveState] != null)
            {
                if (stopMe != null)
                {
                    StopCoroutine(stopMe);
                }
                stopMe = StartCoroutine(TextFade("Overwrote Save " + (selectedSaveState + 1)));
                Destroy(waypoints[selectedSaveState].gameObject);
                if (uim != null)
                {
                    uim.OverwriteSaveState(selectedSaveState);
                }
            }
            else
            {
                if(stopMe!=null)
                {
                    StopCoroutine(stopMe);
                }
                stopMe = StartCoroutine(TextFade("Added Save " + (selectedSaveState + 1)));
                if (uim != null)
                {
                    uim.FillSaveState(selectedSaveState);
                }
            }
            waypoints[selectedSaveState] = go;
        }
    }

    public void LateUpdate()
    {
        if(pauseMovement)
        {
            pauseMovement = false;

            if (stopMe != null)
            {
                StopCoroutine(stopMe);
            }
            stopMe = StartCoroutine(TextFade("Loaded Save " + (selectedSaveState + 1)));
            //sets all the variables in the saveStateVariables attached to it
            transform.parent = null;
            Vector3 playerPos = saveStates[selectedSaveState].pPos;
            //transform.parent = null;
            playerPos.y += .2f;
            transform.position = playerPos;
            transform.rotation = saveStates[selectedSaveState].pRot;
            //transform.localScale = saveStates[selectedSaveState].pScale;

            //makes it so you have to save again to load the state
            saveStates[selectedSaveState].hasBeenSaved = false;
            Destroy(waypoints[selectedSaveState].gameObject);

        }
    }

    private void TeleportPlayer()
    {
        if (stopMe != null)
        {
            StopCoroutine(stopMe);
        }
        stopMe = StartCoroutine(TextFade("Loaded Save " + (selectedSaveState + 1)));
        //sets all the variables in the saveStateVariables attached to it
        transform.parent = null;
        Vector3 playerPos = saveStates[selectedSaveState].pPos;
        //transform.parent = null;
        playerPos.y += .2f;
        transform.position = playerPos;
        transform.rotation = saveStates[selectedSaveState].pRot;
        //transform.localScale = saveStates[selectedSaveState].pScale;

        //makes it so you have to save again to load the state
        saveStates[selectedSaveState].hasBeenSaved = false;
        Destroy(waypoints[selectedSaveState].gameObject);
    }

    /// <summary>
    /// Sets the current text message, then fades it out over a set period of time
    /// </summary>
    /// <param name="message">The message the text should say</param>
    private IEnumerator TextFade(String message)
    {
        _saveStateTextFeedback.GetComponent<TMP_Text>().text = message;
        _saveStateTextFeedback.SetActive(true);
        float timer = _textFadeOutTime;
        Color currColor = _saveStateTextFeedback.GetComponent<TextMeshProUGUI>().faceColor;
        while (timer > 0)
        {
            _saveStateTextFeedback.GetComponent<TextMeshProUGUI>().faceColor = new Color(currColor.r, currColor.g, currColor.b,  (timer / _textFadeOutTime));
            timer -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _saveStateTextFeedback.GetComponent<TextMeshProUGUI>().faceColor = new Color(currColor.r, currColor.g, currColor.b, 1);
        _saveStateTextFeedback.SetActive(false);
    }
}
