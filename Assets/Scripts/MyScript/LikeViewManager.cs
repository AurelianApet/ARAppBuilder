using UnityEngine;
using System.Collections;

public class LikeViewManager : MonoBehaviour {

	public delegate void StartLikeView();
	public static event StartLikeView EventStartLikeView;
	// Use this for initialization
	void Start () {
		Global.bIsLikeView = true;
		Global.bJyroSenser = true;
		GameObject ARCamera = GameObject.Find ("ARCamera");
		ARCamera.transform.position =  new Vector3 (0.0f, this.transform.localScale.y * 1.6f, 0.0f);
		EventStartLikeView ();
		InputController.BackButtonTapped += OnBackButtonTapped;
	}

	void OnDestroy()
	{
		Global.bIsLikeView = false;
		InputController.BackButtonTapped -= OnBackButtonTapped;
	}
	
	// Update is called once per frame
	void Update () {
		InputController.UpdateInput();
	}

	void OnBackButtonTapped()
	{
		Application.LoadLevel ("LikeList");		
	}
}
