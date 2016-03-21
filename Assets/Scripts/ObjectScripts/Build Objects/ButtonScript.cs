using UnityEngine;
using System.Collections;

public class ButtonScript : SwitchScript
{
    [Editable(true)]
    public float WaitTime;

    private float buttonPressedTime=0;

    // Update is called once per frame
    void Update()
    {
        TestIfConnected(false);
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

        if (Time.time >= buttonPressedTime + WaitTime)
        {
            On = false;
        }
    }
    override public void Interact()
    {
        On = true;
        buttonPressedTime = Time.time;
        print("ButtonInteract");
    }
    public override void ValueChanged(object sender, object value)
    {
        
        if (sender.ToString() == "System.Boolean On")
        {
            On = bool.Parse(value.ToString());
            buttonPressedTime = Time.time;
        }
        if (sender.ToString() == "System.Single WaitTime")
        {
            print(sender.ToString());
            WaitTime = float.Parse(value.ToString());
        }
    }
}
