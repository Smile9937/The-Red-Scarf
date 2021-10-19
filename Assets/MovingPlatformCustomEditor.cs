using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

    [CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformCustomEditor : Editor
{
    private void OnSceneGUI()
    {
        MovingPlatform myTarget = target as MovingPlatform;
        if(myTarget.waypoints.Count > 0)
        {
            for (int i = 0; i < myTarget.waypoints.Count; i++)
            {
                if(myTarget.waypoints[i] == null)
                {
                    myTarget.waypoints[i] = myTarget.waypointPrefab;

                    if(myTarget.waypointObjects.Count < myTarget.waypoints.Count)
                    {
                        GameObject currentWaypoint = Instantiate(myTarget.waypoints[i], myTarget.transform.position, Quaternion.identity);
                        currentWaypoint.transform.parent = myTarget.transform.parent;
                        myTarget.waypointObjects.Add(currentWaypoint);
                    }
                }
            }
        }

        if(myTarget.waypointObjects.Count > 0)
        {
            for(int j = 0; j < myTarget.waypointObjects.Count; j++)
            {
                if(j == myTarget.waypoints.Count)
                {
                    DestroyImmediate(myTarget.waypointObjects[j]);
                    myTarget.waypointObjects.RemoveAt(j);
                }
            }
        }
    }
}
