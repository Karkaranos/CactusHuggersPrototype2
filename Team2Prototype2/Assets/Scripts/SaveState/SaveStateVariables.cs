using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveStateVariables
{
    public bool hasBeenSaved;
    public Vector3 pPos;
    public Quaternion pRot;
    public Vector3 pScale;

    public SaveStateVariables(Vector3 pPos, Quaternion pRot, Vector3 pScale, bool hasBeenSaved)
    {
        this.pPos = pPos;
        this.pRot = pRot;
        this.pScale = pScale;
        this.hasBeenSaved = hasBeenSaved;
    }
}
