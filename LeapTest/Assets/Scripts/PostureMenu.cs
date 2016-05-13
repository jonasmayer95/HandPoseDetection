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
	public HandObserver observedHand;
	int index;
    public OutputHand outputHand;
	// Use this for initialization
	void Start () {
		dropDown.ClearOptions ();
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData> ();
		foreach (string pose in Enum.GetNames(typeof(TrainingUnit.Posture))) {
			list.Add (new Dropdown.OptionData(pose));
		}
		dropDown.AddOptions (list);
		fileName.text = DataHandler.instance.fileName;
		onChangePose ();
	}
	
	// Update is called once per frame
	void Update () {
        AngleBasedHandModel currentHand = currentList[index].hand;
		if (Input.GetButton("Fire2")&&observedHand.gameObject.activeInHierarchy)
        {
            currentHand = observedHand.hand;
            Debug.Log("Observing");
        }
        outputHand.visualizeHand(currentHand);
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

	public void onBack()
	{
		index--;
		index %= currentList.Count;
	}

	public void onDelete()
	{
		DataHandler.instance.delete (currentList[index]);
		currentList.RemoveAt (index);
		index %= currentList.Count;
	}

	public void onChangePose()
	{
		current = (TrainingUnit.Posture)dropDown.value;
		currentList = DataHandler.instance.getSublist (current);
		index = 0;
		Debug.Log("Got "+currentList.Count+" elements.");
	}

	public void onDeleteAll()
	{
		DataHandler.instance.deleteAll (current);
	}
}
