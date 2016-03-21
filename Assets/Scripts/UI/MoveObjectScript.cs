using UnityEngine;
using System.Collections;

public class MoveObjectScript : MonoBehaviour {
    public GameObject MoveObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void SetMoveObject(GameObject obj)
    {
        MoveObject = obj;
    }
    public void PlaceMoveObject(Vector3 MoveToPos)
    {
        if (MoveObject.GetComponent<PlaceableObject>() != null)
        {
            MoveObject.GetComponent<PlaceableObject>().Move(MoveToPos);
            MoveObject = null;
        }
        else if (MoveObject.tag == "Player")
        {
            MoveObject.transform.position = MoveToPos;
            MoveObject.GetComponent<SpriteRenderer>().color = new Color(MoveObject.GetComponent<SpriteRenderer>().color.r, 
                MoveObject.GetComponent<SpriteRenderer>().color.g, 
                MoveObject.GetComponent<SpriteRenderer>().color.b, 
                MoveObject.GetComponent<SpriteRenderer>().color.a * 2);
            MoveObject = null;
        }
    }
}
