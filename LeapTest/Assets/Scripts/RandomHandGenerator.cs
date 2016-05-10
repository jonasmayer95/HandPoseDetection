using UnityEngine;
using System.Collections;

public class RandomHandGenerator : MonoBehaviour {
    //finger angles;
    public float DIP_min = .0f, DIP_max = 80.0f, PIP_min = 0, PIP_max = 100.0f, YMCP_min = -10.0f, YMCP_max = 10.0f, XMCP_min = -10, XMCP_max = 90.0f;
    //thumb angles
    public float IP_min = 0, IP_max = 90.0f, MP_min = 0, MP_max = 45, ZTMC_min = 280, ZTMC_max = 305, YTMC_min = 0, YTMC_max = 45, XTMC_min = -20, XTMC_max = 25;

    public OutputHand output;
	// Use this for initialization

    public HandObserver.AngleBasedHandModel createRandom()
    {
        HandObserver.AngleBasedHandModel result = new HandObserver.AngleBasedHandModel();
        float random;
        for (int i = 0; i < result.fingers.Length; i++ )
        {
            HandObserver.AngleBasedFingerModel finger = result.fingers[i];
            finger.mcp = Quaternion.Euler(Random.Range(XMCP_min, XMCP_max), Random.Range(YMCP_min, YMCP_max), 0);
            random = Random.value;
            finger.jointAngles[(int)HandObserver.AngleBasedFingerModel.Fingerjoints.DIP] = DIP_min + random * (DIP_max - DIP_min);
            finger.jointAngles[(int)HandObserver.AngleBasedFingerModel.Fingerjoints.PIP] = PIP_min + random * (PIP_max - PIP_min);
        }
        result.thumb.tmc = Quaternion.Euler(Random.Range(XTMC_min,XTMC_max),Random.Range(YTMC_min,YTMC_max),Random.Range(ZTMC_min,ZTMC_max));
        random = Random.value;
        result.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.IP]= IP_min+random*(IP_max-IP_min);
        result.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.MP] = MP_min + random * (MP_max - MP_min);
        result.rotation = Quaternion.identity;
        result.position = Vector3.zero;
        Debug.Log("Discomfort: "+Discomfort.getDiscomfortAngled(result));
        return result;
    }
    public HandObserver.AngleBasedHandModel createRandom(float disc_min, float disc_max)
    {
        HandObserver.AngleBasedHandModel result;
        do
        {
            result = new HandObserver.AngleBasedHandModel();
            float random;
            for (int i = 0; i < result.fingers.Length; i++)
            {
                HandObserver.AngleBasedFingerModel finger = result.fingers[i];
                finger.mcp = Quaternion.Euler(Random.Range(XMCP_min, XMCP_max), Random.Range(YMCP_min, YMCP_max), 0);
                random = Random.value;
                finger.jointAngles[(int)HandObserver.AngleBasedFingerModel.Fingerjoints.DIP] = DIP_min + random * (DIP_max - DIP_min);
                finger.jointAngles[(int)HandObserver.AngleBasedFingerModel.Fingerjoints.PIP] = PIP_min + random * (PIP_max - PIP_min);
            }
            result.thumb.tmc = Quaternion.Euler(Random.Range(XTMC_min, XTMC_max), Random.Range(YTMC_min, YTMC_max), Random.Range(ZTMC_min, ZTMC_max));
            random = Random.value;
            result.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.IP] = IP_min + random * (IP_max - IP_min);
            result.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.MP] = MP_min + random * (MP_max - MP_min);
            result.rotation = Quaternion.identity;
            result.position = Vector3.zero;
            Debug.Log("Discomfort: " + Discomfort.getDiscomfortAngled(result));
        }
        while (Discomfort.getDiscomfortAngled(result) < disc_min || Discomfort.getDiscomfortAngled(result)>disc_max);
        return result;
    }

    public void showRandHand()
    {
        output.visualizeHand(createRandom());
    }
}
