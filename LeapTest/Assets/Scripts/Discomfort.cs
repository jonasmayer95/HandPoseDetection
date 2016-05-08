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
       // Debug.Log("YAxisComponent: "+getYAxisComponent(otherHand));
        result+= getHyperExtensionComponent(otherHand);
       // Debug.Log("HyperExtenstion: "+getHyperExtensionComponent(otherHand));
        result += getInterFingerComponent(otherHand);
       // Debug.Log("InterFingerangle: " + getInterFingerComponent(otherHand));

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
        string debug = "";
        foreach (HandObserver.AngleBasedFingerModel finger in otherHand.fingers)
        {
            debug += ((Quaternion)finger.mcp).eulerAngles.x+ "; ";
            if (((Quaternion)finger.mcp).eulerAngles.x > 300)
                result += 360-((Quaternion)finger.mcp).eulerAngles.x;
        }
      //  Debug.Log(debug);
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
