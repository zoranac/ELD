using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MouseScript : MonoBehaviour {
   
	//float distance = 3;
	public float buffer;
	ControlScript control;
	GameObject CreateObj;
	public bool Skip = false;
   
    public GameObject Highlight;
    public GameObject PowerLine;
	public GameObject SelectGameObject;
    public GameObject MoveGameObject;
    public GameObject HelpGameObject;
	GameObject objectEditor;
    public GameObject HelpUIObject;
    bool PlacingPowerline = false;
    GameObject tempPowerLine;
    public Texture2D[] MouseIcons = new Texture2D[5];
    private float mouseButtonDownTime =0;
    private Vector2 lastmousePos;
	// Use this for initialization
//	public void ResetTempPowerLine(){
//		tempPowerLine = null;
//		PlacingPowerline = false;
//	}
	void Start () {
		control = GameObject.Find("Control").GetComponent<ControlScript>();
		objectEditor = GameObject.Find("ObjectEditor");

	}

	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(-.3f,.3f,-.75f);
        
        if (ControlScript.CurrentMode != ControlScript.Mode.Play)
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    SetCopying();
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    UndoHandlerWebGL.instance.Undo();
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    UndoHandlerWebGL.instance.Redo();
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    ControlScript.CurrentTool = ControlScript.Tool.Move;
                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    ControlScript.CurrentTool = ControlScript.Tool.Help;
                }
            }
        }


        Cursor.SetCursor(MouseIcons[0], Vector2.zero, CursorMode.Auto);
        if (((Input.mousePosition.x < 0 +buffer || Input.mousePosition.x > Screen.width - buffer) 
		    || (Input.mousePosition.y < 0 +buffer || Input.mousePosition.y > Screen.height - buffer)) && !Skip){
			Skip = true;
		}
		//print (Input.mousePosition + " " + Screen.width);

		CreateObj = control.DrawObject;
		//IF MODE IS EDIT
        if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButton(0) && mouseButtonDownTime > 0 && Time.time < mouseButtonDownTime + 1f)
                {
                    if (UnmovedMouseTest())
                    {

                    }
                    else
                    {
                        mouseButtonDownTime = 0;
                    }
                }


                if (Input.GetMouseButton(0)&&mouseButtonDownTime==0)
                {
                    mouseButtonDownTime = Time.time;
                }

                if ((Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && Time.time >= mouseButtonDownTime + 1f && TestMove() != null)
                {
                    MoveObjectInstantPickUp();
                    //ControlScript.CurrentTool = ControlScript.Tool.Move;
                }
                else if (Input.GetMouseButton(1))
                    EraseConnection();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Select)
                    SelectObject();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Move)
                    MoveObject();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Help)
                    HelpObject();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Erase)
                    EraseConnection();
                else if (CreateObj == PowerLine)
                    DrawPowerLine();
                else
                    CreateConnectionObject();

                if (!Input.GetMouseButton(0))
                {
                    mouseButtonDownTime = 0;
                }
            }
            if (Highlight.activeSelf)
				Highlight.SetActive(false);
        }
        else if (ControlScript.CurrentMode == ControlScript.Mode.Play)
		{
			if (Highlight.activeSelf)
				Highlight.SetActive(false);
		}
        else
        {
			if (!Highlight.activeSelf)
				Highlight.SetActive(true);
            MoveHighlight();
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButton(0) && mouseButtonDownTime > 0 && Time.time < mouseButtonDownTime + 1f)
                {
                    if (UnmovedMouseTest())
                    {

                    }
                    else
                    {
                        mouseButtonDownTime = 0;
                    }
                }
       
                if (Input.GetMouseButton(0)&&mouseButtonDownTime==0)
                {
                    mouseButtonDownTime = Time.time;
                }
                
                if ((Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && Time.time >= mouseButtonDownTime + 1f)
                {
                    MoveObjectInstantPickUp();
                    //ControlScript.CurrentTool = ControlScript.Tool.Move;
                }
                else if (Input.GetMouseButton(1))
                    Erase();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Select)
                    SelectObject();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Move)
                    MoveObject();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Help)
                    HelpObject();
                else if (ControlScript.CurrentTool == ControlScript.Tool.Erase)
                    Erase();
                else
                    Draw();

                if (!Input.GetMouseButton(0))
                {
                    mouseButtonDownTime = 0;
                }
            }
        }


        if (!HelpUIObject.activeSelf && ControlScript.CurrentMode != ControlScript.Mode.Play)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (Camera.main.orthographicSize < 6)
                        Camera.main.orthographicSize = Camera.main.orthographicSize + .25f;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (Camera.main.orthographicSize > .5f)
                        Camera.main.orthographicSize = Camera.main.orthographicSize - .25f;
                }
            }
            if (Input.GetMouseButton(2))
            {
                Cursor.SetCursor(MouseIcons[3], Vector2.zero, CursorMode.Auto);
            }
        }
        if (control.DrawObject == null)
            GetComponent<SpriteRenderer>().sprite = null;
        else if (control.DrawObject.GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().sprite = control.DrawObject.GetComponent<SpriteRenderer>().sprite;
        else
            GetComponent<SpriteRenderer>().sprite = null;
    }
    public void SetCopying()
    {
        control.SetCopying(objectEditor.GetComponent<ObjectEditor>().SelectedObject);
    }
    bool UnmovedMouseTest()
    {
        if (Mathf.Abs(Input.mousePosition.x - lastmousePos.x) < .02f && Mathf.Abs(Input.mousePosition.y - lastmousePos.y) < .02f )
        {
            lastmousePos = Input.mousePosition;
            return true;
        }
        else
        {
            lastmousePos = Input.mousePosition;
            return false;
        }
    }
    void HelpObject()
    {

        Cursor.SetCursor(MouseIcons[4], new Vector2(MouseIcons[4].width/2,MouseIcons[4].height/2), CursorMode.Auto);
        if (Input.GetMouseButtonDown(0))
        {
            foreach (Collider2D col in Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                bool gethelp = false;
                if (col.gameObject.layer == 8 )
                {
                    if (ControlScript.CurrentMode == ControlScript.Mode.Build)
                    {
                        gethelp = true;
                    }
                }
                else if (col.gameObject.layer == 10 )
                {
                    if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
                    {
                        gethelp = true;
                    }
                }
                else
                {
                    gethelp = true;
                }

                if (gethelp)
                {
                    if (col.gameObject.GetComponent<HelpObject>() != null)
                    {
                        HelpObject h = col.gameObject.GetComponent<HelpObject>();
                        HelpUIObject.GetComponent<HelpUI>().ShowHelpUI();
                        HelpUIObject.GetComponent<HelpUI>().SetUpHelpUI(h.ObjectName, h.ObjectDesc, h.sprites);
                        control.DrawObject = SelectGameObject;
                        break;
                    }
                }
            }
        }
    }
    Collider2D TestMove()
    {
        foreach (Collider2D col in Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if ((col.gameObject.layer == 8 && ControlScript.CurrentMode == ControlScript.Mode.Build) ||
                (col.gameObject.layer == 10 && ControlScript.CurrentMode == ControlScript.Mode.Connect ||
                col.tag == "Player"))
            {
                return col;
            }
        }
        return null;
    }
    void MoveObject()
    {
        if (MoveGameObject.GetComponent<MoveObjectScript>().MoveObject == null)
        {
            Cursor.SetCursor(MouseIcons[5], Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(MouseIcons[2], Vector2.zero, CursorMode.Auto);
        }
        if (Input.GetMouseButtonDown(0) && MoveGameObject.GetComponent<MoveObjectScript>().MoveObject == null)
        {
            Collider2D col = TestMove();
            if (col != null)
            {
                MoveGameObject.GetComponent<MoveObjectScript>().SetMoveObject(col.gameObject);
                Color color = col.gameObject.GetComponent<SpriteRenderer>().color;
                col.gameObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a / 2);
            }
        }
        else if (Input.GetMouseButtonDown(0) && MoveGameObject.GetComponent<MoveObjectScript>().MoveObject != null)
		{
            
            GameObject obj = null;
			foreach (Collider2D col in Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
			{
                if (col.gameObject.layer == 8 && ControlScript.CurrentMode == ControlScript.Mode.Build)
                {
                    obj = null;
                    break;
                }
                else if (col.gameObject.layer == 10 && ControlScript.CurrentMode == ControlScript.Mode.Connect)
                {
                    obj = null;
                    break;
                }
                if ((MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.layer == 8 || MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.tag == "Player") &&
                    ControlScript.CurrentMode == ControlScript.Mode.Build)
				{
                    if (col.tag == "Tile")
                        obj = col.gameObject;
						
				}
                else if (MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.layer == 10 && ControlScript.CurrentMode == ControlScript.Mode.Connect)
				{
					if (col.tag == "DotTile")
                        obj = col.gameObject;
				}
			}
            if (obj != null)
            {
                Vector3 v = new Vector3(obj.transform.position.x, obj.transform.position.y, MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.transform.position.z);
                UndoHandlerWebGL.instance.AddMoveAction<Vector3>(MoveGameObject, MoveGameObject.transform.position, v);
                if ((Vector2)MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.transform.position == (Vector2)control.SelectedHighlight.transform.position)
                {
                    control.SetSelected(v);
                }
                if ((Vector2)MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.transform.position == (Vector2)control.CopyingHighlight.transform.position)
                {
                    control.SetCopying(v);
                }
                if (MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.tag == "Player")
                {
                    control.PlayerStartPos = v;
                }
                Color color = MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.GetComponent<SpriteRenderer>().color;
                MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a * 2);
                MoveGameObject.GetComponent<MoveObjectScript>().PlaceMoveObject(v);
            }
		}
    }
    void MoveObjectInstantPickUp()
    {
        if (MoveGameObject.GetComponent<MoveObjectScript>().MoveObject == null)
        {
            Cursor.SetCursor(MouseIcons[5], Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(MouseIcons[2], Vector2.zero, CursorMode.Auto);
        }
        if (MoveGameObject.GetComponent<MoveObjectScript>().MoveObject == null)
        {
            foreach (Collider2D col in Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if ((col.gameObject.layer == 8 && ControlScript.CurrentMode == ControlScript.Mode.Build) ||
                    (col.gameObject.layer == 10 && ControlScript.CurrentMode == ControlScript.Mode.Connect ||
                    col.tag == "Player"))
                {
                    MoveGameObject.GetComponent<MoveObjectScript>().SetMoveObject(col.gameObject);
                    Color color = col.gameObject.GetComponent<SpriteRenderer>().color;
                    col.gameObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a / 2);
                }
            }

        }
        else if (Input.GetMouseButtonUp(0) && MoveGameObject.GetComponent<MoveObjectScript>().MoveObject != null)
        {

            GameObject obj = null;
            foreach (Collider2D col in Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (col.gameObject.layer == 8 && ControlScript.CurrentMode == ControlScript.Mode.Build)
                {
                    obj = null;
                    break;
                }
                else if (col.gameObject.layer == 10 && ControlScript.CurrentMode == ControlScript.Mode.Connect)
                {
                    obj = null;
                    break;
                }
                if ((MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.layer == 8 || MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.tag == "Player") &&
                    ControlScript.CurrentMode == ControlScript.Mode.Build)
                {
                    if (col.tag == "Tile")
                        obj = col.gameObject;

                }
                else if (MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.layer == 10 && ControlScript.CurrentMode == ControlScript.Mode.Connect)
                {
                    if (col.tag == "DotTile")
                        obj = col.gameObject;
                }
            }
            if (obj != null)
            {
                Vector3 v = new Vector3(obj.transform.position.x, obj.transform.position.y, MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.transform.position.z);
                UndoHandlerWebGL.instance.AddMoveAction<Vector3>(MoveGameObject, MoveGameObject.transform.position, v);
                if ((Vector2)MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.transform.position == (Vector2)control.SelectedHighlight.transform.position)
                {
                    control.SetSelected(v);
                }
                if ((Vector2)MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.transform.position == (Vector2)control.CopyingHighlight.transform.position)
                {
                    control.SetCopying(v);
                }
                if (MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.tag == "Player")
                {
                    control.PlayerStartPos = v;
                }
                Color color = MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.GetComponent<SpriteRenderer>().color;
                MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a * 2);
                MoveGameObject.GetComponent<MoveObjectScript>().PlaceMoveObject(v);
            }
            else
            {
                Color color = MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.GetComponent<SpriteRenderer>().color;
                MoveGameObject.GetComponent<MoveObjectScript>().MoveObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a * 2);
                MoveGameObject.GetComponent<MoveObjectScript>().MoveObject = null;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseButtonDownTime = 0;
        }
    }
    void MoveHighlight()
    {
        foreach (Collider2D col in Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if (col.tag == "Tile")
            {
                Vector3 pos = col.transform.position;
                Highlight.transform.position = new Vector3(pos.x, pos.y, -2);
            }
        }
    }
	void SelectObject(){
        Cursor.SetCursor(MouseIcons[0], Vector2.zero, CursorMode.Auto);
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
		{
			foreach (Collider2D col in Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
			{
                if ((col.gameObject.layer == 8 && ControlScript.CurrentMode == ControlScript.Mode.Build) ||
                    (col.gameObject.layer == 10 && ControlScript.CurrentMode == ControlScript.Mode.Connect))
                {
                    control.SetSelected(col.gameObject);
                }
			}
		}
	}
    void DrawPowerLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //distance = 3 + Input.mousePosition.x / 10;

        //Move end of the Line to the mouse pos
        if (PlacingPowerline)
        {
			tempPowerLine.GetComponent<PowerLineScript>().UpdateLinePos(new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,Camera.main.ScreenToWorldPoint(Input.mousePosition).y,-1));
        }
		//while left mouse button is down 
        if (Input.GetMouseButton(0))
        {
			foreach (RaycastHit2D obj in Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction))
			{
				if (obj.collider.gameObject.tag == "DotTile")
                {
                    //Select Object if there is an object there
                    if (obj.collider.GetComponent<DotTileScript>().ObjectOnMe != null)
                    {
                        SelectObject();
                    }
					int count = 0;

					if (tempPowerLine != null)
					{
						if (tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Count > 0)
							count = tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Count;
					}
                    //start placing powerline
                    if (!PlacingPowerline)
                    {
                        GameObject temp = (GameObject)Instantiate(PowerLine, new Vector3(obj.transform.position.x, obj.transform.position.y, -1), Quaternion.identity);
                        temp.name = PowerLine.name.Replace("(Clone)", "");
                        tempPowerLine = temp;
						tempPowerLine.GetComponent<PowerLineScript>().SetUp(obj.transform.position);
                        tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Add(obj.collider.gameObject);
						obj.collider.GetComponent<DotTileScript>().Connections.Add(tempPowerLine);
                        PlacingPowerline = true;
                        break;
                    }
                    else
                    {    
                        //tempPowerLine.GetComponent<PowerLineScript>().SetLinePos(new Vector3 (obj.transform.position.x, .5f, obj.transform.position.z));
                        //tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[0] = obj.collider.gameObject;
                        //tempPowerLine = null;
                        //PlacingPowerline = false;
//						if(){
//
//						}
						//if one of the dots next to the starting dot
						if (obj.collider.gameObject == tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[count-1].GetComponent<DotTileScript>().TileAbove ||
						    obj.collider.gameObject == tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[count-1].GetComponent<DotTileScript>().TileBelow ||
						    obj.collider.gameObject == tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[count-1].GetComponent<DotTileScript>().TileLeft ||
						    obj.collider.gameObject == tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[count-1].GetComponent<DotTileScript>().TileRight)
						{
							tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Add(obj.collider.gameObject);
                            tempPowerLine.GetComponent<PowerLineScript>().SetLinePos(new Vector3(obj.transform.position.x, obj.transform.position.y, -1));
                            
							//add connection for the other dotTile
							obj.collider.GetComponent<DotTileScript>().Connections.Add(tempPowerLine);
							//Enable Collision on PowerLine
							tempPowerLine.GetComponent<BoxCollider2D>().enabled = true;
                            //Add to undo handler
                            //UndoHandler.AddPlaceAction<Vector3>(tempPowerLine);
                            UndoHandlerWebGL.instance.OnPlaced<Vector3>(tempPowerLine);
							//start new connection at last connection's ending point
							GameObject temp = (GameObject)Instantiate(PowerLine, new Vector3(obj.transform.position.x, obj.transform.position.y,-1),  Quaternion.identity);
                            temp.name = PowerLine.name.Replace("(Clone)", "");
							tempPowerLine = temp;
							tempPowerLine.GetComponent<PowerLineScript>().SetUp(obj.transform.position);
							tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Add(obj.collider.gameObject);
							obj.collider.GetComponent<DotTileScript>().Connections.Add(tempPowerLine);
							break;

						}
                    }
                }   
            }
        }
		if (Input.GetMouseButtonUp(0))
		{
			if (tempPowerLine != null)
			{
				if (tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Count > 0)
				{
					//if the last connection point is not dead on a Dot Tile, remove the last point in the line
					if (Camera.main.ScreenToWorldPoint(Input.mousePosition) != tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Count-1].transform.position)
					{
						tempPowerLine.GetComponent<PowerLineScript>().RemoveLinePos();
					}
					//if the connection is incomplete, remove the connection from the dot tile and destory the connection
					if (tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots.Count == 1)
					{
						tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[0].GetComponent<DotTileScript>().Connections.RemoveAt(
							tempPowerLine.GetComponent<PowerLineScript>().ConnectedDots[0].GetComponent<DotTileScript>().Connections.Count-1);
						Destroy(tempPowerLine);
					}
				}
			}
			tempPowerLine = null;
			PlacingPowerline = false;

		}
    }
    void Draw(){
        if (Skip)
        {
            Skip = false;
        }
        else
        {
            

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //distance = 3 + Input.mousePosition.x / 10;

            bool instantiate = false;
            Collider2D Obj = new Collider2D();

            foreach (RaycastHit2D obj in Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction))
            {
                if (Input.GetMouseButton(0))
                {
                    if (obj.collider.tag == "Tile")
                    {
                        instantiate = true;
                        Obj = obj.collider;

                        Ray tileRay = new Ray(obj.transform.position - Vector3.forward, Vector3.forward);

                        foreach (RaycastHit2D obj2 in Physics2D.RaycastAll(obj.transform.position - Vector3.forward, tileRay.direction))
                        {
                            if ((obj2.collider.gameObject.layer == 8 && obj2.collider.tag != "Floor") || obj2.collider.gameObject.tag == "Player")
                            {
                                instantiate = false;
                                SelectObject();
                                break;
                            }
                            else if (obj2.collider.tag == "Floor"  && CreateObj.tag == "Floor")
                            {
                                instantiate = false;
                                SelectObject();
                                break;
                            }
                        }              
                    }
                }
              
            }
            if (instantiate)
            {
                if (CreateObj != null)
                {
                    GameObject tempObj = (GameObject)Instantiate(CreateObj, new Vector3(Obj.transform.position.x, Obj.transform.position.y, 0), Obj.transform.rotation) as GameObject;
                    tempObj.name = tempObj.name.Replace("(Clone)", "");
                    if (GameObject.Find("SkinUI") != null)
                    {
                        GameObject.Find("SkinUI").GetComponent<SkinUI>().HideSkins();
                    }
                    //Add to undo handler
                    //UndoHandler.instance.AddPlaceAction<Vector3>(tempObj);
                    UndoHandlerWebGL.instance.OnPlaced<Vector3>(tempObj);
                    //set as selected obj
                    control.SetSelected(tempObj);
                }
            }
        }
	}
	void Erase(){
		if (Skip){
			Skip = false;
		}else{
            Cursor.SetCursor(MouseIcons[1], Vector2.zero, CursorMode.Auto);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//distance = 3 + Input.mousePosition.x/10;
			
			RaycastHit2D destroyObj = new RaycastHit2D();
            foreach (RaycastHit2D obj in Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction))
            { 
                if (obj.collider.tag == "Tile")
                {
                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                    {
                        Ray tileRay = new Ray(obj.transform.position- Vector3.forward, Vector3.forward);

                        foreach (RaycastHit2D obj2 in Physics2D.RaycastAll(obj.transform.position - Vector3.forward, tileRay.direction))
                        {
                            if (obj2.collider.gameObject.layer == 8)
                            {
                                //Add to undo handler
                                //UndoHandler.instance.AddEraseAction<Vector3>(obj2.collider.gameObject, obj2.transform.position);
                                UndoHandlerWebGL.instance.OnErased<Vector3>(obj2.collider.gameObject);

                                destroyObj = obj2;
                            }
                        }
                    }
                }
			}
            if (destroyObj.collider != null)
            {
                if ((Vector2)destroyObj.transform.position == (Vector2)control.SelectedHighlight.transform.position)
                {
                    control.RemoveSelected();
                }
                if ((Vector2)destroyObj.transform.position == (Vector2)control.CopyingHighlight.transform.position)
                {
                    control.RemoveCopying();
                }
                if (destroyObj.collider.GetComponent<ObjectRequiresConnection>())
                {
                    destroyObj.collider.GetComponent<ObjectRequiresConnection>().dotTile.Blocked = false;
                    destroyObj.collider.GetComponent<ObjectRequiresConnection>().dotTile.ObjectUnderMe = null;
                }
                //IF actually destroy
                //Destroy(destroyObj.collider.gameObject);
                
                //temp hide in case of undo
                destroyObj.collider.gameObject.GetComponent<PlaceableObject>().SetActive(false);
            }
		}
	}
	void EraseConnection(){
		if (Skip){
			Skip = false;
		}else{
            Cursor.SetCursor(MouseIcons[1], Vector2.zero, CursorMode.Auto);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//distance = 3 + Input.mousePosition.x/10;
			
			RaycastHit2D destroyObj = new RaycastHit2D();
			foreach (RaycastHit2D obj in Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction))
			{ 
				if (obj.collider.tag == "DotTile")
				{
                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
					{
						Ray tileRay = new Ray(obj.transform.position- Vector3.forward, Vector3.forward);
						
						foreach (RaycastHit2D obj2 in Physics2D.RaycastAll(obj.transform.position - Vector3.forward, tileRay.direction))
						{
							if (obj2.collider.gameObject.layer == 10)
							{
                                //Add to undo handler
                                //UndoHandler.instance.AddEraseAction<Vector3>(obj2.collider.gameObject, obj2.transform.position);
                                UndoHandlerWebGL.instance.OnErased<Vector3>(obj2.collider.gameObject);
								destroyObj = obj2;
							}
						}
					}
				}
				else if (obj.collider.tag == "PowerLine")
				{
                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
					{
                        //Add to undo handler
                        UndoHandlerWebGL.instance.OnErased<Vector3>(obj.collider.gameObject);

						destroyObj = obj;
						destroyObj.collider.GetComponent<PowerLineScript>().DestroyMeSetUp();
					}
				}
			}
            if (destroyObj.collider != null)
            {
                if ((Vector2)destroyObj.transform.position == (Vector2)control.SelectedHighlight.transform.position)
                {
                    control.RemoveSelected();
                }
                if ((Vector2)destroyObj.transform.position == (Vector2)control.CopyingHighlight.transform.position)
                {
                    control.RemoveCopying();
                }
                if (destroyObj.collider.GetComponent<TempPowerOutput>() != null)
                {
                    //IF actually destroy
                     //destroyObj.collider.GetComponent<TempPowerOutput>().DestroyMe();

                    //temp hide in case of undo
                    destroyObj.collider.GetComponent<TempPowerOutput>().RemoveFromDotTile();
                    destroyObj.collider.gameObject.GetComponent<PlaceableObject>().SetActive(false);
                }
                else if (destroyObj.collider.GetComponent<PowerOutput>() != null)
                {
                    //IF actually destroy
                    //Destroy(destroyObj.collider.gameObject);

                    //temp hide in case of undo
                    destroyObj.collider.GetComponent<PowerOutput>().RemoveFromDotTile();
                    destroyObj.collider.gameObject.GetComponent<PlaceableObject>().SetActive(false);
                } 
                else
                {
                    destroyObj.collider.gameObject.GetComponent<PlaceableObject>().SetActive(false);
                }
            }
		}
	}
	void CreateConnectionObject(){
		if (Skip)
		{
			Skip = false;
		}
		else
		{
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//distance = 3 + Input.mousePosition.x / 10;
			
			bool instantiate = false;
			Collider2D Obj = new Collider2D();
			
			foreach (RaycastHit2D obj in Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction))
			{
				if (obj.collider.tag == "DotTile")
				{
					//Vector3 pos = obj.transform.position;
					//Highlight.transform.position = new Vector3(pos.x, pos.y, pos.z) ;
				}
				if (Input.GetMouseButton(0))
				{
					if (obj.collider.tag == "DotTile")
					{
						instantiate = true;
						Obj = obj.collider;
						
						Ray tileRay = new Ray(obj.transform.position - Vector3.forward, Vector3.forward);
						
						foreach (RaycastHit2D obj2 in Physics2D.RaycastAll(obj.transform.position - Vector3.forward, tileRay.direction))
						{
							if (obj2.collider.gameObject.layer == 10)
							{
								instantiate = false;
                                SelectObject();
								break;
							}
						}
						
						
						
					}
				}
				
			}
			if (instantiate)
			{
                if (CreateObj != null)
                {
                    GameObject tempObj = (GameObject)Instantiate(CreateObj, new Vector3(Obj.transform.position.x, Obj.transform.position.y, -1.25f), Obj.transform.rotation) as GameObject;
                    tempObj.name = tempObj.name.Replace("(Clone)", "");
                    //Add to undo handler
                    //UndoHandler.instance.AddPlaceAction<Vector3>(tempObj);
                    UndoHandlerWebGL.instance.OnPlaced<Vector3>(tempObj);
                    //set as selected obj
                    control.SetSelected(tempObj);
                }
            }
		}
	}
}
