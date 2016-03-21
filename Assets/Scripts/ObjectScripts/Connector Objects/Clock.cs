using UnityEngine;
using System.Collections;

public class Clock : TempPowerOutput {
    
    [Editable(true)]
    public float Frequency = 1;
    private float poweredTime = 0;
    private bool powered = false;
	// Update is called once per frame
	void Update () {
        if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
            HideDotTile();

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
        TestFrequency();
	}

    private void TestFrequency()
    {
         int power = 0;
        foreach (GameObject input in Inputs)
        {
            if (input.GetComponent<PowerLineScript>().Power > 0)
            {
                power = input.GetComponent<PowerLineScript>().Power;
                break;
            }
        }
        if (power > 0)
        {
            if (Time.time >= poweredTime + Frequency)
            {
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(powerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }

                if (powered)
                {
                    poweredTime = Time.time;
                    powered = false;
                }else{
                    powered = true;
                }

            }
            else
            {
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
            }
        }
        else
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
        }
    }
    public override void ValueChanged(object sender, object value)
    {

        if (sender.ToString() == "System.Int32 powerOutput")
        {
            powerOutput = int.Parse(value.ToString());
            foreach (GameObject obj in Outputs)
            {
                obj.GetComponent<PowerLineScript>().SetPower(powerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
        }

        if (sender.ToString() == "System.Single Frequency")
        {
            Frequency = float.Parse(value.ToString());
        }
    }


}
