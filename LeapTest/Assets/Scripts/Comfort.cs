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

		idleStates = DataHandler.instance.getSublist (TrainingUnit.Posture.idle);

		_instance = this;
	}

	public static float getRRPComponent(HandObserver.AngleBasedHandModel otherHand)
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
}
