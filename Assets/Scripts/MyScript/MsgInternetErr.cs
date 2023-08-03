using UnityEngine;
using System.Collections;

public class MsgInternetErr : MonoBehaviour {
	private GUIStyle yesStyle = new GUIStyle();
	private GUIStyle stringStyle = new GUIStyle ();
	private Texture texTop, texCheck, texBack, texBackGround;
	private float 	swidth = 0.0f, startX;
	
	// Use this for initialization
	void Start () {
		swidth = Screen.height * 9 / 16;
		startX = (Screen.width - swidth) / 2;
		texBack = Resources.Load < Texture> ("PopUpMsg/Background");	
		yesStyle.normal.background = Resources.Load ("PopUpMsg/bt_normal") as Texture2D;
		yesStyle.active.background = Resources.Load ("PopUpMsg/bt_normal_r") as Texture2D;
		yesStyle.alignment = TextAnchor.MiddleCenter;
		yesStyle.normal.textColor = Color.white;
		yesStyle.active.textColor = Color.white;
		yesStyle.font = (Font)Resources.Load("HCRDotum");
		yesStyle.fontSize = 11* Screen.height/448;

		stringStyle.alignment = TextAnchor.MiddleCenter;
		stringStyle.normal.textColor = Color.gray;
		stringStyle.font = (Font)Resources.Load("HCRDotum");
		stringStyle.fontSize = 11* Screen.height/448;
	//	stringStyle.wordWrap = true;

		texBackGround = Resources.Load < Texture> ("comNameBack");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI()
	{
		GUI.depth = -10;
		GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), texBackGround, ScaleMode.StretchToFill);
		GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), texBackGround, ScaleMode.StretchToFill);
		
		GUI.DrawTexture (new Rect(startX + swidth*39/640, Screen.height*394/1136, swidth*564/640, Screen.height*343/1136), texBack, ScaleMode.StretchToFill);
		GUI.Label(new Rect(startX + swidth*49/640, Screen.height*490/1136, swidth*544/640, Screen.height*90/1136), "서버와의 접속이 원할하지 않습니다.\n 인터넷상태를 확인해 주세요.", stringStyle);
		if(GUI.Button(new Rect(startX + swidth*215/640, Screen.height*635/1136, swidth*211/640, Screen.height*56/1136), "확인", yesStyle))
		{
			Application.Quit();
		}

	}
}

