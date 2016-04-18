using UnityEngine;
using System.Collections;

public class PassiveConnectorObject : EditableObject
{
    public GameObject dotTile;
    public ControlScript control;
	// Use this for initialization
	void Start () {
        SetDotTile();
        Move(gameObject.transform.position);
        HideDotTile();
        control = GameObject.Find("Control").GetComponent<ControlScript>();
	}
    void OnEnabled()
    {
        SetDotTile();
        HideDotTile();
    }
    public void HideDotTile()
    {
        dotTile.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void ShowDotTile()
    {
        dotTile.GetComponent<SpriteRenderer>().enabled = true;
    }
   public override void SetActive(bool value)
    {
        if (!value)
        {
            ShowDotTile();
            dotTile.GetComponent<DotTileScript>().ObjectOnMe = null;
        }
        else
        {

        }
        gameObject.SetActive(value);
        print("activate");
    }
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
    public override void Move(Vector3 MoveToPos)
    {
        //ResetPower();
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
