﻿using UnityEngine;
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
        rValue.normalize();
        Quaternion temp = new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        return temp;
    }

    public static implicit operator sQuaternion(Quaternion rValue)
    {
        sQuaternion temp = new sQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        temp.normalize();
        return temp;
    }
    public double length()
    {
        return Mathf.Sqrt(x * x + y * y + z * z + w * w);
    }
    public static sQuaternion normalized(sQuaternion quat)
    {
        quat.normalize();
        return quat;
    }

    public sQuaternion normalize()
    {
        while (length() != 1.0)
        {
            x = (float)(x / length());
            y = (float)(y / length());
            z = (float)(z / length());
            w = (float)(w / length());
        }
        return this;
    }

    public sQuaternion mirrorX()
    {
        y = -y;
        z = -z;
        return this.normalize();
    }

	public sQuaternion mirrorY()
	{
		x = -x;
		z = -z;
		return this.normalize();
	}

	public sQuaternion mirroredY()
	{
		sQuaternion temp = new sQuaternion (this.x,this.y,this.z,this.w);
		return temp.mirrorY();
	}
}
