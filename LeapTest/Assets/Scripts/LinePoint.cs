using UnityEngine;
using System.Collections;

//This class displays a point and the line to the next point
public class LinePoint : MonoBehaviour {

	Vector3 own, next;
	bool ownC, nextC;
	public LineRenderer lr;
	public float nearnesFac = 8;

	public double accuracy =0;
	public int samples=0;

	public void init(Vector3 _next)
	{
		lr.SetColors (Color.gray, Color.gray);
		next = _next;
		own = transform.position;
		Debug.Log (_next.ToString());
		lr.SetPositions (new Vector3[]{own, next});
	}
		
	public float getLineDistance(Vector3 other)
	{
		float numerator = Vector3.Cross(other-own,other-next).magnitude;
		float denominatior = Vector3.Distance (own, next);
		return numerator / denominatior;
	}

	public float getPointDistance(Vector3 other)
	{
		return Vector3.Distance (other, own);
	}

	public Vector3 getOwn()
	{
		return transform.position;
	}

	public void checkCompletition(Vector3 other)
	{
		if (Vector3.Distance (other, own) < Vector3.Distance (next, own) / nearnesFac)
			ownC = true;
		if (Vector3.Distance (other, next) < Vector3.Distance (next, own) / nearnesFac)
			nextC = true;
		if (nextC || ownC) {

			Debug.Log ("sgdgd");
			if (nextC && ownC)
				lr.SetColors (Color.green, Color.green);
			else

				lr.SetColors (Color.blue, Color.blue);
		}
			
	}

	public bool isDone()
	{
		return ownC && nextC;
	}

	public void addSample(float value)
	{
		samples++;
		accuracy += value;
	}
}

