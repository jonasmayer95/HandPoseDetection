using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

public class ThreadedKNN{

//TODO: Add real return value
	private TrainingUnit.Posture result = TrainingUnit.Posture.idle;
    Thread newestThread;

	public TrainingUnit.Posture detectPosture(AngleBasedHandModel hand)
    {
		KNNThread thread = new KNNThread (hand, this);
		newestThread = new Thread (new ThreadStart(thread.threadJob));
		newestThread.Start ();
        return result;
    }
	private class KNNThread
	{
		AngleBasedHandModel hand;
		ThreadedKNN mother;
		public KNNThread(AngleBasedHandModel _hand, ThreadedKNN _mother)
		{
			hand = _hand;
			mother = _mother;
		}
		public void threadJob()
		{
			List<ThreadedKNN.poseCompareObject> compareList =  PostureDataHandler.instance.getCompareList (hand);
			compareList.Sort (new poseComparer());
			int[] poseCounts = new int[Enum.GetNames(typeof(TrainingUnit.Posture)).Length];
			for (int k = 0; k < PostureDataHandler.instance.getK (); k++) {
				poseCounts [(int)compareList [k].posture]++;
			}
			mother.result = (TrainingUnit.Posture)poseCounts.ToList ().IndexOf (poseCounts.Max ());
		}

	}

	public class poseCompareObject
	{
		public float distance;
		public TrainingUnit.Posture posture; 

		public poseCompareObject(float dist, TrainingUnit.Posture post)
		{
			distance = dist;
			posture = post;
		}
	}

	public class poseComparer : IComparer<poseCompareObject>
	{
		public int Compare(poseCompareObject x, poseCompareObject y)
		{

			return x.distance.CompareTo(y.distance);
		}
	}
}
