using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoutubeDemoUsage : MonoBehaviour {
	public void DemoPlayback()
    {
        Handheld.PlayFullScreenMovie(YoutubeVideo.Instance.RequestVideo("bc0sJvtKrRM", 720)); //the bc0sJvtKrRM string is the video id you can use the video id or the full url.
        Debug.Log("The video only plays on mobile device, if you receive one big url on console all it's ok, will play in mobile device");
    }

    //public UnityEngine.UI.Text videoUrlInput;
	public string videoUrl;

    public void PlayFromInput()
    {
		Handheld.PlayFullScreenMovie(YoutubeVideo.Instance.RequestVideo(videoUrl, 720)); //play the url that are in the input.
        Debug.Log("The video only plays on mobile device, if you receive one big url on console all it's ok, will play in mobile device");
    }
}
