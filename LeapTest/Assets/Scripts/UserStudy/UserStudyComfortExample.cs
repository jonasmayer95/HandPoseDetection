using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UserStudyComfortExample : MonoBehaviour {


	AngleBasedHandModel comfortable, uncomfortable;
	public OutputHand outH;
	public RandomHandGenerator randHand;
	public GameObject firstPanel, secondPanel;

	bool generating = false;
	float targetdiscomfort = 1000;
	// Use this for initialization
	void Start () {

		if (UserStudyData.instance.right)
			outH.transform.localScale = new Vector3(-outH.transform.localScale.x, outH.transform.localScale.y, outH.transform.localScale.z);
		comfortable = PostureDataHandler.instance.getSublist(TrainingUnit.Posture.idle)[Random.Range(0,PostureDataHandler.instance.getSublist(TrainingUnit.Posture.idle).Count)].hand;
		outH.visualizeHand (comfortable);
		StartCoroutine (getNewRandomHand ());
	}

	public void onNext()
	{
		outH.visualizeHand (uncomfortable);
		firstPanel.SetActive (false);
		secondPanel.SetActive (true);
	}

	public void onContinue()
	{
		SceneManager.LoadScene ("UserStudyComfortEvaluation");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.JoystickButton0)) {
			if (firstPanel.activeInHierarchy)
				onNext ();
			else {
				if (!generating)
					onContinue ();
			}
		}
	}

	IEnumerator getNewRandomHand()
	{
		generating = true;
		int counter = 0;
		Debug.Log(targetdiscomfort);
		do{
			uncomfortable = randHand.createRandom();
			yield return new WaitForEndOfFrame();
			counter++;
			if (counter > 200)
			{
				counter = 0;
				targetdiscomfort -= 25;
				Debug.Log(targetdiscomfort);
			}
		}
		while (Discomfort.getDiscomfortAngled(uncomfortable) + Comfort.getRRPComponent(uncomfortable) < targetdiscomfort || Discomfort.getDiscomfortAngled(uncomfortable) + Comfort.getRRPComponent(uncomfortable) > targetdiscomfort+100);
		generating = false;
	}
}
