using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrainingUnit{

	public enum Posture{idle, flat, fist, grab, pinch, point, bird, metal, crookedPoint, thumbsup, custom1, custom2, custom3} ;
	public Posture posture; //Name of the posture
	public AngleBasedHandModel hand; //actual training data

	public float distance; //distance to current hand posture, used to sort Training Units

	public TrainingUnit(Posture _posture, AngleBasedHandModel _hand)
	{
		posture = _posture;
		hand = _hand;
	}

	public ThreadedKNN.poseCompareObject compare(AngleBasedHandModel otherHand)
	{
		return new ThreadedKNN.poseCompareObject (hand.euclidianDistanceFingers(otherHand),posture);
	}
}
