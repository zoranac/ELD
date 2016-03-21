using UnityEngine;
using System.Collections;

public class Friction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<Rigidbody2D>().velocity.magnitude > 0 && GetComponent<Rigidbody2D>().velocity.magnitude < GetComponent<Rigidbody2D>().mass)
        {
          //  print(GetComponent<Rigidbody2D>().velocity.magnitude);
            //GetComponent<Rigidbody2D>().velocity.Set(0, 0);
        }
	}
}
