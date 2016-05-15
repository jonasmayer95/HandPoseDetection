using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserStudyData {
	
	//singleton structure
	private static UserStudyData _instance;
	public static UserStudyData instance
	{
		get
		{
			if (_instance == null)
				_instance = new UserStudyData ();
			return _instance;
		}
	}
	public UserStudyData()
	{
		if (_instance != null)
			return;
        origins[0] = directions[0] = true;
		_instance = this;
	}

	public TrainingUnit.Posture posture = TrainingUnit.Posture.flat;
	public string Name = "Unnamed";
	public string fileEnding = ".csv";
	public bool[] origins = new bool[6], directions = new bool[6];
	public bool targetShooting, lineDrawing;
    public float discomfort, angleDis, hyperDis, interDis, yaxisDis;
    public AngleBasedHandModel targetHand;
}
