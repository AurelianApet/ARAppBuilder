using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextBehavior : MonoBehaviour {

	public enum TYPE
	{
		COMMON	= 0,
		OBJECT 	= 1
	}
	public enum SHOWTYPE
	{
		NONE	= 0,
		FADEIN 	= 1
	}
	public GameObject panel;
	public GameObject text;
	public string 	showText;
	public TYPE		m_type;
	public SHOWTYPE		m_ShowType; 
	private TextMesh textMesh;
	private Dictionary<char, float> dict;
	private bool bFadeAni;
	private float fadeTime = 2.0f;
	private float delayAlpha = 0.0f;

	// Use this for initialization
	void Start () 
	{
		dict = new Dictionary<char, float> ();
		textMesh = text.GetComponent<TextMesh> ();
		FitToWidth (panel.GetComponent<Renderer>().bounds.size.x - 0.6f);
		if (m_type == TYPE.OBJECT) 
		{
			panel.SetActive (false);
			text.SetActive (false);
		}
	}
	public void FitToWidth(float wantedWidth) {
		
		string oldText = textMesh.text;
		textMesh.text = "";
		
		string[] lines = oldText.Split('\n');
		
		foreach(string line in lines)
		{
			textMesh.text += wrapLine(line, wantedWidth);
			//textMesh.text += "\n";
		}
		if (m_ShowType == SHOWTYPE.FADEIN)
			bFadeAni = true;
	}

	private string wrapLine(string s, float w)
	{
		// need to check if smaller than maximum character length, really...
		if(w == 0 || s.Length <= 0) return s;
		char c;
		char[] charList = s.ToCharArray();
		
		float charWidth = 0;
		float charHeight = 0;
		float wordWidth = 0;

		string word = "";
		string newText = "";
		string oldText = textMesh.text;
		
		for (int i=0; i<charList.Length; i++){
			c = charList[i];
			
			if (dict.ContainsKey(c))
			{
				charWidth = (float)dict[c];
			}
			else 
			{
				textMesh.text = ""+c;
				charWidth = textMesh.GetComponent<Renderer>().bounds.size.x;
				charHeight = textMesh.GetComponent<Renderer>().bounds.size.z;
				dict.Add(c, charWidth);
				//here check if max char length
			}
			
			word += c.ToString();
			wordWidth += charWidth;
			if((wordWidth + charWidth >= w - charWidth) || i == charList.Length - 1)
			{
				//currentWidth = wordWidth;
				newText += word;
				newText += "\n";
				word = "";
				wordWidth = 0;
			}
			textMesh.text = oldText + newText;
			if(textMesh.GetComponent<Renderer>().bounds.size.z >= panel.GetComponent<Renderer>().bounds.size.z)
				break;
		}
		textMesh.text = oldText;
		return newText;
	}

	// Update is called once per frame
	void Update () {
		if (m_type == TYPE.OBJECT) {
			if (this.gameObject.GetComponent<Animation>().isPlaying) 
			{
				panel.SetActive (false);
				text.SetActive (false);
			}
			else 
			{
				panel.SetActive(true);
				text.SetActive(true);
			}
		}
		if(bFadeAni)
		{
			delayAlpha += Time.deltaTime/fadeTime;
			if(delayAlpha <= 1.0f)
			{
				Color textcolor = text.GetComponent<Renderer>().material.color;
				text.GetComponent<Renderer>().material.color = new Color(textcolor.r, textcolor.g, textcolor.b, delayAlpha);
			}
			else
			{
				bFadeAni = false;
				Color textcolor = text.GetComponent<Renderer>().material.color;
				text.GetComponent<Renderer>().material.color = new Color(textcolor.r, textcolor.g, textcolor.b, 1.0f);
			}
		}
	}
}
