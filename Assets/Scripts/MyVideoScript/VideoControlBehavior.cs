using UnityEngine;
using System.Collections;

public class VideoControlBehavior : MonoBehaviour {
    /*
	public GameObject play;
	public GameObject pause;
	public GameObject full;
	public GameObject restore;
	public GameObject parent;		//부모object VideoManager 
	public bool bARCamera = false;
	public bool bPortraitMode = true;

	private Vector3 defaultPos;
	private Vector3 defaultScale;
	private Quaternion defaultRotation;
	private GameObject fullCamObject;
	
	// Use this for initialization
	void Awake()
	{
		fullCamObject = GameObject.Find("FullCamera");
		if (fullCamObject == null)
			Debug.LogError("Full Camera is Null!");
		fullCamObject.GetComponent<Camera>().enabled = false;
	}
	void Start () {
		if (this.parent.GetComponent<VideoPlaybackBehaviour> ().AutoPlay)
			play.SetActive (false);
		else
			pause.SetActive (false);
	}

	public void VideoFull(bool bLeft)
	{
		Debug.Log ("VideoFull");
		if (Global.bVideoFull == true)
			return;
		fullCamObject.GetComponent<Camera>().enabled = true;

		if(parent.transform.parent.transform.GetComponent<LoadContent>() != null)
		    parent.transform.parent.transform.GetComponent<LoadContent>().HideAllObject(this.transform.parent.name);

		defaultPos = parent.transform.localPosition;
		defaultScale = parent.transform.localScale;
		defaultRotation = parent.transform.localRotation;

		bARCamera = false;
		Camera cam = fullCamObject.GetComponent<Camera>();
		float pos = (cam.nearClipPlane + 50.0f);
		parent.transform.position = cam.transform.position + cam.transform.forward * pos;
		parent.transform.LookAt (cam.transform.parent);
	//	parent.transform.Rotate (90.0f, 0.0f, 0.0f);
		float h = (Mathf.Tan(cam.fieldOfView*Mathf.Deg2Rad*0.5f)*pos*2.0f);
		parent.transform.localScale = new Vector3(-h,0.1f, h*cam.aspect)/(10.0f*this.parent.transform.parent.transform.localScale.y);
		parent.transform.Rotate (0.0f, 90.0f, 0.0f);
		
		if (bLeft == false)
		{
			parent.transform.Rotate (0.0f, 180.0f, 0.0f);
		}
	
		bPortraitMode = false;
		full.SetActive (false);
		restore.SetActive (true);

		Global.bVideoFull = true;
	}
	
	public void VideoRestore()
	{
		if (Global.bVideoFull == false)
			return;
		fullCamObject.GetComponent<Camera>().enabled = false;

		if(parent.transform.parent.transform.GetComponent<LoadContent>() != null)
		    parent.transform.parent.transform.GetComponent<LoadContent>().ShowAllObject();

		parent.transform.localPosition = defaultPos;
		parent.transform.localRotation = defaultRotation;
		parent.transform.localScale = defaultScale;

		bPortraitMode = true;
		full.SetActive (true);
		restore.SetActive (false);

		Global.bVideoFull = false;
	}


	public void VideoPlay()
	{
		VideoPlaybackBehaviour VideoCtrl = (VideoPlaybackBehaviour)parent.GetComponent<VideoPlaybackBehaviour> ();
		Debug.Log ("panel'name = " + parent.transform.parent.gameObject.name);
		if (VideoCtrl.mCurrentState == VideoPlayerHelper.MediaState.REACHED_END ||
		    VideoCtrl.mCurrentState == VideoPlayerHelper.MediaState.READY)
			VideoCtrl.VideoPlayer.Play (false, 0);
		else if(VideoCtrl.mCurrentState == VideoPlayerHelper.MediaState.PAUSED ||
		        VideoCtrl.mCurrentState == VideoPlayerHelper.MediaState.STOPPED)
			VideoCtrl.VideoPlayer.Play (false, VideoCtrl.VideoPlayer.GetCurrentPosition());
		play.SetActive (false);
		pause.SetActive (true);
		this.transform.gameObject.SetActive (false);
	}

	public void VideoPause ()
	{
		VideoPlaybackBehaviour VideoCtrl = (VideoPlaybackBehaviour)parent.GetComponent<VideoPlaybackBehaviour> ();
		Debug.Log ("panel'name = " + parent.transform.parent.gameObject.name);
		VideoCtrl.VideoPlayer.Pause ();
		play.SetActive (true);
		pause.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0))
		{
			RaycastHit hit;
			Ray ray;
			//if(fullCamObject != null && fullCamObject.camera.enabled)
			if(Global.bVideoFull)
			{
				ray = fullCamObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			}
			else
			{
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			}
			if(Physics.Raycast(ray, out hit)){
				if(hit.transform.parent.parent.name != this.transform.parent.name)
					return;
				if(hit.transform.gameObject.name == full.name)
				{
					Debug.Log("Click btnFull, panel's name = " + hit.transform.parent.parent.parent.name);
					Global.bJyroSenser = false;
					VideoFull(false);
				}
				if(hit.transform.gameObject.name == play.name)
				{
					Debug.Log("Click btnPlay, panel's name = " + hit.transform.parent.parent.parent.name);
					VideoPlay();
				}
				if(hit.transform.gameObject.name == pause.name)
				{
					Debug.Log("Click btnPause, panel's name = " + hit.transform.parent.parent.parent.name);
					VideoPause();
				}
				if(hit.transform.gameObject.name == restore.name)
				{
					Debug.Log("Click btnRestore, panel's name = " + hit.transform.parent.parent.parent.name);
					Global.bJyroSenser = true;
					VideoRestore();
				}
			}
		}
		VideoPlaybackBehaviour VideoCtrl = (VideoPlaybackBehaviour)parent.GetComponent<VideoPlaybackBehaviour> ();
		if (VideoCtrl.mCurrentState == VideoPlayerHelper.MediaState.REACHED_END)
		{
			play.SetActive (true);
			pause.SetActive (false);
		}
	}
    */
    public GameObject play;
    public GameObject pause;
    public GameObject full;
    public GameObject restore;
    public GameObject parent;		//부모object VideoManager 

