using UnityEngine;
using System.Collections;

public class EditorGUI : MonoBehaviour {
    ControlScript control;
    public GameObject EditModeView;
	// Use this for initialization
	void Start () {
        control = gameObject.GetComponent<ControlScript>();
	}
	
	// Update is called once per frame
	void Update () {

        if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
        {
            EditModeView.SetActive(true);
        }
        else
        {
            EditModeView.SetActive(false);
        }
	}

}
