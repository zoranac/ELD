using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Skin : MonoBehaviour {
    public Sprite MainSprite;
    public List<Sprite> AllSpritesInSkin = new List<Sprite>();
	// Use this for initialization
	void Awake () {
        AllSpritesInSkin.Insert(0, MainSprite);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
