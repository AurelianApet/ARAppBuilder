using UnityEngine;
using System.Collections;

public class LikeListManager : MonoBehaviour {

	private GUIStyle	titleStyle = new GUIStyle();
	// Use this for initialization
	void Start () {
		if(Screen.orientation != ScreenOrientation.Portrait)
			Screen.orientation = ScreenOrientation.Portrait;
		InputController.BackButtonTapped    += OnClickReturn;

		titleStyle.normal.textColor = Color.white;
		titleStyle.font = (Font)Resources.Load("NanumGothic");
		titleStyle.fontSize = 19 * Screen.height/730;
		titleStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	void OnDestroy()
	{
		InputController.BackButtonTapped    -= OnClickReturn;
	}
	
	// Update is called once per frame
	void Update () {
		InputController.UpdateInput();
	}

	public void OnClickReturn()
	{
		Application.LoadLevel("Vuforia-4-CloudRecognition");
	}
}
