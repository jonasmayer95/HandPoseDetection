using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrainingUnit{

	public enum Posture{idle, flat, fist, grab, pinch, point, bird, metal, crookedPoint, thumbsup} ;
	public Posture posture; //Name of the posture
	public HandObserver.AngleBasedHandModel hand; //actual training data

	public float distance; //distance to current hand posture, used to sort Training Units

	public TrainingUnit(Posture _posture, HandObserver.AngleBasedHandModel _hand)
	{
		posture = _posture;
		hand = _hand;
	}

	public ThreadedKNN.poseCompareObject compare(HandObserver.AngleBasedHandModel otherHand)
	{
		return new ThreadedKNN.poseCompareObject (hand.euclidianDistanceFingers(otherHand),posture);
	}
}
