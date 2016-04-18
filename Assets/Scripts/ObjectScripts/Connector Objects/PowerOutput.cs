using UnityEngine;
using System.Collections;
using System.Reflection;
public class PowerOutput : EditableObject {
    public static int MaxPower = 10;
	[Editable(true)]
	public int powerOutput;
    GameObject dotTile;
    public void Start()
    {
        SetDotTile();
    }
	public void OnEnable(){
        SetDotTile();
	}
    public override void SetActive(bool value){
        if (!value)
        {
            RemoveFromDotTile();
        }
        else
        {

        }
        gameObject.SetActive(value);
        print("activate");
    }
    public void RemoveFromDotTile()
    {
        dotTile.GetComponent<DotTileScript>().ObjectOnMe = null;
        dotTile.GetComponent<DotTileScript>().PowerSourceObj = null;
        dotTile.GetComponent<DotTileScript>().Power = 0;
        foreach (GameObject obj in dotTile.GetComponent<DotTileScript>().Connections)
        {
            obj.GetComponent<PowerLineScript>().Power = 0;
            obj.GetComponent<PowerLineScript>().highestPowerObj = null;
            obj.GetComponent<PowerLineScript>().PowerSourceObj = null;
        }
    }
    void SetDotTile()
    {
        foreach (Collider2D col in Physics2D.OverlapPointAll(transform.position))
        {
            if (col.tag == "DotTile")
            {
                col.gameObject.GetComponent<DotTileScript>().ObjectOnMe = gameObject;
                dotTile = col.gameObject;
                
                break;
            }
        }
    }
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
	{
		print (sender.ToString());
		if (sender.ToString() == "System.Int32 powerOutput")
		{
            //Set value in undo handler
            if(AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<int>(gameObject, powerOutput, int.Parse(value.ToString()), sender);

            //Set value
			powerOutput = int.Parse(value.ToString());
		}
	}
    public override void Move(Vector3 MoveToPos)
    {
        print("SET POWER OUTPUT NODE");
        RemoveFromDotTile();
        transform.position = MoveToPos;
        SetDotTile();
    }
}
