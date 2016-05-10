using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Discomfort{

	public List<TrainingUnit> idleStates;

    public float angleFac=1, yaxisFac=1, hyperFac=1, interFac =2;

	private static Discomfort _instance;
	public static Discomfort instance
	{
		get
		{
			if (_instance == null)
				_instance = new Discomfort ();
			return _instance;
		}
	}
		
	public Discomfort()
	{
		if (_instance != null)
			return;
		
		idleStates = DataHandler.instance.getSublist (TrainingUnit.Posture.idle);

		_instance = this;
	}

	public static float getDiscomfortAngled(HandObserver.AngleBasedHandModel otherHand)
	{
		float result = 0.0f;

        result += getAngledComponent(otherHand)         * instance.angleFac;
        result += getYAxisComponent(otherHand)          * instance.yaxisFac;
        result += getHyperExtensionComponent(otherHand) * instance.hyperFac;
        result += getInterFingerComponent(otherHand)    * instance.interFac;

		return result;
	}

    public static float getAngledComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result = 0.0f;
		if (instance.idleStates == null)
			Debug.LogError ("Idle states not initialized?");
		else
		{
            foreach (TrainingUnit tu in instance.idleStates)
            {
			    result += otherHand.euclidianDistanceFingers (tu.hand);
		    }
		}
        result /= instance.idleStates.Count;
        return result;
    }

    public static float getYAxisComponent(HandObserver.AngleBasedHandModel otherHand)
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
