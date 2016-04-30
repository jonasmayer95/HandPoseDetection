using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HandObserver : MonoBehaviour {

	[System.Serializable]
	public class AngleBasedHandModel
	{
		public enum FingerName
		{
			index = 0 , middle = 1, ring = 2 , pinky = 3
		}
		public AngleBasedThumbModel thumb;
		public AngleBasedFingerModel[] fingers = new AngleBasedFingerModel[4];
		public sQuaternion rotation;
		public sVector3 position;

		public AngleBasedHandModel()
		{
			thumb = new AngleBasedThumbModel();
			for(int i =0 ; i<fingers.Length ; i++)
				fingers[i] = new AngleBasedFingerModel();
		}

		public string ToString()
		{
			string result = "Root: Rot" + ((Quaternion)rotation).eulerAngles.ToString() + ", Pos" + position.ToString();
			result = result + " " + thumb.ToString();
			for (int i = 0; i < fingers.Length; i++)
				result += "; "+i+"." + fingers [i].ToString();
			return result;
		}

		public float euclidianDistance(AngleBasedHandModel other)
		{
			float result = Mathf.Pow (euclidianDistanceFingers (other), 2.0f);
			//TODO: make usefull as soon as i need
			return result;
		}
		public float euclidianDistanceFingers(AngleBasedHandModel other)
		{
			float result = Mathf.Pow(thumb.euclidianDistance(other.thumb),2.0f);
			for (int i = 0; i < fingers.Length; i++) {
				result += Mathf.Pow (fingers [i].euclidianDistance (other.fingers [i]), 2);
			}
			return Mathf.Sqrt (result);
		}
	}

	[System.Serializable]
	public class AngleBasedFingerModel
	{
		public enum Fingerjoints
		{
			DIP = 0 , PIP = 1, MCP_UP = 2 , MCP_SIDE = 3
		}

		public float[] jointAngles = new float[4];
        public sQuaternion mcp;

		public string ToString()
		{
			return "Finger: DIP " + jointAngles [0] + ", PIP " + jointAngles [1] + ", MCP_UP " + jointAngles [2] + ", MCP_SIDE " + jointAngles [3];
		}
		public float euclidianDistance(AngleBasedFingerModel other)
		{
			float result = 0;
			for (int i = 0; i < jointAngles.Length - 2; i++) {
				result += Mathf.Pow (jointAngles [i] - other.jointAngles [i], 2.0f);
			}
            result += Mathf.Pow(Quaternion.Angle(other.mcp, mcp),2.0f);
			return Mathf.Sqrt (result);
		}
	}

	[System.Serializable]
	public class AngleBasedThumbModel
	{
		public enum Fingerjoints
		{
			IP = 0 , MP = 1, TMC_X = 2 , TMC_Y = 3, TMC_Z = 4
		}
		public float[] jointAngles = new float[5];
        public sQuaternion tmc;
		public string ToString()
		{
			return "Thumb: IP " + jointAngles [0] + ", MP " + jointAngles [1] + ", TMC_X " + jointAngles [2] + ", TMC_Y " + jointAngles [3]+ ", TMC_Z " + jointAngles [4];
		}
		public float euclidianDistance(AngleBasedThumbModel other)
		{
			float result = 0;
			for (int i = 0; i < jointAngles.Length-2; i++)
				result += Mathf.Pow (jointAngles [i] - other.jointAngles [i], 2.0f);
            result += Mathf.Pow(Quaternion.Angle(other.tmc, tmc), 2.0f);
			return Mathf.Sqrt (result);
		}
	}
	// Use this for initialization

	public TrainingUnit.Posture trainingPosture, currentPosture;
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

		Quaternion temp = Quaternion.Inverse(root.rotation)* thumb1.rotation;
		hand.thumb.tmc = temp;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.TMC_X] = temp.eulerAngles.x;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.TMC_Y] = temp.eulerAngles.y;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.TMC_Z] = temp.eulerAngles.z;
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.MP] = Vector3.Angle (thumb1.forward, thumb2.forward);
		hand.thumb.jointAngles [(int)AngleBasedThumbModel.Fingerjoints.IP] = Vector3.Angle (thumb3.forward, thumb2.forward);

		temp = Quaternion.Inverse(root.rotation)* index1.rotation;
		hand.fingers [(int)AngleBasedHandModel.FingerName.index].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = temp.eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = temp.eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (index1.forward, index2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.index].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (index2.forward, index3.forward);

		temp = Quaternion.Inverse(root.rotation)*middle1.rotation;
		hand.fingers [(int)AngleBasedHandModel.FingerName.middle].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = temp.eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = temp.eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (middle1.forward, middle2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.middle].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (middle2.forward, middle3.forward);

		temp =  Quaternion.Inverse(root.rotation) * ring1.rotation;
		hand.fingers [(int)AngleBasedHandModel.FingerName.ring].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = temp.eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = temp.eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (ring1.forward, ring2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.ring].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (ring2.forward, ring3.forward);

		temp =  Quaternion.Inverse(root.rotation) * pinky1.rotation;
		hand.fingers [(int)AngleBasedHandModel.FingerName.pinky].mcp = temp;
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_SIDE] = temp.eulerAngles.y;
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.MCP_UP] = temp.eulerAngles.x;
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = Vector3.Angle (pinky1.forward, pinky2.forward);
		hand.fingers[(int)AngleBasedHandModel.FingerName.pinky].jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = Vector3.Angle (pinky2.forward, pinky3.forward);

		if (Input.GetKeyDown ("k")) {
			DataHandler.instance.addTrainigData (new TrainingUnit (trainingPosture, hand));
			hand = new AngleBasedHandModel ();
		} else if (Input.GetKeyDown ("s"))
			DataHandler.instance.saveData ();
		else {
			currentPosture = knn.detectPosture (hand);
			poseText.text = "Posture: " + currentPosture + "; Discomfort: " + Discomfort.getDiscomfortAngled (hand);
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

}
