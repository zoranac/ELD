using UnityEngine;
using System.Collections;

public class PoweredObject : ObjectRequiresConnection
{
    [Editable(true)]
    public bool StartPowered = false;
    public bool Powered = false;

	// Update is called once per frame
   
	void FixedUpdate () {
		TestIfPowered();
	}
   
    
	void TestIfPowered(){
        if (StartPowered)
			Powered = !dotTile.Powered;
        else
            Powered = dotTile.Powered;
	}
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
    {
        //Set value in undo handler
        if (AddToUndoList)
            UndoHandlerWebGL.instance.OnValueChanged<bool>(gameObject, StartPowered, bool.Parse(value.ToString()), sender);

        //Set value
        if (sender.ToString() == "System.Boolean StartPowered")
        {
            StartPowered = bool.Parse(value.ToString());
        }
    }
}
