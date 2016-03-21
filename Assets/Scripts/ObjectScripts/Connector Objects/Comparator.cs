using UnityEngine;
using System.Collections;

public class Comparator : TempPowerOutput {

    [Editable(true)]
    public Direction GreaterPowerInput = Direction.Up;

    public bool inputUP = false;
    public bool inputDOWN = false;
    public bool inputLEFT = false;
    public bool inputRIGHT = false;

    bool powerOutputs = false;
    bool greaterPowerInputSet = false;
	// Update is called once per frame
	void Update () {

        Compare();

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
	}

    void Compare()
    {
        if ((GreaterPowerInput == Direction.Up && !inputUP) ||
            (GreaterPowerInput == Direction.Down && !inputDOWN) ||
            (GreaterPowerInput == Direction.Left && !inputLEFT) ||
            (GreaterPowerInput == Direction.Right && !inputRIGHT))
        {
            greaterPowerInputSet = false;
        }

        inputUP = false;
        inputDOWN = false;
        inputLEFT = false;
        inputRIGHT = false;

        int vplusPower = 0;
        int vminusPower = 0;

        


        if (!greaterPowerInputSet)
        {
            GreaterPowerInput = Direction.Right;
        }

        foreach (GameObject input in Inputs)
        {
            //Input UP
            if (input.transform.position.y > dotTile.transform.position.y)
            {
               inputUP  = true;
                if (!greaterPowerInputSet)
                {
                    GreaterPowerInput = Direction.Up;
                }
            }
            //Input Down
            else if (input.transform.position.y < dotTile.transform.position.y)
            {
                inputDOWN = true;
                if (!greaterPowerInputSet && (GreaterPowerInput == Direction.Right || GreaterPowerInput == Direction.Left))
                {
                    GreaterPowerInput = Direction.Down;
                }
            }
            //Input Left
            else if (input.transform.position.x < dotTile.transform.position.x)
            {
               inputLEFT = true;
               if (!greaterPowerInputSet && (GreaterPowerInput == Direction.Right))
               {
                   GreaterPowerInput = Direction.Left;
               }
            }
            //Input right
            else if (input.transform.position.x > dotTile.transform.position.x)
            {
                inputRIGHT = true;
            }
        }

        if (Inputs.Count >= 2 && Outputs.Count >= 1)
        {
            foreach (GameObject input in Inputs)
            {
                //Input UP
                if (input.transform.position.y > dotTile.transform.position.y)
                {

                  
                    if (GreaterPowerInput == Direction.Up)
                    {
                        vplusPower = input.GetComponent<PowerLineScript>().Power;
                    }
                    else
                    {
                        if (input.GetComponent<PowerLineScript>().Power > vminusPower)
                        {
                            vminusPower=input.GetComponent<PowerLineScript>().Power;
                        }
                    }
                   
                }
                //Input Down
                else if (input.transform.position.y < dotTile.transform.position.y)
                {           
                    if (GreaterPowerInput == Direction.Down)
                    {
                        vplusPower = input.GetComponent<PowerLineScript>().Power;
                    }
                    else
                    {
                        if (input.GetComponent<PowerLineScript>().Power > vminusPower)
                        {
                            vminusPower=input.GetComponent<PowerLineScript>().Power;
                        }
                    }
                   
                }
                //Input Left
                else if (input.transform.position.x < dotTile.transform.position.x)
                {
                    if (GreaterPowerInput == Direction.Left)
                    {
                        vplusPower = input.GetComponent<PowerLineScript>().Power;
                    }
                    else
                    {
                        if (input.GetComponent<PowerLineScript>().Power > vminusPower)
                        {
                            vminusPower=input.GetComponent<PowerLineScript>().Power;
                        }
                    }
                   
                }
                //Input right
                else if (input.transform.position.x > dotTile.transform.position.x)
                {
                     if (GreaterPowerInput == Direction.Right)
                    {
                        vplusPower = input.GetComponent<PowerLineScript>().Power;
                    }
                    else
                    {
                        if (input.GetComponent<PowerLineScript>().Power > vminusPower)
                        {
                            vminusPower = input.GetComponent<PowerLineScript>().Power;
                        }
                    }
                    
                }
            }
            if (vplusPower > vminusPower)
            {
                powerOutputs = true;
                foreach (GameObject obj in Outputs)
                {
                    obj.GetComponent<PowerLineScript>().SetPower(powerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
            }
            else
            {
                powerOutputs = false;
                foreach (GameObject obj in Outputs)
                {
                    obj.GetComponent<PowerLineScript>().SetPower(tempPowerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
            }

           
        }
    }

    public override void ValueChanged(object sender, object value)
    {
        if (sender.ToString() == "System.Int32 powerOutput")
        {
            powerOutput = int.Parse(value.ToString());
            if (powerOutputs)
            {
                foreach (GameObject obj in Outputs)
                {
                    obj.GetComponent<PowerLineScript>().SetPower(powerOutput, gameObject, dotTile.GetComponent<DotTileScript>().PowerSourceObj);
                }
            }
        }
        print(sender.ToString());
        if (sender.ToString() == "IOObject+Direction GreaterPowerInput")
        {
            greaterPowerInputSet = true;
            switch (int.Parse(value.ToString()))
            {
                case 0: 
                    if (inputUP)
                        GreaterPowerInput = Direction.Up;
                    else if (inputDOWN)
                        GreaterPowerInput = Direction.Down;
                    else if (inputLEFT)
                        GreaterPowerInput = Direction.Left;
                    else if (inputRIGHT)
                        GreaterPowerInput = Direction.Right;
                    break;
                case 1:
                    if (inputUP && inputDOWN)
                        GreaterPowerInput = Direction.Down;
                    else if ((inputUP || inputDOWN) && inputLEFT)
                        GreaterPowerInput = Direction.Left;
                    else if (inputRIGHT)
                        GreaterPowerInput = Direction.Right;
                    break;
                case 2:
                    if (inputUP && inputDOWN && inputLEFT)
                        GreaterPowerInput = Direction.Left;
                    else if (inputRIGHT)
                        GreaterPowerInput = Direction.Right;
                    break;
                case 3:
                    if (inputUP && inputDOWN && inputLEFT && inputRIGHT)
                        GreaterPowerInput = Direction.Right;
                    break;
                default:
                    break;
            }
        }
    }
}
