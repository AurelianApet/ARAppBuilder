using UnityEngine;
using System.Collections;

public class ThreeDControl : MonoBehaviour {

	public delegate void Complete3DAction(bool found);
	public static event Complete3DAction EventComplete3DAction;

	public enum TYPE 
	{
		COMMON	= 1,
		UPDOWN	= 2,
		FADEIN	=3
	}
	ArrayList ShaderNames = new ArrayList();
	public int 	times= 0;		//반복회수
	public TYPE type ;
	public float brightness = 100;
	private bool hasAni = false;
	private bool	bAni = false;
	private bool bUpdownAni = false;
	private bool bFadeAni = false;
	public float speed = 1.0F;
	public float gravity = 9.80665F;
	private float startTime;
	public float journeyLength;

	public Vector3 FinalPos;
	private float delayTime;
	private Vector3 startPos;
	private Vector3 porPos;
	private float fadeTime = 0.5f;
	private float delayAlpha = 0.0f;
	// Use this for initialization
	void Start () {
		Debug.Log ("Ani Start");
		Animation fbxAni = this.GetComponent <Animation>();
		if(fbxAni == null)
		{
			Debug.Log("Doesn't exist ani animation!");
		}
		else{
			Debug.Log("Found ani animation!");
			this.GetComponent<Animation>().playAutomatically = false;
			fbxAni.Stop("ani");
			hasAni = true;
		}
		SetBrightness ();
		Debug.Log ("Ani-type" + type);

			switch(type)
			{
		case TYPE.COMMON:
			Debug.Log ("hasAni" + hasAni);
				if(hasAni)
					PlayAni();
				break;
			case TYPE.UPDOWN:
				PlayUpdownAni();
				break;
			case TYPE.FADEIN:
				ChangeMaterials();
				delayAlpha = 0.0f;
				bFadeAni = true;
				break;
			}
	}

	void SetBrightness()
	{
/*		
		Renderer[] allRenderer = this.transform.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allRenderer) 
		{
			Color childcolor = child.material.color;
			child.material.color = new Color(brightness/100, brightness/100, brightness/100, childcolor.a);			
		}
*/		
	}

	// Update is called once per frame
	void Update () {
//		Debug.Log ("Update animation---------");
		if(bAni)
		{
			delayTime += Time.deltaTime;
			Debug.Log ("delaytime=" + delayTime);
			Debug.Log ("clip=" + this.GetComponent<Animation>().clip.length * times);
			if(delayTime > this.GetComponent<Animation>().clip.length * times)
			{
				Debug.Log(string.Format (" Animation Play : Times {0} delayTime:{1}", times, delayTime));
				this.GetComponent<Animation>().Stop();
				bAni = false;
			}
		}
		if(bUpdownAni)
		{
			Debug.Log ("Animation Play update-------------");
			float distCovered = (Time.time - startTime) * (Time.time - startTime) * speed * gravity / 2;
			float fracJourney = distCovered / journeyLength;
			this.transform.localPosition = Vector3.Lerp(startPos, porPos, fracJourney);
			if(Vector3.Distance(porPos, this.transform.localPosition) == 0)
			{
				bUpdownAni = false;
				if(hasAni)
					PlayAni();
			}
		}
		if(bFadeAni)
		{
			delayAlpha += fadeTime* Time.deltaTime;
			if(delayAlpha <= 1.0f)
				SetAlphaColor(delayAlpha);
			else
			{
				bFadeAni = false;
				ReturnMaterials(this.gameObject);
				if(hasAni)
					PlayAni();
			}
		}
	}


	private void ChangeMaterials ()
	{
		Renderer[] allRenderer = this.transform.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allRenderer) 
		{
			ShaderNames.Add(child.material.shader.name);
			child.material.shader = Shader.Find("Transparent/Specular");
			Color childcolor = child.material.color;
			child.material.color = new Color(childcolor.r, childcolor.g, childcolor.b, 1.0f);
		}
		Debug.Log("ChangeMaterials ShanderName.count = " + ShaderNames.Count);
	}
	/*private void ChangeBrightMaterials ()
	{
		Renderer[] allRenderer = this.transform.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allRenderer) 
		{
			child.material.shader = Shader.Find("Self-Illumin/Bumped Specular");
			Color childcolor = child.material.color;
			child.material.color = new Color(childcolor.r, childcolor.g, childcolor.b, 0.0f);
		}
	}*/
	void ReturnMaterials(GameObject ThreeDObject)
	{
		int nIndex = 0;
		if(ShaderNames.Count != 0)
		{
			Renderer[] allRenderer = ThreeDObject.transform.GetComponentsInChildren<Renderer>();
			foreach (Renderer child in allRenderer) 
			{
				if(nIndex > ShaderNames.Count - 1 || nIndex < 0)
					Debug.Log("ShaderNames index over its count! nIndex is " + nIndex + "ShanderName.count = " + ShaderNames.Count);
				child.material.shader = Shader.Find(ShaderNames[nIndex].ToString());
				nIndex++;
			}
			ShaderNames.Clear();
		}
	}

	private void SetAlphaColor(float alpha)
	{
		Renderer[] allRenderer = this.transform.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allRenderer) 
		{
			child.material.shader = Shader.Find("Transparent/Specular");
			Color childcolor = child.material.color;
//			child.material.color = new Color(childcolor.r, childcolor.g, childcolor.b, alpha);
			child.material.color = new Color(childcolor.r, childcolor.g, childcolor.b, 1.0f);
		}
	}

	private void PlayUpdownAni()
	{
		porPos = FinalPos;//this.transform.localPosition;
		startPos = new Vector3 (porPos.x, 2.0f, porPos.z);
		startTime = Time.time;
		journeyLength = Vector3.Distance(startPos, porPos);

		bUpdownAni = true;
	}

	private void PlayAni()
	{
		if(Global.bIsLikeView)
			EventComplete3DAction(false);
		bAni = true;
		delayTime = 0.0f;
		this.GetComponent<Animation>().Play("ani");
	}
}
