using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SkinUI : MonoBehaviour {

    public GameObject ObjectPlacementButton;
    public ControlScript control;
    GameObject lastObjectToPlace;
    public Vector3 TransateToPosition;
    Vector3 startingPos;
    List<GameObject> buttons = new List<GameObject>();
	// Use this for initialization
	void Start () {
        //control = GameObject.Find("Control").GetComponent<ControlScript>();
        startingPos = GetComponent<RectTransform>().transform.position;
	}
	
	// Update is called once per frame
	void Update () {

	}
    public void HideSkins()
    {
        gameObject.SetActive(false);
    }
    public void DisplaySkins()
    {
        if (control.DrawObject != null)
        {
            gameObject.SetActive(true);
            
            if (control.DrawObject != lastObjectToPlace)
            {
                lastObjectToPlace = control.DrawObject;
                createButtons();
            }


        }
    }
    void createButtons()
    {
        foreach(GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();

        this.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().sizeDelta =
           new Vector2(this.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().sizeDelta.x + this.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().position.x, (ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.y * control.DrawObject.GetComponent<SkinableObject>().Skins.Count) + Screen.height / 8);

        float y = this.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().position.y - ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.y;
        float x = Screen.width / 4f;
        //Build GUI
        foreach (Skin skin in control.DrawObject.GetComponent<SkinableObject>().Skins)
        {
            GameObject tempButton = Instantiate(ObjectPlacementButton);
           
            tempButton.GetComponent<RectTransform>().SetParent(gameObject.GetComponent<RectTransform>().transform);
            //tempButton.GetComponent<RectTransform>().position = new Vector3(x, y, 0);
            tempButton.transform.position = new Vector3(x, y, 0);
            tempButton.transform.localScale = new Vector3(1, 1, 1);
            tempButton.GetComponent<SkinButton>().SkinToSet = skin;
            tempButton.GetComponent<SkinButton>().image.sprite = skin.MainSprite;
            y -= 35 / (400 / (float)Screen.height);

            buttons.Add(tempButton);
        }

    }
}
