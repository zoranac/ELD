using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GeneratorScript : PlaceableObject {
	public int PowerOutput;
	public List<GameObject> Connections;
	// Use this for initialization
	void Start () {
	
	}
    void Update()
    {
        TestIfChangeOpacity();
        OutputPower();
    }
    void OutputPower()
    {

    }
}
