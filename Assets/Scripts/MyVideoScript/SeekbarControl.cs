using UnityEngine;
using System.Collections;

public class SeekbarControl : MonoBehaviour {
    /*public GameObject bar;
	public GameObject thumb;
	public GameObject parentVideo;
	public GameObject endTime;
	public GameObject foreGround;
	private VideoPlaybackBehaviour VideoCtrl;

	private float MinPos = -5.0f;
	private float MaxPos = 5.0f;
	private float MaxForeGroundSize = 0.985f;
	private float MinForeGroundPos = -4.9f;

	private bool _mouseState = false;
	private Vector3 offset;
	private Vector3 screenSpace;
//	private int totalsecond = 0;

	private Camera controlCam;
	// Use this for initialization
	void Start () {

	}

	private void GetTotalSecond(int nparam)
	{
		int minute = nparam /60;
		int second = nparam - minute * 60;
		string strMinute ="99";
		string strSecond ="99";
		
		if(minute >= 0 && minute < 10)
			strMinute = string.Format("0{0}", minute);
		else
			strMinute = string.Format("{0}", minute);
		if(second >= 0 && second < 10)
			strSecond = string.Format("0{0}", second);
		else
			strSecond = string.Format("{0}", second);
		
		string strEndTime = string.Format("{0}:{1}", strMinute, strSecond) ;
		TextMesh endTimeMesh = (TextMesh)endTime.GetComponent<TextMesh>();
		endTimeMesh.text = strEndTime;
	}
	
	private bool GetClickedthumb (out RaycastHit hit)
	{
		Ray ray = controlCam.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)){
		//	Debug.Log("seekbar clicked name = " + hit.collider.gameObject.name);
			if(hit.collider.gameObject == thumb)
			{
				return true;
			}
		}
		return false;
	}
	private void ProcessForeGround()
	{
		float currentPer = (thumb.transform.localPosition.x - MinPos) / (MaxPos - MinPos);
		float rePos = MinForeGroundPos + currentPer * (0 - MinForeGroundPos);
		foreGround.transform.localPosition = new Vector3 (rePos, foreGround.transform.localPosition.y, foreGround.transform.localPosition.z);
		float reSize = currentPer * MaxForeGroundSize;
		foreGround.transform.localScale = new Vector3 (reSize, foreGround.transform.localScale.y, foreGround.transform.localScale.z);
	}

	IEnumerator Wait() {

		yield return new WaitForSeconds(0.5f);
		_mouseState = false;
	}

	// Update is called once per frame
	void Update () {
		GameObject fullCamObject = GameObject.Find("FullCamera");
		//if(fullCamObject != null && fullCamObject.camera.enabled)
		if(Global.bVideoFull)
		{
			controlCam = fullCamObject.GetComponent<Camera>();
		}
		else
		{
			controlCam = Camera.main;
		}
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			if (GetClickedthumb(out hit)) {
				_mouseState = true;
				screenSpace = controlCam.WorldToScreenPoint (thumb.transform.position);
				offset = thumb.transform.position - controlCam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
				offset = new Vector3(offset.x, offset.y, offset.z);
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			if(_mouseState)
			{
				VideoPlaybackBehaviour VideoCtrl = (VideoPlaybackBehaviour)parentVideo.GetComponent<VideoPlaybackBehaviour> ();
				int duration = (int)VideoCtrl.VideoPlayer.GetLength();
				float currentPer = (thumb.transform.localPosition.x - MinPos) /(MaxPos - MinPos);
				int currentSeek = (int)(currentPer * duration);
			//	Debug.Log("currentPercent = " + currentPer + ",currentSeek = " + currentSeek + ", duration = " + duration);
				VideoCtrl.VideoPlayer.SeekTo(currentSeek);
				StartCoroutine(Wait());
			}
		}
		if (_mouseState) {
			//keep track of the mouse position
			var curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
			//convert the screen mouse position to world point and adjust with offset
			var curPosition = controlCam.ScreenToWorldPoint (curScreenSpace) + offset;
			
			VideoControlBehavior VCBehavior = (VideoControlBehavior) transform.parent.gameObject.GetComponent<VideoControlBehavior> ();
			if(VCBehavior == null)
				Debug.Log("VideoControlBehavior is null");
		//	Debug.Log("seek's parent full = " + VCBehavior.bFullMode);
			if(Global.bVideoFull)		//
				thumb.transform.position = new Vector3(thumb.transform.position.x, thumb.transform.position.y, curPosition.z);
			else
				thumb.transform.position = new Vector3(curPosition.x, thumb.transform.position.y, thumb.transform.position.z);
			if(thumb.transform.localPosition.x > MaxPos)
				thumb.transform.localPosition = new Vector3(MaxPos, thumb.transform.localPosition.y, thumb.transform.localPosition.z);
			else if(thumb.transform.localPosition.x < MinPos)
				thumb.transform.localPosition = new Vector3(MinPos, thumb.transform.localPosition.y, thumb.transform.localPosition.z);
		}
		else{
			VideoPlaybackBehaviour VideoCtrl = (VideoPlaybackBehaviour)parentVideo.GetComponent<VideoPlaybackBehaviour> ();
			int duration = (int)VideoCtrl.VideoPlayer.GetLength();
			int currentSeek = (int)VideoCtrl.VideoPlayer.GetCurrentPosition();
			GetTotalSecond(duration);
		//	Debug.Log("totalsecond:" + duration + "cursecond:" + currentSeek);
			float currentPosX = MinPos + (float)currentSeek / (float)duration * (MaxPos - MinPos);
			if(currentPosX > MaxPos)
				currentPosX = MaxPos;
			else if(currentPosX < MinPos)
				currentPosX = MinPos;
			thumb.transform.localPosition = new Vector3 (currentPosX, thumb.transform.localPosition.y, thumb.transform.localPosition.z);
		}
		ProcessForeGround ();

	}*/

