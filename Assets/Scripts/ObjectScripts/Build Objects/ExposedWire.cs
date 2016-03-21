using UnityEngine;
using System.Collections;

public class ExposedWire : ObjectRequiresConnection {
    public bool blocked = false;
   // DotTileScript dotTile;
	// Use this for initialization
	// Update is called once per frame
	void Update () {
        TestIfBlocked();
	}
    void TestIfBlocked()
    {
        blocked = false;
        foreach (Collider2D col in Physics2D.OverlapAreaAll(transform.position + new Vector3(.15f, .15f), transform.position + new Vector3(-.15f, -.15f)))
        {
            if (col.GetComponent<PushableObject>() != null)
            {
                blocked = true;
                
            }
        }
        dotTile.Blocked = blocked;
    }
}
