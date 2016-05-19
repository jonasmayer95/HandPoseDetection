using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class UserStudyTargetShooting : MonoBehaviour {
	
	Transform palm;
	public Transform palmLeft, palmRight;
	public Text progress;
	public GameObject[] targets;
    List<int> targetList;
	public Text countdownNumber;
	public GameObject startPanel, endPanel;
	public LineRenderer lr;	
	HandObserver hand;
	public HandObserver leftHand, rightHand;
	public LayerMask mask;
    public OutputHand outputHand;
    public string endl = ", ";
	bool playing = false;
	float timer;
	int remainingTargets;
	public int numTargets, parallelNumTargets;
	int current;
	string fileName;

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

		if (UserStudyData.instance.right) {
			hand = rightHand;
			palm = palmRight;
		} else {
			hand = leftHand;
			palm = palmLeft;
		}
		fileName ="TargetShootingData"+UserStudyData.instance.fileEnding;
		if(!File.Exists(fileName))
            File.AppendAllText(fileName, "Name" + endl + "Discomfort" + endl + "Time" + endl + "Precision" + endl + "TargetIndex" + endl + "Posture" + endl + "AngleDis" + endl + "InterDis" + endl + "YAxisDis" + endl + "HyperDis" + endl + AngleBasedHandModel.getCSVHeader(endl, "ActualHand") + endl + AngleBasedHandModel.getCSVHeader(endl, "GivenHand") + Environment.NewLine);
		remainingTargets = numTargets;
        if (UserStudyData.instance.right)
            outputHand.transform.localScale = new Vector3(-outputHand.transform.localScale.x, outputHand.transform.localScale.y, outputHand.transform.localScale.z);
        if (UserStudyData.instance.targetHand != null)
            outputHand.visualizeHand(UserStudyData.instance.targetHand);
        else
            outputHand.gameObject.SetActive(false);

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
                    try
                    {
                        File.AppendAllText(
                            fileName,

                            UserStudyData.instance.Name + endl +
                            UserStudyData.instance.discomfort + endl +
                            timer + endl +
                            (hit.point - hit.collider.transform.position).magnitude + endl +
                            targetList[current-1] +endl +
                            hand.currentPosture + endl +
                            UserStudyData.instance.angleDis + endl +
                            UserStudyData.instance.interDis + endl +
                            UserStudyData.instance.yaxisDis + endl +
                            UserStudyData.instance.hyperDis + endl +
                            hand.hand.ToCSVString(endl) + endl +
                            UserStudyData.instance.targetHand.ToCSVString(endl) + Environment.NewLine
                            );
                    }
                    catch (Exception e)
                    { Debug.LogError("Saving failed!"+e.ToString() + e.StackTrace); 
                    }

					remainingTargets--;
					if (remainingTargets > 0 && current<targets.Length) {
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
            targetList = new List<int>();
            for (int i = 0; (i < numTargets) && (i < targets.Length); i++)
            {
                targetList.Add(i);
            }
            targetList = Utility<int>.shuffleList(targetList);

            current = 0;
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
		targets[targetList [current++]].SetActive (true);
	}

	void endStudy()
	{
		playing = false;
		endPanel.SetActive (true);
		progress.enabled = false;
	}

    public void onEnd()
    {
		if (UserStudyData.instance.lineDrawing)
			SceneManager.LoadScene ("UserStudyLineTracing");
		else {
			if(UserStudyData.instance.remainingIts >0)
				SceneManager.LoadScene ("UserStudyIntro");
			else
				SceneManager.LoadScene ("UserStudyEnd");
		}
    }

    public void onOpenLog()
    {
        System.Diagnostics.Process.Start(fileName);
    }
}
