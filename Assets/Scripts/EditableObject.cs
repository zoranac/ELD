using UnityEngine;
using System.Collections;
using System.Reflection;

public abstract class EditableObject : PlaceableObject{

	public abstract void ValueChanged(object field, object value, bool addToUndoList);

    public override void Move(Vector3 MoveToPos)
    {
        print(this);
        transform.position = MoveToPos;
    }
}
