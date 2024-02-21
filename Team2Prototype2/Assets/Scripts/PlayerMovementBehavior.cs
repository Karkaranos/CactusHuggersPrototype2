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
    private CharacterController cc;
    private PlayerInput _pControls;
    private InputAction _lrMovement;
    private InputAction _fbMovement;
    private InputAction _mPos;
    private InputAction _saveState;
    private InputAction _switchState1;
    private InputAction _switchState2;
    private InputAction _switchState3;
    private InputAction _loadState;
    private InputAction _interact;
    private InputAction _jump;

    private int lrValue;
    private int fbValue;
    private Vector3 mPosValue;
    private SaveStateBehvaior ssBehav;


    [Header("Camera Controls")]
    [SerializeField] private float _horizontalRotationSpeed;
    [SerializeField] private float _verticalRotationSpeed;
    [SerializeField] private float maxCamYAngle;
    [SerializeField] private float minCamYAngle;
    private Transform camTransform;

    [Header("Player Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _speedCap;
    [SerializeField] private float _jumpHeight;
    private bool moving;
    [SerializeField]private bool jumping;

    private bool _isInteracting = false;
    private Vector3 moveDir;

    RigidbodyConstraints defConstraints;

    public float HorizontalRotationSpeed { get => _horizontalRotationSpeed;}
    public float VerticalRotationSpeed { get => _verticalRotationSpeed; }
    public float MaxCamYAngle { get => maxCamYAngle;}
    public float MinCamYAngle { get => minCamYAngle;}
    public Vector3 MPosValue { get => mPosValue; set => mPosValue = value; }
    public bool Interacting { get => _isInteracting; set => _isInteracting = value; }

    /// <summary>
    /// Called on the first frame update. Gets references to scripts and objects. Assigns input actions to functions.
    /// </summary>
    void Start()
    {
        defConstraints = GetComponent<Rigidbody>().constraints;
        cc = GetComponent<CharacterController>();
        camTransform = Camera.main.transform;
        GetComponent<PlayerInput>().currentActionMap.Enable();
        _pControls = GetComponent<PlayerInput>();

        _lrMovement = _pControls.currentActionMap.FindAction("SideMovement");
        _fbMovement = _pControls.currentActionMap.FindAction("ForwardBackMovement");
        _mPos = _pControls.currentActionMap.FindAction("Mouse");
        _saveState = _pControls.currentActionMap.FindAction("Save State");
        _switchState1 = _pControls.currentActionMap.FindAction("SelectState1");
        _switchState2 = _pControls.currentActionMap.FindAction("SelectState2");
        _switchState3 = _pControls.currentActionMap.FindAction("SelectState3");
        _loadState = _pControls.currentActionMap.FindAction("LoadState");
        _interact = _pControls.currentActionMap.FindAction("Interact");
        _jump = _pControls.currentActionMap.FindAction("Jump");

        _lrMovement.performed += contx => lrValue = (int)contx.ReadValue<float>();
        _fbMovement.performed += contx => fbValue = (int)contx.ReadValue<float>();


        _lrMovement.canceled += contx => lrValue = (int)contx.ReadValue<float>();
        _fbMovement.canceled += contx => fbValue = (int)contx.ReadValue<float>();


        _saveState.started += _saveState_started;
        ssBehav = GetComponent<SaveStateBehvaior>();

        _switchState1.started += _switchState1_started;
        _switchState2.started += _switchState2_started;
        _switchState3.started += _switchState3_started;

        _loadState.started += _loadState_started;

        _interact.performed += contx => StartCoroutine(Interact());

        _jump.performed += contx => Jump();


        Cursor.visible = false;
    }

    private void _loadState_started(InputAction.CallbackContext obj)
    {
        ssBehav.LoadState();
    }

    private void _switchState3_started(InputAction.CallbackContext obj)
    {
        ssBehav.SwitchSelectedState(3);
    }

    private void _switchState2_started(InputAction.CallbackContext obj)
    {
        ssBehav.SwitchSelectedState(2);
    }

    private void _switchState1_started(InputAction.CallbackContext obj)
    {
        ssBehav.SwitchSelectedState(1);
    }

    private void _saveState_started(InputAction.CallbackContext obj)
    {
        ssBehav.SetSaveState();
    }

    /// <summary>
    /// Handles player jumping
    /// </summary>
    private void Jump()
    {
        if(!jumping)
        {
            jumping = true;
            StartCoroutine(JumpDecay());
        }
    }

    /// <summary>
    /// Stops the jump after a set amount of time
    /// </summary>
    /// <returns></returns>
    IEnumerator JumpDecay()
    {
        print("should jump");
        yield return new WaitForSeconds(.5f);
        jumping = false;
    }

    /// <summary>
    /// Called if the player interacts with something
    /// </summary>
    /// <returns></returns>
    IEnumerator Interact()
    {
        if(!Interacting)
        {
            Interacting = true;
            yield return new WaitForSeconds(2f);
            print("interacted");
            Interacting = false;
        }
    }



    /// <summary>
    /// Update is called once per frame. It handles movement and reads mouse position.
    /// </summary>
    void Update()
    {
        MPosValue = _mPos.ReadValue<Vector2>();
        mPosValue.x -= Screen.width/2;
        mPosValue.y -= Screen.height/2;
        MPosValue = mPosValue;

        moveDir = camTransform.forward * fbValue + camTransform.right * lrValue;
        if(jumping)
        {
            moveDir.y = _jumpHeight;
        }
        moveDir.y -= 15f * Time.deltaTime;
        moveDir *= Time.deltaTime * _speed;
        cc.Move(moveDir);
        transform.forward = moveDir;

        Vector3 eulerRot = transform.rotation.eulerAngles;
        eulerRot.x = 0;
        eulerRot.z = 0;
        transform.rotation = Quaternion.Euler(0, eulerRot.y, 0);

    }
}
