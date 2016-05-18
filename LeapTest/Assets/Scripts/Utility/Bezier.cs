using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bezier {

    public static Vector3 getPoint(float alpha, Vector3[] points)
    {
        if (points.Length == 1)
            return points[0];
        
        Vector3[] result = new Vector3[points.Length-1];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = Vector3.Lerp(points[i], points[i+1], alpha);
        }

        return getPoint(alpha, result);
    }
}
