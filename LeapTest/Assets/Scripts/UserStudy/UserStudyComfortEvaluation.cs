using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class UserStudyComfortEvaluation : MonoBehaviour {

	public Text comfortNumber, remainingText;
	public Slider slider;
	public OutputHand outHand;
	public RandomHandGenerator randHand;
	public AngleBasedHandModel targethand;
	float targetdiscomfort; 
	string fileName;
	public string endl = ", ";
	int remaining =0;
	// Use this for initialization
	void Start () {
		remaining = UserStudyData.instance.evaluations;
		fileName ="ComfortEvaluationData"+UserStudyData.instance.fileEnding;
		if(!File.Exists(fileName))
			File.AppendAllText(fileName, "Name" + endl + "Rating" + endl +"Discomfort" + endl + "Comfort" + endl + "InterDis" + endl + "AbductionDis" + endl + "HyperDis" + endl + AngleBasedHandModel.getCSVHeader(endl, "RandomHand") + Environment.NewLine);
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeComfortNumer(float newVal)
	{
		comfortNumber.text = ((int)newVal)+"/10";
	}

	public void onNext()
	{
		saveResults ();

		if(--remaining<=0)
		{
			if (UserStudyData.instance.targetShooting || UserStudyData.instance.lineDrawing)
				SceneManager.LoadScene ("UserStudyIntro");
			else
				SceneManager.LoadScene ("UserStudyEnd");
		}

		reset();

	}

	void reset()
	{
		getNewRandomHand ();
		outHand.visualizeHand (targethand);
		slider.value = 5;
		comfortNumber.text = "5/10";
		remainingText.text = "Remaining: " + remaining; 
	}

	public void getNewRandomHand()
	{
		targetdiscomfort = UnityEngine.Random.Range(0, 1000);
		targethand = randHand.createRandom(targetdiscomfort, targetdiscomfort+100);
		Debug.Log (targetdiscomfort);
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
			targethand.ToCSVString(endl) + Environment.NewLine
		);
	}
}
