using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public GameObject popup;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void okExit()
    {
        Application.Quit();
    }

    public void clsExit()
    {
        popup.SetActive(true);
        Debug.Log("click exit button");
    }

    public void cancelExit()
    {
        popup.SetActive(false);
    }
}
