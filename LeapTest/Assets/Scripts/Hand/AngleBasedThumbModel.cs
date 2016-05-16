using UnityEngine;
using System.Collections;

[System.Serializable]
public class AngleBasedThumbModel
{
	public enum Fingerjoints
	{
		IP = 0 , MP = 1, TMC_X = 2 , TMC_Y = 3, TMC_Z = 4
	}
	public float[] jointAngles = new float[5];
	public sQuaternion tmc = new sQuaternion();
	public override string ToString()
	{
		return "Thumb: IP " + jointAngles [0] + ", MP " + jointAngles [1] + ", TMC_X " + jointAngles [2] + ", TMC_Y " + jointAngles [3]+ ", TMC_Z " + jointAngles [4];
	}
	public float euclidianDistance(AngleBasedThumbModel other)
	{
		float result = 0;
		for (int i = 0; i < jointAngles.Length-3; i++)
			result += Mathf.Pow (jointAngles [i] - other.jointAngles [i], 2.0f);
		result += Mathf.Pow(Quaternion.Angle(other.tmc, tmc), 2.0f);
		return Mathf.Sqrt (result);
	}

	public string ToCSVString(string endl)
	{
		string result = "";

		for (int i = 0; i < jointAngles.Length - 3; i++)
		{
			result += jointAngles[i] + endl;
		}
		result += tmc.ToCSVString(endl);

		return result;
	}
	public static string getCSVHeader(string endl, string thumbname)
	{
		string result = "";

		result += thumbname + "IP" + endl;
		result += thumbname + "MP" + endl;
		result += sQuaternion.getCSVHeader(endl, thumbname + "TMC");

		return result;
	}

}
