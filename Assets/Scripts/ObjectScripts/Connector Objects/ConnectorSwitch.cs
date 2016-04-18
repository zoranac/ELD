using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ConnectorSwitch : TempPowerOutput {
	public enum Location
	{
		Top,
		Bottom,
		Left,
		Right
	}; 
	bool Connected = false;
	GameObject connectedObject;
    public Sprite[] Sprites = new Sprite[2]; //0 off 1 on
	[Editable(true)]
	public bool SwitchState = false;
    public GameObject DisconnectedObj;
    public int FramesBetweenUpdates = 4;
    private int frame = 0;
    bool powerResetAfterOff = false;
	//input and output directions

	// Update is called once per frame
	void Update () {
        UpdateSwitchState();
        if (PowerReset)
        {
            if (resetWaitFrames <= 0)
            {
                PowerReset = false;
                powerOutput = lastPowerOutput;
            }
            else
            {
                resetWaitFrames--;
            }
        }
        if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
            HideDotTile();
        if (frame >= FramesBetweenUpdates)
        {
            TestIfConnected();
            frame = 0;
          }
        else
        {
            frame++;
        }
        ApplySwitch();
        if (!Connected)
        {
            if (ControlScript.CurrentMode == ControlScript.Mode.Build)
                DisconnectedObj.SetActive(!Connected);
            else
                DisconnectedObj.SetActive(false);
        }
        else
        {
            DisconnectedObj.SetActive(false);
        }
    }
    void ApplySwitch()
    {
		//test power output based on switch bool
		if (SwitchState)
		{
            GetComponent<SpriteRenderer>().sprite = Sprites[1];
			tempPowerOutput = this.powerOutput;
			//print(Inputs.Count);
			if (Inputs.Count == 0)
			{
                tempPowerOutput = 0;
                foreach (GameObject obj in dotTile.GetComponent<DotTileScript>().Connections)
				{
					obj.GetComponent<PowerLineScript>().SetPower(tempPowerOutput,gameObject,dotTile.GetComponent<DotTileScript>().PowerSourceObj);
				}
			}
            powerResetAfterOff = false;
		}
		else
		{
            GetComponent<SpriteRenderer>().sprite = Sprites[0];
            if (!powerResetAfterOff)
            {
                ResetPower();
                powerResetAfterOff = true;
            }
		}
		//if the dot has power from a 
		if (dotTile.GetComponent<DotTileScript>().Powered)
        {
            tempPowerOutput = this.powerOutput;
            bool powered = false;
            foreach (GameObject obj in Inputs)
            {
                if (obj.GetComponent<PowerLineScript>().Power > 0)
                {
                    powered = true;
                    break;
                }
            }
           
            foreach (GameObject obj in Outputs)
            {
                if (powered)
                {
                    if (SwitchState)
                    {
                        if (obj.GetComponent<PowerLineScript>().Power <= tempPowerOutput)
                            obj.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                    }
                    else
                    {
                        //if ((obj.transform.position.x > dotTile.transform.position.x && obj.GetComponent<PowerLineScript>().CurrentFlowDirection == PowerLineScript.FlowDirection.) ||
                        //    (obj.transform.position.x < dotTile.transform.position.x && OutputLocation == Location.Right) ||
                        //    (obj.transform.position.y > dotTile.transform.position.y && OutputLocation == Location.Top) ||
                        //    (obj.transform.position.y < dotTile.transform.position.y && OutputLocation == Location.Bottom))
                        //if (obj.GetComponent<PowerLineScript>().highestPowerObj == gameObject)
                        obj.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                    }
                }
                else
                {
                    tempPowerOutput = 0;
                    obj.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
            }
           
        }

    }
	void UpdateSwitchState()
	{
        if (connectedObject != null)
		    SwitchState = connectedObject.GetComponent<SwitchScript>().On;
	}
	void TestIfConnected(){
		Connected = false;
		connectedObject = null;
		foreach(Collider2D col in Physics2D.OverlapPointAll(transform.position))
		{
			if (col.gameObject.layer == 8)
			{
				if (col.GetComponent<SwitchScript>() != null)
				{
					Connected = true;
					connectedObject = col.gameObject;
				}
			}
		}
	}
	public override void ValueChanged(object sender, object value, bool AddToUndoList)
	{
		//System.Boolean
		//System.Int32
		//System.Decimal
		if (sender.ToString() == "System.Boolean SwitchState")
		{
            //Set value in undo handler
            if(AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<bool>(gameObject, SwitchState, bool.Parse(value.ToString()), sender);

            //Set value
			SwitchState = bool.Parse(value.ToString());
            if (connectedObject != null)
            {
               
                if (connectedObject.GetComponent<ButtonScript>() != null)
                {
                    if (SwitchState)
                        connectedObject.GetComponent<ButtonScript>().Interact();
                    else
                        connectedObject.GetComponent<SwitchScript>().On = SwitchState;
                }
                else{
                     connectedObject.GetComponent<SwitchScript>().On = SwitchState;
                }
            }
		}
		if (sender.ToString() == "System.Int32 powerOutput")
		{
            //Set value in undo handler
            if (AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<int>(gameObject, powerOutput, int.Parse(value.ToString()), sender);

            //Set value
			powerOutput = int.Parse(value.ToString());
			if (SwitchState)
			{
				foreach (GameObject obj in Outputs)
				{
					obj.GetComponent<PowerLineScript>().SetPower(powerOutput,gameObject,dotTile.GetComponent<DotTileScript>().PowerSourceObj);
				}
			}
		}
	}
}
