using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class UserStudyIntro : MonoBehaviour {

	// Use this for initialization
	public Text countdownNumber;
	public Text recordText;
	public HandObserver leftHand, rightHand;
	HandObserver observedHand;
	public float recordDuration;
	public float recordSampleCount;
	Transform palm;
	public Transform palmLeft, palmRight;
	bool playing =false;
	public LineRenderer lr;
    public GameObject handModel;
    public OutputHand outputHand;
    public RandomHandGenerator randHand;
    public AngleBasedHandModel targethand;
	public TrainingUnit.Posture overridePosture;
	public int pointingFinger = 0;
	public GameObject startPanel;
	bool generating = false;

    public Text comfortNumber;
	public Slider comfortSlider;
	int last =0;

	public Vector3 rayOrigin {
		get{
			return palm.position;
		}
	}

	public Vector3 rayDirection{
		get{
			palm.Rotate(new Vector3(UserStudyData.instance.palmangle,0,0), Space.Self);
			Vector3 result = palm.forward;
			palm.Rotate(new Vector3(-UserStudyData.instance.palmangle,0,0), Space.Self);

			return result;
		}
	}

	void Start () {
		UserStudyData.instance.remainingIts--;
        overridePosture = UserStudyData.instance.posture;
		float targetdiscomfort = UnityEngine.Random.Range(0, 1000);
		StartCoroutine (getNewRandomHand(targetdiscomfort));
        if (UserStudyData.instance.right)
            outputHand.transform.localScale = new Vector3(-outputHand.transform.localScale.x, outputHand.transform.localScale.y, outputHand.transform.localScale.z);
		outputHand.visualizeHand(targethand);
	}
	
	// Update is called once per frame
	void Update () {
		if (playing) {
			lr.SetPositions(new Vector3[] {rayOrigin,rayOrigin+10*rayDirection});
		}
		if (startPanel.activeInHierarchy && Input.GetKeyDown (KeyCode.JoystickButton0))
			onContinueButton ();
		if ((int)Input.GetAxisRaw ("PadHor") != last) {
			comfortSlider.value += (int)Input.GetAxisRaw ("PadHor");
			last = (int)Input.GetAxisRaw ("PadHor");
		}
	}

	public void onContinueButton()
	{
		if (!generating) {
			startPanel.SetActive (false);
			UserStudyData.instance.ComfortEvaluation = (int)comfortSlider.value;
			if (UserStudyData.instance.right) {
				observedHand = rightHand;
				palm = palmRight;
			} else {
				observedHand = leftHand;
				palm = palmLeft;
			}
			StartCoroutine (numberCountdown ());
		}
	}


	public void findPointingFinger()
	{
		float min = 100;
		for (int i = 0 ; i< targethand.fingers.Length; i++) {
			Debug.Log (i+": "+targethand.fingers [i].getTotalBending ());
			if (targethand.fingers [i].getTotalBending () < min) {
				pointingFinger = i+2;
				min = targethand.fingers [i].getTotalBending ();
			}
		}
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
		PostureDataHandler.instance.deleteAll (overridePosture);
		for (int i = 0; i < recordSampleCount; i++) {
			if (observedHand.gameObject.activeInHierarchy) {
				countdownNumber.enabled = false;
				observedHand.saveCurrentAs (overridePosture);
				recordText.text = "Recording... Recorded Posture Sample " + i + " of " + recordSampleCount;
			} else {
				countdownNumber.enabled = true;
				countdownNumber.text = "Wrong Hand!";
				countdownNumber.color = Color.red;
				i--;
			}
			yield return new WaitForSeconds (recordDuration/recordSampleCount);
		}
		recordText.text = "Saving Data";
		PostureDataHandler.instance.saveData ();
		recordText.text = "Done";
		yield return new WaitForSeconds (1);
		recordText.enabled = false;
		playing = true;
		lr.enabled = true;
		yield return new WaitForSeconds (2);

		if (UserStudyData.instance.targetShooting)
			SceneManager.LoadScene ("UserStudyTargetShooting");
		else if (UserStudyData.instance.lineDrawing)
			SceneManager.LoadScene ("UserStudyLineTracing");
	}
		
	void saveData()
	{
        UserStudyData.instance.targetHand = targethand;
		UserStudyData.instance.angleDis = Comfort.getRRPComponent(targethand);
        UserStudyData.instance.hyperDis = Discomfort.getHyperExtensionComponent(targethand);
        UserStudyData.instance.yaxisDis = Discomfort.getAbductionComponent(targethand);
        UserStudyData.instance.interDis = Discomfort.getInterFingerComponent(targethand);
		UserStudyData.instance.discomfort = Discomfort.getDiscomfortAngled(targethand);
		UserStudyData.instance.palmangle = targethand.getAvgMCPAngle ()*0;
		Debug.Log (targethand.getAvgMCPAngle ()*0);
    }

	IEnumerator getNewRandomHand(float targetDiscomfort)
	{
		generating = true;
		int counter = 0;
		Debug.Log(targetDiscomfort);
		do{
			targethand = randHand.createRandom();
			outputHand.visualizeHand(targethand);
			yield return new WaitForEndOfFrame();
			counter++;
			if (counter > 200)
			{
				counter = 0;
				targetDiscomfort = UnityEngine.Random.Range(50, 1000);
				Debug.Log(targetDiscomfort);
			}
		}
		while (Discomfort.getDiscomfortAngled(targethand) + Comfort.getRRPComponent(targethand) < targetDiscomfort || Discomfort.getDiscomfortAngled(targethand) + Comfort.getRRPComponent(targethand) > targetDiscomfort+100);
		generating = false;
		saveData ();
	}

    public void changeComfortNumer(float newVal)
    {
        comfortNumber.text = ((int)newVal) + "/10";
    }
}
