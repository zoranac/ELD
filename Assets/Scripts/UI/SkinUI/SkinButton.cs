using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SkinButton : MonoBehaviour {

    public Skin SkinToSet;
    ControlScript control;
    public Image image;
	// Use this for initialization
	void Start () {
        control = GameObject.Find("Control").GetComponent<ControlScript>();
        //image = GetComponentInChildren<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (control.DrawObject.GetComponent<SpriteRenderer>().sprite == image.sprite)
        {
            GetComponent<Button>().interactable = false;
        }
        else if (!GetComponent<Button>().interactable) 
        {
            GetComponent<Button>().interactable = true;
        }
	}
    public void SetSkin()
    {
        control.DrawObject.GetComponent<SkinableObject>().SetSkin(SkinToSet);
        transform.parent.gameObject.SetActive(false);
    }
}
