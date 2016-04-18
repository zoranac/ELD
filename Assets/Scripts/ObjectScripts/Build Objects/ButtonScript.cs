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
        if (On)
        {
            GetComponent<SpriteRenderer>().sprite = CurrentSkin.AllSpritesInSkin[1];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = CurrentSkin.AllSpritesInSkin[0];
        }
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
    public override void SetActive(bool value)
    {
        base.SetActive(value);
        if (Connected)
        {
            GetComponentInChildren<ConnectorSwitch>().dotTile.GetComponent<DotTileScript>().ObjectOnMe = null;
            GetComponentInChildren<ConnectorSwitch>().dotTile.GetComponent<DotTileScript>().ObjectUnderMe = null;
        }
    }
    override public void Interact()
    {
        On = true;
        buttonPressedTime = Time.time;
        print("ButtonInteract");
    }
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
    {
        
        if (sender.ToString() == "System.Boolean On")
        {
            On = bool.Parse(value.ToString());
            buttonPressedTime = Time.time;
        }
        if (sender.ToString() == "System.Single WaitTime")
        {
            print(sender.ToString());
            if (AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<float>(gameObject, WaitTime, float.Parse(value.ToString()), sender);
            WaitTime = float.Parse(value.ToString());
        }
    }
}
