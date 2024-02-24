using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationBehavior : MonoBehaviour
{
    private Vector2 mPosValue;
    [SerializeField] private float mouseSensX;
    [SerializeField] private float mouseSensY;
    private float yRot;
    private float xRot;
    [SerializeField] private Transform orientation;
    public void Look(Vector2 mpos)
    {
        mPosValue = mpos;

        float mouseX = mPosValue.x * Time.deltaTime * mouseSensX;
        float mouseY = mPosValue.y * Time.deltaTime * mouseSensY;

        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, -90, 90);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);

        /* mPosValue.x -= Screen.width / 2;
         mPosValue.y -= Screen.height / 2;
         float temp = mPosValue.x;
         mPosValue.x = mPosValue.y;
         mPosValue.y = temp;

         MPosValue = mPosValue;
         Camera.main.transform.LookAt(MPosValue);*/
    }
}
