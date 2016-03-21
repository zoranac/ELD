using UnityEngine;
using System.Collections;

public class PoweredDoor : PoweredObject {
    public Sprite[] sprites = new Sprite[2];
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {

        TestEnable();

        if (!GetComponent<BoxCollider2D>().enabled)
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        else
            GetComponent<SpriteRenderer>().sprite = sprites[0];
	}
    void TestEnable()
    {
        bool enable = true;
        foreach (Collider2D col in Physics2D.OverlapAreaAll(transform.position + new Vector3(.25f, .25f), transform.position - new Vector3(.25f, .25f)))
        {
            if (col.tag == "Player")
            {
                enable = false;
            }
        }
        if (enable)
            GetComponent<BoxCollider2D>().enabled = !Powered;
    }
    public override void SetSkin(Skin skin)
    {
        GetComponent<SpriteRenderer>().sprite = skin.MainSprite;
        sprites[0] = skin.AllSpritesInSkin[0];
        sprites[1] = skin.AllSpritesInSkin[1];
    }

}
