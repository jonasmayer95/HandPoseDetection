using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Discomfort{



    public static float yaxisFac=2, hyperFac=2, interFac =1.5f;
    static float ringBonus = 1.3f;

	public static float getDiscomfortAngled(AngleBasedHandModel otherHand)
	{
		float result = 0.0f;

        result += getAbductionComponent(otherHand)          * yaxisFac;
        result += getHyperExtensionComponent(otherHand) * hyperFac;
        result += getInterFingerComponent(otherHand)    * interFac;
		return result;
	}

	//TODO: refine this: split up in comfort/discomfort
    public static float getAbductionComponent(AngleBasedHandModel otherHand)
    {
        float result =.0f;
        foreach (AngleBasedFingerModel finger in otherHand.fingers)
        {
            float temp = ((Quaternion)finger.mcp).eulerAngles.y;
            if (temp > 180)
                temp = 360 - temp;
            result += temp;
        }

		return result;
    }

    public static float[] getAbduction(AngleBasedHandModel otherHand)
    {
        float[] result = new float[otherHand.fingers.Length];
        for (int i = 0; i < otherHand.fingers.Length; i++)
        {
            float temp = ((Quaternion)otherHand.fingers[i].mcp).eulerAngles.y;
            if (temp > 180)
                temp = 360 - temp;
            result[i] = temp;
        }

        return result;
    }

    public static string getAbductionCSVHeader(string endl)
    {
        string result="";

        string[] fingers = System.Enum.GetNames(typeof(AngleBasedHandModel.FingerName));
        foreach (string finger in fingers)
        {
            result += "Abduction" + finger + endl;
        }

        return result;
    }

    public static string getAbductionCSV(AngleBasedHandModel otherHand, string endl)
    {
        string result = "";

        float[] fingers = getAbduction(otherHand);
        foreach (float finger in fingers)
        {
            result += finger + endl;
        }

        return result;
    }

	//Definitely Discomfort
    public static float getHyperExtensionComponent(AngleBasedHandModel otherHand)
    {
        float result = .0f;
        foreach (AngleBasedFingerModel finger in otherHand.fingers)
        {
            if (((Quaternion)finger.mcp).eulerAngles.x > 300)
                result += 360-((Quaternion)finger.mcp).eulerAngles.x;
        }
        return result;
    }
    public static float[] getHyperExtension(AngleBasedHandModel otherHand)
    {
        float[] result = new float[otherHand.fingers.Length];
        for (int i = 0; i < otherHand.fingers.Length; i++)
        {
            if (((Quaternion)otherHand.fingers[i].mcp).eulerAngles.x > 300)
                result[i] = 360 - ((Quaternion)otherHand.fingers[i].mcp).eulerAngles.x;
            else
                result[i] = 0;
        }
        return result;
    }
    public static string getHyperExtensionCSVHeader(string endl)
    {
        string result = "";

        string[] fingers = System.Enum.GetNames(typeof(AngleBasedHandModel.FingerName));
        foreach (string finger in fingers)
        {
            result += "Hyper" + finger + endl;
        }

        return result;
    }
    public static string getHyperExtensionCSV(AngleBasedHandModel otherHand,string endl)
    {
        string result = "";

        float[] fingers = getHyperExtension(otherHand);
        foreach (float finger in fingers)
        {
            result += finger + endl;
        }

        return result;
    }

	//obsolete
    public static float getInterFingerComponent(AngleBasedHandModel otherHand)
    {
        float result = .0f;
        for (int i = 0; i < otherHand.fingers.Length - 1; i++)
        {
            result += Mathf.Abs(otherHand.fingers[i].getTotalBending()-otherHand.fingers[i+1].getTotalBending());
        }
			
        result += (ringBonus - 1) * Mathf.Abs((otherHand.fingers[(int)AngleBasedHandModel.FingerName.middle].getTotalBending() - otherHand.fingers[(int)AngleBasedHandModel.FingerName.ring].getTotalBending()) - (otherHand.fingers[(int)AngleBasedHandModel.FingerName.ring].getTotalBending() - otherHand.fingers[(int)AngleBasedHandModel.FingerName.pinky].getTotalBending()));

        return result;
    }

    public static float getInterFinger(AngleBasedHandModel.FingerName finger, AngleBasedHandModel otherHand)
    {
        return getInterFinger(otherHand)[(int)finger];
    }

    public static float[] getInterFinger(AngleBasedHandModel otherHand)
    {
        float[] result = new float[otherHand.fingers.Length];

        float[] interFingerAngle = new float[otherHand.fingers.Length-1];

        for (int i = 0; i < interFingerAngle.Length; i++)
        {
            interFingerAngle[i] = otherHand.fingers[i].getTotalBending() - otherHand.fingers[i + 1].getTotalBending();
        }

        result[0] = Mathf.Abs(interFingerAngle[0]);
        result[1] = Mathf.Abs(interFingerAngle[1] - interFingerAngle[0]);
        result[2] = Mathf.Abs(interFingerAngle[2] - interFingerAngle[1]);
        result[3] = Mathf.Abs(interFingerAngle[2]);

        return result;
    }

    public static string getInterFingerCSVHeader(string endl)
    {
        string result = "";

        string[] fingers = System.Enum.GetNames(typeof(AngleBasedHandModel.FingerName));
        foreach (string finger in fingers)
        {
            result += "Inter" + finger + endl;
        }

        return result;
    }
    public static string getInterFingerCSV(AngleBasedHandModel otherHand, string endl)
    {
        string result = "";

        float[] fingers = getInterFinger(otherHand);
        foreach (float finger in fingers)
        {
            result += finger + endl;
        }

        return result;
    }

	static void debug(AngleBasedHandModel otherHand)
	{
		string debug = "Abduction: " + getAbductionComponent (otherHand);
		debug += "Hyper: " + getHyperExtensionComponent (otherHand);
		debug += "Inter: " + getInterFingerComponent (otherHand);
		Debug.Log (debug);
	}
}
