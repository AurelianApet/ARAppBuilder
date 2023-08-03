using UnityEngine;
using System.Collections;

public class ImageSlideBehavior : MonoBehaviour {

	public int 	nTotalImgCount;
	private int	currentIndex = 0;
	public int type;	//1:일반형, 	2:3D 액자형
	public int ActionType;		//	0:3초간 자동전환 1:터치시 전환
	public int TransType;		//	0:없음	1:fade	2:슬라이드
	private bool bFadeAni;		//fade효과 전환시 true; 
	private float fadeTime = 1.0f;
	private float delayAlpha = 0.0f;

	private bool bSlideAni;		//slide효과 전환시 true;
	private float slideTime = 0.5f;
	private float sumTime = 0.0f;
	private Vector3 	leftPos, rightPos, midPos;
	private GameObject[] imgObj;


	// Use this for initialization
	void Start () {
		delayAlpha = 0;
		sumTime = 0.0f;

	}

	public void Init()
	{
		if(ActionType == 0)
			StartCoroutine (CountTime ());
		if (TransType == 2) {
			if (type == 1) {
				leftPos = new Vector3 (-0.3f, 0.0f, 0.0f);
				rightPos = new Vector3 (0.3f, 0.0f, 0.0f);
			} else {
				leftPos = new Vector3 (-12.0f, 0.0f, 0.0f);
				rightPos = new Vector3 (12.0f, 0.0f, 0.0f);
			}
			midPos = Vector3.zero;
			imgObj = new GameObject[nTotalImgCount];
			for(int j =0; j<nTotalImgCount; j++)
			{
				imgObj[j] = this.transform.Find (string.Format ("slideimage_{0}", j)).gameObject;
				if(j == 0)
					imgObj[j].transform.localPosition = midPos;
				else if(j == nTotalImgCount -1)
					imgObj[j].transform.localPosition = leftPos;
				else
					imgObj[j].transform.localPosition = rightPos;
			}
		}
	}

	private void OnEnable(){
		if (type == 2)
			Init ();
	}

	IEnumerator CountTime()
	{
		float delayTime = 3.0f;
		if (TransType == 1)
			delayTime += fadeTime;
		else if (TransType == 2)
			delayTime += slideTime;

		yield return new WaitForSeconds (delayTime);
		Next ();
		StartCoroutine (CountTime ());
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0) && ActionType == 1)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit)){
			//	Debug.Log(hit.transform.gameObject.name + "!!!!!!!!!!!!!!!!!!!");
			//	Debug.Log(hit.transform.parent.gameObject.name);
				switch(type)
				{
				case 1:
					if(hit.transform.gameObject.name == string.Format("slideimage_{0}", currentIndex) &&
					   hit.transform.parent.gameObject.name == this.name)
						Next();
					break;
				case 2:
					if(hit.transform.gameObject.name == string.Format("slideimage_{0}", currentIndex) &&
					   hit.transform.parent.parent.gameObject.name == this.transform.parent.name)
						Next();
					break;
				}
			}
		}
		if(bFadeAni)
		{
			delayAlpha += Time.deltaTime/fadeTime;
			if(delayAlpha <= 1.0f)
			{
				GameObject cur = this.transform.Find (string.Format ("slideimage_{0}", currentIndex)).gameObject;
				GameObject next = this.transform.Find (string.Format ("slideimage_{0}", (currentIndex + 1) % nTotalImgCount)).gameObject;
				SetAlphaColor(cur, 1 - delayAlpha);
				SetAlphaColor(next, delayAlpha);
			}
			else
			{
				this.transform.Find (string.Format ("slideimage_{0}", currentIndex)).gameObject.SetActive (false);
				currentIndex = (currentIndex + 1) % nTotalImgCount;
				delayAlpha = 0;
				bFadeAni = false;
			}
		}
		if (bSlideAni) {
			int nextIndex = (currentIndex + 1)%nTotalImgCount;
			delayAlpha += Time.deltaTime/slideTime;
			if(delayAlpha <= 1.0f)
			{
				SetAlphaColor(imgObj[currentIndex], 1 - delayAlpha);
				SetAlphaColor(imgObj[nextIndex], delayAlpha);
			}

			sumTime += Time.deltaTime;
			float fracJourney = sumTime / slideTime;
			imgObj[currentIndex].transform.localPosition = Vector3.Lerp(midPos, leftPos, fracJourney);
			imgObj[nextIndex].transform.localPosition = Vector3.Lerp(rightPos, midPos, fracJourney);
			if(fracJourney >= 1.0f)
			{
				imgObj[currentIndex].SetActive(false);
				imgObj[(currentIndex + nTotalImgCount-1)%nTotalImgCount].transform.localPosition = rightPos;
				currentIndex = (currentIndex + 1)%nTotalImgCount;
				sumTime = 0.0f;
				delayAlpha = 0;
				bSlideAni = false;
			}

		}
	}

	private void Next()
	{
		int nextIndex = (currentIndex + 1) % nTotalImgCount;
		switch (TransType) {
		case 0:
			this.transform.Find (string.Format ("slideimage_{0}", currentIndex)).gameObject.SetActive (false);
			currentIndex = nextIndex;
			this.transform.Find (string.Format ("slideimage_{0}", currentIndex)).gameObject.SetActive (true);
			break;
		case 1:
			this.transform.Find (string.Format ("slideimage_{0}", nextIndex)).gameObject.SetActive (true);
			bFadeAni = true;
			break;
		case 2:
			imgObj[nextIndex].SetActive(true);
			bSlideAni = true;
			break;
		}
	}

	private void SetAlphaColor(GameObject ThreeDObject, float alpha)
	{
		Renderer[] allRenderer = ThreeDObject.transform.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allRenderer) 
		{
			child.material.shader = Shader.Find("Transparent/Diffuse");
			Color childcolor = child.material.color;
			child.material.color = new Color(childcolor.r, childcolor.g, childcolor.b, alpha);
		}
	}

}
