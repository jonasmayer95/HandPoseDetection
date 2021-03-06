﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class PostureDataHandler{
	
	private static PostureDataHandler _instance;
	public string fileFolder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments) + "\\HandPostureData";
	public string filePath
	{
		get
		{
			return fileFolder +"\\";
		}
	}
	public string fileName = "test";
	public string fileEnding = ".jns";
	private List<TrainingUnit> trainingData;
	private AngleBasedHandModel[] randomData; 
	BinaryFormatter formatter;
	FileStream file; 

	public static PostureDataHandler instance
	{
		get
		{
			if (_instance == null)
				_instance = new PostureDataHandler ();
			return _instance;
		}
	}

	public PostureDataHandler()
	{		
		if (_instance != null)
			return;
		formatter = new BinaryFormatter ();
		loadData ();
		_instance = this;
	}

	public void loadData()
	{
		trainingData = new List<TrainingUnit> ();


		if (!Directory.Exists(fileFolder) || !(File.Exists (filePath + fileName + fileEnding))) {
			createUserFolder ();
		}
		else {
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
	public List<ThreadedKNN.poseCompareObject> getCompareList(AngleBasedHandModel hand)
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

	public List<TrainingUnit> getSublist(TrainingUnit.Posture current)
	{
		List<TrainingUnit> result = new List<TrainingUnit>();
		foreach (TrainingUnit tu in trainingData) {
			if (tu.posture == current)
				result.Add (tu);
		}
		return result;
	}

	public void deleteAll(TrainingUnit.Posture current)
	{
		trainingData.RemoveAll (x => x.posture == current);
		Debug.Log("Deleted all "+current+" training data.");
		saveData ();
	}

	public void delete(TrainingUnit tu)
	{
		trainingData.Remove (tu);
		saveData ();
	}

	void createUserFolder()
	{
		Directory.CreateDirectory (fileFolder);
        if (File.Exists(fileName + fileEnding))
        {
            file = File.Open(fileName + fileEnding, FileMode.Open);
            trainingData = (List<TrainingUnit>)formatter.Deserialize(file);
            file.Close();
            saveData();
        }
        else
        {
        }
	}

	public TrainingUnit getRand()
	{
		return trainingData [UnityEngine.Random.Range (0, trainingData.Count-1)];
	}
	public TrainingUnit getRand(TrainingUnit.Posture post)
	{
		return getSublist(post)[UnityEngine.Random.Range (0, getSublist(post).Count-1)];
	}
}
