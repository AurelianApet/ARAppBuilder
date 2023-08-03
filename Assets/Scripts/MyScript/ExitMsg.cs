using UnityEngine;
using System.Collections;
using System.Xml;

public class ExitMsg : MonoBehaviour {
	public delegate void ExitWindowStart();
	public static event ExitWindowStart EventExitWindowStart;
	public delegate void ExitWindowEnd();
	public static event ExitWindowEnd EventExitWindowEnd;


	private GUIStyle noStyle = new GUIStyle();
	private GUIStyle yesStyle = new GUIStyle();
	private GUIStyle stringStyle = new GUIStyle ();
	private Texture texTop, texCheck, texBack, texBackGround;
	private float 	swidth = 0.0f, startX;

	// Use this for initialization
	void Start () {
		swidth = Screen.height * 9 / 16;
		startX = (Screen.width - swidth) / 2;
		texBack = Resources.Load < Texture> ("PopUpMsg/Background");	
		noStyle.normal.background = Resources.Load ("PopUpMsg/bt_no") as Texture2D;
		noStyle.active.background = Resources.Load ("PopUpMsg/bt_no_r") as Texture2D;
		yesStyle.normal.background = Resources.Load ("PopUpMsg/bt_yes") as Texture2D;
		yesStyle.active.background = Resources.Load ("PopUpMsg/bt_yes_r") as Texture2D;

		stringStyle.alignment = TextAnchor.MiddleCenter;
		stringStyle.normal.textColor = Color.gray;
		stringStyle.font = (Font)Resources.Load("HCRDotum");
		stringStyle.fontSize = 11* Screen.height/448;
		texBackGround = Resources.Load < Texture> ("comNameBack");
		EventExitWindowStart ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator GoToMainPage(float secs)
	{
		yield return new WaitForSeconds(secs);
		Application.LoadLevel("ARScene");
	}

	void OnGUI()
	{
		GUI.depth = -10;
	}

	IEnumerator SendLogoutRequest(string url)
	{
		XmlDocument xmlDoc = new XmlDocument ();
		yield return new WaitForSeconds (0.01f);
		xmlDoc.Load (url);
	}
}
