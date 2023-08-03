using UnityEngine;
using System.Collections;

public class Rotate3DObject : MonoBehaviour {

	public enum ROTSTATE {ROT_NONE = 0, ROT_SHOW, ROT_ORG};
	public enum OBJKIND { OBJ_VIDEO = 0, OBJ_IMAGE3D,OBJ_NOTEPAD,OBJ_CHROMAKEY,OBJ_3D};
	public GameObject RotateObject;
	//private bool bSlerp = false;
	private float 		rotSmooth = 7.0f;
	private float		posSmooth = 7.0f;
	private Quaternion 	rotOrg;
	private Quaternion 	rotShow;
	private ROTSTATE 	m_rotState;
	private OBJKIND 	m_objKind = OBJKIND.OBJ_VIDEO;
	private Vector3		posShow = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3		posOrg;
	private bool 		bRotated = false;		//전에 한번이상 회전한적이 있는가

	// Use this for initialization
	void Start () {

		CloudRecoTrackableEventHandler.EventTrackingChanged += OnTrackingChanged;
		CanvasBehavior.EventCompleteCanvasAction += OnTrackingChanged;
		FadeIn3DAni.EventCompleteFadeAction += OnTrackingChanged;
		ThreeDControl.EventComplete3DAction += OnTrackingChanged;
		LoadContent.EventSlerpEnd += OnSlerpEnd;
		m_rotState = ROTSTATE.ROT_NONE;
	}

	void OnDestroy()
	{
		CloudRecoTrackableEventHandler.EventTrackingChanged -= OnTrackingChanged;
		CanvasBehavior.EventCompleteCanvasAction += OnTrackingChanged;
		FadeIn3DAni.EventCompleteFadeAction -= OnTrackingChanged;
		ThreeDControl.EventComplete3DAction -= OnTrackingChanged;
		LoadContent.EventSlerpEnd -= OnSlerpEnd;
	}
	// Update is called once per frame
	void Update () {
		if (m_rotState == ROTSTATE.ROT_SHOW) 
		{
			if(RotateObject != null)
			{
				if(RotateObject.name.Contains("Image"))
				{
					CanvasBehavior canBe =  (CanvasBehavior) RotateObject.GetComponent<CanvasBehavior>();
					if(canBe.bShowSlide)
					{
						RotateObject.transform.position = Vector3.Slerp(RotateObject.transform.position, posShow, Time.deltaTime * posSmooth);
						RotateObject.transform.rotation = Quaternion.Slerp (RotateObject.transform.rotation, rotShow, Time.deltaTime * rotSmooth);
					}
				}
				else
				{
					RotateObject.transform.position = Vector3.Slerp(RotateObject.transform.position, posShow, Time.deltaTime * posSmooth);
					RotateObject.transform.rotation = Quaternion.Slerp (RotateObject.transform.rotation, rotShow, Time.deltaTime * rotSmooth);
				}			
			}
		}
		else if(m_rotState == ROTSTATE.ROT_ORG)
		{
			if(RotateObject != null)
			{
				RotateObject.transform.rotation = rotOrg;
				RotateObject.transform.position = posOrg;
			}
		}
	}

	private void OnTrackingChanged(bool bFound)
	{
		if (bFound == false) 
		{
			bRotated = true;
			m_rotState = ROTSTATE.ROT_SHOW;
			if(RotateObject != null)
			{
				rotOrg = RotateObject.transform.rotation;
				posOrg = RotateObject.transform.position;
			}

			switch(m_objKind)
			{
			case OBJKIND.OBJ_VIDEO:
				rotShow = Quaternion.Euler (270.0f, 0.0f, 0.0f);
				posShow = posOrg;
				break;
			case OBJKIND.OBJ_IMAGE3D:
				rotShow = Quaternion.Euler (90.0f, 0.0f, 180.0f);
				//posShow = posOrg - new Vector3(0.0f, 0.0f, -30.0f);
				posShow = posOrg;
				break;
			case OBJKIND.OBJ_NOTEPAD:
				rotShow = Quaternion.Euler (0.0f, 180.0f, 0.0f);
				posShow = posOrg;
				break;
			case OBJKIND.OBJ_CHROMAKEY:
#if UNITY_IPHONE
				rotShow = Quaternion.Euler (270.0f, 0.0f, 0.0f);
#else
				rotShow = Quaternion.Euler (90.0f, 0.0f, 0.0f);
#endif
				posShow = posOrg;
				break;
			case OBJKIND.OBJ_3D:
#if UNITY_IPHONE
				rotShow = Quaternion.Euler (90.0f, 0.0f, 180.0f);
#else
				//rotShow = Quaternion.Euler (90.0f, 0.0f, 180.0f);
				rotShow = Quaternion.Euler (-90.0f, 180.0f, 180.0f);
#endif
				posShow = posOrg;
				break;
			}
		}
		else
		{
			if(bRotated)
				m_rotState = ROTSTATE.ROT_ORG;
		}
	}
	private void OnSlerpEnd()
	{
		m_rotState = ROTSTATE.ROT_NONE;
	}
	public void SetObjKind(OBJKIND kind)
	{
		m_objKind = kind;
	}
}
