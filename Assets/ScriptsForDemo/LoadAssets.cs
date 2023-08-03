using UnityEngine;
using System.Collections;

public class LoadAssets : BaseLoader {

	public string assetBundleName = "cube1.unity3d";
	public string assetName = "cube";

	void Start()
	{
	}
	// Use this for initialization
	/*IEnumerator Start () {

		yield return StartCoroutine(Initialize() );

		// Load asset.
	//	yield return StartCoroutine(Load (assetBundleName, assetName) );
	//	LoadPrefab ();
		// Unload assetBundles.
		AssetBundleManager.UnloadAssetBundle(assetBundleName);
	}*/

	IEnumerator LoadPrefab(){
		Debug.Log("start loading prefab-----------------");
		yield return StartCoroutine(Initialize() );

		Debug.Log ("start loading asset-------------");
		yield return StartCoroutine(LoadAsset (assetBundleName, assetName) );

//		Debug.Log ("start loadign myasset---------------");
	//	MyLoadAsset (assetBundleName, assetName);
	//	AssetBundleManager.UnloadAssetBundle(assetBundleName);
		UnloadAssetBundle ();
	}

	public void UnloadAssetBundle(){
		Debug.Log ("UnloadAssetBundle........");
		AssetBundleManager.UnloadAssetBundle(assetBundleName);
	}

	public void SetVariable(string as_bun, string as_name)
	{
		assetBundleName = as_bun;
		assetName = as_name;
//		assetName = "table";
	}
}
