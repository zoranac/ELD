using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class IOObject : EditableObject
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };
    public GameObject dotTile;
    public ControlScript control;
    public List<GameObject> Inputs = new List<GameObject>();
    public List<GameObject> Outputs = new List<GameObject>();
	// Use this for initialization
	void Start () {
        SetDotTile();
        Move(gameObject.transform.position);
        HideDotTile();
        control = GameObject.Find("Control").GetComponent<ControlScript>();
	}
	
	// Update is called once per frame
    public void SetDotTile()
    {
        foreach (Collider2D col in Physics2D.OverlapPointAll(transform.position))
        {
            if (col.tag == "DotTile")
            {
                dotTile = col.gameObject;
                dotTile.GetComponent<DotTileScript>().ObjectOnMe = gameObject;
                break;
            }
        }
    }
    public void HideDotTile()
    {
        dotTile.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void ShowDotTile()
    {
        dotTile.GetComponent<SpriteRenderer>().enabled = true;
    }
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
    public override void ValueChanged(object sender, object value)
    {
    }
}
