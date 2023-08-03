using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;	
using Vuforia;

public class GUIManager : MonoBehaviour
{
	public delegate void FindingMarker();
	public static event FindingMarker	EventFindingMarker;

	float startTime;
	private Texture texShoot;
	private Texture texCamChange;
	private Texture	texFlashoff;
	private Texture texFlashon;
	private Texture	texClose;
	private Texture	texSlide;
	private Texture[]	texCaptureSlide;
	private Texture texFullvideo;
	private Texture texDownload;
	private Texture texLoading;
	private Texture texLikeAfter;
	private Texture texLikeBefore;
	private Texture texLikeList, texString;
	private Texture[]	texSearch = new Texture[5];

	private int 	curIndexofCaptureSlide = 0;
	private GUIStyle testStyle = new GUIStyle();
	private GUIStyle captureStyle = new GUIStyle();
	string saveDir = "SmartHanCamera"; // 저장 폴더 이름.
	private bool	hideGUI;
	private bool 	bHasVideo = false;
	private bool bIsLike = false;
	private int 	current = 0;

	private float curAngle = 0.0f, rotAngle = 0.0f;
	bool bIncrease = false;
	private enum PhoneType {PORTRAIT, LANDSLEFT, LANDSRIGHT};	//0:potrait		1:landscapeleft(홈버튼 윈쪽)	2:landscapeRight(홈버튼 오른쪽)
	private PhoneType mPhoneType = PhoneType.PORTRAIT;

//	private float 	guiRatio;
//	private float 	sWidth;
//	private Vector3 GUIsF;
//	bool kkk = false;

	public class CaptureSlide
	{
		private string 	url;
		private float 	posX;
		private float 	posY;
		private float 	width;
		private float 	height;
		public CaptureSlide(string surl, float x, float y, float w, float h)
		{
			url = surl;
			posX = x;
			posY = y;
			width = w;
			height = h;
		}
		public string Url 
		{
			get {return url; }
			set {url = value;}
		}
		public float PosX 
		{
			get {return posX; }
			set {posX = value;}
		}
		public float PosY
		{
			get {return posY; }
			set {posY = value;}
		}
		public float Width
		{
			get {return width; }
			set {width = value;}
		}
		public float Height 
		{
			get {return height; }
			set {height = value;}
		}

	}
	//added by jmh 2015-01-12
	private Hashtable HashCaptureList = null;
	//
	public List<CaptureSlide> CaptureSlideUrlArray;
	private Rect rectCapSlide;
	bool 	bClosebtn = false;				//마커가 검출 되였었는가
	bool 	bIsLikeView	= false;			//좋아요 보기 인가
	bool	bFlashlight = false;			//Camera Flashlight on/off
	bool	bCameraFacingFront = false;		//camera front or Back
	bool	bRelateCapture = false;			//capture 관련 button 현시할것인가
	private float rate = 0.0f;
	private string m_fullVideoUrl;

	bool 	bdownloading = false;
	bool 	bEnableGUI = true;

	private AudioClip sndShot;

#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern bool saveToGallery(string path);
	#endif

