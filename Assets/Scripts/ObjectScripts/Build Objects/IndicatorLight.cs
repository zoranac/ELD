using UnityEngine;
using System.Collections;

public class IndicatorLight : ObjectRequiresConnection {
    public Sprite[] sprites = new Sprite[2];
	
	// Update is called once per frame
	void Update () {
	    if (!dotTile.Powered)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }else
        {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
	}
    public override void SetSkin(Skin skin)
    {
        GetComponent<SpriteRenderer>().sprite = skin.MainSprite;
        sprites[0] = skin.AllSpritesInSkin[0];
        sprites[1] = skin.AllSpritesInSkin[1];
    }
}
