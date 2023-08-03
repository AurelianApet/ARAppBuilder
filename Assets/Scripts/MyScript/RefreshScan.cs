using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshScan : MonoBehaviour {
	public delegate void ExitWindowStart();
	public static event ExitWindowStart EventExitWindowStart;
	// Use this for initialization
	void Start () {
		
	}

	public void OnReScan()
	{
		EventExitWindowStart ();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
