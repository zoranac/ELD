using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class EditModeScript : MonoBehaviour {
    public Sprite Build;
    public Sprite Connect;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (ControlScript.CurrentMode == ControlScript.Mode.Build)
        {
            GetComponent<Image>().sprite = Build;
        }
        else if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
        {
            GetComponent<Image>().sprite = Connect;
        }
	}
}
