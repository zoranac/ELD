using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class ControlScript : MonoBehaviour {
    public enum Mode
    {
        Build,
        Connect,
        Play
    };
    public enum Tool
    {
        Select,
        Erase,
        Move,
        Help,
        None
    };

    static public Tool CurrentTool = Tool.Select;
    static public Mode CurrentMode = Mode.Build;
    GameObject player;
	public Vector3 PlayerStartPos;
	public GameObject DrawObject;
	public List<GameObject> PlaceableObjs = new List<GameObject>();
	public List<GameObject> ConnectionObjs = new List<GameObject>();
	public GameObject BuildGUI;
	public GameObject ConnectionGUI;
    public GameObject ObjectPlacementButton;
    public GameObject canvas;
    public GameObject skinUI;
	public float PlayModeZoom;
	Mode lastMode = Mode.Build;
	GameObject editModeButton;
	GameObject eraseButton;
	Vector3 posBeforePlay;
	float zoomBeforePlay;
	GameObject drawObjBeforePlay;
    GameObject objectEditor;
    GameObject moveButton;
    GameObject selectButton;
    MySaveGame savedGame = new MySaveGame();
	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
		Screen.fullScreen = false;
        player = GameObject.Find("Player");
        PlayerStartPos = player.transform.position;
        objectEditor = GameObject.Find("ObjectEditor");
		CreateGUIButtons();
		editModeButton = GameObject.Find("EditModeButton");
		eraseButton = GameObject.Find("EraseButton");
        moveButton = GameObject.Find("MoveButton");
        selectButton = GameObject.Find("SelectButton");
        if (savedGame == null)
        {
            savedGame = new MySaveGame();
        }
        foreach (GameObject obj in PlaceableObjs)
        {
            print(obj.name);
            obj.GetComponent<SkinableObject>().SetSkin(obj.GetComponent<SkinableObject>().Skins[0]);
        }
    }
	void CreateGUIButtons(){

        BuildGUI.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().sizeDelta =
            new Vector2(BuildGUI.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().sizeDelta.x, (ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.y * PlaceableObjs.Count)+Screen.height/8);
        float y = BuildGUI.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().position.y - ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.y;
        float x = ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.x / 1.75f;
		//Build GUI
        foreach (GameObject obj in PlaceableObjs)
        {
            GameObject tempButton = Instantiate(ObjectPlacementButton);
            
            //tempButton.GetComponent<RectTransform>().position = new Vector3(x, y, 0);
            tempButton.GetComponent<RectTransform>().SetParent(BuildGUI.transform.FindChild("Viewport").FindChild("Content"), false);
            tempButton.transform.position = new Vector3(x, y, 0);
            tempButton.GetComponent<ObjectPlacementButtonScript>().PlaceObject = obj;
            tempButton.transform.localScale = new Vector3(1, 1, 1);
            y -= 25/ (400/(float)Screen.height);
        }

        ConnectionGUI.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().sizeDelta =
            new Vector2(ConnectionGUI.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().sizeDelta.x, (ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.y * ConnectionObjs.Count)+Screen.height/8);

		//Connect GUI
        y = BuildGUI.transform.FindChild("Viewport").FindChild("Content").GetComponent<RectTransform>().position.y - ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.y;
        x = ObjectPlacementButton.GetComponent<RectTransform>().sizeDelta.x/1.75f;
		foreach (GameObject obj in ConnectionObjs)
		{
			GameObject tempButton = Instantiate(ObjectPlacementButton);

			//tempButton.GetComponent<RectTransform>().position = new Vector3(x, y, 0);
            tempButton.GetComponent<RectTransform>().SetParent(ConnectionGUI.transform.FindChild("Viewport").FindChild("Content"), false);
			tempButton.transform.position = new Vector3(x, y, 0);
			tempButton.GetComponent<ObjectPlacementButtonScript>().PlaceObject = obj;
            tempButton.transform.localScale = new Vector3(1, 1, 1);
			y -= 25/ (400/(float)Screen.height);
		}

    

		ConnectionGUI.SetActive(false);
	}
    public void ChangeModeBetweenPlaceAndEdit()
    {
        //setup for build mode
        if (CurrentMode == Mode.Connect)
        {
            skinUI.transform.parent.gameObject.SetActive(true);
			BuildGUI.SetActive(true);
			ConnectionGUI.SetActive(false);
            player.SetActive(true);
			CurrentMode = Mode.Build;
            ChangeConnectionObjectVisability();
            foreach (GameObject obj in ConnectionObjs)
            {
                if (DrawObject == obj)
                {
                    DrawObject = PlaceableObjs[0];
                    break;
                }
            }
			
        }
        //setup for connect mode
        else if (CurrentMode == Mode.Build)
        {
            skinUI.transform.parent.gameObject.SetActive(false);
			BuildGUI.SetActive(false);
			ConnectionGUI.SetActive(true);
            player.SetActive(false);
			CurrentMode = Mode.Connect;
            ChangeConnectionObjectVisability();
            foreach (GameObject obj in PlaceableObjs)
            {
                if (DrawObject == obj)
                {
                    DrawObject = ConnectionObjs[0];
                    break;
                }
            }
        }
        
    }
    public void SetToPlayMode()
    {
        //Set Up for Play mode
		if (CurrentMode != Mode.Play)
		{
            player.SetActive(true);
            player.transform.position = PlayerStartPos;
            player.transform.rotation = Quaternion.identity;
			drawObjBeforePlay = DrawObject;
            skinUI.transform.parent.gameObject.SetActive(false);
			posBeforePlay = Camera.main.transform.position;
			zoomBeforePlay = Camera.main.orthographicSize;
			Camera.main.orthographicSize = PlayModeZoom;
			DrawObject = null;
			editModeButton.SetActive(false);
			eraseButton.SetActive(false);
			BuildGUI.SetActive(false);
			ConnectionGUI.SetActive(false);
            objectEditor.SetActive(false);
            selectButton.SetActive(false);
            moveButton.SetActive(false);
            setDefaultOnSwitches();
			lastMode = CurrentMode;
        	CurrentMode = Mode.Play;
            ChangeConnectionObjectVisability();
		}
        //Set up for leaving play mode
		else{
			player.transform.position = PlayerStartPos;
            player.transform.rotation = Quaternion.identity;
            resetPushableObjects();
			eraseButton.SetActive(true);
			DrawObject = drawObjBeforePlay;
			CurrentMode = lastMode;
			Camera.main.transform.position = posBeforePlay;
			Camera.main.orthographicSize = zoomBeforePlay;
			editModeButton.SetActive(true);
			if (CurrentMode == Mode.Connect)
			{
				ConnectionGUI.SetActive(true);
                player.SetActive(false);
			}
			if (CurrentMode == Mode.Build)
			{
				BuildGUI.SetActive(true);
                skinUI.transform.parent.gameObject.SetActive(true);
                player.SetActive(true);
			}
            resetSwitches();
            objectEditor.SetActive(true);
            selectButton.SetActive(true);
            moveButton.SetActive(true);
            ChangeConnectionObjectVisability();
		}
		
    }
    private void resetPushableObjects()
    {
        if (FindGameObjectsWithLayer(8) != null)
        {
            foreach (GameObject obj in FindGameObjectsWithLayer(8))
            {
                if (obj.GetComponent<PushableObject>() != null)
                {
                    obj.GetComponent<PushableObject>().SetTransformToDefault();
                }
            }
        }
    }
    private void resetSwitches()
    {
        if (FindGameObjectsWithLayer(8) != null)
        {
            foreach (GameObject obj in FindGameObjectsWithLayer(8))
            {
                if (obj.GetComponent<SwitchScript>() != null)
                {
                    obj.GetComponent<SwitchScript>().ResetOn();
                }
            }
        }
    }
    private void setDefaultOnSwitches()
    {
        if (FindGameObjectsWithLayer(8) != null)
        {
            foreach (GameObject obj in FindGameObjectsWithLayer(8))
            {
                if (obj.GetComponent<SwitchScript>() != null)
                {
                    obj.GetComponent<SwitchScript>().SetDefaultOn();
                }
            }
        }
    }
	private void ChangeConnectionObjectVisability()
	{
       
		if (FindGameObjectsWithLayer(10) != null)
		{
			foreach (GameObject obj in FindGameObjectsWithLayer(10))
			{
                if (CurrentMode == Mode.Connect)
                {
                    obj.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    obj.GetComponent<SpriteRenderer>().enabled = false;
                }

                if (obj.GetComponent<TempPowerOutput>()!=null)
                {
                    obj.GetComponent<TempPowerOutput>().ResetPower();
                }
			}
		}
		if (GameObject.FindGameObjectsWithTag("PowerLine") != null){
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PowerLine"))
			{
                if (CurrentMode == Mode.Connect)
                {
                    obj.GetComponent<LineRenderer>().enabled =true;
                    if (obj.GetComponentInChildren<SpriteRenderer>() != null)
                        obj.GetComponentInChildren<SpriteRenderer>().enabled = !obj.GetComponentInChildren<SpriteRenderer>().enabled;
                }
                else
                {
                    obj.GetComponent<LineRenderer>().enabled = false;
                    if (obj.GetComponentInChildren<SpriteRenderer>() != null)
                        obj.GetComponentInChildren<SpriteRenderer>().enabled = false;
                }
			}
		}
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("DotTile"))
		{
            if (CurrentMode == Mode.Connect)
            {
                obj.GetComponent<SpriteRenderer>().enabled = true;
                obj.GetComponent<SpriteRenderer>().color = new Color(obj.GetComponent<SpriteRenderer>().color.r, obj.GetComponent<SpriteRenderer>().color.g, obj.GetComponent<SpriteRenderer>().color.b, 1);
            }
            else
            {
                if (CurrentMode == Mode.Build)
                {
                    if (obj.GetComponent<DotTileScript>().Connections.Count > 0 || obj.GetComponent<DotTileScript>().Power > 0)
                    {
                        obj.GetComponent<SpriteRenderer>().enabled = true;
                        obj.GetComponent<SpriteRenderer>().color = new Color(obj.GetComponent<SpriteRenderer>().color.r, obj.GetComponent<SpriteRenderer>().color.g, obj.GetComponent<SpriteRenderer>().color.b, .5f);
                    }
                    else
                    {
                        obj.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
                else
                {
                    obj.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
		}
	}
	public GameObject[] FindGameObjectsWithLayer (int layer){
		GameObject[] goArray = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		List<GameObject> goList = new List<GameObject>();
		for (var i = 0; i < goArray.Length; i++) {
			if (goArray[i].layer == layer) {
				goList.Add(goArray[i]);
			}
		}
		if (goList.Count == 0) {
			return null;
		}
		return goList.ToArray();
	}
	// Update is called once per frame
	void Update () {

	}
    public void SaveGame()
    {
        savedGame.Objects.Clear();
        foreach(PlaceableObject obj in GameObject.FindObjectsOfType<PlaceableObject>())
        {
            savedGame.Objects.Add(obj.gameObject);
        }
        foreach (PowerLineScript obj in GameObject.FindObjectsOfType<PowerLineScript>())
        {
            savedGame.Objects.Add(obj.gameObject);
        }
        SaveGameSystem.SaveGame(savedGame, "MySaveGame");
       
    }
    public void LoadGame()
    {
        MySaveGame savedGame2 = SaveGameSystem.LoadGame("MySaveGame") as MySaveGame;
        print(savedGame2);
        foreach (GameObject obj in savedGame2.Objects)
        {
            Instantiate(obj);
        }
    }
}
