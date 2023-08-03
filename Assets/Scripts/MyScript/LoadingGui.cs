using UnityEngine;
using System.Collections;

public class LoadingGui : MonoBehaviour {

	private Texture texLoading;
	private GUIStyle	textStyle = new GUIStyle();
	private int realPercent =0;
	private int stepPercent =0;
	private float delta = 0.0f;
	// Use this for initialization
	void Start () {
		Global.bLoading = true;
		realPercent = 0;
		texLoading = Resources.Load<Texture> ("image/loading");	
		textStyle.normal.textColor = Color.white;
		textStyle.alignment = TextAnchor.MiddleCenter;
	}
	void OnDestroy()
	{
		Global.bLoading = false;
		Global.TotalLoadingCount = 0;
		Global.CurLoadingCount = 0;
		texLoading = null;
		Resources.UnloadUnusedAssets ();
	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		textStyle.fontSize = 25*Screen.width/490;
		int percent = (int)((float)Global.CurLoadingCount/(float)Global.TotalLoadingCount*100);
		if(stepPercent != percent)
		{
			realPercent = percent;
			stepPercent = percent;
			delta = 0.0f;
		}
		else
		{
			delta += 1.0f/(float)Global.TotalLoadingCount;
			realPercent = percent + (int)delta;
			int nextStep = (int)((float)(Global.CurLoadingCount + 1)/(float)Global.TotalLoadingCount*100);
			if(realPercent > nextStep)
				realPercent = nextStep;
			if(realPercent > 100)
				realPercent = 100;
		}
		string percentStr = realPercent + "%";
		GUI.Label(new Rect(Screen.width/2 - Screen.width/10/2, Screen.height/2 -Screen.width/10/2, Screen.width/10, Screen.width/10), percentStr, textStyle);
		float rotAngle = Time.frameCount*2;
		Rect loadingRect = new Rect(Screen.width/2 - Screen.width/5/2, Screen.height/2 -Screen.width/5/2, Screen.width/5, Screen.width/5);
		GUIUtility.RotateAroundPivot(rotAngle, loadingRect.center);
		GUI.DrawTexture(loadingRect, texLoading, ScaleMode.StretchToFill,true);
	}
}
