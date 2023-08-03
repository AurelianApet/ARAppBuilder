using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridLikeList : MonoBehaviour {
	
	// `Item` 프리팹이 연결될 객체
	public GameObject item;
	public GameObject NoMarkerMsg;

	public List<string> markerNameArray = new List<string>();
	private List<GameObject> listLike = new List<GameObject>();
	/** 처음 객체가 로딩될 때, 초기화 함수 호출 */
	void Start () {
		InitLikeList ();
		LikeItemClick.EventDelelteLikeItem += RefreshList;
	}

	void OnDestroy()
	{
		LikeItemClick.EventDelelteLikeItem -= RefreshList;
	}

	void RefreshList(int nIndex)
	{
		if (Global.likeNames.Count == 0)
			NoMarkerMsg.SetActive (true);
		for(int i = 0; i<listLike.Count; i++)
		{
			if(i >= nIndex + 1)
			{
				listLike[i].transform.localPosition = new Vector3(0f,  526-((i-1)*209), 0f);
				listLike[i].transform.gameObject.GetComponent<LikeItemClick>().Index = i - 1;
			}
		}
		DestroyObject (listLike [nIndex]);
		listLike.RemoveAt (nIndex);
	}

	void InitLikeList()
	{
		if (Global.likeNames.Count == 0)
			NoMarkerMsg.SetActive (true);
		for (int i= 0; i < Global.likeNames.Count; i++) 
		{
			GameObject obj = Instantiate(item, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
			obj.transform.parent = this.transform;
			obj.transform.localPosition = new Vector3(0f,  526-(i*209), 0f);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.name = Global.likeNames[i].ToString();
			listLike.Add(obj);
			
			// Item 하위의 자식 요소들에 대한 객체를 얻어냅니다.
			UITexture texture = GetChildObj (obj, "Marker").GetComponent<UITexture>(); 
			string markerPath = string.Format("file:///{0}{1}.png"
			                                  ,Global.MarkerSavePath
			                                  ,Global.likeNames[i].ToString());
			//	Debug.Log("markerPath = " + markerPath);
			StartCoroutine(setImage(texture, markerPath));
		}
	}

	IEnumerator setImage(UITexture tex, string url)
	{
		WWW www = new WWW (url);
		yield return www;

		tex.mainTexture = www.texture;
	}

	/** 객체의 이름을 통하여 자식 요소를 찾아서 리턴하는 함수 */
	GameObject GetChildObj( GameObject source, string strName  ) { 
		Transform[] AllData = source.GetComponentsInChildren< Transform >(); 
		GameObject target = null;
		
		foreach( Transform Obj in AllData ) { 
			if( Obj.name == strName ) { 
				target = Obj.gameObject;
				break;
			} 
		}
		
		return target;
	}

	void Update()
	{


	}
}
