using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;

using Vuforia;

public class LoadContent : MonoBehaviour {
	private bool is_destory;
	public delegate void SlerpEnd();
	public static event SlerpEnd EventSlerpEnd;
	public delegate void VideoTag(string videoUrl);
	public static event VideoTag EventVideoTag;
	public delegate void AddCaptureSlide(string captureName, List<GUIManager.CaptureSlide> urls);
	public static event AddCaptureSlide EventAddCaptureSlide;

	private float baseRate = 1.0f;
	private List<GameObject> mTrackableView;
	public bool 	bEnable3DSlerp = false;
	public bool 	bSlerp = false;

	private GameObject ARCamera;
	private float 	posSmooth = 7.0f;
	private float	rotSmooth = 7.0f;

	private GameObject air;

	private Quaternion ARCamrot = Quaternion.Euler(90.0f, 180.0f, 0.0f);			//마커를 잃은 다음 전화면에 object를 보여주기위함

	public delegate void AudioPlayHandler(int audioID);
	public static event AudioPlayHandler OnAudioPlay;


	// Use this for initialization
	void Start () {
		is_destory = false;
		mTrackableView = new List<GameObject>();
		ARCamera = GameObject.Find ("ARCamera");

		CloudRecoTrackableEventHandler.EventFoundMarker += StartLoadContent;
		LikeViewManager.EventStartLikeView += StartLoadLikedContent;
		GUIManager.EventFindingMarker += OnFindingMarker;
		WebViewObject.EventWebViewClose += OnWebViewClose;
		ButtonBehavior.EventWebview += OnWebView;
		ButtonBehavior.EventCapture += OnShowCapture;

	}

	void OnDestroy()
	{
		CloudRecoTrackableEventHandler.EventFoundMarker -= StartLoadContent;
		LikeViewManager.EventStartLikeView -= StartLoadLikedContent;
		GUIManager.EventFindingMarker -= OnFindingMarker;
		WebViewObject.EventWebViewClose -= OnWebViewClose;
		ButtonBehavior.EventWebview -= OnWebView;
		ButtonBehavior.EventCapture -= OnShowCapture;

	}

