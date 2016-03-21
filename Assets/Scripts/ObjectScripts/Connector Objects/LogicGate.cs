using UnityEngine;
using System.Collections;

public class LogicGate : TempPowerOutput {
	//[Editable(true)]
    public enum GateType
    {
        AND,
        OR,
        XOR,
        NOT,
        NAND,
        NOR,
        XNOR
    };
    public Sprite[] SpritesOff = new Sprite[7]; //0 AND 1 OR 2 XOR 3 NOT 4 NAND 5 NOR 6 XNOR
    public Sprite[] SpritesOn = new Sprite[7];
	bool GateOpen = false;
    [Editable(true)]
    public GateType gateType = GateType.AND;
	// Update is called once per frame
	void Update () {
        switch (gateType)
        {
            case GateType.AND: TestANDGate();
                break;
            case GateType.OR: TestORGate();
                break;
            case GateType.XOR:  TestXORGate();
                break;
            case GateType.NOT: TestNOTGate();
                break;
            case GateType.NAND: TestNANDGate();
                break;
            case GateType.NOR: TestNORGate();
                break;
            case GateType.XNOR: TestXNORGate();
                break;
            default:
                break;
        }
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
	}
	void TestANDGate(){
		if (Inputs.Count >= 2 && Outputs.Count >= 1)
		{
			GateOpen = false;
			tempPowerOutput = 0;
			foreach(GameObject input in Inputs)
			{
				if(input.GetComponent<PowerLineScript>().Power <= 0)
				{
					foreach(GameObject output in Outputs)
					{
						output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput,gameObject,dotTile.GetComponent<DotTileScript>().PowerSourceObj);
					}
					GateOpen = false;
					return;
				}
			}
			tempPowerOutput = this.powerOutput;
			foreach(GameObject output in Outputs)
			{
				if (output.GetComponent<PowerLineScript>().Power == 0)
					output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput,gameObject,dotTile.GetComponent<DotTileScript>().PowerSourceObj);
			}
			GateOpen = true;
		}
		else
		{
			foreach(GameObject output in Outputs)
			{
				output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput,gameObject,dotTile.GetComponent<DotTileScript>().PowerSourceObj);
			}
			GateOpen = false;
		}
        if (GateOpen)
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOn[0];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOff[0];
        }
	}
    void TestXORGate()
    {
        if (Inputs.Count >= 2 && Outputs.Count >= 1)
        {
            GateOpen = false;
            int poweredCount = 0;
            tempPowerOutput = 0;
            foreach (GameObject input in Inputs)
            {
                if (input.GetComponent<PowerLineScript>().Power > 0)
                {
                    poweredCount++;
                }
            }
            if (poweredCount == 1 || poweredCount == 3)
            {
                tempPowerOutput = this.powerOutput;
                foreach (GameObject output in Outputs)
                {
                    if (output.GetComponent<PowerLineScript>().Power == 0)
                        output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = true;
            }
            else
            {
                tempPowerOutput = 0;
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = false;
            }
        }
        else
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
            GateOpen = false;
        }
        if (GateOpen)
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOn[2];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOff[2];
        }
    }
    void TestNANDGate()
    {
        if (Inputs.Count >= 2 && Outputs.Count >= 1)
        {
            GateOpen = false;
            int poweredCount = 0;
            tempPowerOutput = 0;
            foreach (GameObject input in Inputs)
            {
                if (input.GetComponent<PowerLineScript>().Power > 0)
                {
                    poweredCount++;
                }
            }
            if (poweredCount < Inputs.Count)
            {
                tempPowerOutput = this.powerOutput;
                foreach (GameObject output in Outputs)
                {
                    if (output.GetComponent<PowerLineScript>().Power == 0)
                        output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = true;
            }
            else
            {
                tempPowerOutput = 0;
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = false;
            }
        }
        else
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
            GateOpen = false;
        }
        if (GateOpen)
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOn[4];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOff[4];
        }
    }
    void TestXNORGate()
    {
        if (Inputs.Count >= 2 && Outputs.Count >= 1)
        {
            GateOpen = false;
            int poweredCount = 0;
            tempPowerOutput = 0;
            foreach (GameObject input in Inputs)
            {
                if (input.GetComponent<PowerLineScript>().Power > 0)
                {
                    poweredCount++;
                }
            }
            if (poweredCount == 2 || poweredCount == 0)
            {
                tempPowerOutput = this.powerOutput;
                foreach (GameObject output in Outputs)
                {
                    if (output.GetComponent<PowerLineScript>().Power == 0)
                        output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = true;
            }
            else
            {
                tempPowerOutput = 0;
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = false;
            }
        }
        else
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
            GateOpen = false;
        }
        if (GateOpen)
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOn[6];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOff[6];
        }
    }
    void TestNORGate()
    {
        if (Inputs.Count >= 2 && Outputs.Count >= 1)
        {
            GateOpen = false;
            int poweredCount = 0;
            tempPowerOutput = 0;
            foreach (GameObject input in Inputs)
            {
                if (input.GetComponent<PowerLineScript>().Power > 0)
                {
                    poweredCount++;
                }
            }
            if (poweredCount == 0)
            {
                tempPowerOutput = this.powerOutput;
                foreach (GameObject output in Outputs)
                {
                    if (output.GetComponent<PowerLineScript>().Power == 0)
                        output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = true;
            }
            else
            {
                tempPowerOutput = 0;
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = false;
            }
        }
        else
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
            GateOpen = false;
        }
        if (GateOpen)
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOn[5];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOff[5];
        }
    }
    void TestORGate()
    {
        if (Inputs.Count >= 2 && Outputs.Count >= 1)
        {
            GateOpen = false;
            int poweredCount = 0;
            tempPowerOutput = 0;
            foreach (GameObject input in Inputs)
            {
                if (input.GetComponent<PowerLineScript>().Power > 0)
                {
                    poweredCount++;
                }
            }
            if (poweredCount > 0)
            {
                tempPowerOutput = this.powerOutput;
                foreach (GameObject output in Outputs)
                {
                    if (output.GetComponent<PowerLineScript>().Power == 0)
                        output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = true;
            }
            else
            {
                tempPowerOutput = 0;
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = false;
            }
        }
        else
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
            GateOpen = false;
        }
        if (GateOpen)
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOn[1];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOff[1];
        }
    }
    void TestNOTGate()
    {
        if (Inputs.Count == 1 && Outputs.Count >= 1)
        {
            GateOpen = false;
            int poweredCount = 0;
            tempPowerOutput = 0;
            foreach (GameObject input in Inputs)
            {
                if (input.GetComponent<PowerLineScript>().Power > 0)
                {
                    poweredCount++;
                }
            }
            if (poweredCount == 0)
            {
                tempPowerOutput = this.powerOutput;
                foreach (GameObject output in Outputs)
                {
                    if (output.GetComponent<PowerLineScript>().Power == 0)
                        output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = true;
            }
            else
            {
                tempPowerOutput = 0;
                foreach (GameObject output in Outputs)
                {
                    output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
                GateOpen = false;
            }
        }
        else
        {
            foreach (GameObject output in Outputs)
            {
                output.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
            }
            GateOpen = false;
        }
        if (GateOpen)
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOn[3];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpritesOff[3];
        }
    }
	public override void ValueChanged(object sender, object value)
	{
        if (sender.ToString() == "System.Int32 powerOutput")
		{
			powerOutput = int.Parse(value.ToString());
			if (GateOpen)
			{
				foreach (GameObject obj in Outputs)
				{
					obj.GetComponent<PowerLineScript>().SetPower(powerOutput,gameObject,dotTile.GetComponent<DotTileScript>().PowerSourceObj);
				}
			}
		}
        print(sender.ToString());
        if (sender.ToString() == "LogicGate+GateType gateType")
        {
            switch (int.Parse(value.ToString()))
            {
                case 0: gateType = GateType.AND;
                    break;
                case 1: gateType = GateType.OR;
                    break;
                case 2: gateType = GateType.XOR;
                    break;
                case 3: gateType = GateType.NOT;
                    break;
                case 4: gateType = GateType.NAND;
                    break;
                case 5: gateType = GateType.NOR;
                    break;
                case 6: gateType = GateType.XNOR;
                    break;
                default:
                    break;
            }
        }
	}
}
