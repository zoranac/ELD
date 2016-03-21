using UnityEngine;
using System.Collections;

public class PressurePlate : ObjectRequiresConnection
{
    public bool On = false;
    
	// Update is called once per frame
    void Update()
    {

        TestIfCovered();

    }
    void TestIfCovered()
    {
        On = false;
        foreach (Collider2D col in Physics2D.OverlapAreaAll(transform.position + new Vector3(.225f, .225f), transform.position + new Vector3(-.225f, -.225f)))
        {
            if (col.GetComponent<PushableObject>() != null || col.tag == "Player")
            {
                On = true;
                break;
            }
        }
        dotTile.Blocked = !On;
    }

}
