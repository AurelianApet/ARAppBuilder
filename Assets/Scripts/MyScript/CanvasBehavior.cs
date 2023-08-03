using UnityEngine;
using System.Collections;

public class CanvasBehavior : MonoBehaviour {
	public delegate void CompleteCanvasAction(bool found);
	public static event CompleteCanvasAction EventCompleteCanvasAction;
	public bool bShowSlide= false;
	public GameObject m_imageSlide;
	// Use this for initialization
	void Start () {
		m_imageSlide.SetActive (bShowSlide);

	}
	
	// Update is called once per frame
	void Update () {
		if(!bShowSlide)
		{
			if(this.GetComponent<Animation>().IsPlaying("Take 001") == false)
			{
				bShowSlide = true;
				m_imageSlide.SetActive(bShowSlide);
			//	StartCoroutine(waitInit());
				if(Global.bIsLikeView)
					EventCompleteCanvasAction(false);
			}
		}
	}

	IEnumerator waitInit()
	{
		yield return new WaitForSeconds(3.0f);
		ImageSlideBehavior sldBehavior = (ImageSlideBehavior)m_imageSlide.GetComponent("ImageSlideBehavior");
	//	ImageSlideBehavior sldBehavior = (ImageSlideBehavior)slide.GetComponent ("ImageSlideBehavior");
		sldBehavior.Init();
	}
}