	// Update is called once per frame
	void Update () {
		if(Global.TotalLoadingCount >= Global.CurLoadingCount && Global.TotalLoadingCount != 0)
		{
			//	int percent = (int)((float)Global.CurLoadingCount/(float)Global.TotalLoadingCount*100);
			if(Global.TotalLoadingCount == Global.CurLoadingCount)
			{
				Global.TotalLoadingCount = 0;
				if(this.transform.gameObject.GetComponent<LoadingGui>())
				{
					Destroy(this.transform.gameObject.GetComponent<LoadingGui>());
				}
				ShowAllObject();
			}
			//	Debug.Log("!!!!!!" + percent + "cur" + GlobalValue.CurLoadingCount);
		}
		if(bSlerp && this.GetComponent<CloudRecoTrackableEventHandler>().GetCurrentStatus() == TrackableBehaviour.Status.NOT_FOUND)
		{
			ARCamera.transform.position = Vector3.Slerp(ARCamera.transform.position, Global.ARCampos, Time.deltaTime * posSmooth);
			ARCamera.transform.rotation = Quaternion.Slerp(ARCamera.transform.rotation, ARCamrot, Time.deltaTime * rotSmooth);
			//	Debug.Log("Camera Slerping!");
		}
		if(bSlerp && Vector3.Distance(ARCamera.transform.position, Global.ARCampos) == 0.0f)
		{
			bSlerp = false;
			if(bEnable3DSlerp)
				EventSlerpEnd();
		}
		if(Input.GetMouseButtonUp(0))
		{
			if(Global.ShowingCustomWebview)
				return;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit)){
				if(hit.transform.gameObject.name.Contains("Media_chromakey"))
				{
					PlayVideoChromakey(hit.transform.gameObject);
				}
				if(hit.transform.gameObject.name.Contains("Media_CTV"))
				{
					GameObject videoPanel = hit.transform.Find("Video").gameObject;
					//PlayPauseVideo(videoPanel);
				}
				if(hit.transform.gameObject.name.Contains("Media_LCD"))
				{
					GameObject LCDPanel = hit.transform.Find("Video").gameObject;
					//PlayPauseVideo(LCDPanel);
				}
			}
		}
	}

	public void ShowAllObject()
	{
		/*if (Global.CurMarkerName.Contains ("COAR_109")) {
			AddAirplane3D ();
		}*/
		for(int i = 0; i<mTrackableView.Count; i++)
		{
			mTrackableView[i].SetActive(true);
			Renderer[] rendererComponents = mTrackableView[i].GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = mTrackableView[i].GetComponentsInChildren<Collider>(true);

			// Enable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}
			// Enable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}
		}
	}

	public bool HideAllObject(string name)
	{
		for(int i = 0; i<mTrackableView.Count; i++)
		{
			if (mTrackableView[i].name.Contains(name)/* ||
			    mTrackableView[i].name.Contains("Media_CTV") ||
			    mTrackableView[i].name.Contains("Media_LCD") ||
			    mTrackableView[i].name.Contains("Media_chromakey")*/)
				continue;

			if(mTrackableView[i].name.Contains("ImgaeSlide_") == false)
				mTrackableView[i].SetActive(false);
			Renderer[] rendererComponents = mTrackableView[i].GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = mTrackableView[i].GetComponentsInChildren<Collider>(true);

			// Enable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}
			// Enable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}
		}
		return true;
	}

	void StartLoadContent()
	{
		string xmlUrl;
		xmlUrl = string.Format ("{0}{1}&isscan=1", Global.SERVER_URL, Global.CurMarkerName);
		StartCoroutine(LoadXml(xmlUrl));
		//	LoadXml (xmlUrl);
	}

	void AddAirplane3D(){
		air = Instantiate( Resources.Load("airplane/airplane_1", typeof(GameObject))) as GameObject;
		air.transform.parent = this.transform;
		air.transform.localScale = new Vector3 (0.35f, 0.35f, 0.35f);
		air.transform.localPosition = new Vector3 (-0.12f, 0.12f, -0.12f);
		air.transform.localRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
	}

	void StartLoadLikedContent()
	{
		string xmlUrl;
		xmlUrl = string.Format ( "{0}{1}&isscan=0", Global.SERVER_URL, Global.likeNames[Global.nCurMarkerIndex]);
		StartCoroutine(LoadXml(xmlUrl));
		//	LoadXml (xmlUrl);
	}

	IEnumerator LoadXml(string url)
	{
		XmlDocument xmlDoc = new XmlDocument();
		yield return new WaitForSeconds(0.5f);
		try{
			xmlDoc.Load(url);
		}catch(XmlException) {
		}
		//xmlDoc.LoadXml(www.text);
		int ApiOver;
		XmlNode apiOverNode = xmlDoc.SelectSingleNode("contents/apis_over");
		if(apiOverNode == null)
			ApiOver = 0;
		else
			ApiOver = Convert.ToInt32(apiOverNode.InnerText);
		if(ApiOver == 0)
		{
			ProcessMarker(xmlDoc.SelectNodes("contents/marker"));
			//			ProcessImage(xmlDoc.SelectNodes("contents/image"));
			//			ProcessImageSlide(xmlDoc.SelectNodes("contents/slide"));
			ProcessVideos(xmlDoc.SelectNodes("contents/video"));
			ProcessWeb(xmlDoc.SelectNodes("contents/web"));
			//			ProcessCapture(xmlDoc.SelectNodes("contents/capture"));
			ProcessPhone(xmlDoc.SelectNodes("contents/tel"));
			Process3DObject(xmlDoc.SelectNodes("contents/three"));
			//			ProcessTextBoard(xmlDoc.SelectNodes("contents/notepad"));
			//			ProcessAudio(xmlDoc.SelectNodes("contents/audio"));
			//			ProcessGoolgeButton(xmlDoc.SelectNodes("contents/googlemap"));
			//			ProcessChromaKeyVideo(xmlDoc.SelectNodes("contents/chromakey"));
		}
		else
		{
			ErrorMsg.New("Over Api", "Please buy new product.");
		}
		if(Global.TotalLoadingCount == 0)
		{
			Global.TotalLoadingCount = 1;
			StartCoroutine(Wait(1.0f));
		}
		this.transform.gameObject.AddComponent<LoadingGui>();
		Debug.Log("GlobalValue.TotalLoadingCount = " + Global.TotalLoadingCount);
		Debug.Log("GlobalValue.CurLoadingCount = " + Global.CurLoadingCount);	
	}

	private void ProcessMarker(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessMarker");
			//	int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			float posX = 50;			float posY = 50;			float width =100;			float height = 100;
			string url = node.SelectSingleNode("url").InnerText;
			float rate = (float)Convert.ToDouble(node.SelectSingleNode("rate").InnerText);
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			//	float basePosX, basePosY;
			Vector3	pos;			Vector3 scale;
			pos.x = (posX - 50) / 100;
			pos.y = depth * 0.0f;
			pos.z = (posY - 50) / 100;

			if (rate < 1.0f){
				scale.x = (float)rate * (width / 100) * 0.1f;
				scale.y = 0.1f;
				scale.z = 0.1f * (height / 100);
			}else{
				scale.x = 0.1f * (width / 100);
				scale.y = 0.1f;
				scale.z = 1 / (float)rate  * (height / 100)  * 0.1f;
			}
			baseRate = rate;
			string path = Global.MarkerSavePath + Global.CurMarkerName+ ".png";
			if(!File.Exists(path))
			{
				Global.TotalLoadingCount ++;
				StartCoroutine(DownMarker(url));
			}
		}
	}

	private void ProcessImage(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessImage");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			string url = node.SelectSingleNode("url").InnerText;

			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			Global.TotalLoadingCount ++;
			//		Debug.Log("image xml url = " + url + ",posx = "+ posX);
			float basePosX, basePosY;
			Vector3	pos;
			Vector3 scale;

			Vector3 leftPos = Vector3.zero;
			Vector3 rightPos = Vector3.zero;
			float sildebtnUnit = 5.6f + 3.0f;	//정방형으로 현시되는 left,right button scale + delta
			float sWidth = width;		//xml에서 받은 width값

			if (baseRate < 1.0f)
			{
				basePosX = 100 * (1 - baseRate) / 2;
				width *= baseRate;

				pos.x = (posX * baseRate + basePosX - 50) / 100;
				pos.y = depth * 0.0f;
				pos.z = (posY - 50) / 100;

				leftPos.x = ((posX - sWidth/2 - sildebtnUnit/2) * baseRate + basePosX - 50) / 100;
				leftPos.y = pos.y;
				leftPos.z = pos.z;

				rightPos.x = ((posX + sWidth/2 + sildebtnUnit/2) * baseRate + basePosX - 50) / 100;
				rightPos.y = pos.y;
				rightPos.z = pos.z;
			}
			else
			{
				basePosY = 100 * (1 - 1 / baseRate) / 2;
				height *= 1 / baseRate;

				pos.x = (posX - 50)  / 100;
				pos.y = depth * 0.0f;
				pos.z = (posY * 1 / baseRate + basePosY - 50) / 100;

				leftPos.x = (posX - sWidth/2 - sildebtnUnit/2 - 50)  / 100;
				leftPos.y = pos.y;
				leftPos.z = pos.z;

				rightPos.x = (posX + sWidth/2 + sildebtnUnit/2 - 50)  / 100;
				rightPos.y = pos.y;
				rightPos.z = pos.z;
			}

			scale.x = (width / 100) * 0.1f; 
			scale.y = 0.1f;
			scale.z = 0.1f * (height / 100);

			ShowImage(id, type, pos, scale, url);
		}
	}

	private void ShowImage(int id, int type, Vector3 pos, Vector3 scale, string url)
	{
		if(id != 1)
		{
			if(type == 0){
				GameObject newslide = Instantiate( Resources.Load("imageslide/image", typeof(GameObject))) as GameObject;
				newslide.name = string.Format ("Imgae_{0}", id);

				newslide.transform.parent = this.transform;
				newslide.transform.localPosition = Vector3.zero;
				newslide.transform.rotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
				newslide.transform.localScale = Vector3.one;
				GameObject panel = newslide.transform.Find ("Background").gameObject;
				panel.transform.localPosition = pos; 
				panel.transform.localScale = scale; 
				StartCoroutine(work (panel, url));
				//	panel.renderer.material.color = HexToColor (hexColor);
				ImageSlideBehavior sldBehavior = (ImageSlideBehavior)newslide.GetComponent ("ImageSlideBehavior");
				mTrackableView.Add (newslide);

				HideObjectRenderer();
			}else{
				GameObject newPlane = Instantiate(Resources.Load ("Prefabs/CanvasImage", typeof(GameObject)))as GameObject;
				newPlane.name = string.Format ("Image_{0}", id);
				newPlane.transform.parent = this.transform;
				newPlane.transform.localPosition = pos;
				newPlane.transform.localScale = new Vector3(scale.x*10, scale.x*10, scale.x*10);

				GameObject slide = newPlane.transform.Find("image").Find("Background").gameObject;
				StartCoroutine(work (slide, url));

				Rotate3DObject rotateScript = (Rotate3DObject)newPlane.GetComponent("Rotate3DObject");
				rotateScript.SetObjKind(Rotate3DObject.OBJKIND.OBJ_IMAGE3D);
				bEnable3DSlerp = true;

				mTrackableView.Add (newPlane);
				mTrackableView[mTrackableView.Count - 1].SetActive(false);
				HideObjectRenderer();
			}
		}
	}

	private void ProcessImageSlide(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessImageSlide");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			int playtype = Convert.ToInt32(node.SelectSingleNode("playtype").InnerText);
			int showtype = Convert.ToInt32(node.SelectSingleNode("showtype").InnerText);

			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			ArrayList urls = new ArrayList();
			for(int i = 1; i<=10; i++)
			{
				string nodeName = string.Format("url{0}", i); 
				string slide = node.SelectSingleNode(nodeName).InnerText;
				if(slide != "") 
				{
					urls.Add(slide);
				}
			}
			//	if(type == 0)
			Global.TotalLoadingCount += urls.Count;
			//		Debug.Log("image xml url = " + url + ",posx = "+ posX);
			float basePosX, basePosY;
			Vector3	pos;
			Vector3 scale;

			Vector3 leftPos = Vector3.zero;
			Vector3 rightPos = Vector3.zero;
			float sildebtnUnit = 5.6f + 3.0f;	//정방형으로 현시되는 left,right button scale + delta
			float sWidth = width;		//xml에서 받은 width값

			if (baseRate < 1.0f)
			{
				basePosX = 100 * (1 - baseRate) / 2;
				width *= baseRate;

				pos.x = (posX * baseRate + basePosX - 50) / 100;
				pos.y = depth * 0.0f;
				pos.z = (posY - 50) / 100;

				leftPos.x = ((posX - sWidth/2 - sildebtnUnit/2) * baseRate + basePosX - 50) / 100;
				leftPos.y = pos.y;
				leftPos.z = pos.z;

				rightPos.x = ((posX + sWidth/2 + sildebtnUnit/2) * baseRate + basePosX - 50) / 100;
				rightPos.y = pos.y;
				rightPos.z = pos.z;
			}
			else
			{
				basePosY = 100 * (1 - 1 / baseRate) / 2;
				height *= 1 / baseRate;

				pos.x = (posX - 50)  / 100;
				pos.y = depth * 0.0f;
				pos.z = (posY * 1 / baseRate + basePosY - 50) / 100;

				leftPos.x = (posX - sWidth/2 - sildebtnUnit/2 - 50)  / 100;
				leftPos.y = pos.y;
				leftPos.z = pos.z;

				rightPos.x = (posX + sWidth/2 + sildebtnUnit/2 - 50)  / 100;
				rightPos.y = pos.y;
				rightPos.z = pos.z;
			}

			scale.x = (width / 100) * 0.1f; 
			scale.y = 0.1f;
			scale.z = 0.1f * (height / 100);

			ShowImageSlide(id, type, pos, scale, urls, leftPos, rightPos, playtype, showtype);
		}
	}

	private void ShowImageSlide(int id, int type, Vector3 pos, Vector3 scale, ArrayList urls, Vector3 leftPos, Vector3 rightPos, int playtype, int showtype)
	{
		if(id != 1)
		{
			if(type == 0)
			{
				GameObject newslide = Instantiate( Resources.Load("imageslide/imageslide", typeof(GameObject))) as GameObject;
				newslide.name = string.Format ("ImgaeSlide_{0}", id);
				/* added bond 2015-06-12*/
				for(int i = 0; i < 10; i++)
				{
					GameObject ithImage = newslide.transform.Find(string.Format("slideimage_{0}", i)).gameObject;
					if(i < urls.Count)
					{
						ithImage.transform.localScale = scale; 
						StartCoroutine(work (ithImage, urls[i].ToString()));
					}
					else{
						DestroyObject(ithImage);
					}
				}
				newslide.transform.parent = this.transform;
				newslide.transform.localPosition = pos;
				newslide.transform.rotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
				newslide.transform.localScale = Vector3.one;
				//	panel.renderer.material.color = HexToColor (hexColor);
				ImageSlideBehavior sldBehavior = (ImageSlideBehavior)newslide.GetComponent ("ImageSlideBehavior");
				sldBehavior.nTotalImgCount = urls.Count;
				sldBehavior.type = 1;
				sldBehavior.ActionType = playtype;
				sldBehavior.TransType = showtype;
				sldBehavior.Init();
				mTrackableView.Add (newslide);

				HideObjectRenderer();
			}else
			{
				GameObject newPlane = Instantiate(Resources.Load ("Prefabs/Canvas", typeof(GameObject)))as GameObject;
				newPlane.name = string.Format ("ImageSlide_{0}", id);
				newPlane.transform.parent = this.transform;
				newPlane.transform.localPosition = pos;
				newPlane.transform.localScale = new Vector3(scale.x*10, scale.x*10, scale.x*10);

				GameObject slide = newPlane.transform.Find("imageslide").gameObject;
				for(int i = 0; i < 10; i++)
				{
					GameObject ithImage = slide.transform.Find(string.Format("slideimage_{0}", i)).gameObject;
					if(i < urls.Count)
					{
						ithImage.transform.localScale = Vector3.one;
						StartCoroutine(work (ithImage, urls[i].ToString()));
					}
					else{
						DestroyObject(ithImage);
					}
				}
				ImageSlideBehavior sldBehavior = (ImageSlideBehavior)slide.GetComponent ("ImageSlideBehavior");
				sldBehavior.nTotalImgCount = urls.Count;
				sldBehavior.type = 2;
				sldBehavior.ActionType = playtype;
				sldBehavior.TransType = showtype;

				Rotate3DObject rotateScript = (Rotate3DObject)newPlane.GetComponent("Rotate3DObject");
				rotateScript.SetObjKind(Rotate3DObject.OBJKIND.OBJ_IMAGE3D);
				bEnable3DSlerp = true;

				mTrackableView.Add (newPlane);
				mTrackableView[mTrackableView.Count - 1].SetActive(false);
				HideObjectRenderer();
			}
		}
	}

	private IEnumerator DownCustomButton(string url)
	{
		WWW www =  new WWW(url);
		yield return www;
		string path = Global.CustomButtonPath + Global.CurCustomButtonName;
		try	{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllBytes(path, www.bytes);
		}catch (Exception exception){
			Debug.LogError(exception.ToString());
		}
	}

	private void ProcessVideos(XmlNodeList nodes)
	{	
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessVideo");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			string url = node.SelectSingleNode("url").InnerText;
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			int runOptin = Convert.ToInt32(node.SelectSingleNode("run_opt").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			string playbutton = node.SelectSingleNode("playButton").InnerText;
			string addFile = node.SelectSingleNode("addFile").InnerText;
			Global.CurCustomButtonName = addFile;
			string customurl = "\\uploads\\videoButtonImage\\" + addFile;
			DownCustomButton (customurl);

			float basePosX, basePosY;
			Vector3	pos;	Vector3 scale;
			if (baseRate < 1.0f){
				basePosX = 100 * (1 - baseRate) / 2;
				width *= baseRate;

				pos.x = (posX * baseRate + basePosX - 50) / 100;
				pos.y = depth * 0.01f;
				pos.z = (posY - 50) / 100;
			}else{
				basePosY = 100 * (1 - 1 / baseRate) / 2;
				height *= 1/baseRate;

				pos.x = (posX - 50)  / 100;
				pos.y = depth * 0.01f;
				pos.z = (posY * 1 / baseRate + basePosY - 50) / 100;
			}
			scale.x = (width / 100);
			scale.z = 0.1f * (height / 100);
			scale.y = 0.5f;
			if (addFile == null || addFile == "") {
				if (url.IndexOf ("http://www.youtube.com") > -1 || url.IndexOf ("https://www.youtube.com") > -1) {
					playYoutubePlay (id, type, pos, scale, url, runOptin);
				} else {
					PlayVideo(id, type, pos, scale, url, runOptin);
				}
			} else {
				PlayVideo(id, type, pos, scale, url, runOptin);
			}
		}
	}

	private void playYoutubePlay(int id, int type, Vector3 pos, Vector3 scale, string url, int runOptin)
	{
		StartCoroutine (StartLoadingScreen(url));
	}

	IEnumerator StartLoadingScreen(string url)
	{
		yield return new WaitForSeconds (0.3f);
//		StartCoroutine (CheckVideoCanPlay(url));
		GameObject.Find ("YoutubeManager").transform.GetComponent<YoutubeDemoUsage> ().videoUrl = url;
		GameObject.Find ("YoutubeManager").transform.GetComponent<YoutubeDemoUsage> ().PlayFromInput ();
	}

	IEnumerator CheckVideoCanPlay(string url)
	{
		yield return new WaitForSeconds (1.0f);
		WWWForm form = new WWWForm ();
		string requestURL = url;
		WWW www = new WWW (requestURL, form);
		yield return www;
		if (www.error != null) {
		}
	}

	private void PlayVideo(int id, int type, Vector3 pos, Vector3 scale, string url, int runOptin)
	{
		Debug.Log ("Video pos");
		Debug.Log (pos.x + ":" + pos.y + ":" + pos.z);
		Debug.Log (scale.x + ":" + scale.y + ":" + scale.z);
        /*
		GameObject newObject = null;	GameObject panel = null;
		switch(type)
		{
		case 0:
			newObject = Instantiate(Resources.Load("Video/VideoManager", typeof(GameObject))) as GameObject;
			newObject.transform.parent = this.transform;
			newObject.transform.localPosition = pos;
			#if UNITY_IPHONE
			//	scale.x = -scale.x;
			//	scale.z = -scale.z;
			newObject.transform.localScale = scale;
			//	newObject.transform.rotation = Quaternion.Euler(270.0f, 180.0f, 180.0f);
			#else 
			newObject.transform.localScale = scale;
			#endif
			newObject.name = string.Format ("Media_common_{0}", id);
			mTrackableView.Add (newObject);
			Global.videoName = newObject.name;
			Global.bExistVideo = true;
			Global.orgVideoPos = newObject.transform.localPosition;
			Global.orgVideoQuat = newObject.transform.localRotation.eulerAngles;
			Global.orgVideoScale = newObject.transform.localScale;

			VideoPlaybackBehaviour aa = (VideoPlaybackBehaviour)newObject.GetComponent ("VideoPlaybackBehaviour");
			aa.m_path = url;
			aa.m_autoPlay = (runOptin == 1)? false: true;
			//	newObject.renderer.enabled = false;
			break;
		case 1:
			newObject = Instantiate(Resources.Load("Prefabs/Tv", typeof(GameObject))) as GameObject;
			newObject.transform.parent = this.transform;
			newObject.transform.localPosition = new Vector3(pos.x, 0.0f, pos.z);
			newObject.transform.localScale = new Vector3(Math.Abs(scale.z)*20, Math.Abs(scale.z)*20, Math.Abs(scale.z)*20);
			newObject.name = string.Format ("Media_CTV_{0}", id);
			mTrackableView.Add (newObject);
			panel = newObject.transform.Find ("Video").gameObject;
			VideoPlaybackBehaviour CTVCtrl = (VideoPlaybackBehaviour)panel.GetComponent ("VideoPlaybackBehaviour");
			CTVCtrl.m_path = url;
			CTVCtrl.m_autoPlay = (runOptin == 1)? false: true;
			//	panel.renderer.enabled = false;
			Rotate3DObject rotateScript = (Rotate3DObject)newObject.GetComponent("Rotate3DObject");
			rotateScript.SetObjKind(Rotate3DObject.OBJKIND.OBJ_VIDEO);
			bEnable3DSlerp = true;
			break;
		case 2:
			newObject = Instantiate(Resources.Load("Prefabs/LCDTV", typeof(GameObject))) as GameObject;
			newObject.transform.parent = this.transform;
			newObject.transform.localPosition = new Vector3(pos.x, 0.0f, pos.z);
			newObject.transform.localScale = new Vector3(Math.Abs(scale.z)*15, Math.Abs(scale.z)*15, Math.Abs(scale.z)*15);
			newObject.name = string.Format ("Media_LCD_{0}", id);
			mTrackableView.Add (newObject);
			panel = newObject.transform.Find ("Video").gameObject;
			VideoPlaybackBehaviour LCDCtrl = (VideoPlaybackBehaviour)panel.GetComponent ("VideoPlaybackBehaviour");
			LCDCtrl.m_path = url;
			LCDCtrl.m_autoPlay = (runOptin == 1)? false: true;

			Rotate3DObject rotateScript1 = (Rotate3DObject)newObject.GetComponent("Rotate3DObject");
			rotateScript1.SetObjKind(Rotate3DObject.OBJKIND.OBJ_VIDEO);
			bEnable3DSlerp = true;
			break;		
		}
		mTrackableView[mTrackableView.Count - 1].SetActive(false);
		HideObjectRenderer();

		newObject.transform.FindChild("VideoControl").gameObject.SetActive(false);
		EventVideoTag (url);
        */

        GameObject newObject = null;

        newObject = Instantiate(Resources.Load("Video/VideoManager", typeof(GameObject))) as GameObject;
        newObject.transform.parent = this.transform;
        newObject.transform.localPosition = pos;
#if UNITY_IPHONE
		scale.x = -scale.x;
		scale.z = -scale.z;
		newObject.transform.localScale = scale;
		newObject.transform.rotation = Quaternion.Euler(270.0f, 180.0f, 180.0f);
#else
        newObject.transform.localScale = scale;
        newObject.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 180.0f);
        //newObject.renderer.material = new Material(Shader.Find("Custom/NewShader"));