    public GameObject bar;
    public GameObject thumb;
    public GameObject parentVideo;
    public GameObject endTime;
    public GameObject currentTime;
    public GameObject foreGround;
    private MediaPlayerCtrl VideoCtrl;

    private float MinPos = -5.0f;
    private float MaxPos = 5.0f;
    private float MaxForeGroundSize = 0.985f;
    private float MinForeGroundPos = -4.9f;

    private bool _mouseState = false;
    private Vector3 offset;
    private Vector3 screenSpace;
    float delta = 0;
    //	private int totalsecond = 0;

    private Camera controlCam;
    // Use this for initialization
    void Start()
    {

    }

    private void GetTotalSecond(int nparam)
    {
        int minute = nparam / 60;
        int second = nparam - minute * 60;
        string strMinute = "99";
        string strSecond = "99";

        if (minute >= 0 && minute < 10)
            strMinute = string.Format("0{0}", minute);
        else
            strMinute = string.Format("{0}", minute);
        if (second >= 0 && second < 10)
            strSecond = string.Format("0{0}", second);
        else
            strSecond = string.Format("{0}", second);

        string strEndTime = string.Format("{0}:{1}", strMinute, strSecond);
        TextMesh endTimeMesh = (TextMesh)endTime.GetComponent<TextMesh>();
        endTimeMesh.text = strEndTime;
    }

