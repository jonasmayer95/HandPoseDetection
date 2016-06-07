using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class UserStudyTargetShooting : MonoBehaviour {

    public ARTPositioner art;

	Transform palm;
    public GameObject activeHandVis;
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

	float holdcounter = 0f;

    public Vector3 rayOrigin
    {
        get
        {
            if (art.isTracking())
                return art.getPosition();
            else
                return palm.position;
        }
    }

    public Vector3 rayDirection
    {
        get
        {
            Transform target;
            if (art.isTracking())
                target = art.target;
            else
                target = palm;

            target.Rotate(new Vector3(UserStudyData.instance.palmangle, 0, 0), Space.Self);
            Vector3 result = target.forward;
            target.Rotate(new Vector3(-UserStudyData.instance.palmangle, 0, 0), Space.Self);

            return result;
        }
    }
	// Use this for initialization
	void Start () {
        HandPostureUtils.reload();
		if (UserStudyData.instance.right) {
			hand = rightHand;
			palm = palmRight;
		} else {
			hand = leftHand;
			palm = palmLeft;
		}
        string fileHeader = "Name" + endl + "UserEvaluation" + endl + "Discomfort" + endl + "Time" + endl + "Precision" + endl + "Postureholdtime" + endl + "TargetIndex" + endl + "Posture" + endl + "AngleDis" + endl + "InterDis" + endl + "YAxisDis" + endl + "HyperDis" + endl + AngleBasedHandModel.getCSVHeader(endl, "ActualHand") + endl + AngleBasedHandModel.getCSVHeader(endl, "GivenHand");
		fileName = PostureDataHandler.instance.filePath + "TargetShootingData"+UserStudyData.instance.fileEnding;

        if (!File.Exists(fileName))
            File.AppendAllText(fileName, fileHeader+Environment.NewLine);
        else
        {
            StreamReader read = new StreamReader(fileName);
            string oldHeader = read.ReadLine();
            read.Close();
            if (!oldHeader.Equals(fileHeader))
            {
                Debug.Log("Fileheader not matching. Creating new file.");
                File.Delete(fileName);
                File.AppendAllText(fileName, fileHeader+Environment.NewLine);
            }
        }
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
		if (startPanel.activeInHierarchy && Input.GetKeyDown (KeyCode.JoystickButton0))
			onContinue ();
		if (endPanel.activeInHierarchy && Input.GetKeyDown (KeyCode.JoystickButton0))
			onEnd ();
		if (!HandPostureUtils.isHolding(UserStudyData.instance.posture, hand.hand) && !Input.GetButton("Emergency"))
		{
			progress.text = "Please correct your hand posture!";
		}
		else
			progress.text = remainingTargets+" of "+numTargets+" targets remaining.";
		
		lr.SetPositions(new Vector3[] {rayOrigin,rayOrigin+10*rayDirection});
        lr.enabled = hand.gameObject.activeInHierarchy || art.isTracking();


        activeHandVis.SetActive(hand.gameObject.activeInHierarchy || art.isTracking());
        activeHandVis.transform.position = rayOrigin;
		if (playing) 
		{
			if (HandPostureUtils.isHolding(UserStudyData.instance.posture, hand.hand))
				{
				holdcounter+= Time.deltaTime;
				}
			timer += Time.deltaTime;
			if (HandPostureUtils.isHolding(UserStudyData.instance.posture, hand.hand) || Input.GetButton("Emergency"))
            {
				countdownNumber.enabled = false;
				if (Input.GetButtonDown ("Fire1")) {
					RaycastHit hit;
					if (Physics.Raycast (rayOrigin, rayDirection, out hit, 10, mask)) {

						float holdperc = holdcounter / timer;
						try {
							File.AppendAllText (
								fileName,

								UserStudyData.instance.Name + endl +
								UserStudyData.instance.ComfortEvaluation + endl +
								UserStudyData.instance.discomfort + endl +
								timer + endl +
								(hit.point - hit.collider.transform.position).magnitude + endl +
								holdperc + endl +
								targetList [current - 1] + endl +
								hand.currentPosture + endl +
								UserStudyData.instance.angleDis + endl +
								UserStudyData.instance.interDis + endl +
								UserStudyData.instance.yaxisDis + endl +
								UserStudyData.instance.hyperDis + endl +
								hand.hand.ToCSVString (endl) + endl +
								UserStudyData.instance.targetHand.ToCSVString (endl) + Environment.NewLine
							);
						} catch (Exception e) {
							Debug.LogError ("Saving failed!" + e.ToString () + e.StackTrace); 
						}

						remainingTargets--;
						if (remainingTargets > 0 && current < targets.Length) {
							setRandTargetActive ();
							hit.collider.gameObject.SetActive (false);
						} else
							endStudy ();
					}
				}
			} else {
				countdownNumber.enabled = true;
			}

			Debug.Log("Distance: " + HandPostureUtils.getMinDistanceToPosture(UserStudyData.instance.posture, hand.hand));
		}
			

	}

	public void onContinue()
	{
		if (gameObject.activeInHierarchy) {
			startPanel.SetActive (false);
			progress.enabled = true;
            foreach (GameObject target in targets)
                target.SetActive(false);
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

        targetList = new List<int>();
        for (int i = 0; (i < numTargets) && (i < targets.Length); i++)
        {
            targetList.Add(i);
        }
        targetList = Utility<int>.shuffleList(targetList);

        current = 0;
        for (int i = 0; i < parallelNumTargets; i++)
        {
            setRandTargetActive();
        }
		countdownNumber.enabled = false;
		countdownNumber.text = "Please correct your hand posture!";
		countdownNumber.color = Color.red;
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
        art.closeSock();
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
