using UnityEngine;
using System.Collections;

public class LoadScenes : BaseLoader {

	public string sceneAssetBundle = "scene.unity3d";
	public string sceneName = "testScene";

	public bool loadLevelAdditive = false;


	// Use this for initialization
	public IEnumerator StartLoadScene () {
		
		yield return StartCoroutine(Initialize() );

		// Load level.
		yield return StartCoroutine(LoadLevel (sceneAssetBundle, sceneName, loadLevelAdditive) );

		// Unload assetBundles.
		AssetBundleManager.UnloadAssetBundle(sceneAssetBundle);
	}
}
