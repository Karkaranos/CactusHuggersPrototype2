/*****************************************************************************
// File Name :         ButtonShiftBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     February 25. 2024
//
// Brief Description : Shifts what button is active when a trigger is hit
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonShiftBehavior : MonoBehaviour
{
    [SerializeField] private GameObject _firstButton;
    [SerializeField] private GameObject _secondButton;

    private void OnTriggerEnter(Collider other)
    {
        _firstButton.SetActive(false);
        _secondButton.SetActive(true);
    }
}
