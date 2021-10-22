using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovingPlatform)), CanEditMultipleObjects]
public class MovingPlatformCustomInspector : Editor
{
    private MovingPlatform[] movingPlaforms;
    private void OnEnable()
    {
        Object[] myTargets = targets;
        movingPlaforms = new MovingPlatform[myTargets.Length];
        for(int i = 0; i < movingPlaforms.Length; i++)
        {
            movingPlaforms[i] = myTargets[i] as MovingPlatform;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        for(int i = 0; i < movingPlaforms.Length; i++)
        {
            if (Application.isPlaying)
                return;

            movingPlaforms[i].UpdateWaypoints();
        }
    }
}
