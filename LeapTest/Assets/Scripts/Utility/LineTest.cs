using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class LineTest : MonoBehaviour {

    public List<LinePoint> points = new List<LinePoint>();
    public Transform kek;
	void Start () {
        for (int i = 0; i < points.Count - 1; i++)
        {
            points[i].init(points[i + 1].getOwn());
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Debug.Log(points[i].getLineDistance(kek.position));
        }
	}
}
