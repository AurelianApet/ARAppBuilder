using UnityEngine;
using System.Collections;

public class play : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		MovieTexture mt = (MovieTexture)GetComponent<Renderer>().material.mainTexture;
		mt.loop = true;
		mt.Play();
		#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
