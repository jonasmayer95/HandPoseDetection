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

		return result;


	}
}
