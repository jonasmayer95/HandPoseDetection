using UnityEngine;
using System.Collections;

[System.Serializable]
public struct sVector3{
    public float x, y, z;

    public sVector3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _y;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }

    public static implicit operator Vector3(sVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator sVector3(Vector3 rValue)
    {
        return new sVector3(rValue.x, rValue.y, rValue.z);
    }
}

