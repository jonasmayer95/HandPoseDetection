using UnityEngine;
using System.Collections;

public class RandomHandGenerator : MonoBehaviour {
    //finger angles;
    public float DIP_min = .0f, DIP_max = 80.0f, PIP_min = 0, PIP_max = 100.0f, XMCP_min = -10, XMCP_max = 90.0f;
    //thumb angles
    public float IP_min = 0, IP_max = 90.0f, MP_min = 0, MP_max = 45, ZTMC_min = 280, ZTMC_max = 305, YTMC_min = 0, YTMC_max = 45, XTMC_min = -20, XTMC_max = 25;

	public float abduct_index = 30, abduct_ring = -15, abduct_pinky = -45;

	public static float randomfac = 0.3f, proceduralfac = 0.2f, morphfac = 0.4f, fromdatafac = 0.5f;

    public OutputHand output;
	// Use this for initialization

	public enum ThumbState
	{
		Under, Downward, Aligned, Sideways
	}

	public enum FingerState
	{
		HyperExtended, Flat, Fist, ForwardFist
	}

	public AngleBasedHandModel createRandom()
	{
		float random = Random.Range (0, randomfac + proceduralfac + fromdatafac + morphfac);
		if (random <= randomfac)
			return createRandomRandom ();
		if (random <= randomfac + proceduralfac)
			return createRandomProcedural ();
		if (random <= randomfac + proceduralfac+morphfac)
			return createRandomFromSavedMorph ();
		else
			return createRandomFromSaved ();
	}

    public AngleBasedHandModel createRandomRandom()
    {
        AngleBasedHandModel result = new AngleBasedHandModel();
		float random = Mathf.Pow(Random.value,2.0f);

		result.fingers[(int)AngleBasedHandModel.FingerName.index].mcp = Quaternion.Euler(Random.Range(XMCP_min, XMCP_max), random*abduct_index, 0);
		result.fingers[(int)AngleBasedHandModel.FingerName.middle].mcp = Quaternion.Euler(Random.Range(XMCP_min, XMCP_max), 0 , 0);
		result.fingers[(int)AngleBasedHandModel.FingerName.ring].mcp = Quaternion.Euler(Random.Range(XMCP_min, XMCP_max), random*abduct_ring, 0);
		result.fingers[(int)AngleBasedHandModel.FingerName.pinky].mcp = Quaternion.Euler(Random.Range(XMCP_min, XMCP_max), random*abduct_pinky, 0);

		for (int i = 0; i < result.fingers.Length; i++)
		{
			AngleBasedFingerModel finger = result.fingers[i];
			random = Random.value;
			finger.jointAngles[(int)AngleBasedFingerModel.Fingerjoints.DIP] = DIP_min + random * (DIP_max - DIP_min);
			finger.jointAngles[(int)AngleBasedFingerModel.Fingerjoints.PIP] = PIP_min + random * (PIP_max - PIP_min);
		}

		random = Random.value;

		result.thumb.tmc = Quaternion.Lerp(PostureDataHandler.instance.getRand().hand.thumb.tmc,Quaternion.Euler(Random.Range(XTMC_min, XTMC_max), Random.Range(YTMC_min, YTMC_max), Random.Range(ZTMC_min, ZTMC_max)),0.5f);
		result.thumb.jointAngles[(int)AngleBasedThumbModel.Fingerjoints.IP] = IP_min + random * (IP_max - IP_min);
		result.thumb.jointAngles[(int)AngleBasedThumbModel.Fingerjoints.MP] = MP_min + random * (MP_max - MP_min);

		result.rotation = Quaternion.identity;
		result.position = Vector3.zero;
        return result;
    }

	public AngleBasedHandModel createRandomFromSaved()
	{
		AngleBasedHandModel temp;
		AngleBasedHandModel result = new AngleBasedHandModel();
		for (int i = 0; i < result.fingers.Length; i++) {			
			temp = PostureDataHandler.instance.getRand ().hand;
			result.fingers [i] = temp.fingers [i];
		}
		temp = PostureDataHandler.instance.getRand ().hand;
		result.thumb = temp.thumb;
		return result;
	}

