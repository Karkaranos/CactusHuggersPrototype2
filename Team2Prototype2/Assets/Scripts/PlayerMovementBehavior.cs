/*****************************************************************************
// File Name :         PlayerMovementBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     February 16. 2024
//
// Brief Description : Handles basic player movement and player controls
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovementBehavior : MonoBehaviour
{
    private PlayerInput _pControls;
    private InputAction _lrMovement;
    private InputAction _fbMovement;
    private InputAction _mPos;

    private int lrValue;
    private int fbValue;
    private Vector3 mPosValue;

    private Camera pov;

    [Header("Player Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float speedCap;


    [Header("Value Clamps")]
    [SerializeField]private float maxCamYAngle;
    [SerializeField] private float minCamYAngle;

    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerInput>().currentActionMap.Enable();
        _pControls = GetComponent<PlayerInput>();
        pov = GetComponentInChildren<Camera>();

        _lrMovement = _pControls.currentActionMap.FindAction("SideMovement");
        _fbMovement = _pControls.currentActionMap.FindAction("ForwardBackMovement");
        _mPos = _pControls.currentActionMap.FindAction("Mouse");

        _lrMovement.performed += contx => lrValue = (int)contx.ReadValue<float>();
        _fbMovement.performed += contx => fbValue = (int)contx.ReadValue<float>();


        _lrMovement.canceled += contx => lrValue = (int)contx.ReadValue<float>();
        _fbMovement.canceled += contx => fbValue = (int)contx.ReadValue<float>();


        Cursor.visible = false;
    }





    // Update is called once per frame
    void Update()
    {
        if(_mPos != null)
        {
            mPosValue = _mPos.ReadValue<Vector2>();
            pov.transform.LookAt(mPosValue);
        }

        if (lrValue != 0 || fbValue != 0)
        {
            Vector3 moveDir = new Vector3(fbValue, 0, lrValue);
            moveDir *= Time.deltaTime * speed;
            moveDir.y = 0;
            Vector3.ClampMagnitude(moveDir, speedCap);
            transform.Translate(moveDir);
        }


    }
}
