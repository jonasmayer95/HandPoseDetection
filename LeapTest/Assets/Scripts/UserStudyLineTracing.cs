using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;

public class UserStudyLineTracing : MonoBehaviour {

	public Transform palm;
	public GameObject startPanel, endPanel;
	public LineRenderer lr;
	public HandObserver hand;
	public OutputHand outputHand;
	public string endl = ", ";
	bool playing = false;
	float timer;
	public Text countdownNumber;
	public Text progress;
	string fileName;
	public List<LinePoint> points;
	public LayerMask lineMask;

	public float maxX=6;
	public float maxY = 4;
	public float steps = 10;
	public float linePlane = 7;
	public GameObject pointPrefab;

	int currentPoint, currentFrame;
	float accuracy = 0; 


	public Vector3 rayOrigin {
		get{
			return palm.position;
		}
	}

	public Vector3 rayDirection{
		get{
			return palm.forward;
		}
	}

	// Use this for initialization
	void Start () {
		generateRandCurve ();
		for (int i = 0; i < points.Count - 1; i++) {
			points [i].init (points[i+1].getOwn());
		}
//		outputHand.visualizeHand(UserStudyData.instance.targetHand);
		fileName ="LineTracingData"+UserStudyData.instance.fileEnding;
		if(!File.Exists(fileName))
			File.AppendAllText(fileName, "Name" + endl + "Discomfort" + endl + "Time" + endl + "Accuracy" + endl + "Posture" + endl + "AngleDis" + endl + "InterDis" + endl + "YAxisDis" + endl + "HyperDis" + endl + AngleBasedHandModel.getCSVHeader(endl, "ActualHand") + endl + AngleBasedHandModel.getCSVHeader(endl, "GivenHand") + Environment.NewLine);
	}
	
	// Update is called once per frame
	void Update () {
		if(hand.currentPosture !=UserStudyData.instance.posture)
		{
			progress.text = "Wrong Hand Posture!";
		}
		else
			progress.text = "Dist: "+accuracy +", Progress: "+getProgress();
		
		lr.SetPositions(new Vector3[] {rayOrigin,rayOrigin+10*rayDirection});

		if (playing) {
			timer += Time.deltaTime;
			if (hand.currentPosture == UserStudyData.instance.posture) {

				RaycastHit hit;
				if (Physics.Raycast (rayOrigin, rayDirection, out hit, 10, lineMask)) {
					accuracy = getDistanceToCurve (hit.point);
					if (isDone ())
						endStudy ();
				}
			}
		}
	}

	public void onContinue()
	{
		if (gameObject.activeInHierarchy) {
			startPanel.SetActive (false);
			progress.enabled = true;



			StartCoroutine (numberCountdown ());
		}
	}

	IEnumerator numberCountdown()
	{
		countdownNumber.enabled = true;
		for (int i = 3; i > 0; i--) {
			countdownNumber.text = ""+i;
			yield return new WaitForSeconds (1);
		}
		yield return new WaitForSeconds (1);
		countdownNumber.enabled = false;
		playing = true;
	}

	void endStudy()
	{
		File.AppendAllText(
			fileName, 

			UserStudyData.instance.Name+endl+
			UserStudyData.instance.discomfort + endl +
			timer + endl +
			getTotalAccuracy() + endl +
			hand.currentPosture + endl +
			UserStudyData.instance.angleDis + endl +
			UserStudyData.instance.interDis + endl +
			UserStudyData.instance.yaxisDis + endl +
			UserStudyData.instance.hyperDis + endl +
			hand.hand.ToCSVString(endl) + endl +
			UserStudyData.instance.targetHand.ToCSVString(endl) + Environment.NewLine
		);
		playing = false;
		endPanel.SetActive (true);
		progress.enabled = false;
	}

	public void onEnd()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	public void onOpenLog()
	{
		System.Diagnostics.Process.Start(fileName);
	}
	public float getDistanceToCurve(Vector3 point)
	{
		int index = -1;
		float min = float.PositiveInfinity;
		for (int i = 0; i < points.Count - 1; i++) {
			points [i].checkCompletition (point);
			float temp = points [i].getLineDistance (point);
			if (temp < min) {
				min = temp;
				index = i;
			}
		}
		points [index].addSample (min);
		return min;
	}

	bool isDone()
	{
		bool result = true;
		for (int i = 0; i < points.Count - 1; i++) {
			result = result && points [i].isDone ();
		}
		return result;
	}

	public float getTotalAccuracy()
	{
		double result =0;
		int samples=0;
		for (int i = 0; i < points.Count - 1; i++) {
			samples += points [i].samples;
			result += points [i].accuracy;
		}
		result /= samples;
		Debug.Log ("Total Acc: "+result);
		return (float)result;
	}
	public float getProgress()
	{
		float result =0;
		for (int i = 0; i < points.Count - 1; i++) {
			if (points [i].isDone())
				result+=1;
		}
		result /= points.Count - 1;

		return result;
	}

	public void generateRandCurve ()
	{
		float pointX = -maxX;
		float pointY = 0;
		for (int i = 0; i < steps; i++) {
			GameObject point = (GameObject)Instantiate (pointPrefab, new Vector3 (pointX, pointY, linePlane), Quaternion.identity);
			points.Add (point.GetComponent<LinePoint>());
			pointX += (2 * maxX) / steps;
			pointY += UnityEngine.Random.Range ((2 * maxX) / steps,-(2 * maxX) / steps)+(pointY/maxY*0.4f);
		}
	}
}
