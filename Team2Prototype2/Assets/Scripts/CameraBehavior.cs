using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform eyes;

    // Update is called once per frame
    void Update()
    {
        transform.position = eyes.position;
    }
}
