using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Record : MonoBehaviour {
	public Text text;
	public HandObserver hand;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "RRP-Component: " + Mathf.Max (0.0f, Comfort.getRRPComponent(hand.hand)-20);
	}
}
