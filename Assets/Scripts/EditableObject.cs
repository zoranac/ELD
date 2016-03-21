using UnityEngine;
using System.Collections;
using System.Reflection;

public abstract class EditableObject : PlaceableObject{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public abstract void ValueChanged(object field, object value);

    public override void Move(Vector3 MoveToPos)
    {
        print(this);
        transform.position = MoveToPos;
    }
}
