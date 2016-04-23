using UnityEngine;
using System.Collections;

public class handleBarManipulator : MonoBehaviour {


	public HandObserver left, right;
	public LayerMask mask;
	private bool grabbing = false;
	private GameObject other;
	public LineRenderer lr;
	public Transform pivot;
	private float initalDist;
	public float timeout = 0.1f;
	private float to;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lr.SetPositions(new Vector3[] {(left.root.position),left.root.position+(right.root.position - left.root.position)});
		if ((left.currentPosture == TrainingUnit.Posture.grab || left.currentPosture == TrainingUnit.Posture.fist || left.currentPosture == TrainingUnit.Posture.pinch || left.currentPosture == TrainingUnit.Posture.bird) && (right.currentPosture == TrainingUnit.Posture.grab || right.currentPosture == TrainingUnit.Posture.fist || right.currentPosture == TrainingUnit.Posture.pinch || right.currentPosture == TrainingUnit.Posture.bird)) 
		{
			to = timeout;
			if (!grabbing) 
			{
				
				RaycastHit hit;
				if (Physics.Raycast (left.root.position, (right.root.position) - left.root.position, out hit, Vector3.Magnitude (right.root.position - left.root.position), mask))
				{
					other = hit.collider.gameObject;
					Debug.Log ("Grabbing: "+other.ToString());

					initalDist = Vector3.Distance (left.root.position, right.root.position);

					pivot.localScale = Vector3.one;
					pivot.position = 0.5f * (left.root.position + right.root.position);
					pivot.LookAt (left.root);

					other.transform.parent = pivot;

					lr.SetColors (Color.green, Color.green);
					grabbing = true;
					
				}

			} 
			else 
			{
				pivot.localScale = (Vector3.Distance (left.root.position, right.root.position) / initalDist)*Vector3.one;
				pivot.position = 0.5f * (left.root.position + right.root.position);
				pivot.LookAt (left.root);
			}
		} 
		else 
		{
			if (grabbing) {
				to -= Time.deltaTime;
				Debug.Log ("Timing out: "+to);
				if (to <= 0) {
					Debug.Log ("Ungrabbing;");
					other.transform.parent = null;
					lr.SetColors (Color.gray, Color.gray);
					grabbing = false;
				}
			}
		}
	}
}
