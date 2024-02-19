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

    [Header("Variables To Save: ")]
    [SerializeField] private Vector3 pPos;
    [SerializeField] private Quaternion pRot;
    [SerializeField] private Vector3 pScale;

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

    }

    public void LoadState()
    {

    }
    public void SetSaveState()
    {
        pPos = transform.position;
        pRot = transform.rotation;
        pScale = transform.localScale;
        hasSaveState = true;
    }

    public void LoadSaveState()
    {
        transform.position = pPos;
        transform.rotation = pRot;
        transform.localScale = pScale;

        hasSaveState = false;
    }
}
