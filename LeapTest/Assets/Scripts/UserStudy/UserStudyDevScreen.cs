using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class UserStudyDevScreen : MonoBehaviour {
	public Dropdown poseDropDown;
	public Toggle right;
	public Toggle target, line, rating;
	public InputField nameField;
	public int iterations = 1;
	void Start () {
		poseDropDown.ClearOptions ();
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData> ();
		foreach (string pose in Enum.GetNames(typeof(TrainingUnit.Posture))) {
			list.Add (new Dropdown.OptionData(pose));
		}
		poseDropDown.AddOptions (list);
		poseDropDown.value = (int)UserStudyData.instance.posture;
		nameField.text = UserStudyData.instance.Name;
	}

	public void onContinueButton()
	{
		saveData ();
		//Load UserStudyIntro
		if (UserStudyData.instance.comfortEvaluation) {
			//load scene; 
		} else {
			if (UserStudyData.instance.lineDrawing || UserStudyData.instance.targetShooting)
				SceneManager.LoadScene ("UserStudyIntro");
			else
				Debug.LogError ("Please select a Test Case!");
		}
	}

	void saveData()
	{
		UserStudyData.instance.Name = nameField.text;
		if (UserStudyData.instance.Name == "")
			UserStudyData.instance.Name = "Unnamed";
		UserStudyData.instance.posture = (TrainingUnit.Posture)poseDropDown.value;
		UserStudyData.instance.right = right.isOn;
		UserStudyData.instance.targetShooting = target.isOn;
		UserStudyData.instance.lineDrawing = line.isOn;
		UserStudyData.instance.comfortEvaluation = rating.isOn;
		UserStudyData.instance.remainingIts = iterations;

	}
}
