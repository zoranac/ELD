using UnityEngine;
using System.Collections;
public class InteractableObject : SkinableObject {
    public bool AttachedToGenerator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	virtual public void Interact()
	{

	}
    public override void ValueChanged(object field, object value, bool AddToUndoList)
    {

    }
}
