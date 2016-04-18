using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ObjectPlacementButtonScript : MonoBehaviour {
    public GameObject PlaceObject;
	public ControlScript Control;
	public GameObject MyCamera;
    public ControlScript.Tool setTool = ControlScript.Tool.None;
	// Use this for initialization
	void Start () {
		Control = GameObject.Find("Control").GetComponent<ControlScript>();
		MyCamera = GameObject.Find("Camera");
       
        if (PlaceObject != null)
            GetComponentInChildren<Text>().text = PlaceObject.name;
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (ControlScript.CurrentTool == setTool && GetComponent<Button>().IsInteractable())
        {
            if (setTool == ControlScript.Tool.None)
            {
                if (Control.DrawObject == PlaceObject)
                {
                    GetComponent<Button>().interactable = false;
                }
                else
                {
                    GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                GetComponent<Button>().interactable = false;
            } 
        }
        else if (ControlScript.CurrentTool == setTool)
        {
            if (setTool == ControlScript.Tool.None)
            {
                if (Control.DrawObject == PlaceObject)
                {
                    GetComponent<Button>().interactable = false;
                }
                else
                {
                    GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            GetComponent<Button>().interactable = true;
        }
	}
	public void ButtonPress(){
        Control.RemoveCopying();
        if (setTool != ControlScript.Tool.None)
        {
            ControlScript.CurrentTool = setTool;
            
        }
        else
        {
            Control.DrawObject = PlaceObject;
            ControlScript.CurrentTool = setTool;
        }
        if (ControlScript.CurrentMode == ControlScript.Mode.Build && PlaceObject != null)
        {
            Control.skinUI.GetComponent<SkinUI>().DisplaySkins();
        }
	}

}
