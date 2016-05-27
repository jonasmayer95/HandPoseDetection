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
	public InputField nameField, evalsInp, itersInp;
	public int iterations = 1;
	public int evaluations = 5;
	void Start () {
		evalsInp.text = evaluations+""; 
		itersInp.text = iterations+"";
		poseDropDown.ClearOptions ();
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData> ();
		foreach (string pose in Enum.GetNames(typeof(TrainingUnit.Posture))) {
			list.Add (new Dropdown.OptionData(pose));
		}
		poseDropDown.AddOptions (list);
		poseDropDown.value = (int)TrainingUnit.Posture.custom1;
		nameField.text = UserStudyData.instance.Name;
	}

	public void onContinueButton()
	{
		saveData ();
		//Load UserStudyIntro
		if (UserStudyData.instance.comfortEvaluation) {
			SceneManager.LoadScene ("UserStudyComfortExample");
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
		UserStudyData.instance.remainingIts = Convert.ToInt32(itersInp.text);
		UserStudyData.instance.evaluations = Convert.ToInt32(evalsInp.text);
	}
}
