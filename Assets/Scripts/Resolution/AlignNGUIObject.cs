using UnityEngine;
using System.Collections;

public class AlignNGUIObject : MonoBehaviour {
	private float fRate;
	private float fDesignRate = 9.0f / 16.0f;
	private float fScaleRate = 0.0f;
	// Use this for initialization
	void Awake()
	{
		
	}
	void Start () {
		fRate = (float)Screen.width / (float)Screen.height;
		fScaleRate = fDesignRate / fRate;
	//	Debug.Log ("AAAAAAAAAAA width = " + Screen.width + " height = " + Screen.height);
	//	Debug.Log ("AAAAAAAAAAA fRate = " + fRate + " fDesignRate = " + fDesignRate);

		Vector3 pos = this.transform.localPosition;
		this.transform.localPosition = new Vector3 (pos.x / fScaleRate, pos.y, pos.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