	void Start () {
//		texShoot = Resources.Load<Texture> ("image/cameraCapture");
//		texCamChange = Resources.Load<Texture> ("image/cameraChange");
		texFlashoff = Resources.Load<Texture> ("image/flash");
		texFlashon = Resources.Load<Texture> ("image/flashon");
		texClose = Resources.Load<Texture> ("image/close");
//		texSlide = Resources.Load<Texture> ("image/slide");
		texFullvideo = Resources.Load<Texture> ("image/full");
//		texDownload = Resources.Load<Texture> ("image/download");
		texLoading = Resources.Load<Texture> ("UserInterface/spinner_XHight");
		texLikeList = Resources.Load<Texture> ("Like/likelist");
		texLikeAfter = Resources.Load<Texture> ("Like/like_after");
		texLikeBefore = Resources.Load<Texture> ("Like/like_before");
		texString = Resources.Load<Texture> ("image/text");

//		texSearch [0] = Resources.Load<Texture> ("image/search/searching_1");
//		texSearch [1] = Resources.Load<Texture> ("image/search/searching_2");
//		texSearch [2] = Resources.Load<Texture> ("image/search/searching_3");
//		texSearch [3] = Resources.Load<Texture> ("image/search/searching_4");
//		texSearch [4] = Resources.Load<Texture> ("image/search/searching_5");
//		captureStyle.normal.background = Resources.Load<Texture2D> ("image/camera_on");
//		captureStyle.active.background = Resources.Load<Texture2D> ("image/camera_off");
		sndShot = Resources.Load<AudioClip> ("sound/camera-shutter-click-01");
		StartCoroutine (CountTime());
	
		//Screen.SetResolution (Screen.width, Screen.height, true);
		Debug.Log ("Screen.width = " + Screen.width + ", Screen.height = " + Screen.height );
		hideGUI = false;

		CloudRecoTrackableEventHandler.EventFoundMarker += OnFoundMarker;
		ButtonBehavior.EventCapture += OnShowCapture;
		LoadContent.EventVideoTag += OnSetVideoTag;
		LoadContent.EventAddCaptureSlide += OnAddCaptureSlide;
		LikeViewManager.EventStartLikeView += LikeViewSetting;
		ExitMsg.EventExitWindowStart += OnDisableGUI;
		ExitMsg.EventExitWindowEnd += OnEnableGUI;
	}
	void OnDestroy()
	{
		CloudRecoTrackableEventHandler.EventFoundMarker -= OnFoundMarker;
		ButtonBehavior.EventCapture -= OnShowCapture;
		LoadContent.EventVideoTag -= OnSetVideoTag;
		LoadContent.EventAddCaptureSlide -= OnAddCaptureSlide;
		LikeViewManager.EventStartLikeView -= LikeViewSetting;
		ExitMsg.EventExitWindowStart -= OnDisableGUI;
		ExitMsg.EventExitWindowEnd -= OnEnableGUI;
	}

	void jyroUpdate()
	{
		PhoneType tmpPhoneType = mPhoneType;
		if(Input.acceleration.y < -0.4f)
			mPhoneType = PhoneType.PORTRAIT;
		if(Input.acceleration.x > 0.4f)
			mPhoneType = PhoneType.LANDSLEFT;
		if(Input.acceleration.x < -0.4f)
			mPhoneType = PhoneType.LANDSRIGHT;
		if(Input.acceleration.y < -0.4f &&  Mathf.Abs(Input.acceleration.x) > 0.4f)
		{
			if(Mathf.Abs(Input.acceleration.y)-Mathf.Abs(Input.acceleration.x) > 0.15f)
				mPhoneType = PhoneType.PORTRAIT;
			else if(Mathf.Abs(Input.acceleration.y)-Mathf.Abs(Input.acceleration.x) < -0.15f)
			{
				if(Input.acceleration.x > 0)
					mPhoneType = PhoneType.LANDSLEFT;
				else
					mPhoneType = PhoneType.LANDSRIGHT;
			}
		}
		
		if(tmpPhoneType != mPhoneType)
		{
			Debug.Log ("x = " + Input.acceleration.x + ": y = " + Input.acceleration.y);
			switch(mPhoneType)
			{
			case PhoneType.LANDSLEFT:
				rotAngle = 270.0f;
				bIncrease = curAngle == 90.0f ? true: false;
				if(curAngle == 0.0f)
					curAngle += 360;
				break;
			case PhoneType.LANDSRIGHT:
				rotAngle = 90.0f;
				bIncrease = curAngle == 0.0f ? true: false;
				break;
			case PhoneType.PORTRAIT:
				rotAngle = 0.0f;
				bIncrease = curAngle == 90.0f ? false: true;
				break;
			}
		}
	}

	void CalcRotation(){
		
		if(bIncrease){
			curAngle = (curAngle + Time.deltaTime * 120.0f) % 360;
		}else{
			curAngle = (curAngle - Time.deltaTime * 120.0f) % 360;
		}
		if (Mathf.Abs (curAngle - rotAngle) < 5.0f)
			curAngle = rotAngle;
		//	Debug.Log (curAngle);
	}

