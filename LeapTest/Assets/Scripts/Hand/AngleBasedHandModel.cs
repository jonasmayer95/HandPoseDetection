using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AngleBasedHandModel
{
	public enum FingerName
	{
		index = 0 , middle = 1, ring = 2 , pinky = 3
	}
	public AngleBasedThumbModel thumb;
	public AngleBasedFingerModel[] fingers = new AngleBasedFingerModel[4];
	public sQuaternion rotation;
	public sVector3 position;

	public AngleBasedHandModel()
	{
		rotation = new sQuaternion ();
		position = new sVector3 (0, 0, 0);
		thumb = new AngleBasedThumbModel();
		for(int i =0 ; i<fingers.Length ; i++)
			fingers[i] = new AngleBasedFingerModel();
	}

	public override string ToString()
	{
		string result = "Root: Rot" + ((Quaternion)rotation).eulerAngles.ToString() + ", Pos" + position.ToString();
		result = result + " " + thumb.ToString();
		for (int i = 0; i < fingers.Length; i++)
			result += "; "+i+"." + fingers [i].ToString();
		return result;
	}

	public float euclidianDistance(AngleBasedHandModel other)
	{
		float result = Mathf.Pow (euclidianDistanceFingers (other), 2.0f);
		//TODO: make usefull as soon as i need
		return Mathf.Sqrt (result);
	}
	public float euclidianDistanceFingers(AngleBasedHandModel other)
	{
		float result = Mathf.Pow(thumb.euclidianDistance(other.thumb),2.0f);
		for (int i = 0; i < fingers.Length; i++) {
			result += Mathf.Pow (fingers [i].euclidianDistance (other.fingers [i]), 2);
		}
		return Mathf.Sqrt (result);
	}

	public float getAvgMCPAngle()
	{
		float result = 0;
		for (int i = 0; i < fingers.Length; i++) {
			float temp = ((Quaternion)fingers [i].mcp).eulerAngles.x;
			if (temp >= 180)
				temp -= 360;
			result += temp;
		}
		result /= fingers.Length;
		return result;
	}


	public string ToCSVString(string endl)
	{
		string result = "";

		result += thumb.ToCSVString(endl) + endl;
		for (int i = 0; i < fingers.Length; i++)
		{
			result += fingers[i].ToCSVString(endl)+endl;
		}
		result += rotation.ToCSVString(endl) + endl;
		result += position.ToCSVString(endl);

		return result;
	}
	public static string getCSVHeader(string endl, string handname)
	{
		string result = "";

		result += AngleBasedThumbModel.getCSVHeader(endl, handname+"Thumb") + endl;
		for (int i = 0; i < System.Enum.GetNames(typeof(FingerName)).Length; i++)
		{
			result += AngleBasedFingerModel.getCSVHeader(endl,handname+System.Enum.GetNames(typeof(FingerName))[i]) + endl;
		}
		result += sQuaternion.getCSVHeader(endl, handname + "Rot") + endl;
		result += sVector3.getCSVHeader(endl, handname + "Pos");

		return result;
	}
	public int parseCSV(string[] input, int first)
	{
		first = thumb.FromCSV (input, first);
		for (int i = 0; i < fingers.Length; i++)
		{
			first = fingers [i].FromCSV (input, first);
		}
		first = rotation.FromCSV (input, first);
		first = position.FromCSV (input, first);
		return first;
	}

	public static AngleBasedHandModel Lerp(AngleBasedHandModel first, AngleBasedHandModel second, float t)
	{
		AngleBasedHandModel result = new AngleBasedHandModel ();
		t = Mathf.Clamp (t, 0, 1);
		result.rotation = Quaternion.Lerp (first.rotation, second.rotation, t);
		result.position = Vector3.Lerp (first.position, second.position, t);

		for (int i = 0; i < result.fingers.Length; i++) {
			result.fingers [i] = AngleBasedFingerModel.Lerp (first.fingers [i], second.fingers [i], t);
		}
		result.thumb =AngleBasedThumbModel.Lerp (first.thumb, second.thumb, t);
		return result;

	}
}