using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] private Point[] points;
    public Point GetPoint(Point lastPoint)
    {
        List<Point> pointList = new List<Point>();
        pointList.AddRange(points);
        if(lastPoint != null)
        {
            Debug.Log(lastPoint);
            pointList.Remove(lastPoint);
        }

        int randomNum = Random.Range(0, pointList.Count);
        return pointList[randomNum];
    }
}