	void OnGUI()
	{
		GUI.depth = 1;
		GUI.enabled = bEnableGUI;
		if (Global.ShowingCustomWebview)
			return;
		if (Global.bVideoFull)
			return;

		if (curAngle != rotAngle) {
			CalcRotation ();
		}

		float sWidth = Screen.width;
		float sHeight = Screen.height;
		Rect tempRect;
		if(bRelateCapture && texCaptureSlide[curIndexofCaptureSlide] != null)
		{
			rate = getCapSlideRect();
			GUIUtility.RotateAroundPivot (curAngle, rectCapSlide.center);
			GUI.DrawTexture(rectCapSlide, texCaptureSlide[curIndexofCaptureSlide], ScaleMode.ScaleToFit, true, rate);	
			GUI.matrix = Matrix4x4.identity;
		}

		if(!hideGUI)
		{
			if(bRelateCapture)
			{
				tempRect =  new Rect ((sWidth - sWidth/7 - 5)/2, (sHeight - sWidth/7) - sHeight/45, sWidth/7, sWidth/7);
				GUIUtility.RotateAroundPivot (curAngle, tempRect.center);
				if (GUI.Button (tempRect, "", captureStyle)) {
					GetComponent<AudioSource>().PlayOneShot(sndShot);
					StartCoroutine(screenCapture());	//Save ("ARSS", "Screenshot", true)
					StartCoroutine(HideGUI());
					Debug.Log ("Clicked the photoCapture Button!");
				}
				GUI.matrix = Matrix4x4.identity;

				tempRect = new Rect (sWidth/100, (sHeight - sWidth/8)- sHeight/45, sWidth/8, sWidth/8);
				GUIUtility.RotateAroundPivot (curAngle, tempRect.center);
				if (GUI.Button (tempRect, texCamChange, testStyle))
				{
					if(!bCameraFacingFront)
					{
						if(ChangeCameraDirection(CameraDevice.CameraDirection.CAMERA_FRONT)) {
							bCameraFacingFront = true;
						}
						else {
							ChangeCameraDirection(CameraDevice.CameraDirection.CAMERA_BACK);
							bCameraFacingFront = false;
						}
					}
					else
					{
						ChangeCameraDirection(CameraDevice.CameraDirection.CAMERA_BACK);
						bCameraFacingFront = false;
					}
					Debug.Log ("Clicked the CameraChange Button.");
				}
				GUI.matrix = Matrix4x4.identity;

				tempRect = new Rect (sWidth - sWidth/30 - sWidth/8, (sHeight - sWidth/8)- sHeight/45, sWidth/8, sWidth/8);
				GUIUtility.RotateAroundPivot (curAngle, tempRect.center);
				if (GUI.Button (tempRect, texSlide, testStyle))
				{
					Debug.Log ("Clicked the slide GUIbutton.");
					curIndexofCaptureSlide++;
					if(curIndexofCaptureSlide >= CaptureSlideUrlArray.Count)
						curIndexofCaptureSlide = 0;
				}
				GUI.matrix = Matrix4x4.identity;
			}
			/*else{
				if(!bIsLikeView)
				{
					if (GUI.Button (new Rect ( Screen.width - Screen.width/30 - Screen.width/9*2, Screen.height/35, Screen.width/9, Screen.width/9), texLikeList, testStyle))
					{
						Debug.Log ("Clicked the LikeList GUIbutton.");
						Application.LoadLevel("LikeList");
					}
				}
			}*/

/*			tempRect = new Rect (sWidth/30, sHeight/35, sWidth/9, sWidth/9);
			if(bFlashlight)
			{
				GUIUtility.RotateAroundPivot (curAngle, tempRect.center);
				if (GUI.Button (tempRect, texFlashoff, testStyle))
				{
					Debug.Log ("Clicked the Flashoff GUIbutton.");
					bFlashlight = false;
					TurnonoffFlashlight(bFlashlight);
				}
				GUI.matrix = Matrix4x4.identity;
			}
			else{
				GUIUtility.RotateAroundPivot (curAngle, tempRect.center);
				if (GUI.Button (tempRect, texFlashon, testStyle))
				{
					Debug.Log ("Clicked the Flashon GUIbutton.");
					bFlashlight = true;
					TurnonoffFlashlight(bFlashlight);
				}
				GUI.matrix = Matrix4x4.identity;
			}
*/
			if(!bClosebtn)
			{
				if (GUI.Button (new Rect ( Screen.width - Screen.width/30 - Screen.width/9 - 20, Screen.height/35 + 70, Screen.width/9, Screen.width/9), texClose, testStyle))
				{
					if(!this.transform.GetComponent<ExitMsg>())
						this.transform.gameObject.AddComponent<ExitMsg>();
				//	Application.Quit();
				}
/*
				GUI.DrawTexture (new Rect(Screen.width/2 - Screen.width* 604/1080/2, Screen.height*95/1920, Screen.width* 604/1080, Screen.height*46/1920) ,texString, ScaleMode.ScaleToFit);
				if (current != 0)
					GUI.DrawTexture (new Rect(Screen.width/2 - (Screen.height*100/960)* 3.5f/2, Screen.height*488/960, (Screen.height*100/960)* 3.5f, Screen.height*100/960) ,texSearch[current - 1]);
*/					
			}
			else
			{
/*				
				if(!bRelateCapture && !bIsLikeView)
				{
					if(!bIsLike)
					{
						if (GUI.Button (new Rect ( Screen.width - Screen.width/30 - Screen.width/9*3, Screen.height/35, Screen.width/9, Screen.width/9), texLikeBefore, testStyle))
						{
							bIsLike = true;
							AddLikeList();
						}
					}else{
						if (GUI.Button (new Rect ( Screen.width - Screen.width/30 - Screen.width/9*3, Screen.height/35, Screen.width/9, Screen.width/9), texLikeAfter, testStyle))
						{
							bIsLike = false;
							RemoveLikeList();
						}	
					}
				}
*/			
				tempRect =  new Rect ( sWidth - sWidth/3.5f, sHeight/35 + 10, sWidth/9, sWidth/9);
				GUIUtility.RotateAroundPivot (curAngle, tempRect.center);
				if (GUI.Button (tempRect, texClose, testStyle))
				{
					if(bIsLikeView){
						bIsLikeView = false;
						Application.LoadLevel("LikeList");
					}else{
						if(!Global.bLoading)
						{
							bClosebtn = false;
							bRelateCapture = false;
							CancelInvoke("jyroUpdate");
							if(bCameraFacingFront)
							{
								ChangeCameraDirection(CameraDevice.CameraDirection.CAMERA_BACK);
								bCameraFacingFront = false;
							}
							if(HashCaptureList != null)
							{
								HashCaptureList.Clear();
								HashCaptureList = null;
								texCaptureSlide = null;
								Resources.UnloadUnusedAssets();
							}
							EventFindingMarker();
							bHasVideo = false;
						}
						else{
							Debug.Log ("Clicked the Close GUIbutton.");
						}
					}
				}
				GUI.matrix = Matrix4x4.identity;
				/*if(bHasVideo && !bRelateCapture)
				{
					if (GUI.Button (new Rect ( Screen.width/2 + Screen.width/18, Screen.height*33/35- Screen.width/9, Screen.width/9, Screen.width/9), texFullvideo ,testStyle))
					{
						Screen.orientation = ScreenOrientation.LandscapeLeft;
						Handheld.PlayFullScreenMovie(m_fullVideoUrl, UnityEngine.Color.black, FullScreenMovieControlMode.Full);		//"file:///sdcard/DCIM/AR/1.mp4"
					}
					if(!bdownloading)
					{
						if (GUI.Button (new Rect ( Screen.width/2 - Screen.width*3/18, Screen.height*33/35 - Screen.width/9, Screen.width/9, Screen.width/9), texDownload ,testStyle))
						{
							Debug.Log ("click download button!");
							StartCoroutine(DownloadStreamingVideo(m_fullVideoUrl));
							bdownloading = true;
						}
					}
					else
					{
						float thisAngle = Time.frameCount*5;
						
						Rect thisRect = new Rect(Screen.width/2 - Screen.width*3/18, Screen.height*33/35 - Screen.width/9, Screen.width/9, Screen.width/9);
						
						GUIUtility.RotateAroundPivot(thisAngle, thisRect.center);
						GUI.DrawTexture(thisRect, texLoading, ScaleMode.StretchToFill,true);
					}
				}
*/
			}
		}
		else 
			screenCapture();
	}