    public bool bARCamera = false;
    public bool bPortraitMode = true;

    private Vector3 defaultPos;
    private Vector3 defaultScale;
    private Quaternion defaultRotation;
    private GameObject fullCamObject;

    // Use this for initialization
    void Awake()
    {
        fullCamObject = GameObject.Find("FullCamera");
        if (fullCamObject == null)
            Debug.LogError("Full Camera is Null!");
        fullCamObject.GetComponent<Camera>().enabled = false;
    }
    void Start()
    {
        if (this.parent.GetComponent<MediaPlayerCtrl>().m_bAutoPlay)
        {
            play.SetActive(false);
        }
        else
            pause.SetActive(false);
    }

    public void VideoFull(bool bLeft)
    {
        Debug.Log("VideoFull");
        if (Global.bVideoFull == true)
            return;

        fullCamObject.GetComponent<Camera>().enabled = true;

        if (parent.transform.parent.transform.GetComponent<LoadContent>() != null)
            parent.transform.parent.transform.GetComponent<LoadContent>().HideAllObject(this.transform.parent.name);

        defaultPos = parent.transform.localPosition;
        defaultScale = parent.transform.localScale;
        defaultRotation = parent.transform.localRotation;

        bARCamera = false;
        Camera cam = fullCamObject.GetComponent<Camera>();
        float distance = (cam.nearClipPlane + 50f);

        parent.transform.position = cam.transform.position + cam.transform.forward * distance;
        parent.transform.LookAt(cam.transform.parent); //비디오 프리팹을 Full카메라가 비치는 정면으로 이동 카메라로부터 distance만한 거리에로 이동

        float camera_projection_width = (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * distance * 2.0f); //Full Camera의 distance만한 거리만큼 떨어진곳의 투영면의 너비
        Vector3 xyz = parent.gameObject.GetComponent<MeshRenderer>().bounds.size; //Full Camera로부터 distance만한 거리에 놓여있는 video 프리팹의 크기
        float size = Mathf.Max(xyz.x, xyz.y, xyz.z);

        float rate = camera_projection_width / size; //투영면의 너비와 비디오 프리팹의 크기에 기초하여 스케일 배수 계산
        parent.transform.localScale *= rate;

        parent.transform.rotation = Quaternion.Euler(new Vector3(90f, -90f, 0));

        //전화면 방식일때는 Video Controller를 y축 정방향으로 0.2만큼 이동
        transform.localPosition += new Vector3(0, 0.2f, 0);

        if (bLeft == false)
        {
            parent.transform.rotation = Quaternion.Euler(new Vector3(90f, 90f, 0));
        }

        bPortraitMode = false;
        full.SetActive(false);
        restore.SetActive(true);

        Global.bVideoFull = true;
    }

