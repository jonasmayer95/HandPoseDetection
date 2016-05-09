using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

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
	public InputField nameField;
    public GameObject handModel;
    public OutputHand outputHand;
    public RandomHandGenerator randHand;
    public HandObserver.AngleBasedHandModel targethand;

	void Start () {
        overridePosture = UserStudyData.instance.posture; 
		poseDropDown.ClearOptions ();
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData> ();
		foreach (string pose in Enum.GetNames(typeof(TrainingUnit.Posture))) {
			list.Add (new Dropdown.OptionData(pose));
		}
		poseDropDown.AddOptions (list);
		poseDropDown.value = (int)overridePosture;

        for (int i = 0; i < UserStudyData.instance.origins.Length; i++)
        {
            rayOrigin[i].isOn = UserStudyData.instance.origins[i];
            rayDirection[i].isOn = UserStudyData.instance.directions[i];
        }
        nameField.text = UserStudyData.instance.Name;

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
        handModel.SetActive(true);
        float targetdiscomfort = UnityEngine.Random.Range(100, 800);
        targethand = randHand.createRandom(targetdiscomfort, targetdiscomfort+100);
        outputHand.visualizeHand(targethand);
        UserStudyData.instance.discomfort = Discomfort.instance.getDiscomfortAngled(targethand);
		saveData ();
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
		yield return new WaitForSeconds (2);
        SceneManager.LoadScene(1);
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
	void saveData()
	{
		UserStudyData.instance.Name = nameField.text;
        if (UserStudyData.instance.Name == "")
            UserStudyData.instance.Name = "Unnamed";
		UserStudyData.instance.posture = overridePosture;
		for (int i = 0; i < rayReference.Length; i++) {
			UserStudyData.instance.origins [i] = rayOrigin [i].isOn;
			UserStudyData.instance.directions [i] = rayDirection [i].isOn;
		}
	}
}
