/*****************************************************************************
// File Name :          CinemachineOverrides.cs
// Author :             Cade R. Naylor
// Creation Date :      February 16. 2024
//
// Brief Description :  Handles camera rotation and overrides for Cinemachine
//
//Tutorial used :       Cinemachine First Person Controller w/Input System by samyam
//Tutorial link :       https://www.youtube.com/watch?v=5n_hmqHdijM&t=8s
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
public class CinemachineOverrides : CinemachineExtension
{
    private PlayerMovementBehavior pmb;
    private Vector3 camRotate;
    private static float SENSITIVITY_ADJUSTMENT = .2f;

    /// <summary>
    /// Overrides the awake function to initialize angles and get references to other scripts
    /// </summary>
    protected override void Awake()
    {
        camRotate = transform.localEulerAngles;
        pmb = FindObjectOfType<PlayerMovementBehavior>();
        base.Awake();
    }

    /// <summary>
    /// Overrides Cinemachine's basic process to create custom camera aiming
    /// </summary>
    /// <param name="vcam">Cinemachine's current camera</param>
    /// <param name="stage">Cinemachine's current stage</param>
    /// <param name="state">The current state of the camera</param>
    /// <param name="deltaTime">Change in time</param>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                if (camRotate == null)
                {
                    camRotate = transform.localRotation.eulerAngles;
                }
                if(pmb == null)
                {
                    pmb = FindObjectOfType<PlayerMovementBehavior>();
                }

                Vector2 mousePos = pmb.MPosValue;
                camRotate.x += mousePos.x * Time.deltaTime * pmb.HorizontalRotationSpeed * SENSITIVITY_ADJUSTMENT;
                camRotate.y += mousePos.y * Time.deltaTime * pmb.VerticalRotationSpeed;

                camRotate.y = Mathf.Clamp(camRotate.y, pmb.MinCamYAngle, pmb.MaxCamYAngle);
                state.RawOrientation = Quaternion.Euler(-camRotate.y, camRotate.x, 0f);

            }
        }
    }
}
