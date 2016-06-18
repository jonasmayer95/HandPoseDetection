using UnityEngine;
using System.Collections;

[System.Serializable]
public struct sVector3{
    public float x, y, z;

    public sVector3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
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

    public string ToCSVString(string endl)
    {
        string result = "";

        result += x + endl;
        result += y + endl;
        result += z;

        return result;
    }

    public static string getCSVHeader(string endl, string vectorName)
    {
        string result = "";

        result += vectorName + "x" + endl;
        result += vectorName + "y" + endl;
        result += vectorName + "z";

        return result;
    }
	public int FromCSV(string[] input, int first)
	{
		x = float.Parse (input [first++]);
		y = float.Parse (input [first++]);
		z = float.Parse (input [first++]);
		return first;
	}
}

