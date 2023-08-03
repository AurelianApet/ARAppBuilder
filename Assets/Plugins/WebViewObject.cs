/*
 * Copyright (C) 2011 Keijiro Takahashi
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Callback = System.Action<string>;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
public class UnitySendMessageDispatcher
{
	public static void Dispatch(string name, string method, string message)
	{
		GameObject obj = GameObject.Find(name);
		if (obj != null)
			obj.SendMessage(method, message);
	}
}
#endif

public class WebViewObject : MonoBehaviour
{
	Callback callback;
	private Texture texClose;
	private Texture texBack;
	private Texture texFore;
	private Texture texBar;
	private GUIStyle testStyle = new GUIStyle();
	public delegate void WebViewClose();
	public static event WebViewClose EventWebViewClose;
	public string urlToEdit = "Hello World";
	private string defaultUrl = "Input Url";
	private int barH;
	private GUISkin mUISkin;
	private bool bDefaultWeb = false;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	IntPtr webView;
	bool visibility;
	Rect rect;
	Texture2D texture;
	string inputString;
#elif UNITY_IPHONE
	IntPtr webView;
#elif UNITY_ANDROID
	AndroidJavaObject webView;
	Vector2 offset;
#elif UNITY_WEBPLAYER
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	[DllImport("WebView")]
	private static extern IntPtr _WebViewPlugin_Init(
		string gameObject, int width, int height, bool ineditor);
	[DllImport("WebView")]
	private static extern int _WebViewPlugin_Destroy(IntPtr instance);
	[DllImport("WebView")]
	private static extern void _WebViewPlugin_SetRect(
		IntPtr instance, int width, int height);
	[DllImport("WebView")]
	private static extern void _WebViewPlugin_SetVisibility(
		IntPtr instance, bool visibility);
	[DllImport("WebView")]
	private static extern void _WebViewPlugin_LoadURL(
		IntPtr instance, string url);
	[DllImport("WebView")]
	private static extern void _WebViewPlugin_StopLoading(
		IntPtr instance);
	[DllImport("WebView")]
	private static extern void _WebViewPlugin_EvaluateJS(
		IntPtr instance, string url);
	[DllImport("WebView")]
	private static extern bool _WebViewPlugin_CanGoBack(
		IntPtr instance);
	[DllImport("WebView")]
	private static extern bool _WebViewPlugin_CanGoForward(
		IntPtr instance);
	[DllImport("WebView")]
	private static extern bool _WebViewPlugin_IsLoading(
		IntPtr instance);
	[DllImport("WebView")]
	private static extern void _WebViewPlugin_ShowStatusBar(
		IntPtr instance, bool bShow);
	[DllImport("WebView")]
	private static extern float _WebViewPlugin_GetStatusBarHPixel(
		IntPtr instance);
	[DllImport("WebView")]
	private static extern void _WebViewPlugin_Update(IntPtr instance,
		int x, int y, float deltaY, bool down, bool press, bool release,
		bool keyPress, short keyCode, string keyChars, int textureId);
#elif UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern IntPtr _WebViewPlugin_Init(string gameObject);
	[DllImport("__Internal")]
	private static extern int _WebViewPlugin_Destroy(IntPtr instance);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_SetMargins(
		IntPtr instance, int left, int top, int right, int bottom);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_SetVisibility(
		IntPtr instance, bool visibility);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_LoadURL(
		IntPtr instance, string url);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_StopLoading(
		IntPtr instance);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_EvaluateJS(
		IntPtr instance, string url);
	[DllImport("__Internal")]
	private static extern bool _WebViewPlugin_CanGoBack(
		IntPtr instance);
	[DllImport("__Internal")]
	private static extern bool _WebViewPlugin_CanGoForward(
		IntPtr instance);
	[DllImport("__Internal")]
	private static extern bool _WebViewPlugin_IsLoading(
		IntPtr instance);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_ShowStatusBar(
		IntPtr instance, bool bShow);
	[DllImport("__Internal")]
	private static extern float _WebViewPlugin_GetStatusBarHPixel(
		IntPtr instance);
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	private void CreateTexture(int x, int y, int width, int height)
	{
		int w = 1;
		int h = 1;
		while (w < width)
			w <<= 1;
		while (h < height)
			h <<= 1;
		rect = new Rect(x, y, width, height);
		texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
	}
#endif

	public void Init(Callback cb /*= null*/, bool defaultWeb)
	{
		callback = cb;
		bDefaultWeb = defaultWeb;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		CreateTexture(0, 0, Screen.width, Screen.height);
		webView = _WebViewPlugin_Init(name, Screen.width, Screen.height,
			Application.platform == RuntimePlatform.OSXEditor);
#elif UNITY_IPHONE
		webView = _WebViewPlugin_Init(name);
#elif UNITY_ANDROID
		offset = new Vector2(0, 0);
		webView = new AndroidJavaObject("net.gree.unitywebview.WebViewPlugin");
		webView.Call("Init", name);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("unityWebView.init", name);
#endif
		texClose = Resources.Load<Texture> ("image/close");
//		texBack = Resources.Load<Texture> ("image/left");
		texFore = Resources.Load<Texture> ("image/right");
//		texBar = Resources.Load<Texture> ("image/upbar");
		barH = Screen.height / 15;
		if(bDefaultWeb)
			SetMargins (0, barH, 0, 0);
		else
		{
#if UNITY_ANDROID
			SetMargins (0, 0, 0, Screen.height*90/1280);
#elif UNITY_IPHONE
			barH = (int)_WebViewPlugin_GetStatusBarHPixel(webView);
			if((UnityEngine.iOS.Device.generation.ToString()).IndexOf("iPad") > -1)
				SetMargins (0, 20, 0, Screen.height*90/1280);
			else
				SetMargins (0, 40, 0, Screen.height*90/1280);
#endif
		}
		mUISkin = Resources.Load("UserInterface/TextSkin") as GUISkin;
		mUISkin.textField.fontSize = 80 * barH/170;
		urlToEdit = defaultUrl;
	}

	public void HideStatusBar(bool bShow)
	{
#if UNITY_EDITOR || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		Debug.Log("Called ShowStatusBar of Unity!");
		_WebViewPlugin_ShowStatusBar(webView, bShow);
#endif
	}

	public void goBack(){
		//	EvaluateJS("history.back();");
#if UNITY_EDITOR || UNITY_IPHONE
		EvaluateJS("history.back();");
		return;
#elif UNITY_ANDROID
		if (webView == null)
			return;
		webView.Call("GoBack");
#endif
	}

	public void goForword(){
	//	EvaluateJS("history.forward();");
#if UNITY_EDITOR || UNITY_IPHONE
		EvaluateJS("history.forward();");
		return;
#elif UNITY_ANDROID
		if (webView == null)
			return;
		webView.Call("GoForward");
#endif
	}

	public void stopLoading(){
		#if UNITY_EDITOR || UNITY_IPHONE || UNITY_STANDALONE_OSX
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_StopLoading(webView);
		#elif UNITY_ANDROID
		if (webView == null)
			return;
		webView.Call("StopLoading");
		#endif
	}

	public void onReload(){
		EvaluateJS("location.reload()");
	}

	void OnDestroy()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_Destroy(webView);
