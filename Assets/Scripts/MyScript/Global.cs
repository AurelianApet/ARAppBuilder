using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Global {
	public readonly static string AppVer = "1.0.0";

    public static string SERVER_URL = "http://3.209.251.149:19780/Account/getXML.aspx?content_id=";
    public static string WEB_SERVER_URL = "http://3.209.251.149:19780/";

    //public static string SERVER_URL = "http://localhost:19780/Account/getXML.aspx?content_id=";
    //public static string WEB_SERVER_URL = "http://localhost:19780/";

    public static string CurMarkerName;
	public static string ThreeUnityName;
	public static string CurCustomButtonName;
	public static bool bLoadedPackage = false;
	public static GameObject goCharacter;

	/*		loading relative		*/
	public static bool bLoading;		//loading xml contents
	public static int TotalLoadingCount;
	public static int TotalLoadingAdvCount;
	public static int CurLoadingCount;
	public static int CurLoadingAdvCount;

	/*		LikeView Relative		*/
	public static bool bIsLikeView;
	public static string MarkerSavePath = Application.persistentDataPath + "/markers/";
	public static string CustomButtonPath = Application.persistentDataPath + "/uploads/custombutton/";
	public static string AdvImagePath = Application.persistentDataPath + "/uploads/advers/";
	public static string strLikeInfoPath = Application.persistentDataPath + "/like.dat";
	public static string ThreeUnitySavePath = Application.persistentDataPath + "/uploads/three/";
	public static List<string> likeNames = new List<string>();
	public static int 	nCurMarkerIndex;

	/*		Relative video 			*/
	public static bool	bVideoFull;
	public static bool 	bExistVideo;
	public static bool 	bJyroSenser;
	public static bool 	bLandscapeVideo;
	public static string videoName;
	public static Vector3		orgVideoPos;
	public static Vector3	orgVideoQuat;
	public static Vector3		orgVideoScale;

	/*		Relative Webview 		*/
	public static bool ShowingCustomWebview;

	/*		Relative RotateCamera	*/
	public static Vector3	ARCampos;

	/*		Relative Rotate3DObject		*/
//	public static bool 	bEnableRotate3D;
	public static bool bFirstPlay = true;
}
