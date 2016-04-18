using UnityEngine;
using System.Collections;

public class PressurePlate : ObjectRequiresConnection
{
    public bool On = false;
    
	// Update is called once per frame
    void Update()
    {

        TestIfCovered();
     
        if (On)
        {
            GetComponent<SpriteRenderer>().sprite = CurrentSkin.AllSpritesInSkin[1];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = CurrentSkin.AllSpritesInSkin[0];
        }
    }
    void TestIfCovered()
    {
 
        if (!dotTile.Powered)
        {
            dotTile.Blocked = true;
            return;
        }
     
        dotTile.Blocked = !On;
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<PushableObject>() != null || col.tag == "Player")
        {
            print("ASDASD");
            On = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<PushableObject>() != null || col.tag == "Player")
        {
            print("ASDASD");
            On = false;
        }
    }

}
