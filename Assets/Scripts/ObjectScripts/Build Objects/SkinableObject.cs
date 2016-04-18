using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SkinableObject : EditableObject {

    public List<Skin> Skins = new List<Skin>();
    public Skin CurrentSkin;
    public override void ValueChanged(object field, object value, bool AddToUndoList)
    {

    }
    public virtual void SetSkin(Skin skin)
    {
        CurrentSkin = skin;
        GetComponent<SpriteRenderer>().sprite = skin.MainSprite;
    }
}
