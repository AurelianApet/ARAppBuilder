using UnityEngine;
using System.Collections;

public class ButtonBehavior : MonoBehaviour {

	public enum KIND
	{
		BTN_WEB 	= 1,
		BTN_CAPTURE = 2,
		BTN_PHONE 	= 3,
		BTN_AUDIO 	= 4,
		BTN_GOOGLE 	= 5,
	}
	public enum VIEW
	{
		LINK	= 1,
		INSIDE 	= 2
	}
	public enum TYPEBTN
	{
		COMMON = 1,
		CUSTOM = 2
	}
	public string mUrl;
	WebViewObject webViewObject;
	public KIND 	mKind;
	public VIEW 	mView;
	public TYPEBTN	mType;
    public bool		m_bAutoPlay = true;

	private AudioClip   m_clip;
	public delegate void Capture(string captureName);
	public static event Capture EventCapture;
	public delegate void Webview();
	public static event Webview EventWebview;
	// Use this for initialization
	void Start () {

        if(mKind == KIND.BTN_AUDIO && m_clip == null)
		{	
			StartCoroutine( "LoadSound");
		}
	/*	switch (mKind)
		{
		case KIND.BTN_WEB:
			panel.renderer.material = web_Material;
			break;
		case KIND.BTN_CAPTURE:
			panel.renderer.material = capture_Material;
			break;
		case KIND.BTN_PHONE:
			panel.renderer.material = phone_Material;
			break;
		case KIND.BTN_SOUND:
			panel.renderer.material = sound_Material;
			break;
		case KIND.BTN_GOOGLE:
			panel.renderer.material = google_Material;
			break;
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0))
		{
			Debug.Log ("MouseButton");
			if(Global.ShowingCustomWebview)
				return;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit)){
				//	Debug.Log(hit.transform.gameObject.name + "!!!!!!!!!!!!!!!!!!!");
				//about WebButton
				if(hit.transform.gameObject.name == "panel" && hit.transform.parent.gameObject.name == this.name)
				{	
					//Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" +hit.transform.parent.gameObject.name + " this.name " + this.name);
					if(mKind == KIND.BTN_WEB)
					{
						if(mView == VIEW.LINK)
						{
							Application.OpenURL(mUrl);
						}
						else{
							Debug.Log("Inside web button click!");
						//	if(Application.platform == RuntimePlatform.Android) 
						//	{
								webViewObject =	(new GameObject("WebViewObject")).AddComponent<WebViewObject>();
								webViewObject.Init((msg)=>{
									Debug.Log(string.Format("CallFromJS[{0}]", msg));
								}, true);
								webViewObject.LoadURL(mUrl);
								webViewObject.SetVisibility(true);
						//	}
							EventWebview();
						}
					}
					else if(mKind == KIND.BTN_CAPTURE)
					{
						EventCapture(this.name);
						Debug.Log("Capture Button clicked!");
					}
					else if(mKind == KIND.BTN_PHONE)
					{
						string telUrl = "tel:" + mUrl;
						Application.OpenURL(telUrl);
						Debug.Log("phone Button clicked!");
					}
					else if(mKind == KIND.BTN_AUDIO)
					{
						Debug.Log("Sound Button Clicked");
						if(m_clip.isReadyToPlay)
						{
							if(gameObject.GetComponent<AudioSource>().isPlaying)
								gameObject.GetComponent<AudioSource>().Pause();
							else
								gameObject.GetComponent<AudioSource>().Play();
						}
					}
					else if(mKind == KIND.BTN_GOOGLE)
					{
						Debug.Log("Sound Button Clicked");
						Application.OpenURL(mUrl);
					}
				}
			}
		}
		if (Application.platform == RuntimePlatform.Android && mKind == KIND.BTN_WEB && mView == VIEW.INSIDE)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				//TrueManager.GetInstance().webViewObject.SetVisibility(false);

				Destroy(webViewObject);
				//Application.Quit();
				return;
			}
		}

	}
	public IEnumerator LoadSound()
	{
		Debug.Log("LoadSound");
		WWW www = new WWW(mUrl);
		yield return www;

		Debug.Log ("www is loaded");
		if (www.error == null)
		{
			m_clip = www.GetAudioClip(false,true);
			gameObject.GetComponent<AudioSource>().clip = www.GetAudioClip(false,true);
			if(m_bAutoPlay && m_clip.isReadyToPlay)
			{
				Debug.Log("Sound is played");
				gameObject.GetComponent<AudioSource>().Play();
			}
		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}
	}
}
