using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurve : MonoBehaviour
{
    [SerializeField]
    float pointPerUnits = 10;
    [SerializeField]
    List<Transform> curveTransforms = new List<Transform>();

    public List<Vector3> curvePoints = new List<Vector3>();


    void OnDrawGizmos()
    {
        if(pointPerUnits != 0)
        {
            curvePoints.Clear();
            Gizmos.color = Color.red;
            Vector3 point1 = curveTransforms[0].position;
            Vector3 point2;
            curvePoints.Add(point1);
            for(int i = 0; i < curveTransforms.Count - 1; i += 2)
            {
                float increment = 1/(Vector3.Distance(curveTransforms[i].position, curveTransforms[i+2].position) / pointPerUnits);
                for(float t = 0; t < 1; t += increment)
                {
                    point2 = (1-t) * ((1-t)*curveTransforms[i].position + t * curveTransforms[i+1].position) + t * ((1 - t) * curveTransforms[i+1].position + t * curveTransforms[i+2].position);
                    Gizmos.DrawLine(point1, point2);
                    curvePoints.Add(point2);
                    point1 = point2;
                }
            }
        }
    }
}