	void RemoveLikeList()
	{
		Global.likeNames.Remove (Global.CurMarkerName);
		BinaryFormatter bf = new BinaryFormatter ();
		
		if (File.Exists(Global.strLikeInfoPath))
		{
			File.Delete(Global.strLikeInfoPath);
		}
		FileStream fdata = File.Create(Global.strLikeInfoPath);
		try{
			bf.Serialize (fdata, Global.likeNames);
		}
		catch( SerializationException e)
		{
			Debug.LogError(e.Message);
		}
		finally{
			fdata.Close();
		}
	}

	void AddLikeList()
	{
		Global.likeNames.Add (Global.CurMarkerName);
		BinaryFormatter bf = new BinaryFormatter ();
		
		if (File.Exists(Global.strLikeInfoPath))
		{
			File.Delete(Global.strLikeInfoPath);
		}
		FileStream fdata = File.Create(Global.strLikeInfoPath);
		try{
			bf.Serialize (fdata, Global.likeNames);
		}
		catch( SerializationException e)
		{
			Debug.LogError(e.Message);
		}
		finally{
			fdata.Close();
		}
	}

	void OnApplicationPause(bool tf)
	{
		//When the video finishes playing on fullscreen mode, Unity application unpauses and that's when we need to switch to potrait
		//in order to display the UI menu options properly
		if(!tf) {
			Screen.orientation = ScreenOrientation.Portrait;
		}
	}

