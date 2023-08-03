using UnityEngine;
using System.Collections;

public class RotateVideo : MonoBehaviour {

	private enum PhoneType {PORTRAIT, LANDSLEFT, LANDSRIGHT};	//0:potrait		1:landscapeleft(홈버튼 윈쪽)	2:landscapeRight(홈버튼 오른쪽)
	private PhoneType mPhoneType = PhoneType.PORTRAIT;
	// Use this for initialization
	void Start () {
		InvokeRepeating("jyroUpdate", 0.5f, 1.0f);
		CloudRecoTrackableEventHandler.EventMatchingVideo += RestoreOrg;
	}

	void OnDestroy()
	{
		CancelInvoke("jyroUpdate");
		CloudRecoTrackableEventHandler.EventMatchingVideo -= RestoreOrg;
	}
	// Update is called once per frame
	void Update () {
	
	}
	void OnRotateOrg(){
		GameObject rotVideo = this.transform.FindChild (Global.videoName).gameObject;
		if(rotVideo == null)
			Debug.LogError("RotateVideo is null!");

		Global.bLandscapeVideo = false;
#if UNITY_ANDROID
		rotVideo.transform.localPosition = Global.orgVideoPos;
		rotVideo.transform.localRotation = Quaternion.Euler(Global.orgVideoQuat.x, 
		                                                    Global.orgVideoQuat.y, Global.orgVideoQuat.z);
		rotVideo.transform.localScale = new Vector3(Global.orgVideoScale.x,
		                                            Global.orgVideoScale.y, Global.orgVideoScale.z);
#elif UNITY_IPHONE
		rotVideo.transform.localPosition = Global.orgVideoPos;
		rotVideo.transform.localRotation = Quaternion.Euler(Global.orgVideoQuat.x, 
		                                                    Global.orgVideoQuat.y, Global.orgVideoQuat.z);
		rotVideo.transform.localScale = new Vector3(Global.orgVideoScale.x,
		                                            Global.orgVideoScale.y, Global.orgVideoScale.z);
#endif
	}
	void RestoreOrg()
	{
		this.GetComponent<LoadContent>().ShowAllObject();
		Global.bLandscapeVideo = false;
		GameObject rotVideo = this.transform.FindChild (Global.videoName).gameObject;
		rotVideo.transform.FindChild("VideoControl").GetComponent<VideoControlBehavior>().VideoRestore();
	}

	void jyroUpdate()
	{
		if(!Global.bJyroSenser)
			return;
		PhoneType tmpPhoneType = mPhoneType;
		if(Input.acceleration.y < -0.4f)
			mPhoneType = PhoneType.PORTRAIT;
		if(Input.acceleration.x > 0.4f)
			mPhoneType = PhoneType.LANDSRIGHT;
		if(Input.acceleration.x < -0.4f)
			mPhoneType = PhoneType.LANDSLEFT;

		if(Input.acceleration.y < -0.4f &&  Mathf.Abs(Input.acceleration.x) > 0.4f)
		{
			if(Mathf.Abs(Input.acceleration.y)-Mathf.Abs(Input.acceleration.x) > 0.15f)
				mPhoneType = PhoneType.PORTRAIT;
			else if(Mathf.Abs(Input.acceleration.y)-Mathf.Abs(Input.acceleration.x) < -0.15f)
			{
				if(Input.acceleration.x > 0)
					mPhoneType = PhoneType.LANDSRIGHT;
				else
					mPhoneType = PhoneType.LANDSLEFT;
			}
		}

		GameObject rotVideo = this.transform.FindChild (Global.videoName).gameObject;
		if(rotVideo == null)
			Debug.LogError("RotateVideo is null!");

		if(tmpPhoneType != mPhoneType)
		{
			Debug.Log ("x = " + Input.acceleration.x + ": y = " + Input.acceleration.y);
			switch(mPhoneType)
			{
			case PhoneType.LANDSLEFT:
			//	this.GetComponent<LoadContent>().HideAllObject();
				Global.bLandscapeVideo = true;
			/*	rotVideo.transform.localPosition = Vector3.zero;
				rotVideo.transform.localRotation = Quaternion.Euler(0, 90, 0);
				rotVideo.transform.localScale = new Vector3(0.158531f, Global.orgVideoScale.y, -0.08565517f);
				rotVideo.transform.FindChild("VideoControl").FindChild("full").gameObject.SetActive(false);
				rotVideo.transform.FindChild("VideoControl").FindChild("restore").gameObject.SetActive(true);*/
				rotVideo.transform.FindChild("VideoControl").GetComponent<VideoControlBehavior>().VideoFull(true);
				break;
			case PhoneType.LANDSRIGHT:
			//	this.GetComponent<LoadContent>().HideAllObject();
				Global.bLandscapeVideo = true;
				/*rotVideo.transform.localPosition = Vector3.zero;
				rotVideo.transform.localRotation = Quaternion.Euler(0, 270, 0);
				rotVideo.transform.localScale = new Vector3(0.158531f, Global.orgVideoScale.y, -0.08565517f);
				rotVideo.transform.FindChild("VideoControl").FindChild("full").gameObject.SetActive(false);
				rotVideo.transform.FindChild("VideoControl").FindChild("restore").gameObject.SetActive(true);*/
				rotVideo.transform.FindChild("VideoControl").GetComponent<VideoControlBehavior>().VideoFull(false);
				break;
			case PhoneType.PORTRAIT:
				this.GetComponent<LoadContent>().ShowAllObject();
				Global.bLandscapeVideo = false;
				/*rotVideo.transform.localPosition = Global.orgVideoPos;
				rotVideo.transform.localRotation = Quaternion.Euler(Global.orgVideoQuat.x, 
				                                                    Global.orgVideoQuat.y, Global.orgVideoQuat.z);
				rotVideo.transform.localScale = new Vector3(Global.orgVideoScale.x,
				                                            Global.orgVideoScale.y, Global.orgVideoScale.z);
				rotVideo.transform.FindChild("VideoControl").FindChild("full").gameObject.SetActive(true);
				rotVideo.transform.FindChild("VideoControl").FindChild("restore").gameObject.SetActive(false);*/
				rotVideo.transform.FindChild("VideoControl").GetComponent<VideoControlBehavior>().VideoRestore();
				break;
			}
		}
//#endif
	}
}
