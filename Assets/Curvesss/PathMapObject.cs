using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierCurve))]
public class PathMapObject : BaseCurveMapObject
{
    public float pathWidth = 3f;
    //public float pathPartLength = 1f;

    void Start()
    {
        //bezierCurve.pointPerUnits = pathPartLength;
    }
}