    private bool GetClickedthumb(out RaycastHit hit)
    {
        Ray ray = controlCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //	Debug.Log("seekbar clicked name = " + hit.collider.gameObject.name);
            if (hit.collider.gameObject == thumb)
            {
                return true;
            }
        }
        return false;
    }
    private void ProcessForeGround()
    {
        float currentPer = (thumb.transform.localPosition.x - MinPos) / (MaxPos - MinPos);
        float rePos = MinForeGroundPos + currentPer * (0 - MinForeGroundPos);
        foreGround.transform.localPosition = new Vector3(rePos, foreGround.transform.localPosition.y, foreGround.transform.localPosition.z);
        float reSize = currentPer * MaxForeGroundSize;
        foreGround.transform.localScale = new Vector3(reSize, foreGround.transform.localScale.y, foreGround.transform.localScale.z);
    }

    IEnumerator Wait()
    {

        yield return new WaitForSeconds(0.5f);
        _mouseState = false;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject fullCamObject = GameObject.Find("FullCamera");
        //if(fullCamObject != null && fullCamObject.camera.enabled)
        if (Global.bVideoFull)
        {
            controlCam = fullCamObject.GetComponent<Camera>();
        }
        else
        {
            controlCam = Camera.main;
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (GetClickedthumb(out hit))
            {
                _mouseState = true;
                screenSpace = controlCam.WorldToScreenPoint(thumb.transform.position);
                offset = thumb.transform.position - controlCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
                offset = new Vector3(offset.x, offset.y, offset.z);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_mouseState)
            {
                MediaPlayerCtrl VideoCtrl = parentVideo.GetComponent<MediaPlayerCtrl>();
                int duration = (int)VideoCtrl.GetDuration();
                float currentPer = (thumb.transform.localPosition.x - MinPos) / (MaxPos - MinPos);

                //	Debug.Log("currentPercent = " + currentPer + ",currentSeek = " + currentSeek + ", duration = " + duration);
                VideoCtrl.SetSeekBarValue(currentPer);

                StartCoroutine(Wait());
            }
        }
        if (_mouseState)
        {
            //keep track of the mouse position
            var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            //convert the screen mouse position to world point and adjust with offset
            var curPosition = controlCam.ScreenToWorldPoint(curScreenSpace) + offset;

            VideoControlBehavior VCBehavior = (VideoControlBehavior)transform.parent.gameObject.GetComponent<VideoControlBehavior>();
            if (VCBehavior == null)
                Debug.Log("VideoControlBehavior is null");
            //	Debug.Log("seek's parent full = " + VCBehavior.bFullMode);
            if (Global.bVideoFull)      //
                thumb.transform.position = new Vector3(thumb.transform.position.x, thumb.transform.position.y, curPosition.z);
            else
                thumb.transform.position = new Vector3(curPosition.x, thumb.transform.position.y, thumb.transform.position.z);
            if (thumb.transform.localPosition.x > MaxPos)
                thumb.transform.localPosition = new Vector3(MaxPos, thumb.transform.localPosition.y, thumb.transform.localPosition.z);
            else if (thumb.transform.localPosition.x < MinPos)
                thumb.transform.localPosition = new Vector3(MinPos, thumb.transform.localPosition.y, thumb.transform.localPosition.z);
        }
        else
        {
            MediaPlayerCtrl VideoCtrl = parentVideo.GetComponent<MediaPlayerCtrl>();
            int duration = VideoCtrl.GetDuration() / 1000;
            // Debug.Log("Total Duration: " + duration);
            float currentSeek = VideoCtrl.GetSeekBarValue();
            //Debug.Log("Current Seek: " + currentSeek);			

            float currentPosX = MinPos + currentSeek * (MaxPos - MinPos);
            if (currentPosX > MaxPos)
                currentPosX = MaxPos;
            else if (currentPosX < MinPos)
                currentPosX = MinPos;

            thumb.transform.localPosition = new Vector3(currentPosX, thumb.transform.localPosition.y, thumb.transform.localPosition.z);

            if (delta > 1f)
            {
                GetTotalSecond(duration);

                //1초에 한번씩 현재 시간 반영
                int curTime = (int)(duration * currentSeek);
                int minute = curTime / 60;
                int second = curTime - minute * 60;
                string strMinute = "99";
                string strSecond = "99";

                if (minute >= 0 && minute < 10)
                    strMinute = string.Format("0{0}", minute);
                else
                    strMinute = string.Format("{0}", minute);
                if (second >= 0 && second < 10)
                    strSecond = string.Format("0{0}", second);
                else
                    strSecond = string.Format("{0}", second);

                string strTime = string.Format("{0}:{1}", strMinute, strSecond);
                TextMesh timeMesh = (TextMesh)currentTime.GetComponent<TextMesh>();
                timeMesh.text = strTime;
                delta = 0;
            }

            delta += Time.deltaTime;
        }
        ProcessForeGround();
    }
}
