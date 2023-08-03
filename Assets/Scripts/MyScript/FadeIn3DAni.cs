using UnityEngine;
using System.Collections;

public class FadeIn3DAni : MonoBehaviour {
	public delegate void CompleteFadeAction(bool found);
	public static event CompleteFadeAction EventCompleteFadeAction;

	ArrayList ShaderNames = new ArrayList();
	private float fadeTime = 0.5f;
	private float delayAlpha = 0.0f;
	private bool bFadeAni = false;

	// Use this for initialization
	void Start () {
		delayAlpha = 0;
		if(this.name.Contains("textBoard") == false)
		{
			ChangeMaterials(this.gameObject);
			bFadeAni = true;
		}
		else{
			StartCoroutine (wait());
		}
	}
	private IEnumerator wait()
	{
		yield return new WaitForSeconds(0.05f);
		ChangeMaterials(this.gameObject);
		bFadeAni = true;
	}
	// Update is called once per frame
	void Update () {
		if(bFadeAni)
		{
			delayAlpha += fadeTime* Time.deltaTime;
			if(delayAlpha <= 1.0f)
			{
				SetAlphaColor(this.gameObject, delayAlpha);
			}
			else
			{
				bFadeAni = false;
				ReturnMaterials(this.gameObject);
				if(Global.bIsLikeView)
					EventCompleteFadeAction(false);
			}
		}
	}
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
	
	void ChangeMaterials(GameObject ThreeDObject)
	{
		Renderer[] allRenderer = ThreeDObject.transform.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allRenderer) 
		{
			ShaderNames.Add(child.material.shader.name);
			child.material.shader = Shader.Find("Transparent/Specular");
			Color childcolor = child.material.color;
			child.material.color = new Color(childcolor.r, childcolor.g, childcolor.b, 0.0f);
		}
		Debug.Log("ChangeMaterials ShanderName.count = " + ShaderNames.Count);
	}
	
	private void SetAlphaColor(GameObject ThreeDObject, float alpha)
	{
		Renderer[] allRenderer = ThreeDObject.transform.GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allRenderer) 
		{
			child.material.shader = Shader.Find("Transparent/Specular");
			Color childcolor = child.material.color;
			child.material.color = new Color(childcolor.r, childcolor.g, childcolor.b, alpha);
		}
	}
}
