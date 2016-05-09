using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Discomfort{

	public List<TrainingUnit> idleStates;

    public float angleFac=1, yaxisFac=1, hyperFac=1, interFac =1;

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

	public float getDiscomfortAngled(HandObserver.AngleBasedHandModel otherHand)
	{
		float result = 0.0f;
		if (idleStates == null)
			Debug.LogError ("Penis");
		else
		{
		    foreach (TrainingUnit tu in idleStates) {
			    result += otherHand.euclidianDistanceFingers (tu.hand);
		    }
		}
		result /= idleStates.Count                      *   angleFac;

        result += getYAxisComponent(otherHand)          *   yaxisFac;
        result += getHyperExtensionComponent(otherHand) *   hyperFac;
        result += getInterFingerComponent(otherHand)    *   interFac;

		return result;
	}

    public float getYAxisComponent(HandObserver.AngleBasedHandModel otherHand)
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
    public float getHyperExtensionComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result = .0f;
        foreach (HandObserver.AngleBasedFingerModel finger in otherHand.fingers)
        {
            if (((Quaternion)finger.mcp).eulerAngles.x > 300)
                result += 360-((Quaternion)finger.mcp).eulerAngles.x;
        }
        return result;
    }
    public float getInterFingerComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result = .0f;
        for (int i = 0; i < otherHand.fingers.Length - 1; i++)
        {
            result += Mathf.Abs(otherHand.fingers[i].getTotalBending()-otherHand.fingers[i+1].getTotalBending());
        }
        return result;
    }
}
