using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Discomfort{

	public List<TrainingUnit> idleStates;

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
		result /= idleStates.Count;
        result += getYAxisComponent(otherHand);
        Debug.Log("YAxisComponent: "+getYAxisComponent(otherHand));
        result+= getHyperExtensionComponent(otherHand);
        Debug.Log("HyperExtenstion: "+getHyperExtensionComponent(otherHand));
        result += getInterFingerComponent(otherHand);
        Debug.Log("InterFingerangle: " + getInterFingerComponent(otherHand));

		return result;
	}

    public float getYAxisComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result =.0f;
        foreach (HandObserver.AngleBasedFingerModel finger in otherHand.fingers)
        {
            result += ((Quaternion)finger.mcp).eulerAngles.y;
        }
        return result;
    }
    public float getHyperExtensionComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result = .0f;
        foreach (HandObserver.AngleBasedFingerModel finger in otherHand.fingers)
        {
            if (((Quaternion)finger.mcp).eulerAngles.x>0)
            result += ((Quaternion)finger.mcp).eulerAngles.x;
        }
        return result;
    }
    public float getInterFingerComponent(HandObserver.AngleBasedHandModel otherHand)
    {
        float result = .0f;
        for (int i = 0; i < otherHand.fingers.Length - 1; i++)
        {
            result += Mathf.Abs(otherHand.fingers[i].getTotalBending()+otherHand.fingers[i].getTotalBending());
        }
        return result;
    }
}
