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
		Debug.Log (result);
		return result;
	}
}
