using UnityEngine;
using System.Collections;

public class PoweredTrapDoorScript : PoweredObject {
    public Sprite[] sprites = new Sprite[2];
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Powered)
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        else
            GetComponent<SpriteRenderer>().sprite = sprites[0];
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && !Powered)
        {
            col.GetComponent<PlayerScript>().DestroyPlayer();
        }
    }
    public override void SetSkin(Skin skin)
    {
        GetComponent<SpriteRenderer>().sprite = skin.MainSprite;
        sprites[0] = skin.AllSpritesInSkin[0];
        sprites[1] = skin.AllSpritesInSkin[1];
    }
}