#endif
        if (type == 1)
        {
#if UNITY_IPHONE
			newObject.transform.rotation = Quaternion.Euler(180.0f, 180.0f, 180.0f);
#else
            newObject.transform.rotation = new Quaternion(90.0f, 0.0f, 0.0f, 0.0f);
#endif
            //4.521544
            float height = Math.Abs(newObject.transform.lossyScale.y) / 2 * 8.0f;
            Vector3 wPos = newObject.transform.position;
            newObject.transform.position = new Vector3(wPos.x, height, wPos.z);
        }
        newObject.name = string.Format("Media_common_{0}", id);
        mTrackableView.Add(newObject);

        HideObjectRenderer();
        MediaPlayerCtrl aa = (MediaPlayerCtrl)newObject.GetComponent("MediaPlayerCtrl");
        aa.m_strFileName = url;
        if (runOptin == 1)
        {
            aa.m_bAutoPlay = false;
            aa.m_bLoop = true;
        }

        Global.videoName = newObject.name;
        Global.bExistVideo = true;
        Global.orgVideoPos = newObject.transform.localPosition;
        Global.orgVideoQuat = newObject.transform.localRotation.eulerAngles;
        Global.orgVideoScale = newObject.transform.localScale;
    }

	private void ProcessWeb(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessWeb");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			string url = node.SelectSingleNode("url").InnerText;
			url = UnityEngine.WWW.UnEscapeURL(url);

			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			int view = Convert.ToInt32(node.SelectSingleNode("view").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			string hexColor = node.SelectSingleNode("color").InnerText;
			if(hexColor.Length != 1)
				hexColor = hexColor.Substring(1);
			int typebase = Convert.ToInt32(node.SelectSingleNode("mode_index").InnerText);
			string backurl = node.SelectSingleNode("backurl").InnerText;

			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			Vector3 pos = GetButtonPosition(posX, posY, depth);
			Vector3 scale = GetButtonScale(width, height);
			ShowWebButton(id, type, view, pos, scale, url, hexColor, typebase, backurl);
		}
	}

	private void ShowWebButton(int id, int type, int view, Vector3 pos, Vector3 scale, string url, string hexColor, int typebase, string backurl)
	{
		GameObject newButton = null;
		if(type == 0)
		{
			newButton = Instantiate(Resources.Load ("Button/common/CommonBtn", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			newButton.transform.Find ("background").GetComponent<Renderer>().material.color = HexToColor (hexColor);

			string strWebBtnImage = string.Format("Button/common/web_{0}", typebase);
			panel.GetComponent<Renderer>().material = Instantiate ( Resources.Load(strWebBtnImage, typeof(Material))) as Material;
			panel.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}
		else if (type == 1)
		{
			newButton = Instantiate(Resources.Load ("Button/custom/CustomButton", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			StartCoroutine(work(panel, backurl));
			Global.TotalLoadingCount += 1;
		}
		else
			Debug.LogError("The type of WebButton is invalid type = " + type);

		newButton.name = string.Format ("webButton_{0}", id);
		newButton.transform.parent = this.transform;
		newButton.transform.localPosition = pos;
		newButton.transform.localScale = scale;

		ButtonBehavior btnBehavior = (ButtonBehavior)newButton.GetComponent ("ButtonBehavior");
		btnBehavior.mUrl = url;
		btnBehavior.mKind = ButtonBehavior.KIND.BTN_WEB;
		if(view == 0)
			btnBehavior.mView = ButtonBehavior.VIEW.LINK;
		else
			btnBehavior.mView = ButtonBehavior.VIEW.INSIDE;
		mTrackableView.Add (newButton);

		HideObjectRenderer();
	}

	private void ProcessPhone(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessPhone");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			int threemode = Convert.ToInt32(node.SelectSingleNode("threemode").InnerText);
			if(threemode == 1)
				type = 2;
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			string number = node.SelectSingleNode("tel_no").InnerText;

			string hexColor = node.SelectSingleNode("color").InnerText;
			if(hexColor.Length != 1)
				hexColor = hexColor.Substring(1);
			int typebase = Convert.ToInt32(node.SelectSingleNode("mode_index").InnerText);
			string backurl = node.SelectSingleNode("backurl").InnerText;
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			Vector3 pos = GetButtonPosition(posX, posY, depth);
			Vector3 scale = GetButtonScale(width, height);
			ShowPhoneButton(id, type, pos, scale, number, hexColor, typebase, backurl);
		}
	}
	private void ShowPhoneButton(int id, int type, Vector3 pos, Vector3 scale, string number, string hexColor, int typebase, string backurl)
	{
		GameObject newButton = null;
		if(type == 0) {
			newButton = Instantiate(Resources.Load ("Button/common/CommonBtn", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			newButton.transform.Find ("background").gameObject.GetComponent<Renderer>().material.color = HexToColor (hexColor);
			string strPhoneBtnImage = string.Format("Button/common/phone_{0}", typebase);
			panel.GetComponent<Renderer>().material = Instantiate ( Resources.Load(strPhoneBtnImage, typeof(Material))) as Material;
			panel.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		} else if (type == 1) {
			newButton = Instantiate(Resources.Load ("Button/custom/CustomButton", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			StartCoroutine(work(panel, backurl));
			Global.TotalLoadingCount += 1;
		} else if (type == 2) {
			newButton = Instantiate(Resources.Load ("Prefabs/phone", typeof(GameObject))) as GameObject;
			newButton.SetActive(false);
		} else
			Debug.LogError("The type of WebButton is invalid type = " + type);

		newButton.name = string.Format ("phoneButton_{0}", id);
		newButton.transform.parent = this.transform;
		newButton.transform.localPosition = pos;
		if(type != 2)
			newButton.transform.localScale = scale;
		else
			newButton.transform.localScale = new Vector3(scale.x*40, scale.x*40, scale.x*40);

		ButtonBehavior btnBehavior = (ButtonBehavior)newButton.GetComponent ("ButtonBehavior");
		btnBehavior.mUrl = number;
		btnBehavior.mKind = ButtonBehavior.KIND.BTN_PHONE;
		mTrackableView.Add (newButton);

		HideObjectRenderer();
	}

	private void Process3DObject(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);

			string url = "";
			#if UNITY_ANDROID
			url= node.SelectSingleNode("url1").InnerText;
			#elif UNITY_IPHONE
			url= node.SelectSingleNode("url2").InnerText;
			#endif

			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);

			Debug.Log ("posx" + posX);
			Debug.Log ("posy" + posY);

			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			int times = Convert.ToInt32(node.SelectSingleNode("times").InnerText);
			if(times == 0)
				times = 1000;
			float angle = Convert.ToSingle(node.SelectSingleNode("angle").InnerText );
			int brightness = Convert.ToInt32(node.SelectSingleNode("brightness").InnerText );
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);
			Debug.Log ("depth" + depth);
			Vector3 pos = GetButtonPosition(posX,  posY, depth);
			Debug.Log ("width" + width);
			Debug.Log ("height" + height);

			Vector3 scale = GetButtonScale(width, height);
			scale = new Vector3(scale.y * 15, scale.y * 15, scale.y * 15);

			string filename = url;
			while (filename.IndexOf ("/") > -1) {
				filename = filename.Substring (filename.IndexOf ("/") + 1);
			}

			Global.ThreeUnityName = filename;
			string path = Global.ThreeUnitySavePath + Global.ThreeUnityName;
			if (File.Exists (path)) {
				//				File.Delete (path);
				StartCoroutine(Down3D_Load_Unity(id, url, pos, angle, brightness, scale, times, type, true));
			}
			else
			{
				StartCoroutine(Down3D_Load_Unity(id, url, pos, angle, brightness, scale, times, type, false));
			}
		}
	}

	private void SetInfoLoadedObject(Vector3 pos, Vector3 scale){
		Debug.Log ("SetInfoLoadingObject.....");
		Global.bLoadedPackage = false;
		Global.goCharacter.transform.parent = this.transform;
		Debug.Log ("scale....." + scale.x + ":" + scale.y + ":" + scale.z);
		Global.goCharacter.transform.localScale = scale;
		Debug.Log ("pos=" + pos.x + ":" + pos.y + ":" + pos.z);
		Global.goCharacter.transform.position = pos;
		Global.goCharacter.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		Global.goCharacter.transform.name = string.Format ("starAR_{0}", Global.ThreeUnityName.Substring(0, Global.ThreeUnityName.Length - 4));
		Debug.Log ("Added name = " + Global.goCharacter.name);
		//	TimeSpan diff = System.DateTime.Now - loadunity3d;
		//	Debug.Log(string.Format("unity3d loading time : {0:ss-fff}", diff));
	}

	private void ProcessTextBoard(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessTextBoard");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			int type = Convert.ToInt32(node.SelectSingleNode("view").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			string content = UnityEngine.WWW.UnEscapeURL(node.SelectSingleNode("content").InnerText);
			string textcolor = node.SelectSingleNode("color").InnerText;
			if(textcolor.Length != 1)
				textcolor = textcolor.Substring(1);
			int boardtype = Convert.ToInt32(node.SelectSingleNode("boardtype").InnerText);
			string backcolor = node.SelectSingleNode("boardcolor").InnerText;
			if(backcolor.Length != 1)
				backcolor = backcolor.Substring(1);
			int showmode = Convert.ToInt32(node.SelectSingleNode("textplaymode").InnerText);

			Debug.Log(content);
			//string content = node.SelectSingleNode("content").InnerText;
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);	
			Vector3 pos = GetButtonPosition(posX, posY, depth);
			Vector3 scale = GetButtonScale(width, height);
			ShowTextBoard(id, type, pos, scale, content, textcolor, backcolor, boardtype, showmode);
		}
	}
	private void ShowTextBoard(int id, int type, Vector3 pos, Vector3 scale, string content, 
		string textcolor, string backcolor, int boardtype, int showmode)
	{
		GameObject newBoard = null;
		if (type == 0) {
			newBoard = Instantiate (Resources.Load ("Prefabs/TextBoard", typeof(GameObject))) as GameObject;
		} else if (type == 1) {
			newBoard = Instantiate (Resources.Load ("Prefabs/calendar", typeof(GameObject))) as GameObject;
			Rotate3DObject rotatescript = (Rotate3DObject)newBoard.GetComponent("Rotate3DObject");
			rotatescript.SetObjKind(Rotate3DObject.OBJKIND.OBJ_NOTEPAD);
			bEnable3DSlerp = true;
		} 
		else
			Debug.LogError("The type of TextBoard is invalid type = " + type);

		GameObject panel = newBoard.transform.Find ("panel").gameObject;
		GameObject text = newBoard.transform.Find ("text").gameObject;
		text.GetComponent<Renderer>().material.SetColor("_Color", HexToColor (textcolor));
		panel.GetComponent<Renderer>().material.color = HexToColor (backcolor);
		if(boardtype > 1)
			panel.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture> (string.Format("textboard/textboard_{0}", boardtype-1));

		newBoard.name = string.Format ("textBoard_{0}", id);
		newBoard.transform.parent = this.transform;
		newBoard.transform.localPosition = pos;
		if (type == 0){
			newBoard.transform.localScale = Vector3.one;
			panel.transform.localScale = new Vector3(scale.x, scale.z, scale.y);
		}else{
			newBoard.transform.localScale = new Vector3(scale.x * 25, scale.x * 25, scale.x * 25);
		}
		TextMesh textMesh = text.GetComponent<TextMesh> ();
		textMesh.text = content;

		TextBehavior boardBehavoir = (TextBehavior) newBoard.GetComponent("TextBehavior");
		boardBehavoir.showText = content;
		boardBehavoir.m_type = (TextBehavior.TYPE)type;
		boardBehavoir.m_ShowType = (TextBehavior.SHOWTYPE)showmode;
		mTrackableView.Add (newBoard);
		mTrackableView [mTrackableView.Count - 1].SetActive (false);
		HideObjectRenderer();
	}

	private void ProcessAudio(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessAudio");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);		//3d관련
			int btnkind = Convert.ToInt32(node.SelectSingleNode("btnkind").InnerText);	//2d버튼 관련
			if(type == 0)
				type = type + btnkind;
			else
				type = type + 2;

			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			string url = node.SelectSingleNode("url").InnerText;
			string backurl = node.SelectSingleNode("btn_url").InnerText;
			int run_opt = 0;	//Convert.ToInt32(node.SelectSingleNode("run_opt").InnerText);
			string hexColor = node.SelectSingleNode("color").InnerText;
			if(hexColor.Length != 1)
				hexColor = hexColor.Substring(1);
			int typebase = Convert.ToInt32(node.SelectSingleNode("mode_index").InnerText);		//기본 버튼 형태관련
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);
			Vector3 pos = GetButtonPosition(posX, posY, depth);
			Vector3 scale = GetButtonScale(width, height);
			ShowAudioButton(id, type, pos, scale, url, run_opt, hexColor, typebase, backurl);
		}
	}
	private void ShowAudioButton(int id, int type, Vector3 pos, Vector3 scale, string soundurl, int runopt, string hexColor, int typebase, string backurl)
	{
		GameObject newButton = null;
		GameObject panel = null;
		switch(type)
		{
		case 0:
			newButton = Instantiate (Resources.Load ("Button/custom/CustomButton", typeof(GameObject))) as GameObject;
			panel = newButton.transform.Find ("panel").gameObject;
			panel.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("blank");
			break;
		case 1:
			newButton = Instantiate (Resources.Load ("Button/common/CommonBtn", typeof(GameObject))) as GameObject;
			panel = newButton.transform.Find ("panel").gameObject;
			if(typebase <= 2)
				newButton.transform.Find ("background").GetComponent<Renderer>().material.color = HexToColor (hexColor);
			else
				newButton.transform.Find ("background").gameObject.SetActive(false);
			/*GameObject text = newButton.transform.Find ("text").gameObject;
			text.GetComponent<TextMesh>().text = label;*/
			string strAudioBtnImage = string.Format("Button/common/sound_{0}", typebase);
			panel.GetComponent<Renderer>().material = Instantiate ( Resources.Load(strAudioBtnImage, typeof(Material))) as Material;
			panel.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			break;
		case 2:
			newButton = Instantiate (Resources.Load ("Button/custom/CustomButton", typeof(GameObject))) as GameObject;
			panel = newButton.transform.Find ("panel").gameObject;
			StartCoroutine(work(panel, backurl));
			Global.TotalLoadingCount += 1;
			break;
		case 3:
			newButton = Instantiate (Resources.Load ("Prefabs/Audio", typeof(GameObject))) as GameObject;
			break;
		case 4:
			newButton = Instantiate (Resources.Load ("Prefabs/ClAudio", typeof(GameObject))) as GameObject;
			break;
		default:
			Debug.LogError("The type of AudioButton is invalid type = " + type);
			break;
		}

		newButton.name = string.Format ("audioButton_{0}", id);
		newButton.transform.parent = this.transform;
		newButton.transform.localPosition = pos;
		if(type <= 2)
			newButton.transform.localScale = scale;
		else
			newButton.transform.localScale = new Vector3(scale.x * 12, scale.x * 12, scale.x * 12);

		ButtonBehavior btnBehavior = (ButtonBehavior)newButton.GetComponent ("ButtonBehavior");
		btnBehavior.mUrl = soundurl;
		btnBehavior.mKind = ButtonBehavior.KIND.BTN_AUDIO;

		if (runopt == 1)
			btnBehavior.m_bAutoPlay = false;
		mTrackableView.Add (newButton);
		mTrackableView[mTrackableView.Count - 1].SetActive(false);
	}

	private void ProcessGoolgeButton(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessGoogleButton");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			string backurl = node.SelectSingleNode("backurl").InnerText;
			int depth = Convert.ToInt32 (node.SelectSingleNode("depth").InnerText);
			string hexColor = node.SelectSingleNode("color").InnerText;
			if(hexColor.Length != 1)
				hexColor = hexColor.Substring(1);
			int typebase = Convert.ToInt32(node.SelectSingleNode("mode_index").InnerText);
			string coordstr = node.SelectSingleNode("coordinate").InnerText;
			string[] coords = coordstr.Split(',');
			float fLat;		float fLong;
			if(coords.Length == 2) {
				fLat = Convert.ToSingle (coords[0]);
				fLong = Convert.ToSingle (coords[1]);
			} else {
				fLat = 126;
				fLong = 38;
			}

			string url = "";
			int tmpLat = (int)fLat;
			if(fLat - tmpLat > 0)
				url = string.Format("http://maps.google.com/maps?z=15&t=m&q=loc:{0}+{1}", fLat, fLong);
			else if(fLat - tmpLat == 0)
				url = string.Format("http://maps.google.com/maps?z=15&t=m&q=loc:{0}.0+{1}", fLat, fLong);
			Vector3 pos = GetButtonPosition(posX, posY, depth);
			Vector3 scale = GetButtonScale(width, height);
			ShowGoolgeButton(id, type, pos, scale, backurl, hexColor, typebase, url);
		}
	}
	private void ShowGoolgeButton(int id, int type, Vector3 pos, Vector3 scale, string backurl, string hexcolor, int typebase, string mapUrl)
	{
		GameObject newButton = null;
		if (type == 0){
			newButton = Instantiate (Resources.Load ("Button/common/CommonBtn", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			newButton.transform.Find ("background").GetComponent<Renderer>().material.color = HexToColor (hexcolor);
			string strGoogleBtnImage = string.Format("Button/common/google_{0}", typebase);
			panel.GetComponent<Renderer>().material = Instantiate ( Resources.Load(strGoogleBtnImage, typeof(Material))) as Material;
			panel.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		} else if (type == 1) {
			newButton = Instantiate (Resources.Load ("Button/custom/CustomButton", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			StartCoroutine(work(panel, backurl));
			Global.TotalLoadingCount += 1;
		} 
		else
			Debug.LogError("The type of TextBoard is invalid type = " + type);

		newButton.name = string.Format ("goolgeMap_{0}", id);
		newButton.transform.parent = this.transform;
		newButton.transform.localPosition = pos;
		newButton.transform.localScale = scale;

		ButtonBehavior boardBehavoir = (ButtonBehavior) newButton.GetComponent("ButtonBehavior");
		boardBehavoir.mKind = ButtonBehavior.KIND.BTN_GOOGLE;
		boardBehavoir.mUrl = mapUrl;
		mTrackableView.Add (newButton);
		HideObjectRenderer();
	}

	private void ProcessChromaKeyVideo(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessChromaKey");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			string url = node.SelectSingleNode("url").InnerText;
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			int runOptin = Convert.ToInt32(node.SelectSingleNode("run_opt").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			float basePosX, basePosY;
			Vector3	pos;	Vector3 scale;

			if (baseRate < 1.0f) {
				basePosX = 100 * (1 - baseRate) / 2;
				width *= baseRate;

				pos.x = (posX * baseRate + basePosX - 50) / 100;
				pos.y = depth * 0.0f;
				pos.z = (posY - 50) / 100;
			} else {
				basePosY = 100 * (1 - 1 / baseRate) / 2;
				height *= 1/baseRate;

				pos.x = (posX - 50)  / 100;
				pos.y = depth * 0.0f;
				pos.z = (posY * 1 / baseRate + basePosY - 50) / 100;
			}
			scale.x = -0.1f * (width / 100)*1.25f;
			scale.y = (height / 100) * 0.1f*1.25f;
			scale.z = -0.1f*1.25f;

			PlayChromaKey(id, type, pos, scale, url, runOptin);
		}
	}
	private void PlayChromaKey(int id, int type, Vector3 pos, Vector3 scale, string url, int runOptin)
	{
		Debug.Log("Showvideo type = " + type);
		GameObject newObject = null;
		GameObject panel = null;

		newObject = Instantiate(Resources.Load("EasyMovieTexture/Prefab/VideoChromakey", typeof(GameObject))) as GameObject;
		newObject.transform.parent = this.transform;
		newObject.transform.localPosition = pos;
		#if UNITY_IPHONE
		scale.x = -scale.x;
		scale.z = -scale.z;
		newObject.transform.localScale = scale;
		newObject.transform.rotation = Quaternion.Euler(270.0f, 180.0f, 180.0f);
		#else 
		newObject.transform.localScale = scale;
		newObject.transform.rotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
		//newObject.renderer.material = new Material(Shader.Find("Custom/NewShader"));
		#endif
		if(type == 1)
		{
			#if UNITY_IPHONE
			newObject.transform.rotation = Quaternion.Euler(180.0f, 180.0f, 180.0f);
			#else
			newObject.transform.rotation = new Quaternion(90.0f,0.0f,0.0f,0.0f);
			#endif
			//4.521544
			float height = Math.Abs(newObject.transform.lossyScale.y)/2 * 8.0f;
			Vector3 wPos = newObject.transform.position;
			newObject.transform.position = new Vector3(wPos.x, height, wPos.z);
		}
		newObject.name = string.Format ("Media_chromakey_{0}", id);
		mTrackableView.Add (newObject);

		HideObjectRenderer();
		MediaPlayerCtrl aa = (MediaPlayerCtrl)newObject.GetComponent ("MediaPlayerCtrl");
		aa.m_strFileName = url;
		if(runOptin == 1)
		{
			aa.m_bAutoPlay = false;
		}
		//	newObject.renderer.enabled = false;

		//aa.isChromakey = true;

		Rotate3DObject rotateScript = (Rotate3DObject)newObject.GetComponent ("Rotate3DObject");
		rotateScript.SetObjKind (Rotate3DObject.OBJKIND.OBJ_CHROMAKEY);
		bEnable3DSlerp = true;
		//	EventVideoTag (url);
		//	EventVideoTag (url);
	}

	private void ProcessCapture(XmlNodeList nodes)
	{
		foreach (XmlNode node in nodes)
		{
			Debug.Log("ProcessCapture");
			int id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
			int type = Convert.ToInt32(node.SelectSingleNode("type").InnerText);
			float posX = Convert.ToSingle(node.SelectSingleNode("posX").InnerText );
			float posY = Convert.ToSingle(node.SelectSingleNode("posY").InnerText);
			float width = Convert.ToSingle(node.SelectSingleNode("width").InnerText);
			float height = Convert.ToSingle(node.SelectSingleNode("height").InnerText);

			string hexColor = node.SelectSingleNode("color").InnerText;
			if(hexColor.Length != 1)
				hexColor = hexColor.Substring(1);
			int typebase = Convert.ToInt32(node.SelectSingleNode("buttontype").InnerText);
			string backurl = node.SelectSingleNode("backurl").InnerText;
			int depth = Convert.ToInt32(node.SelectSingleNode("depth").InnerText);

			List<GUIManager.CaptureSlide> urls = new List<GUIManager.CaptureSlide>();
			for(int i = 1; i<=5; i++)
			{
				string nodeName = string.Format("slide{0}", i); 
				string slide = node.SelectSingleNode(nodeName).InnerText;
				if(slide != "") 
				{
					string nodePosX = string.Format("posX{0}", i); 
					string nodePosY = string.Format("posY{0}", i); 
					string nodewidth = string.Format("width{0}", i); 
					string nodeheight = string.Format("height{0}", i); 
					float posX2 = Convert.ToSingle(node.SelectSingleNode(nodePosX).InnerText );
					float posY2 = Convert.ToSingle(node.SelectSingleNode(nodePosY).InnerText);
					float width2 = Convert.ToSingle(node.SelectSingleNode(nodewidth).InnerText);
					float height2 = Convert.ToSingle(node.SelectSingleNode(nodeheight).InnerText);
					GUIManager.CaptureSlide item = new GUIManager.CaptureSlide(slide, posX2, posY2, width2, height2);
					urls.Add(item);
				}
			}
			Vector3 pos = GetButtonPosition(posX, posY, depth);
			Vector3 scale = GetButtonScale(width, height);
			ShowCaptureButton(id, type, pos, scale, hexColor, typebase, backurl, urls);
		}
	}

	private void ShowCaptureButton(int id, int type, Vector3 pos, Vector3 scale, string hexColor, int typebase, string backurl,List<GUIManager.CaptureSlide> urls)
	{
		GameObject newButton = null;
		if(type == 0) {
			newButton = Instantiate(Resources.Load ("Button/common/CommonBtn", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			newButton.transform.Find ("background").GetComponent<Renderer>().material.color = HexToColor (hexColor);
			string strCaptureBtnImage = string.Format("Button/common/capture_{0}", typebase + 1);
			panel.GetComponent<Renderer>().material = Instantiate ( Resources.Load(strCaptureBtnImage, typeof(Material))) as Material;
			panel.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		} else if (type == 1) {
			newButton = Instantiate(Resources.Load ("Button/custom/CustomButton", typeof(GameObject))) as GameObject;
			GameObject panel = newButton.transform.Find ("panel").gameObject;
			StartCoroutine(work(panel, backurl));
			Global.TotalLoadingCount += 1;
		} else
			Debug.LogError("The type of WebButton is invalid type = " + type);

		newButton.name = string.Format ("captureButton_{0}", id);
		newButton.transform.parent = this.transform;
		newButton.transform.localPosition = pos;
		newButton.transform.localScale = scale;

		ButtonBehavior btnBehavior = (ButtonBehavior)newButton.GetComponent ("ButtonBehavior");
		btnBehavior.mKind = ButtonBehavior.KIND.BTN_CAPTURE;
		mTrackableView.Add (newButton);

		HideObjectRenderer();
		EventAddCaptureSlide(newButton.name, urls);
	}

	Vector3 GetButtonPosition(float posX, float posY, int depth)
	{
		float basePosX, basePosY;	Vector3 pos = Vector3.zero;
		if (baseRate < 1.0f){
			basePosX = 100 * (1 - baseRate) / 2;
			pos.x = (posX * baseRate + basePosX - 50) / 100;
			pos.y = depth * 0.01f;
			pos.z = (posY - 50) / 100;
		} else {
			basePosY = 100 * (1 - 1 / baseRate) / 2;
			pos.x = (posX - 50)  / 100;
			pos.y = depth * 0.01f;
			pos.z = (posY * 1 / baseRate + basePosY - 50) / 100;
		}
		return pos;
	}

	Vector3 GetButtonScale(float width, float height)
	{
		Vector3 scale = Vector3.zero;
		if (baseRate < 1.0f)
			width *= baseRate;
		else
			height *= 1 / baseRate;

		scale.x = (width / 100) * 0.1f;
		scale.y = 0.1f * (height / 100);
		scale.z = 0.1f;
		return scale;
	}

	private IEnumerator work(GameObject newPlane, string url)
	{
		WWW www =  new WWW(url);
		yield return www;
		Global.CurLoadingCount++;
		if(newPlane != null)
			newPlane.GetComponent<Renderer>().material.mainTexture = (Texture2D)www.texture;	
		if (newPlane.name.Contains("slideimage_") || newPlane.name.Contains("Background")) {
			float rate = (float)((float)www.texture.height / (float)www.texture.width);
			if(rate <= 1)
				newPlane.transform.localScale = new Vector3 (newPlane.transform.localScale.x, newPlane.transform.localScale.y, newPlane.transform.localScale.x * rate);
			else
				newPlane.transform.localScale = new Vector3 (newPlane.transform.localScale.z / rate, newPlane.transform.localScale.y, newPlane.transform.localScale.z);
			if( newPlane.name != "slideimage_0" && newPlane.name != "Background")
				newPlane.SetActive(false);
		}
	}

	IEnumerator Wait(float delaytime)
	{
		yield return new WaitForSeconds (delaytime);
		Global.CurLoadingCount = 1;
	}

	IEnumerator DownMarker(string url)
	{
		WWW www =  new WWW(url);

		yield return www;
		Global.CurLoadingCount++;
		string path = Global.MarkerSavePath + Global.CurMarkerName + ".png";
		try	{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllBytes(path, www.bytes);
		}catch (Exception exception){
			Debug.LogError(exception.ToString());
		}
	}

	IEnumerator Down3D_Load_Unity(int id, string urlString, Vector3 pos, float angle, int brightness, Vector3 scale, int times, int type, bool isDowned)
	{
		while (!is_destory) {
			yield return new WaitForSeconds (0.1f);
		}

		if (!isDowned) {
			WWW www =  new WWW(urlString);
			yield return www;
			string path = Global.ThreeUnitySavePath + Global.ThreeUnityName;
			try	{

				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.WriteAllBytes(path, www.bytes);
				string extractPath = Global.ThreeUnitySavePath + @"extract";

				//				string []dirs = Directory.GetFiles(extractPath, "*.*");
				//				foreach (string dir in dirs)
				//				{
				//					File.Delete(dir);
				//				}

				DecompressFile (path, extractPath);
			}
			catch(Exception) {

			}
		}

		while (!isDowned)
			yield return new WaitForSeconds (0.01f);

		try	{
			Debug.Log("222222");
			this.GetComponent<LoadAssets> ().assetBundleName = string.Format ("{0}.unity3d", Global.ThreeUnityName.Substring(0, Global.ThreeUnityName.Length - 4));
			this.GetComponent<LoadAssets> ().assetName = string.Format ("{0}", Global.ThreeUnityName.Substring(0, Global.ThreeUnityName.Length - 4));

			this.GetComponent<LoadAssets> ().SetVariable(string.Format ("{0}.unity3d", Global.ThreeUnityName.Substring(0, Global.ThreeUnityName.Length - 4)), 
				string.Format ("{0}", Global.ThreeUnityName.Substring(0, Global.ThreeUnityName.Length - 4)));

			this.GetComponent<LoadAssets> ().StartCoroutine("LoadPrefab");

		}
		catch(Exception)
		{

		}

		while (!Global.bLoadedPackage) {
			yield return new WaitForSeconds (0.1f);
		}

			Debug.Log("333333");
			if (Global.goCharacter == null) {
				Debug.Log ("unity3d file not found!");
				yield return null;
			}
			else {
				try{
					SetInfoLoadedObject (pos, scale);
					Global.goCharacter.transform.parent = this.transform;
					Global.goCharacter.transform.localScale = scale;
					Global.goCharacter.transform.localPosition = pos;
					if(type == 1)
					{
						Global.goCharacter.transform.localPosition = new Vector3(pos.x, 2.0f, pos.z);
					}
					Global.goCharacter.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
					Global.goCharacter.transform.name = string.Format ("starAR_{0}", Global.ThreeUnityName.Substring(0, Global.ThreeUnityName.Length - 4));
					Global.goCharacter.transform.rotation = Quaternion.Euler(0,angle,0);

					Debug.Log ("unity3d file found!");
					mTrackableView.Add (Global.goCharacter);
					mTrackableView[mTrackableView.Count - 1].SetActive(false);
					HideObjectRenderer();

					Rotate3DObject rotateScript = (Rotate3DObject)Global.goCharacter.AddComponent <Rotate3DObject>();
					rotateScript.SetObjKind (Rotate3DObject.OBJKIND.OBJ_3D);
					rotateScript.RotateObject = Global.goCharacter;
					bEnable3DSlerp = true;

					ThreeDControl TDctrlScript = (ThreeDControl)Global.goCharacter.AddComponent <ThreeDControl>();
					Debug.Log("ThreeControl Start ----------");
					TDctrlScript.times = times;
					TDctrlScript.brightness = (float)brightness;
					if(type == 0)
						TDctrlScript.type = ThreeDControl.TYPE.COMMON;
					else if(type == 1) {
						TDctrlScript.type = ThreeDControl.TYPE.UPDOWN;
						TDctrlScript.FinalPos = pos;
					} else 
						TDctrlScript.type = ThreeDControl.TYPE.FADEIN;
					Debug.Log("ThreeControl Setting Finished ----------");

				}catch (Exception exception){
					Debug.LogError(exception.ToString());
				}			
			}
	}

	private void HideObjectRenderer()
	{
		Renderer[] rendererComponents = mTrackableView[mTrackableView.Count - 1].GetComponentsInChildren<Renderer>(true);
		Collider[] colliderComponents = mTrackableView[mTrackableView.Count - 1].GetComponentsInChildren<Collider>(true);
		// Disable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = false;
		}
		// Disable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = false;
		}
	}

	private void OnFindingMarker()
	{
		//	Global.TotalLoadingCount = 0;
		//	Global.CurLoadingCount = 0;
		Global.bExistVideo = false;
		Global.bJyroSenser = false;
		Global.bLandscapeVideo = false;

		UnityEngine.Object.Destroy (air);
		air = null;
		if (mTrackableView != null) 
		{
			foreach (GameObject viewObject in mTrackableView) 
			{
				GameObject.DestroyImmediate(viewObject);
			}
			mTrackableView.Clear();
			is_destory = true;
		}
		if(Global.goCharacter != null)
		{
			GC.Collect ();
//			AssetBundleManager.UnloadAssetBundle (Global.ThreeUnityName);
//			Global.goCharacter = null;
//			AssetBundleManager.Unload();
//			ab.Unload (true);
		}
		// Start finder again if we lost the current trackable
		/*		ObjectTracker imageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		if(imageTracker != null)
		{
			imageTracker.TargetFinder.ClearTrackables(false);
			imageTracker.TargetFinder.StartRecognition();
		}
		bFoundMarker = false;*/
		bEnable3DSlerp = false;
		if(Screen.orientation != ScreenOrientation.Portrait)
			Screen.orientation = ScreenOrientation.Portrait;
	}

	private void OnWebView()
	{
		Global.ShowingCustomWebview = true;
	}

	private void OnWebViewClose()
	{
		GameObject webobj = GameObject.Find ("WebViewObject");
		GameObject.Destroy (webobj);
		Global.ShowingCustomWebview = false;
	}

	private void OnShowCapture(string captureName)
	{
		//	Global.TotalLoadingCount = 0;
		//	Global.CurLoadingCount = 0;
		Global.bExistVideo = false;
		Global.bJyroSenser = false;
		Global.bLandscapeVideo = false;
		bEnable3DSlerp = false;
		if (mTrackableView != null) 
		{
			foreach (GameObject viewObject in mTrackableView) 
			{
				GameObject.DestroyObject(viewObject);
			}
			mTrackableView.Clear();
		}
	}

	/*private void PlayPauseVideo(GameObject videoObj)
	{
		VideoPlaybackBehaviour vm = (VideoPlaybackBehaviour)videoObj.GetComponent ("VideoPlaybackBehaviour");
		vm.PlayPause ();
	}*/

	private void PlayVideoChromakey(GameObject videoObj)
	{
		MediaPlayerCtrl vm = (MediaPlayerCtrl)videoObj.GetComponent ("MediaPlayerCtrl");
		vm.Play ();
	}

	private Color HexToColor(string hex)
	{
		if (hex == "0")
			hex = "000000";
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}

	public void DecompressFile(string sourceFilePath, string targetDirectoryPath)
	{
		DirectoryInfo targetDirectoryInfo = new DirectoryInfo(targetDirectoryPath);

		if (!targetDirectoryInfo.Exists)
		{
			targetDirectoryInfo.Create();
		}

		FileStream sourceFileStream = new FileStream(sourceFilePath, FileMode.Open);

		ZipInputStream zipInputStream = new ZipInputStream(sourceFileStream);

		byte[] bufferByteArray = new byte[2048];

		while (true)
		{
			ZipEntry zipEntry = zipInputStream.GetNextEntry();

			if (zipEntry == null)
			{
				break;
			}

			if (zipEntry.Name.LastIndexOf('\\') > 0)
			{
				string subdirectory = zipEntry.Name.Substring(0, zipEntry.Name.LastIndexOf('\\'));

				if (!Directory.Exists(Path.Combine(targetDirectoryInfo.FullName, subdirectory)))
				{
					targetDirectoryInfo.CreateSubdirectory(subdirectory);
				}
			}

			FileInfo targetFileInfo = new FileInfo(Path.Combine(targetDirectoryInfo.FullName, zipEntry.Name));

			using (FileStream targetFileStream = targetFileInfo.Create())
			{
				while (true)
				{
					int readCount = zipInputStream.Read(bufferByteArray, 0, bufferByteArray.Length);

					if (readCount == 0)
					{
						break;
					}

					targetFileStream.Write(bufferByteArray, 0, readCount);
				}
			}
		}
		zipInputStream.Close();
		Debug.Log("111111");
	}
}
