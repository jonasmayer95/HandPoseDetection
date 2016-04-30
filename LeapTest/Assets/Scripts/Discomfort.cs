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
		idleStates = DataHandler.instance.getSublist (TrainingUnit.Posture.idle);
	}

	public static float getDiscomfortAngled(HandObserver.AngleBasedHandModel otherHand)
	{
		float result = 0.0f;

		foreach (TrainingUnit tu in instance.idleStates) {
			result += otherHand.euclidianDistanceFingers (tu.hand);
		}
		result /= instance.idleStates.Count;

		return result;


	}
}
