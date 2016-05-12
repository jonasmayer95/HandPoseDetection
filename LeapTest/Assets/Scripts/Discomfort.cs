using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Discomfort{



    public static float yaxisFac=1, hyperFac=1, interFac =2;

	public static float getDiscomfortAngled(HandObserver.AngleBasedHandModel otherHand)
	{
		float result = 0.0f;

        result += getAbductionComponent(otherHand)          * yaxisFac;
        result += getHyperExtensionComponent(otherHand) * hyperFac;
        result += getInterFingerComponent(otherHand)    * interFac;

		return result;
	}

	//TODO: refine this: split up in comfort/discomfort
    public static float getAbductionComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result =.0f;
        foreach (HandObserver.AngleBasedFingerModel finger in otherHand.fingers)
        {
            float temp = ((Quaternion)finger.mcp).eulerAngles.y;
            if (temp > 180)
                temp = 360 - temp;
            result += temp;
        }
        return result;
    }

	//Definitely Discomfort
    public static float getHyperExtensionComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result = .0f;
        foreach (HandObserver.AngleBasedFingerModel finger in otherHand.fingers)
        {
            if (((Quaternion)finger.mcp).eulerAngles.x > 300)
                result += 360-((Quaternion)finger.mcp).eulerAngles.x;
        }
        return result;
    }

	//Definitely Discomfort
    public static float getInterFingerComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result = .0f;
        for (int i = 0; i < otherHand.fingers.Length - 1; i++)
        {
            result += Mathf.Abs(otherHand.fingers[i].getTotalBending()-otherHand.fingers[i+1].getTotalBending());
        }
        return result;
    }
}