	public AngleBasedHandModel createRandomFromSavedMorph()
	{
		TrainingUnit first = PostureDataHandler.instance.getRand ();
		TrainingUnit second;
		do 
		{
			second = PostureDataHandler.instance.getRand ();
		} 
		while(first.posture == second.posture);


		AngleBasedHandModel temp1 = first.hand;
		AngleBasedHandModel temp2 = second.hand;
		AngleBasedHandModel result = new AngleBasedHandModel();
		for (int i = 0; i < result.fingers.Length; i++) {			
			result.fingers [i] = AngleBasedFingerModel.Lerp(temp1.fingers[i], temp2.fingers[i],((float)i)/((float)result.fingers.Length) );
		}

		//Debug.Log ("First: "+first.posture+", Second: "+second.posture);
		temp1= PostureDataHandler.instance.getRand ().hand;
		result.thumb = temp1.thumb;
		return result;
	}

	public AngleBasedHandModel createRandomProcedural()
	{
		AngleBasedHandModel result = new AngleBasedHandModel();

		FingerState[] fingers = new FingerState[result.fingers.Length];
		for (int i = 0; i < fingers.Length; i++) {
			fingers [i] = (FingerState)Random.Range (0,System.Enum.GetNames(typeof(FingerState)).Length);
		}
		//abduction
		float random = Mathf.Pow(Random.value,2.0f);
		float[] abductions = {random*abduct_index,0,random*abduct_ring,random*abduct_pinky};
		for (int i = 0; i < result.fingers.Length; i++)
		{
			float xmcp =0;
			if (fingers [i] == FingerState.ForwardFist || fingers [i] == FingerState.Flat)
				xmcp = 0;
			if (fingers [i] == FingerState.Fist)
				xmcp = XMCP_max;
			if (fingers [i] == FingerState.HyperExtended)
				xmcp = XMCP_min;
			
			result.fingers [i].mcp = Quaternion.Euler (xmcp,abductions[i],0);

			AngleBasedFingerModel finger = result.fingers[i];
			if (fingers [i] == FingerState.HyperExtended || fingers [i] == FingerState.Flat) {
				finger.jointAngles [(int)AngleBasedFingerModel.Fingerjoints.DIP] = DIP_min;
				finger.jointAngles [(int)AngleBasedFingerModel.Fingerjoints.PIP] = PIP_min;
			}
			if (fingers [i] == FingerState.Fist || fingers [i] == FingerState.ForwardFist) {
				finger.jointAngles [(int)AngleBasedFingerModel.Fingerjoints.DIP] = DIP_max;
				finger.jointAngles [(int)AngleBasedFingerModel.Fingerjoints.PIP] = PIP_max;
			}
		}

		ThumbState thumb = (ThumbState)Random.Range (0,System.Enum.GetNames(typeof(ThumbState)).Length);

		result.thumb = PostureDataHandler.instance.getRand ().hand.thumb;

		result.rotation = Quaternion.identity;
		result.position = Vector3.zero;

		result = AngleBasedHandModel.Lerp (result, PostureDataHandler.instance.getRand (TrainingUnit.Posture.idle).hand, 0.2f);

		return result;
	}

    public AngleBasedHandModel createRandom(float disc_min, float disc_max)
    {
        AngleBasedHandModel result;
        do
			result = createRandom();
		while (Discomfort.getDiscomfortAngled(result) + Comfort.getRRPComponent(result) < disc_min || Discomfort.getDiscomfortAngled(result)+Comfort.getRRPComponent(result)>disc_max);
		Debug.Log("Discomfort: "+Discomfort.getDiscomfortAngled(result)+", Comfort: "+Comfort.getRRPComponent(result));
        return result;
    }

    public void showRandHand()
    {
        AngleBasedHandModel result = createRandom();
		output.visualizeHand(result);
        Debug.Log("Comfort+Discomfort: "+ (Discomfort.getDiscomfortAngled(result)+Comfort.getRRPComponent(result)));
    }
}
