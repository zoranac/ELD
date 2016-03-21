using UnityEngine;
using System.Collections;

public class ObjectRequiresConnection : SkinableObject
{
    public DotTileScript dotTile;
    void Start()
    {
        GetDotTile();
    }
    public void GetDotTile()
    {
        foreach (Collider2D col in Physics2D.OverlapPointAll(transform.position))
        {
            if (col.tag == "DotTile")
            {
                dotTile = col.gameObject.GetComponent<DotTileScript>();
                dotTile.ObjectUnderMe = gameObject;
            }
        }
    }
	// Use this for initialization
    public override void Move(Vector3 MoveToPos)
    {
        //print(this);
        dotTile.ObjectUnderMe = null;
        transform.position = MoveToPos;
        GetDotTile();
    }

	
}
