using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Discomfort{



    public static float yaxisFac=2, hyperFac=2, interFac =1f;

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
			result += Mathf.Max(0, temp-20);
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

	//Definitely Discomfort
    public static float getInterFingerComponent(AngleBasedHandModel otherHand)
    {
        float result = .0f;
        for (int i = 0; i < otherHand.fingers.Length - 1; i++)
        {
            result += Mathf.Abs(otherHand.fingers[i].getTotalBending()-otherHand.fingers[i+1].getTotalBending());
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
