﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class UserStudyComfortEvaluation : MonoBehaviour {

	public Text comfortNumber, remainingText;
	public Slider slider;
	public OutputHand outputHand;
	public RandomHandGenerator randHand;
	public AngleBasedHandModel targethand;
	float targetdiscomfort; 
	string fileName;
	public string endl = ", ";
	int remaining =0;
    bool generating = false;
	int last =0;

	// Use this for initialization
	void Start () {
		remaining = UserStudyData.instance.evaluations;
		fileName =PostureDataHandler.instance.filePath + "ComfortEvaluationData"+UserStudyData.instance.fileEnding;

		string fileHeader = 
			"Name" + endl + 
			"Rating" + endl +
			"Discomfort" + endl + 
			"Comfort" + endl + 
			"InterDis" + endl + 
			"AbductionDis" + endl + 
			"HyperDis" + endl + 
			Discomfort.getInterFingerCSVHeader(endl) +
			Discomfort.getAbductionCSVHeader(endl) +
			Discomfort.getHyperExtensionCSVHeader(endl) +
			Comfort.getRRPCSVHeader(endl) +
			AngleBasedHandModel.getCSVHeader(endl, "RandomHand");
		
		if(!File.Exists(fileName))
			{
			File.AppendAllText(fileName,fileHeader+Environment.NewLine);
			}
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
		reset ();
        if (UserStudyData.instance.right)
            outputHand.transform.localScale = new Vector3(-outputHand.transform.localScale.x, outputHand.transform.localScale.y, outputHand.transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
		if ((int)Input.GetAxisRaw ("PadHor") != last) {
			slider.value += (int)Input.GetAxisRaw ("PadHor");
			last = (int)Input.GetAxisRaw ("PadHor");
		}
		if (Input.GetKeyDown (KeyCode.JoystickButton0))
			onNext ();
	}

	public void changeComfortNumer(float newVal)
	{
		comfortNumber.text = ((int)newVal)+"/10";
	}

	public void onNext()
	{
        if (!generating)
        {
            saveResults();

            if (--remaining <= 0)
            {
                if (UserStudyData.instance.targetShooting || UserStudyData.instance.lineDrawing)
                    SceneManager.LoadScene("UserStudyIntro");
                else
                    SceneManager.LoadScene("UserStudyEnd");
            }

            reset();
        }

	}

	void reset()
	{
		StartCoroutine (getNewRandomHand());
		slider.value = 5;
		comfortNumber.text = "5/10";
		remainingText.text = "Remaining: " + remaining; 
	}

	IEnumerator getNewRandomHand()
	{
        generating = true;
		targetdiscomfort = UnityEngine.Random.Range(50, 1000);
        int counter = 0;
        Debug.Log(targetdiscomfort);
        do{
		    targethand = randHand.createRandom();
            outputHand.visualizeHand(targethand);
            yield return new WaitForEndOfFrame();
            counter++;
            if (counter > 200)
            {
                counter = 0;
                targetdiscomfort = UnityEngine.Random.Range(50, 1000);
                Debug.Log(targetdiscomfort);
            }
        }
        while (Discomfort.getDiscomfortAngled(targethand) + Comfort.getRRPComponent(targethand) < targetdiscomfort || Discomfort.getDiscomfortAngled(targethand) + Comfort.getRRPComponent(targethand) > targetdiscomfort+100);
        generating = false;
	}

	void saveResults()
	{
		File.AppendAllText(
			fileName, 

			UserStudyData.instance.Name+endl+
			slider.value+endl+
			Discomfort.getDiscomfortAngled(targethand) + endl +
			Comfort.getRRPComponent(targethand) + endl +
			Discomfort.getInterFingerComponent(targethand) + endl +
			Discomfort.getAbductionComponent(targethand) + endl +
			Discomfort.getHyperExtensionComponent(targethand) + endl +
            Discomfort.getInterFingerCSV(targethand, endl) +
            Discomfort.getAbductionCSV(targethand, endl) +
            Discomfort.getHyperExtensionCSV(targethand, endl) +
            Comfort.getRRPCSV(targethand, endl) +
			targethand.ToCSVString(endl) + Environment.NewLine
		);
	}
}
