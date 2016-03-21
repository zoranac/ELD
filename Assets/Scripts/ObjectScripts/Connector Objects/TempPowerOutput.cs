using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TempPowerOutput : IOObject
{
	[Editable(true)]
	public int powerOutput;

	public int tempPowerOutput;
    public int lastPowerOutput;
    public bool PowerReset = false;
    public int resetWaitFrames = 0;
	// Use this for initialization
	void Start () {
		tempPowerOutput = 0;
        SetDotTile();
        Move(gameObject.transform.position);
        HideDotTile();
        control = GameObject.Find("Control").GetComponent<ControlScript>();
        powerOutput = PowerOutput.MaxPower;

	}
    //Resets the power of the outputs and inputs for MaxPower frames and turns off the power of the object. This is to do a total refresh of power (erases powered "memory")
	public void ResetPower(){
        if (powerOutput > 0)
        {
            lastPowerOutput = powerOutput;
            powerOutput = 0;

            PowerReset = true;
            foreach (GameObject line in dotTile.GetComponent<DotTileScript>().Connections)
            {
                line.GetComponent<PowerLineScript>().Power = 0;
                line.GetComponent<PowerLineScript>().highestPowerObj = null;
                line.GetComponent<PowerLineScript>().PowerSourceObj = null;
            }
            dotTile.GetComponent<DotTileScript>().PowerSourceObj = null;
            dotTile.GetComponent<DotTileScript>().Power = 0;
            resetWaitFrames = PowerOutput.MaxPower + 1;
        }
	}
    public void DestroyMe()
    {
        ShowDotTile();
        Destroy(gameObject);
    }
    //Resets the power of the outputs and inputs for 10 frames. This is a softer reset compared to ResetPower 
    public void ResetConnectionPower()
    {
        foreach (GameObject obj in dotTile.GetComponent<DotTileScript>().Connections)
        {
            obj.GetComponent<PowerLineScript>().Power = 0;
            obj.GetComponent<PowerLineScript>().highestPowerObj = null;
            obj.GetComponent<PowerLineScript>().PowerSourceObj = null;
        }
    }
    //For Object Editor
    public override void ValueChanged(object sender, object value)
    {
        if (sender.ToString() == "System.Int32 powerOutput")
        {
            powerOutput = int.Parse(value.ToString());
        }
    }
    //For Move Tool
    public override void Move(Vector3 MoveToPos)
    {
        //ResetPower();
        ResetIO();
        ShowDotTile();
        dotTile.GetComponent<DotTileScript>().ObjectOnMe = null;
        //dotTile.GetComponent<DotTileScript>().PowerSourceObj = null;
        //dotTile.GetComponent<DotTileScript>().Power = 0;
        ResetConnectionPower();
        transform.position = MoveToPos;
        SetDotTile();
        //ResetPower();
        ResetConnectionPower();
    }
}
