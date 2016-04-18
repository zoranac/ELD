using UnityEngine;
using System.Collections;

public class PowerDelay : TempPowerOutput {

    [Editable(true)]
    public float DelayAmount = 1;

    private float poweredTime = 0;
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
        TestDelay();
	}

    private void TestDelay()
    {
        if (poweredTime == 0)
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }

            foreach (GameObject input in Inputs)
            {
                if (input.GetComponent<PowerLineScript>().Power > 0)
                {
                    poweredTime = Time.time;
                    break;
                }
            }
        }
        else
        {
            if (Inputs.Count > 0)
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
                if (power == 0)
                {
                    poweredTime = 0;
                }
            }
            else
            {
                poweredTime = 0;
            }

            if (Time.time >= poweredTime + DelayAmount)
            {
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(powerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
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
    }
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
    {

        if (sender.ToString() == "System.Int32 powerOutput")
        {
            //Set value in undo handler
            if(AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<int>(gameObject, powerOutput, int.Parse(value.ToString()), sender);

            //Set value
            powerOutput = int.Parse(value.ToString());
            foreach (GameObject obj in Outputs)
            {
                obj.GetComponent<PowerLineScript>().SetPower(powerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
        }

        if (sender.ToString() == "System.Single DelayAmount")
        {
            //Set value in undo handler
            if(AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<float>(gameObject, DelayAmount, float.Parse(value.ToString()), sender);

            //Set value
            DelayAmount = float.Parse(value.ToString());
        }
    }
}