    public void VideoRestore()
    {
        if (Global.bVideoFull == false)
            return;
        fullCamObject.GetComponent<Camera>().enabled = false;

        if (parent.transform.parent.transform.GetComponent<LoadContent>() != null)
            parent.transform.parent.transform.GetComponent<LoadContent>().ShowAllObject();

        parent.transform.localPosition = defaultPos;
        parent.transform.localRotation = defaultRotation;
        parent.transform.localScale = defaultScale;

        bPortraitMode = true;
        full.SetActive(true);
        restore.SetActive(false);

        Global.bVideoFull = false;
        //작은화면 방식일때는 Video Controller를 y축 부방향으로 0.2만큼 이동
        transform.localPosition += new Vector3(0, -0.2f, 0);
    }


    public void VideoPlay()
    {
        MediaPlayerCtrl VideoCtrl = (MediaPlayerCtrl)parent.GetComponent<MediaPlayerCtrl>();
        Debug.Log("panel'name = " + parent.transform.parent.gameObject.name);
        if (VideoCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END ||
            VideoCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY)
            VideoCtrl.Play();
        else if (VideoCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED ||
                VideoCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
            VideoCtrl.Play();
        play.SetActive(false);
        pause.SetActive(true);
        //this.transform.gameObject.SetActive (false);
        StartCoroutine(HideControlBar());
    }

    IEnumerator HideControlBar()
    {
        yield return new WaitForSeconds(3f);
        this.transform.gameObject.SetActive(false);
    }


    public void VideoPause()
    {
        MediaPlayerCtrl VideoCtrl = parent.GetComponent<MediaPlayerCtrl>();
        Debug.Log("panel'name = " + parent.transform.parent.gameObject.name);
        VideoCtrl.Pause();
        play.SetActive(true);
        pause.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray;
            //if(fullCamObject != null && fullCamObject.camera.enabled)
            if (Global.bVideoFull)
            {
                ray = fullCamObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.parent.parent.name != this.transform.parent.name)
                    return;

                if (hit.transform.gameObject.name == full.name)
                {
                    Debug.Log("Click btnFull, panel's name = " + hit.transform.parent.parent.parent.name);
                    Global.bJyroSenser = false;

                    //VideoFull(false);
                    VideoFull(true);

                }
                if (hit.transform.gameObject.name == play.name)
                {

                    Debug.Log("Click btnPlay, panel's name = " + hit.transform.parent.parent.parent.name);
                    VideoPlay();
                }
                if (hit.transform.gameObject.name == pause.name)
                {
                    Debug.Log("Click btnPause, panel's name = " + hit.transform.parent.parent.parent.name);
                    VideoPause();
                }
                if (hit.transform.gameObject.name == restore.name)
                {
                    Debug.Log("Click btnRestore, panel's name = " + hit.transform.parent.parent.parent.name);
                    Debug.Log("VideoControl Behaviour, JyroSensor = true");
                    Global.bJyroSenser = true;
                    VideoRestore();
                }
            }
        }
        MediaPlayerCtrl VideoCtrl = parent.GetComponent<MediaPlayerCtrl>();
        if (VideoCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
        {
            play.SetActive(true);
            pause.SetActive(false);
        }
    }
}
