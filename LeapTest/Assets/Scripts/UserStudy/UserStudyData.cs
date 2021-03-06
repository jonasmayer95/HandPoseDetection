﻿using UnityEngine;
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
		_instance = this;
	}

	public TrainingUnit.Posture posture = TrainingUnit.Posture.flat;
	public string Name = "Unnamed";
	public string fileEnding = ".csv";
	public bool comfortEvaluation, targetShooting, lineDrawing;
    public float discomfort, angleDis, hyperDis, interDis, yaxisDis;
    public AngleBasedHandModel targetHand;
	public bool right = false;
	public int remainingIts = 0, evaluations = 0;
	public float palmangle=0;
    public int ComfortEvaluation = 0;
}
