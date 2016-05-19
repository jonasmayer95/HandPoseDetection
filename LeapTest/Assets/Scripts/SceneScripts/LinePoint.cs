using UnityEngine;
using System.Collections;

//This class displays a point and the line to the next point
public class LinePoint : MonoBehaviour {

	Vector3 own, next;
	bool ownC, nextC;
	public LineRenderer lr;
	public float nearness = 0.2f;

	public double accuracy =0;
	public int samples=0;

    public Color standard = Color.gray;

	public void init(Vector3 _next)
	{
		lr.SetColors (standard, standard);
		next = _next;
		own = transform.position;
		lr.SetPositions (new Vector3[]{own, next});
	}
		
	public float getLineDistance(Vector3 other)
	{
        float t = Vector3.Dot(other-own, (next-own).normalized);
        //Debug.Log("t: "+t);
        if (t < 0)
            return getPointDistance(other);
        if (t > Vector3.Distance(own, next))
            return Vector3.Distance(other, next);
        float numerator = Vector3.Cross(other - own, other - next).magnitude;
        float denominatior = Vector3.Distance(own, next);
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
		if (Vector3.Distance (other, own) < nearness)
			ownC = true;
		if (Vector3.Distance (other, next) < nearness)
			nextC = true;
		if (nextC || ownC) {
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

