/*****************************************************************************
// File Name :         SaveStateBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     February 17, 2024
//
// Brief Description : Handles stave stating and reverting back
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStateBehvaior : MonoBehaviour
{
    [Header("Save State Cooldown: ")]
    [SerializeField] private float maxCooldownTime;

    [Header("Variables In Save States: "), Tooltip("DO NOT TOUCH THESE VARIABLES")]
    [SerializeField] private SaveStateVariables[] saveStates = new SaveStateVariables[3];
    [SerializeField] private int currentStateSelected;

    [Header("Debug Variables: ")]
    [SerializeField] private float currentCooldownTime;
    private bool onCooldown;
    [SerializeField] private bool hasSaveState;
    [SerializeField] private int selectedSaveState;


    // Start is called before the first frame update
    void Start()
    {
        currentCooldownTime = maxCooldownTime;
        onCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (onCooldown)
        {
            if (!(currentCooldownTime <= 0))
            {
                currentCooldownTime -= Time.deltaTime;
            }
            else
            {
                currentCooldownTime = maxCooldownTime;
                onCooldown = false;
            }
        }
    }

    public void SaveButtonPressed()
    {
        if (!onCooldown)
        {
            onCooldown = true;
            if (hasSaveState)
            {
                LoadSaveState();
            }
            else
            {
                SetSaveState();
            }
        }
    }

    public void SwitchSelectedState(int switchTo)
    {
        //-1 because selected save state is the index of the save state
        selectedSaveState = switchTo - 1;
    }

    public void LoadState()
    {

    }
    public void SetSaveState()
    {
        saveStates[selectedSaveState].pPos = transform.position;
        saveStates[selectedSaveState].pRot = transform.rotation;
        saveStates[selectedSaveState].pScale = transform.localScale;
        hasSaveState = true;
    }

    public void LoadSaveState()
    {
        transform.position = saveStates[selectedSaveState].pPos;
        transform.rotation = saveStates[selectedSaveState].pRot;
        transform.localScale = saveStates[selectedSaveState].pScale;

        hasSaveState = false;
    }
}