	IEnumerator CountTime ()
	{
		yield return new WaitForSeconds (0.5f);
		current++;
		if(current > 5)
			current = 1;
		if(current <= 5)
			StartCoroutine (CountTime());
	}

	IEnumerator DownloadStreamingVideo(string strURL)
	{
		strURL = strURL.Trim();
		
		Debug.Log ("DownloadStreamingVideo : " + strURL);
		
		
		WWW www = new WWW(strURL);
		
		yield return www;
		
		if(string.IsNullOrEmpty(www.error))
		{
			string write_path = "";
		
		//	string dirPath;
#if UNITY_IPHONE
			write_path = Application.persistentDataPath + "/" + saveDir;
			if (!Directory.Exists(write_path))
			{
				Directory.CreateDirectory(write_path);
			}
			write_path = string.Format("{0}/video{1}.mp4", write_path, System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
#elif UNITY_ANDROID	
			write_path = "mnt/sdcard/DCIM/" + saveDir;
			if (!Directory.Exists(write_path))
			{
				Directory.CreateDirectory(write_path);
			}
			write_path = string.Format("{0}/video{1}.mp4", write_path, System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
#endif
			
			if(System.IO.File.Exists(write_path) == true)
			{
				Debug.Log("Delete : " + write_path);
				System.IO.File.Delete(write_path);
			}
			
			System.IO.File.WriteAllBytes(write_path, www.bytes);
			if(www.isDone)
			{
				bdownloading = false;
				yield return 0;
			}
		}
		else
		{
			Debug.Log(www.error);
			
		}
		
		www.Dispose();
		www = null;
		Resources.UnloadUnusedAssets();
	}

	private bool ChangeCameraDirection(CameraDevice.CameraDirection direction)
	{
		// This takes care of stopping and starting the targetFinder internally upon switching the camera
		CloudRecoBehaviour cloudRecoBehaviour = GameObject.FindObjectOfType(typeof(CloudRecoBehaviour)) as CloudRecoBehaviour;
		cloudRecoBehaviour.CloudRecoEnabled = false;
		
		bool directionSupported = false;
		CameraDevice.Instance.Stop();
		CameraDevice.Instance.Deinit();
		if(CameraDevice.Instance.Init(direction)) {
			directionSupported = true;
		}
		CameraDevice.Instance.Start();

		return directionSupported;
	}

	void TurnonoffFlashlight(bool tf)
	{
		if(tf)
		{
			CameraDevice.Instance.SetFlashTorchMode(true);
		}
		else 
		{
			CameraDevice.Instance.SetFlashTorchMode(false);
		}
	}
	void OnSetVideoTag (string url)
	{
		bHasVideo = true;
		bdownloading = false;
		m_fullVideoUrl = url;
	}

	void OnAddCaptureSlide(string objName, List<CaptureSlide> urls)
	{
		if (HashCaptureList == null) 
		{
			HashCaptureList = new Hashtable();
		}
		HashCaptureList.Add (objName, urls);

		CaptureSlideUrlArray = (List<CaptureSlide>)HashCaptureList [objName];
		curIndexofCaptureSlide = 0;
		texCaptureSlide = new Texture[CaptureSlideUrlArray.Count];
		for(int i =0; i < CaptureSlideUrlArray.Count; i++)
			StartCoroutine ( SetUrlTexture(i));
	}

	void OnShowCapture(string captureName)
	{
		curIndexofCaptureSlide = 0;
		bRelateCapture = true;
		InvokeRepeating("jyroUpdate", 0.5f, 1.0f);
	}
	void OnFoundMarker()
	{
		bClosebtn = true;
		bIsLike = Global.likeNames.Contains (Global.CurMarkerName);
	}
	void LikeViewSetting()
	{
		bClosebtn = true;
		bIsLikeView = true;
	}

	void OnEnableGUI(){
		bEnableGUI = true;
	}

	void OnDisableGUI(){
		bEnableGUI = true;
	}

	IEnumerator HideGUI ()
	{
		hideGUI = true;
		
		yield return new WaitForEndOfFrame();
		
		hideGUI = false;	
	}

	public IEnumerator screenCapture()
	{
		bool photoSaved = false;
		yield return new WaitForEndOfFrame();
		Texture2D captureTexture;
		captureTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true); // texture formoat setting
		captureTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true); // readPixel
		captureTexture.Apply(); // readPixel data apply

	//	byte[] bytes = captureTexture.EncodeToPNG();
	//	File.WriteAllBytes(getCaptureName(), bytes);
		int imageHeight = Screen.height;
		int imageWidth = (int)((float)imageHeight * 9 / 16);
		byte[] bytes;
		Texture2D saveTexture;
		Color[] capturecolor = captureTexture.GetPixels ();
		Color[] saveColor = new Color[imageWidth*imageHeight];
		saveTexture = new Texture2D (imageWidth, imageHeight, TextureFormat.RGB24, true);
		for (int i = 0; i< Screen.height; i++) {
			for(int j = 0; j<Screen.width; j++){
				int delta = (Screen.width - imageWidth)/2;
				if(j >= delta && j < delta + imageWidth)
					saveColor[i*imageWidth + (j-delta)] = capturecolor[i*Screen.width + j];
			}
		}
		saveTexture.SetPixels (saveColor);
		saveTexture.Apply ();
		saveColor = null;
		////////////////////////////
		if (mPhoneType != PhoneType.PORTRAIT) {
			Texture2D totalTexture;
			Color[] tempcolor = saveTexture.GetPixels ();
			Color[] totalColor = new Color[tempcolor.Length];
			Debug.Log ("asdfasdfasdfasdf" + tempcolor.Length);

			totalTexture = new Texture2D (imageHeight, imageWidth, TextureFormat.RGB24, true);
			if (mPhoneType == PhoneType.LANDSRIGHT) {
				for (int i = 0; i< imageHeight; i++) {
					for (int j = 0; j<imageWidth; j++) {
						totalColor [(imageHeight - i - 1) + j * imageHeight] = tempcolor [i * imageWidth + j];
					}
				}
			} else if (mPhoneType == PhoneType.LANDSLEFT) {
				for (int i = 0; i< imageHeight; i++) {
					for (int j = 0; j<imageWidth; j++) {
						totalColor [i + (imageWidth - j - 1) * imageHeight] = tempcolor [i * imageWidth + j];
					}
				}
			}
			totalTexture.SetPixels (totalColor);
			totalTexture.Apply ();
			totalColor = null;
			bytes = totalTexture.EncodeToPNG ();
			File.WriteAllBytes (getCaptureName (), bytes);

		} else if (mPhoneType == PhoneType.PORTRAIT) {
			bytes = saveTexture.EncodeToPNG ();
			File.WriteAllBytes (getCaptureName (), bytes);
		}


		///////////////////////////
	
	

		#if UNITY_ANDROID
		//REFRESHING THE ANDROID PHONE PHOTO GALLERY IS BEGUN
		try {
			AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");        
			AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");        
			AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2]{"android.intent.action.MEDIA_MOUNTED", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + getCaptureName())});        
			objActivity.Call ("sendBroadcast", objIntent);
		}catch (System.Exception) {
			try{
				AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");        
				AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");        
				AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2]{"android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + getCaptureName())});        
				objActivity.Call ("sendBroadcast", objIntent);
			}catch(System.Exception){
				AndroidJavaClass obj = new AndroidJavaClass("com.ryanwebb.androidscreenshot.MainActivity");
				while(!photoSaved) 
				{
					photoSaved = obj.CallStatic<bool>("addImageToGallery", getCaptureName());
					
				/*	yield return */StartCoroutine(GUIManager.Wait(.5f));
				}
			}
		}
		//REFRESHING THE ANDROID PHONE PHOTO GALLERY IS COMPLETE

		#elif UNITY_IPHONE

		while(!photoSaved) 
		{
			photoSaved = saveToGallery(getCaptureName());
			
			yield return StartCoroutine(GUIManager.Wait(.5f));
		}
		
		UnityEngine.iOS.Device.SetNoBackupFlag(getCaptureName());
		#endif

		Debug.Log(string.Format("Capture Success: {0}", getCaptureName()));
	}
	float getCapSlideRect()
	{
		float W = Screen.width;
		float H = Screen.height;
		float x = CaptureSlideUrlArray [curIndexofCaptureSlide].PosX * H/100;
		float y = CaptureSlideUrlArray [curIndexofCaptureSlide].PosY * W/100;
		float wid = CaptureSlideUrlArray [curIndexofCaptureSlide].Width * H/100;
		float high = CaptureSlideUrlArray [curIndexofCaptureSlide].Height * W/100;
		
		float tx = W - y - high + (high - wid) / 2;
		float ty = x - (high - wid) / 2;
		
		rectCapSlide = new Rect(tx, ty , wid, high);
		return wid / high;
	}
	public string getCaptureName()
	{
		string dirPath = "";
#if UNITY_IPHONE
		dirPath = Application.persistentDataPath + "/" + saveDir;
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}
		return string.Format("{0}/capture_{1}.png", dirPath, System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
#elif UNITY_ANDROID	
		dirPath = "mnt/sdcard/DCIM" + "/" + saveDir;
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}
		return string.Format("{0}/capture_{1}.jpg", dirPath, System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
		//	return string.Format("mnt/sdcard/DCIM/Camera/numberaa.png");
#else
		if( Application.isEditor == true ){ 
			dirPath = "" + saveDir;
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
			return string.Format("{0}/capture_{1}.jpg", dirPath, System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
		} 
#endif
		return "";
	}

	private IEnumerator SetUrlTexture(int itemNo)
	{

		WWW www =  new WWW(CaptureSlideUrlArray[itemNo].Url);
		yield return www;
		
		Texture texTempImage = (Texture2D)www.texture;
		texCaptureSlide [itemNo] = texTempImage;
	}

	public static IEnumerator Wait(float delay)
	{
		float pauseTarget = Time.realtimeSinceStartup + delay;
		
		while(Time.realtimeSinceStartup < pauseTarget)
		{
			yield return null;	
		}
	}

	public static int ScreenShotNumber 
	{
		set { PlayerPrefs.SetInt("screenShotNumber", value); }
		
		get { return PlayerPrefs.GetInt("screenShotNumber"); }
	}
}
