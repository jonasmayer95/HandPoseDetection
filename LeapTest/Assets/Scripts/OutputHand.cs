using UnityEngine;
using System.Collections;

public class OutputHand : MonoBehaviour {

    public HandObserver.AngleBasedHandModel observedHand;
    public float rotationSpeed;
    public Transform root, thumb1, thumb2, thumb3, index1, index2, index3, middle1, middle2, middle3, ring1, ring2, ring3, pinky1, pinky2, pinky3;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, Time.deltaTime * rotationSpeed), Space.World);
	}

    public void visualizeHand(HandObserver.AngleBasedHandModel currentHand)
    {
        if(observedHand != currentHand)
            observedHand = currentHand;

        thumb1.localRotation = currentHand.thumb.tmc.mirroredY();
        thumb2.localRotation = Quaternion.Euler(0, -currentHand.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.MP], 0);
        thumb3.localRotation = Quaternion.Euler(0, -currentHand.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.IP], 0);

        index1.localRotation = currentHand.fingers[0].mcp.mirroredY();
        index2.localRotation = Quaternion.Euler(-currentHand.fingers[0].jointAngles[1], 0, 0);
        index3.localRotation = Quaternion.Euler(-currentHand.fingers[0].jointAngles[0], 0, 0);

        middle1.localRotation = currentHand.fingers[1].mcp.mirroredY();
        middle2.localRotation = Quaternion.Euler(-currentHand.fingers[1].jointAngles[1], 0, 0);
        middle3.localRotation = Quaternion.Euler(-currentHand.fingers[1].jointAngles[0], 0, 0);

        ring1.localRotation = currentHand.fingers[2].mcp.mirroredY();
        ring2.localRotation = Quaternion.Euler(-currentHand.fingers[2].jointAngles[1], 0, 0);
        ring3.localRotation = Quaternion.Euler(-currentHand.fingers[2].jointAngles[0], 0, 0);

        pinky1.localRotation = currentHand.fingers[3].mcp.mirroredY();
        pinky2.localRotation = Quaternion.Euler(-currentHand.fingers[3].jointAngles[1], 0, 0);
        pinky3.localRotation = Quaternion.Euler(-currentHand.fingers[3].jointAngles[0], 0, 0);
    }

    public void visualizeHand()
    {
        thumb1.localRotation = observedHand.thumb.tmc.mirroredY();
        thumb2.localRotation = Quaternion.Euler(0, -observedHand.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.MP], 0);
        thumb3.localRotation = Quaternion.Euler(0, -observedHand.thumb.jointAngles[(int)HandObserver.AngleBasedThumbModel.Fingerjoints.IP], 0);

        index1.localRotation = observedHand.fingers[0].mcp.mirroredY();
        index2.localRotation = Quaternion.Euler(-observedHand.fingers[0].jointAngles[1], 0, 0);
        index3.localRotation = Quaternion.Euler(-observedHand.fingers[0].jointAngles[0], 0, 0);

        middle1.localRotation = observedHand.fingers[1].mcp.mirroredY();
        middle2.localRotation = Quaternion.Euler(-observedHand.fingers[1].jointAngles[1], 0, 0);
        middle3.localRotation = Quaternion.Euler(-observedHand.fingers[1].jointAngles[0], 0, 0);

        ring1.localRotation = observedHand.fingers[2].mcp.mirroredY();
        ring2.localRotation = Quaternion.Euler(-observedHand.fingers[2].jointAngles[1], 0, 0);
        ring3.localRotation = Quaternion.Euler(-observedHand.fingers[2].jointAngles[0], 0, 0);

        pinky1.localRotation = observedHand.fingers[3].mcp.mirroredY();
        pinky2.localRotation = Quaternion.Euler(-observedHand.fingers[3].jointAngles[1], 0, 0);
        pinky3.localRotation = Quaternion.Euler(-observedHand.fingers[3].jointAngles[0], 0, 0);
    }
}
