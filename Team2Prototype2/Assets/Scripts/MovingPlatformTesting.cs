/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovingPlatformBehavior))]
public class MovingPlatformTesting : Editor
{

    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MovingPlatformBehavior behavior = (MovingPlatformBehavior)target;

        if (GUILayout.Button("Launch Player"))
        {
            behavior.LaunchPlayer();
        }

        if (GUILayout.Button("Reset Player"))
        {
            ResetPlayer(behavior.playerPos);
        }
    }

    void ResetPlayer(Vector3 playerPos) {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        player.transform.position = playerPos;
    }
}
*/