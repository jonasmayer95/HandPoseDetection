using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public struct sQuaternion
{
    public float x, y, z, w;

    public sQuaternion(float _x, float _y, float _z, float _w)
    {
        x = _x;
        y = _y;
        z = _y;
        w = _w;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
    }

    public static implicit operator Quaternion(sQuaternion rValue)
    {
        return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }

    public static implicit operator sQuaternion(Quaternion rValue)
    {
        return new sQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }
}
