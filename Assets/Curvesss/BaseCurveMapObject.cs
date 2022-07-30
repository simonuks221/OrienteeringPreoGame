using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierCurve))]
public class BaseCurveMapObject : MonoBehaviour
{
    [HideInInspector]
    public BezierCurve bezierCurve;

    void Awake()
    {
        bezierCurve = GetComponent<BezierCurve>();
    }

    virtual public Color[,] mapTexturePixels()
    {
        return null;
    }
}
