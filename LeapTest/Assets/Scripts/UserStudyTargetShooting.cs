using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;

public class UserStudyTargetShooting : MonoBehaviour {

	public Transform[] references;
	private List<Transform> origins = new List<Transform> ();
	private List<Transform> directions = new List<Transform> ();
	public Text progress;
	public GameObject[] targets;
	public Text countdownNumber;
	public GameObject startPanel, endPanel;
	public LineRenderer lr;
	public HandObserver hand;
	public LayerMask mask;
	bool playing = false;
	float timer;
	int remainingTargets;
	public int numTargets, parallelNumTargets;
	int current;
	string fileName;
	public Vector3 rayOrigin {
		get{
			Vector3 result = Vector3.zero;
			if (origins.Count > 0) {
				for (int i = 0; i < origins.Count; i++)
					result += origins [i].position;
				result /= origins.Count;
			} else
				result = references [0].position;
			return result;
		}
	}

	public Vector3 rayDirection{
		get{
			Vector3 result = Vector3.forward;
			if (origins.Count > 0) {
				for (int i = 0; i < origins.Count; i++)
					result += origins [i].forward;
				result /= origins.Count;
			} else
				result = references [0].forward;
			return result;
		}
	}
	// Use this for initialization
	void Start () {
		fileName ="TargetShooting_"+ UserStudyData.instance.Name+UserStudyData.instance.fileEnding;
		File.AppendAllText (fileName, System.DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss: "+Environment.NewLine));
		remainingTargets = numTargets;
		for(int i = 0; i<references.Length; i++)
		{
			if (UserStudyData.instance.origins [i])
				origins.Add (references [i]);
			if (UserStudyData.instance.directions [i])
				directions.Add (references [i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(hand.currentPosture !=UserStudyData.instance.posture)
		{
			progress.text = "Wrong Hand Posture!";
		}
		else
			progress.text = remainingTargets+" of "+numTargets+" targets remaining.";
		
		lr.SetPositions(new Vector3[] {rayOrigin,rayOrigin+10*rayDirection});
		if (playing) 
		{

			timer += Time.deltaTime;
			if (Input.GetButtonDown ("Fire1") && hand.currentPosture == UserStudyData.instance.posture) 
			{
				RaycastHit hit;
				if (Physics.Raycast (rayOrigin, rayDirection, out hit, 10, mask)) {
					File.AppendAllText (fileName," Hit at " + timer + " with precision: " + (hit.point - hit.collider.transform.position).magnitude +" at a discomfort of " + hand.getDiscomfort()+Environment.NewLine);
					Debug.Log ("Hit at " + timer + " with precision: " + (hit.point - hit.collider.transform.position).magnitude +" at a discomfort of " + hand.getDiscomfort());
					remainingTargets--;
					if (remainingTargets > 0) {
						setRandTargetActive ();
						hit.collider.gameObject.SetActive (false);
					} else
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
			foreach (GameObject target in targets)
				target.SetActive (false);

			current = UnityEngine.Random.Range (0, targets.Length - 1);
			for (int i = 0; i < parallelNumTargets; i++) {
				setRandTargetActive ();
			}
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

	void setRandTargetActive()
	{
		while(targets[current].activeInHierarchy)
			current = UnityEngine.Random.Range (0, targets.Length-1);
		targets [current].SetActive (true);
	}

	void endStudy()
	{
		playing = false;
		endPanel.SetActive (true);
		progress.enabled = false;
	}
}
