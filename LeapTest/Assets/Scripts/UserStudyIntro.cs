using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UserStudyIntro : MonoBehaviour {

	// Use this for initialization
	public TrainingUnit.Posture overridePosture = TrainingUnit.Posture.custom1;
	public GameObject startPanel, devPanel;
	public Text countdownNumber;
	public Text recordText;
	public HandObserver observedHand;
	public float recordDuration;
	public float recordSampleCount;
	public Dropdown poseDropDown;
	public Toggle[] rayOrigin, rayDirection;
	public Transform[] rayReference;
	public Vector3 rayOriginVec, rayDirectionVec;
	bool playing =false;
	public LineRenderer lr;

	void Start () {
		poseDropDown.ClearOptions ();
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData> ();
		foreach (string pose in Enum.GetNames(typeof(TrainingUnit.Posture))) {
			list.Add (new Dropdown.OptionData(pose));
		}
		poseDropDown.AddOptions (list);
		poseDropDown.value = (int)overridePosture;
	}
	
	// Update is called once per frame
	void Update () {
		if (playing) {
			updateRayCoords ();
			lr.SetPositions(new Vector3[] {rayOriginVec,rayOriginVec+10*rayDirectionVec});
		}
	}

	public void onUserContinueButton()
	{
		startPanel.SetActive (false);
		updateRayCoords ();
		StartCoroutine (numberCountdown ());
	}
	public void onDevContinueButton()
	{
		overridePosture = (TrainingUnit.Posture)poseDropDown.value;
		devPanel.SetActive (false);
		startPanel.SetActive (true);
	}

	IEnumerator numberCountdown()
	{

		countdownNumber.enabled = true;
		for (int i = 3; i > 0; i--) {
			countdownNumber.text = ""+i;
			yield return new WaitForSeconds (1);
		}
		countdownNumber.text = "Recording...";
		countdownNumber.color = Color.red;
		StartCoroutine (record ());
		yield return new WaitForSeconds (1);
		countdownNumber.enabled = false;
	}

	IEnumerator record()
	{
		recordText.enabled = true;
		DataHandler.instance.deleteAll (overridePosture);
		for (int i = 0; i < recordSampleCount; i++) {
			observedHand.saveCurrentAs (overridePosture);
			recordText.text = "Recording... Recorded Posture Sample " + i + " of " + recordSampleCount;
			yield return new WaitForSeconds (recordDuration/recordSampleCount);
		}
		recordText.text = "Saving Data";
		DataHandler.instance.saveData ();
		recordText.text = "Done";
		yield return new WaitForSeconds (1);
		recordText.enabled = false;
		playing = true;
		lr.enabled = true;
	}

	void updateRayCoords()
	{
		rayOriginVec = new Vector3 ();
		rayDirectionVec = new Vector3 (); 
		int or = 0;
		for (int i = 0; i < rayReference.Length; i++) {
			if (rayOrigin [i].isOn) {or++; rayOriginVec += rayReference [i].position; }
			if (rayDirection [i].isOn) {rayDirectionVec += rayReference [i].forward; }
		}
		rayDirectionVec.Normalize ();
		rayOriginVec /= or;
	}

}
