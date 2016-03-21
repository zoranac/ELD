using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayModeButton : MonoBehaviour {
	GameObject control;
    public Sprite Play;
    public Sprite Return;
	// Use this for initialization
	void Start () {
		control = GameObject.Find("Control");
	}
	
	// Update is called once per frame
	void Update () {
        if (ControlScript.CurrentMode == ControlScript.Mode.Play)
        {
            GetComponent<Image>().sprite = Return;
		}else{
            GetComponent<Image>().sprite = Play;
		}
	}
}
