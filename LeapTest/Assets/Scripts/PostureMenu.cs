using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PostureMenu : MonoBehaviour {

	public InputField fileName;
	public Dropdown dropDown;
	public Button next;
	TrainingUnit.Posture current;
	List<TrainingUnit> currentList;
	int index;
	// Use this for initialization
	void Start () {
		dropDown.ClearOptions ();
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData> ();
		foreach (string pose in Enum.GetNames(typeof(TrainingUnit.Posture))) {
			list.Add (new Dropdown.OptionData(pose));
		}
		dropDown.AddOptions (list);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeFileName()
	{
		DataHandler.instance.fileName = fileName.text;
		DataHandler.instance.loadData ();
	}
	public void onNext()
	{
		index++;
		index %= currentList.Count;
	}

	public void onChangePose()
	{
		current = (TrainingUnit.Posture)dropDown.value;
		currentList = DataHandler.instance.getSublist (current);
		index = 0;
		Debug.Log("Got "+currentList.Count+" elements.");
	}
}
