using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HandObserver : MonoBehaviour {


	// Use this for initialization

	public TrainingUnit.Posture trainingPosture, currentPosture;
    public bool rightHanded;
    public Text poseText;
	public Transform root, thumb1, thumb2, thumb3, index1, index2, index3, middle1, middle2, middle3, ring1, ring2, ring3, pinky1, pinky2, pinky3;
	public AngleBasedHandModel hand;
	ThreadedKNN knn = new ThreadedKNN();


	void Start () {
		hand = new AngleBasedHandModel ();
	}
	
	// Update is called once per frame
	void Update () {

		//TODO: this seems to not be correct; position values are funny
		hand.rotation = root.rotation;
		hand.position = root.position;

		sQuaternion temp = Quaternion.Inverse(root.rotation)* thumb1.rotation;
		if (rightHanded)
			temp.mirrorX();
		hand.thumb.tmc = temp;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.TMC_X] = ((Quaternion)temp).eulerAngles.x;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.TMC_Y] = ((Quaternion)temp).eulerAngles.y;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.TMC_Z] = ((Quaternion)temp).eulerAngles.z;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.MP] = Vector3.Angle (thumb1.forward, thumb2.forward);
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.IP] = Vector3.Angle (thumb3.forward, thumb2.forward);

		temp = Quaternion.Inverse(root.rotation)* index1.rotation;
		if (rightHanded)
			temp.mirrorX();
		hand.fingers [(int)AngleBasedHandModel.FingerName.index].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = ((Quaternion)temp).eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = ((Quaternion)temp).eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (index1.forward, index2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (index2.forward, index3.forward);

		temp = Quaternion.Inverse(root.rotation)*middle1.rotation;
		if (rightHanded)
			temp.mirrorX();
		hand.fingers [(int)AngleBasedHandModel.FingerName.middle].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = ((Quaternion)temp).eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = ((Quaternion)temp).eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (middle1.forward, middle2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (middle2.forward, middle3.forward);

		temp =  Quaternion.Inverse(root.rotation) * ring1.rotation;
		if (rightHanded)
			temp.mirrorX();
		hand.fingers [(int)AngleBasedHandModel.FingerName.ring].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = ((Quaternion)temp).eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = ((Quaternion)temp).eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (ring1.forward, ring2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (ring2.forward, ring3.forward);

		temp =  Quaternion.Inverse(root.rotation) * pinky1.rotation;
		if (rightHanded)
			temp.mirrorX();
		hand.fingers [(int)AngleBasedHandModel.FingerName.pinky].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = ((Quaternion)temp).eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = ((Quaternion)temp).eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (pinky1.forward, pinky2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (pinky2.forward, pinky3.forward);

		if (Input.GetKeyDown ("k")) {
			saveCurrentAs (trainingPosture);
		} else if (Input.GetKeyDown ("s"))
			DataHandler.instance.saveData ();
		else {
			currentPosture = knn.detectPosture (hand);
			if(poseText)
				poseText.text = "Posture: " + currentPosture + "; Discomfort: " + getDiscomfort();
		}

	}
	void UpdateFinger(AngleBasedHandModel hand, int index)
	{
		//Maybe use this for refactoring
	}
	void UpdateFinger(AngleBasedHandModel hand, AngleBasedFingerModel.Fingerjoints finger)
	{
		UpdateFinger (hand, (int)finger);
	}

	public void saveCurrentAs(TrainingUnit.Posture posture)
	{
		DataHandler.instance.addTrainigData (new TrainingUnit (posture, hand));
		hand = new AngleBasedHandModel ();
	}

	public float getDiscomfort()
	{
		return Discomfort.getDiscomfortAngled (hand);
	}

}
