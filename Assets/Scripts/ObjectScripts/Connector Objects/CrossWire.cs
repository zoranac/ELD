using UnityEngine;
using System.Collections;

public class CrossWire : IOObject
{
    bool outputUP = false;
    bool outputDOWN = false;
    bool outputLEFT = false;
    bool outputRIGHT = false;
    int NSPower = 0;
    int EWPower = 0;
    GameObject NSSource = null;
    GameObject EWSource = null;
    public Sprite[] Sprites = new Sprite[4]; //0 off 1 NS 2 EW 3 NSEW
	// Use this for initialization
    void Update()
    {
        ContinuePower();
        if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
            HideDotTile();
    }
    void ContinuePower()
    {
        outputUP = false;
        outputDOWN = false;
        outputLEFT = false;
        outputRIGHT = false;
        NSPower = 0;
        EWPower = 0;
        NSSource = null;
        EWSource = null;
        foreach (GameObject input in Inputs)
        {
            //Input UP
            if (input.transform.position.y > dotTile.transform.position.y)
            {
                outputDOWN = true;
                NSPower = input.GetComponent<PowerLineScript>().Power - 1;
                NSSource = input.GetComponent<PowerLineScript>().PowerSourceObj;
            }
            //Input Down
            else if (input.transform.position.y < dotTile.transform.position.y)
            {
                outputUP = true;
                NSPower = input.GetComponent<PowerLineScript>().Power - 1;
                NSSource = input.GetComponent<PowerLineScript>().PowerSourceObj;
            }
            //Input Left
            else if (input.transform.position.x < dotTile.transform.position.x)
            {
                outputRIGHT = true;
                EWPower = input.GetComponent<PowerLineScript>().Power - 1;
                EWSource = input.GetComponent<PowerLineScript>().PowerSourceObj;
            }
            //Input right
            else if (input.transform.position.x > dotTile.transform.position.x)
            {
                outputLEFT = true;
                EWPower = input.GetComponent<PowerLineScript>().Power - 1;
                EWSource = input.GetComponent<PowerLineScript>().PowerSourceObj;
            }
        }
        GetComponent<SpriteRenderer>().sprite = Sprites[0];
        foreach (GameObject output in Outputs)
        {
            //Output UP
            if (output.transform.position.y > dotTile.transform.position.y)
            {
                if (outputUP)
                {
                    output.GetComponent<PowerLineScript>().SetPower(NSPower, gameObject, NSSource);
                    if (NSPower > 0)
                        GetComponent<SpriteRenderer>().sprite = Sprites[1];
                }
            }
            //Output Down
            else if (output.transform.position.y < dotTile.transform.position.y)
            {
                if (outputDOWN)
                {
                    output.GetComponent<PowerLineScript>().SetPower(NSPower, gameObject, NSSource);
                    if (NSPower > 0)
                        GetComponent<SpriteRenderer>().sprite = Sprites[1];
                }
            }
            //Output Left
            else if (output.transform.position.x < dotTile.transform.position.x)
            {
                if (outputLEFT)
                {
                    output.GetComponent<PowerLineScript>().SetPower(EWPower, gameObject, EWSource);
                    if (EWPower > 0)
                        GetComponent<SpriteRenderer>().sprite = Sprites[2];
                }
            }
            //Output right
            else if (output.transform.position.x > dotTile.transform.position.x)
            {
                if (outputRIGHT)
                {
                    output.GetComponent<PowerLineScript>().SetPower(EWPower, gameObject, EWSource);
                    if (EWPower > 0)
                        GetComponent<SpriteRenderer>().sprite = Sprites[2];
                }
            }
        }
        if (NSPower <= 0 && EWPower <= 0 || (EWPower > 0 && GetComponent<SpriteRenderer>().sprite != Sprites[2]) || (NSPower > 0 && GetComponent<SpriteRenderer>().sprite != Sprites[1]))
        {
            GetComponent<SpriteRenderer>().sprite = Sprites[0];
        }
        if (NSPower > 0 && EWPower > 0)
        {
            GetComponent<SpriteRenderer>().sprite = Sprites[3];
        }
        //print("NS POWER: " + NSPower + " EW POWER: " + EWPower);
    }
}
