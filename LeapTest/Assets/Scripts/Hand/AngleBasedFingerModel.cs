using UnityEngine;
using System.Collections;

[System.Serializable]
public class AngleBasedFingerModel
{
	public enum Fingerjoints
	{
		DIP = 0 , PIP = 1, MCP_UP = 2 , MCP_SIDE = 3
	}

	public float[] jointAngles = new float[4];
	public sQuaternion mcp = new sQuaternion();

	public override string ToString()
	{
		return "Finger: DIP " + jointAngles [0] + ", PIP " + jointAngles [1] + ", MCP_UP " + jointAngles [2] + ", MCP_SIDE " + jointAngles [3];
	}

	public float euclidianDistance(AngleBasedFingerModel other)
	{
		float result = 0;
		for (int i = 0; i < jointAngles.Length - 2; i++) {
			result += Mathf.Pow (jointAngles [i] - other.jointAngles [i], 2.0f);
		}
		result += Mathf.Pow(Quaternion.Angle(other.mcp, mcp),2.0f);
		return Mathf.Sqrt (result);
	}

	public float getTotalBending()
	{
		float result=.0f;

		result += jointAngles[0];
		result += jointAngles[1];
		float temp = ((Quaternion)mcp).eulerAngles.x;
		if (temp > 180)
			temp -= 360;
		result += temp;

		return result;
	}

	public string ToCSVString(string endl)
	{
		string result = "";

		for (int i = 0; i < jointAngles.Length - 2; i++)
		{
			result += jointAngles[i] + endl;
		}
		result += mcp.ToCSVString(endl);

		return result;
	}

	public static string getCSVHeader(string endl, string fingername)
	{
		string result = "";

		result += fingername + "DIP" + endl;
		result += fingername + "PIP" + endl;
		result += sQuaternion.getCSVHeader(endl, fingername+"MCP");

		return result;
	}

	public static AngleBasedFingerModel Lerp(AngleBasedFingerModel first, AngleBasedFingerModel second, float t)
	{
		AngleBasedFingerModel result = new AngleBasedFingerModel ();
		t = Mathf.Clamp (t, 0, 1);
		result.mcp = Quaternion.Lerp (first.mcp, second.mcp, t);

		for (int i = 0; i < result.jointAngles.Length; i++) {
			result.jointAngles [i] = Mathf.Lerp (first.jointAngles [i], second.jointAngles [i], t);
		}
		return result;

	}

}