#elif UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_Destroy(webView);
#elif UNITY_ANDROID
		if (webView == null)
			return;
		webView.Call("Destroy");
#elif UNITY_WEBPLAYER
		Application.ExternalCall("unityWebView.destroy", name);
#endif
	}

	public void SetMargins(int left, int top, int right, int bottom)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (webView == IntPtr.Zero)
			return;
		int width = Screen.width - (left + right);
		int height = Screen.height - (bottom + top);
		CreateTexture(left, bottom, width, height);
		_WebViewPlugin_SetRect(webView, width, height);
#elif UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_SetMargins(webView, left, top, right, bottom);
#elif UNITY_ANDROID
		if (webView == null)
			return;
		offset = new Vector2(left, top);
		webView.Call("SetMargins", left, top, right, bottom);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("unityWebView.setMargins", name, left, top, right, bottom);
#endif
	}

	public void SetVisibility(bool v)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (webView == IntPtr.Zero)
			return;
		visibility = v;
		_WebViewPlugin_SetVisibility(webView, v);
#elif UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_SetVisibility(webView, v);
#elif UNITY_ANDROID
		if (webView == null)
			return;
		webView.Call("SetVisibility", v);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("unityWebView.setVisibility", name, v);
#endif
	}

	public void LoadURL(string url)
	{
		urlToEdit = url;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_LoadURL(webView, url);
#elif UNITY_ANDROID
		if (webView == null)
			return;
		webView.Call("LoadURL", url);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("unityWebView.loadURL", name, url);
#endif
	}

	public void EvaluateJS(string js)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_EvaluateJS(webView, js);
#elif UNITY_ANDROID
		if (webView == null)
			return;
		webView.Call("LoadURL", "javascript:" + js);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("unityWebView.evaluateJS", name, js);
#endif
	}

	public string GetUrl()
	{
		string url = "";
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		return url;
#elif UNITY_ANDROID
		if (webView == null)
			return null;
		url = webView.Call<string>("GetUrlJS");
		return url;
#endif
	}

	public bool IsLoading()
	{
		bool bloading = false;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return false;
		bloading = _WebViewPlugin_IsLoading(webView);
		return bloading;
#elif UNITY_ANDROID
		if (webView == null)
			return false;
		return webView.Call<bool>("IsLoading");
#endif
	}

	public bool canGoBack()
	{
		bool cangoback = false;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return false;
		cangoback = _WebViewPlugin_CanGoBack(webView);
		return cangoback;
		#elif UNITY_ANDROID
		if (webView == null)
			return false;
		cangoback = webView.Call<bool>("CanGoBack");
		return cangoback;
		#endif
	}

	public bool canGoForward()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return false;
		return _WebViewPlugin_CanGoForward(webView);
		#elif UNITY_ANDROID
		if (webView == null)
			return false;
		return webView.Call<bool>("CanGoForward");
		#endif
	}

	public void CallFromJS(string message)
	{
		if (callback != null)
			callback(message);
	}

//#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	void Update()
	{
		//inputString += Input.inputString;
	}

	void OnGUI()
	{
		if(bDefaultWeb)
		{
			GUI.skin = mUISkin;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, barH), texBar, ScaleMode.StretchToFill, true);

		//	urlToEdit = GUI.TextField(new Rect(barH*16/10, barH/10, Screen.width - barH*16/10 - barH*256/205 - barH*9/10, barH*8/10), urlToEdit, 300);
			if (GUI.Button (new Rect (0, barH/10, barH*4/5, barH*4/5), texBack, testStyle)) 
			{
				goBack();
			}
			if (GUI.Button (new Rect (barH*2, barH/10, barH*4/5, barH*4/5), texFore, testStyle)) 
			{
				goForword();
				string url = GetUrl();
				Debug.Log("url = " + url);
			}

			if (GUI.Button (new Rect (Screen.width - barH*9/10, barH/10, barH*4/5, barH*4/5), texClose, testStyle)) 
			{
				Debug.Log(this.gameObject.name);
				EventWebViewClose();
			}
		}
	}
//#endif
}
