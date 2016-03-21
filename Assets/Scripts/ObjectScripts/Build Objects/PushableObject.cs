﻿using UnityEngine;
using System.Collections;

public class PushableObject : SkinableObject {
    [Editable(true)]
    public float Weight = 1;

    private Vector3 defaultPos;

	// Use this for initialization
	void Start () {
        UpdateWeight();
        defaultPos = transform.position;
	}
    public void SetTransformToDefault()
    {
        transform.position = defaultPos;
    }
    public void UpdateWeight()
    {
        GetComponent<Rigidbody2D>().mass = Weight;
    }
    public override void Move(Vector3 MoveToPos)
    {
        //print(this);
        transform.position = MoveToPos;
        defaultPos = MoveToPos;
    }
    public override void ValueChanged(object sender, object value)
    {
        if (sender.ToString() == "System.Single Weight")
        {
            Weight = float.Parse(value.ToString());
            UpdateWeight();
        }
    }
}
