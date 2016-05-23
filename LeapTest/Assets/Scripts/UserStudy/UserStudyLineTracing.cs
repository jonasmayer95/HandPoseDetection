using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class UserStudyLineTracing : MonoBehaviour {

	Transform palm;
	public Transform palmLeft, palmRight;
	public GameObject startPanel, endPanel;
	public LineRenderer lr;
	HandObserver hand;
	public HandObserver leftHand, rightHand;
	public OutputHand outputHand;
	public string endl = ", ";
	bool playing = false;
	float timer;
	public Text countdownNumber;
	public Text progress;
	string fileName;
	static List<LinePoint> points = new List<LinePoint>();
    List<LinePoint> myPoints = new List<LinePoint>();
    GameObject currentLinePoint;
    bool isDrawing = false;
    public float timeOut = .5f, mySteps = .5f;
	float toClick, toPosture;
	bool holdingPosture = true;

	public LayerMask lineMask;

	public float maxX=6;
	public float maxY = 4;
	public float steps = 10;
	public float linePlane = 7;
	public GameObject pointPrefab, bezierControllPoint, myPointPrefab;


    public int bezierPoints = 5;

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
        HandPostureUtils.reload();
		toClick = toPosture = timeOut;
		if (UserStudyData.instance.right) {
			hand = rightHand;
			palm = palmRight;
		} else {
			hand = leftHand;
			palm = palmLeft;
		}

		points = new List<LinePoint>();
			generateRandCurve ();
			for (int i = 0; i < points.Count - 1; i++) {
				points [i].init (points[i+1].getOwn());
			}
			fileName ="LineTracingData"+UserStudyData.instance.fileEnding;
			if(!File.Exists(fileName))
				File.AppendAllText(fileName, "Name" + endl + "Discomfort" + endl + "Time" + endl + "Accuracy" + endl + "Posture" + endl + "AngleDis" + endl + "InterDis" + endl + "YAxisDis" + endl + "HyperDis" + endl + AngleBasedHandModel.getCSVHeader(endl, "ActualHand") + endl + AngleBasedHandModel.getCSVHeader(endl, "GivenHand") + Environment.NewLine);
            if (UserStudyData.instance.right)
                outputHand.transform.localScale = new Vector3(-outputHand.transform.localScale.x, outputHand.transform.localScale.y, outputHand.transform.localScale.z);
        if (UserStudyData.instance.targetHand != null)
				outputHand.visualizeHand (UserStudyData.instance.targetHand);
			else
				outputHand.gameObject.SetActive (false);

	}
	
	// Update is called once per frame
	void Update () {
		if (!HandPostureUtils.isHolding(UserStudyData.instance.posture,hand.hand)) {
			if (holdingPosture) {
				progress.text = "Loosing Hand Posture!";
				progress.color = Color.yellow;
			} else {
				progress.text = "Wrong Hand Posture!";
				progress.color = Color.red;
			}
			toPosture-=Time.deltaTime;
			if (toPosture < 0)
				holdingPosture = false;

		} else {
			progress.text = "Dist: " + accuracy + ", Progress: " + getProgress ();
			progress.color = Color.blue;
			toPosture = timeOut;
			holdingPosture = true;
		}
		
			lr.SetPositions (new Vector3[] { rayOrigin, rayOrigin + 10 * rayDirection });
		lr.enabled = hand.gameObject.activeInHierarchy;

			if (playing) {
			countdownNumber.enabled = !holdingPosture;
				timer += Time.deltaTime;
                if (Input.GetButton("Fire1"))
                {
                    toClick = timeOut;
				if (holdingPosture)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 10, lineMask))
                        {
                            accuracy = getDistanceToCurve(hit.point);
                            if (isDrawing == false)
                            {
                                isDrawing = true;
                                currentLinePoint = (GameObject)Instantiate(myPointPrefab, new Vector3(hit.point.x, hit.point.y, linePlane), Quaternion.identity);
                                myPoints.Add(currentLinePoint.GetComponent<LinePoint>());
                                currentLinePoint = (GameObject)Instantiate(myPointPrefab, new Vector3(hit.point.x, hit.point.y, linePlane), Quaternion.identity);
                            }
                            currentLinePoint.transform.position = new Vector3(hit.point.x, hit.point.y, linePlane);
                            myPoints[myPoints.Count - 1].init(currentLinePoint.transform.position);

                            if (myPoints[myPoints.Count - 1].getPointDistance(currentLinePoint.transform.position) > mySteps)
                            {
                                getDistanceToCurveAndSave(currentLinePoint.transform.position);
                                myPoints.Add(currentLinePoint.GetComponent<LinePoint>());
                                currentLinePoint = (GameObject)Instantiate(myPointPrefab, new Vector3(hit.point.x, hit.point.y, linePlane), Quaternion.identity);
                            }

                            if (isDone())
                                endStudy();
                        }
                    }
                }
                else
                {
                    if (isDrawing)
                    {
                        if (toClick < 0)
                        {
                            Debug.Log("Timed Out!");
                            endStudy();
                        }
                        toClick -= Time.deltaTime;
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
		countdownNumber.text = "Wrong Hand Posture!";
		countdownNumber.color = Color.red;
		playing = true;
	}

	void endStudy()
	{
		playing = false;
		endPanel.SetActive (true);
		progress.enabled = false;

        try
        {
            File.AppendAllText(
                fileName,

                UserStudyData.instance.Name + endl +
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
        }
        catch (Exception e)
        {
            Debug.LogError("Saving Failed! "+e.ToString() + e.StackTrace);
        }

	}

	public void onEnd()
	{
			if(UserStudyData.instance.remainingIts > 0)
				SceneManager.LoadScene ("UserStudyIntro");
		else
			SceneManager.LoadScene ("UserStudyEnd");
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
			float temp = points [i].getLineDistance (point);
			if (temp < min) {
				min = temp;
				index = i;
			}
		}
		return min;
	}

    public float getDistanceToCurveAndSave(Vector3 point)
    {
        int index = -1;
        float min = float.PositiveInfinity;
        for (int i = 0; i < points.Count - 1; i++)
        {
            float temp = points[i].getLineDistance(point);
            if (temp < min)
            {
                min = temp;
                index = i;
            }
        }
        points[index].addSample(min);
        return min;
    }

    public float getDistanceToMyCurveAndSave(Vector3 point)
    {
        int index = 0;
        float min = myPoints[0].getLineDistance(point);
        for (int i = 1; i < myPoints.Count - 1; i++)
        {
            float temp = myPoints[i].getLineDistance(point);
            if (temp < min)
            {
                min = temp;
                index = i;
            }
        }
        myPoints[index].addSample(min);
        return min;
    }


	bool isDone()
	{
		if (points.Count == 0)
			return false;
		bool result = true;
		for (int i = 0; i < points.Count - 1; i++) {
			result = result && points [i].isDone ();
		}
		return result;
	}

	public float getTotalAccuracy()
	{
		if (points.Count == 0)
			return 0;
		double result =0;
		int samples=0;
		for (int i = 0; i < points.Count - 1; i++) {
			samples += points [i].samples;
			result += points [i].accuracy;
            getDistanceToMyCurveAndSave(points[i].getOwn());
		}
		result /= samples;

        if (points.Count == 0)
            return 0;
        double result2 = 0;
        int samples2 = 0;
        for (int i = 0; i < myPoints.Count - 1; i++)
        {
            samples2 += myPoints[i].samples;
            result2 += myPoints[i].accuracy;
        }
        result2 /= samples2;


		Debug.Log ("Total Acc: old: "+result+", new: "+result2);
		return (float)(result+result2);
	}
	public float getProgress()
	{
		if (points.Count == 0)
			return 0;
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
        Vector3[] pointsArr = new Vector3[bezierPoints];
        pointsArr[0] = new Vector3(-maxX, 0, linePlane);
        for (int i = 1; i < bezierPoints-1; i++)
        {
            pointsArr[i] = new Vector3(-maxX + 2 * maxX * (((float)i)/(bezierPoints-1)), UnityEngine.Random.Range(-maxY,maxY), linePlane);
        }
        pointsArr[bezierPoints-1] = new Vector3(maxX, 0, linePlane);

        foreach (Vector3 point in pointsArr)
        {
            Instantiate(bezierControllPoint,point,Quaternion.identity);
        }
		float pointX = -maxX;
		float pointY = 0;
		for (int i = 0; i < steps; i++) {
			GameObject point = (GameObject)Instantiate (pointPrefab, Bezier.getPoint(((float) i)/steps,pointsArr), Quaternion.identity);
			points.Add (point.GetComponent<LinePoint>());
			pointX += (2 * maxX) / steps;
			pointY += UnityEngine.Random.Range ((2 * maxX) / steps,-(2 * maxX) / steps)-((pointY/maxY)*0.8f);
		}
	}
}
