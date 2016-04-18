using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class IOObject : PassiveConnectorObject
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };
   
    public List<GameObject> Inputs = new List<GameObject>();
    public List<GameObject> Outputs = new List<GameObject>();

    public void ResetIO()
    {
        Inputs.Clear();
        Outputs.Clear();
    }
    public override void Move(Vector3 MoveToPos)
    {
        //ResetPower();
        ResetIO();
        ShowDotTile();
        dotTile.GetComponent<DotTileScript>().ObjectOnMe = null;
        //dotTile.GetComponent<DotTileScript>().PowerSourceObj = null;
        //dotTile.GetComponent<DotTileScript>().Power = 0;
        transform.position = MoveToPos;
        SetDotTile();
    }
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
    {
    }
}
