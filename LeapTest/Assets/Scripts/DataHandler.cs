using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class DataHandler{
	
	private static DataHandler _instance;
	public string filePath;
	public string fileName = "test";
	public string fileEnding = ".jns";
	private List<TrainingUnit> trainingData;
	BinaryFormatter formatter;
	FileStream file; 

	public static DataHandler instance
	{
		get
		{
			if (_instance == null)
				_instance = new DataHandler ();
			return _instance;
		}
	}

	public DataHandler()
	{
		if (_instance != null)
			return;
		_instance = this;
		formatter = new BinaryFormatter ();
		loadData ();
	}

	void loadData()
	{
		trainingData = new List<TrainingUnit> ();

		if (File.Exists (filePath + fileName + fileEnding)) {
			file = File.Open (filePath + fileName + fileEnding, FileMode.Open);
			trainingData = (List<TrainingUnit>) formatter.Deserialize (file);
			file.Close ();
			Debug.Log ("Loaded "+trainingData.Count+" Elements.");
			debugPoseCounts ();
		}
	}

	public void addTrainigData (TrainingUnit data)
	{
		trainingData.Add (data);
		Debug.Log ("Added "+ data.ToString());
	}

	public void saveData()
	{
		file = File.Create (filePath+fileName+fileEnding);
		formatter.Serialize (file, trainingData);
		file.Close ();
		Debug.Log ("Saved "+ trainingData.Count+" Elements.");
		debugPoseCounts ();
	}
	public List<ThreadedKNN.poseCompareObject> getCompareList(HandObserver.AngleBasedHandModel hand)
	{
		List<ThreadedKNN.poseCompareObject> result = new List<ThreadedKNN.poseCompareObject> ();
		foreach (TrainingUnit tu in trainingData) {
			result.Add (tu.compare(hand));
		}
		return result;
	}

	public int getK()
	{
		return (int) Mathf.Sqrt (trainingData.Count);
	}

	public int[] getPoseCounts()
	{
		int[] result = new int[Enum.GetNames(typeof(TrainingUnit.Posture)).Length];
		foreach (TrainingUnit tu in trainingData)
			result [(int)tu.posture]++;
		return result;
	}

	public string getPoseCountsString()
	{
		int[] counts = getPoseCounts ();
		string result = "Number of TrainingUnits: ";
		string[] poses = Enum.GetNames(typeof(TrainingUnit.Posture));
		for (int i = 0; i<counts.Length; i++) {
			result += poses [i] + ": " + counts [i]+"; ";  
		}
		return result;
	}

	public void debugPoseCounts()
	{
		Debug.Log (getPoseCountsString ());
	}
}
