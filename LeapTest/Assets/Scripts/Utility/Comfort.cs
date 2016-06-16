using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Comfort{

	public List<TrainingUnit> idleStates;

	private static Comfort _instance;
	public static Comfort instance
	{
		get
		{
			if (_instance == null)
				_instance = new Comfort ();
			return _instance;
		}
	}

	public Comfort()
	{
		if (_instance != null)
			return;

		idleStates = PostureDataHandler.instance.getSublist (TrainingUnit.Posture.idle);

		_instance = this;
	}

	public static float getRRPComponent(AngleBasedHandModel otherHand)
	{
		float result = float.PositiveInfinity;
		if (instance.idleStates == null) {
			Debug.LogError ("Idle states not initialized?");
			return -1;
		}
		
		else
		{
			foreach (TrainingUnit tu in instance.idleStates)
			{
				if(result > otherHand.euclidianDistanceFingers (tu.hand))
					result = otherHand.euclidianDistanceFingers (tu.hand);
			}
		}
		return result;
	}

    public static float[] getRRP(AngleBasedHandModel otherHand)
    {
        float[] result = new float[otherHand.fingers.Length+1];
        for (int i = 0; i < result.Length; i++)
            result[i] = float.PositiveInfinity;
        if (instance.idleStates == null)
        {
            Debug.LogError("Idle states not initialized?");
            return result;
        }

        else
        {
            foreach (TrainingUnit tu in instance.idleStates)
            {
              //  if (result > otherHand.euclidianDistanceFingers(tu.hand))
               //     result = otherHand.euclidianDistanceFingers(tu.hand);
                for (int i = 0; i < otherHand.fingers.Length; i++)
                {
                    if (result[i] > otherHand.fingers[i].euclidianDistance(tu.hand.fingers[i]))
                        result[i] = otherHand.fingers[i].euclidianDistance(tu.hand.fingers[i]);
                }
                if (result[otherHand.fingers.Length] > otherHand.thumb.euclidianDistance(tu.hand.thumb))
                    result[otherHand.fingers.Length] = otherHand.thumb.euclidianDistance(tu.hand.thumb);
            }
        }
        return result; 
    }

    public static string getRRPCSVHeader(string endl)
    {
        string result = "";

        string[] fingers = System.Enum.GetNames(typeof(AngleBasedHandModel.FingerName));
        foreach (string finger in fingers)
        {
            result += "RRP" + finger + endl;
        }
        result += "RRPThumb" + endl;

        return result;
    }
    public static string getRRPCSV(AngleBasedHandModel otherHand, string endl)
    {
        string result = "";

        float[] fingers = getRRP(otherHand);
        foreach (float finger in fingers)
        {
            result += finger + endl;
        }

        return result;
    }
}
