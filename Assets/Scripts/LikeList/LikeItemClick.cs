using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LikeItemClick : MonoBehaviour {

	public delegate void DeleteLikeItem(int nIndex);
	public static event DeleteLikeItem EventDelelteLikeItem;

	private Texture	texTitleBack;
	private GUIStyle	blankStyle = new GUIStyle();
	private bool		bDelClicked = false;

	private int index;
	public int Index
	{
		get {	return index;}
		set {	index = value;}
	}
	// Use this for initialization
	void Start () {
		index = 0;
		
		// Item 객체의 부모(=Grid) 하위의 모든 자식요소(=Item들)을 스캔한다.
		foreach (Transform child in this.transform.parent) {
			// 특정 자식 요소와 나 자신이 동일하다면 반복을 멈춘다.
			if (child == transform) {
				// 여기서 멈추게 되면, 현재의 index값이 나 스스로의 번호가 된다.
				break;
			}
			// 인덱스 값을 1씩 증가한다.
			index++;
		}

		texTitleBack = Resources.Load ("like/DelPopBack") as Texture2D;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnClickItem() {
		Debug.Log (index + "th Item clicked." + Global.likeNames [index].ToString ());
	/*	GlobalValue.bGoToSlide = true;
		GlobalValue.current = 1;
		GlobalValue.videoPos = 0.0f;*/
		Global.nCurMarkerIndex = index;
	
		Application.LoadLevel ("LikeView");
	}
	public void OnClickBtnDelete()
	{
		bDelClicked = true;
		return;
	}

	void RemoveItem()
	{
		Global.likeNames.RemoveAt (index);
		BinaryFormatter bf = new BinaryFormatter ();

		if (File.Exists (Global.strLikeInfoPath)) {
			File.Delete (Global.strLikeInfoPath);
		}
		FileStream fdata = File.Create (Global.strLikeInfoPath);
		try {
			bf.Serialize (fdata, Global.likeNames);
		} catch (SerializationException e) {
				Debug.LogError (e.Message);
		} finally {
				fdata.Close ();
		}

		EventDelelteLikeItem (index);
	}

	void OnGUI()
	{
		if (bDelClicked == true) 
		{
			int nX,nY;

			nX = (Screen.width - Screen.width*548/1080) / 2;
			nY = (Screen.height - Screen.height*190/1920) / 2;

			GUI.DrawTexture (new Rect (nX, nY, Screen.width*548/1080, Screen.height*190/1920), texTitleBack, ScaleMode.StretchToFill);

			if(GUI.Button(new Rect(nX + Screen.width*88/1080, nY + Screen.height*108/1920, Screen.width*160/1080, Screen.height*50/1920), (Texture)null, blankStyle))
			{//취소버튼
				bDelClicked = false;
			}
			else if(GUI.Button(new Rect(nX + Screen.width*308/1080, nY + Screen.height*108/1920, Screen.width*160/1080, Screen.height*50/1920), (Texture)null, blankStyle))
			{//확인버튼
				RemoveItem();
				bDelClicked = false;
			}
		}

		return;
	}
}
