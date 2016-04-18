using UnityEngine;
using System.Collections;

public class SwitchScript : InteractableObject {
    [Editable(true)]
	public bool On;
    bool defaultOn;
    public bool Connected = false;
    public GameObject ConnectorSwitchPrefab;
    public GameObject DisconnectedObj;
	// Use this for initialization
	void Start () {
        bool create = true;
        foreach (Collider2D col in Physics2D.OverlapPointAll(transform.position))
        {
            if (col.gameObject.layer == 10)
            {
                create = false;
                break;
            }
        }
        if (create)
        {
            GameObject temp = (GameObject)Instantiate(ConnectorSwitchPrefab, new Vector3(transform.position.x, transform.position.y,-1.25f), Quaternion.identity);
            temp.transform.parent = gameObject.transform;
            temp.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            Connected = false;
            GameObject temp = (GameObject)Instantiate(ConnectorSwitchPrefab, new Vector3(transform.position.x, transform.position.y, -1.25f), Quaternion.identity);
            temp.transform.parent = gameObject.transform;
            temp.SetActive(false);
        }
        defaultOn = On;
    }
    public override void SetActive(bool value)
    {
        if (Connected)
        {
            GetComponentInChildren<ConnectorSwitch>().dotTile.GetComponent<DotTileScript>().ObjectOnMe = null;
            GetComponentInChildren<ConnectorSwitch>().dotTile.GetComponent<DotTileScript>().ObjectUnderMe = null;
        }
        base.SetActive(value);
    }
    public void TestIfConnected(bool setOn)
    {
        Connected = false;
        foreach (Collider2D col in Physics2D.OverlapPointAll(transform.position))
        {
            if (col.gameObject.layer == 10)
            {
                if (col.gameObject.GetComponent<ConnectorSwitch>() != null)
                {
                    Connected = true;
                    if (setOn)
                    {
                        col.gameObject.GetComponent<ConnectorSwitch>().SwitchState = On;
                    }
                }
            }
        }
    }
    void Update()
    {
        TestIfConnected(false);
        if (On)
        {
            GetComponent<SpriteRenderer>().sprite = CurrentSkin.AllSpritesInSkin[0];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = CurrentSkin.AllSpritesInSkin[1];
        }
        if (!Connected)
        {
            if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
                DisconnectedObj.SetActive(!Connected);
            else
                DisconnectedObj.SetActive(false);
        }
        else
        {
            DisconnectedObj.SetActive(false);
        }
    }
    override public void Interact()
	{
		On = !On;
	}
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
    {
        if (sender.ToString() == "System.Boolean On")
        {
            //Set value in undo handler
            if (AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<bool>(gameObject, On, bool.Parse(value.ToString()),sender);

            //Set Value
            On = bool.Parse(value.ToString());
        }
    }
    public void SetDefaultOn()
    {
        defaultOn = On;
    }
    public void ResetOn()
    {
        On = defaultOn;
        TestIfConnected(true);
    }
}
