using UnityEngine;
using System.Collections;

public class Resistor : IOObject {
    [Editable(true)]
    public int Resistance;
    private int outputPower;
    private GameObject powersource;
    void Start()
    {
        SetDotTile();
        Move(gameObject.transform.position);
        HideDotTile();
        control = GameObject.Find("Control").GetComponent<ControlScript>();
    }
	// Update is called once per frame
	void Update () {
        Resist();
        if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
            HideDotTile();
	}
    void GetHighestPower()
    {
        int highestpower = 0;
        foreach (GameObject input in Inputs)
        {
            if (input.GetComponent<PowerLineScript>().Power > highestpower)
            {
                highestpower = input.GetComponent<PowerLineScript>().Power;
                powersource = input.GetComponent<PowerLineScript>().PowerSourceObj;
            }
        }
        outputPower = highestpower - Resistance -1;
        if (outputPower < 0)
            outputPower = 0;
    }
    void Resist()
    {
        GetHighestPower();
        foreach(GameObject output in Outputs)
        {
            output.GetComponent<PowerLineScript>().SetPower(outputPower,gameObject,powersource);
        }
    }
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
    {
        //System.Boolean
        //System.Int32
        if (sender.ToString() == "System.Int32 Resistance")
        {
            //Set value in undo handler
            if(AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<int>(gameObject, Resistance, int.Parse(value.ToString()), sender);

            //Set value
            Resistance = int.Parse(value.ToString());
        }
    }
}
