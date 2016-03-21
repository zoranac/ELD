using UnityEngine;
using System.Collections;

public class PlaceableObject : MonoBehaviour {
    public float mouseHoverAlpha = .75f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //TestIfChangeOpacity();
    }
    public virtual void Move(Vector3 MoveToPos)
    {
        print(this);
        transform.position = MoveToPos;
    }
    protected void TestIfChangeOpacity()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance = 3 + Input.mousePosition.x / 10;
        bool hit = false;
//        Collider Obj = new Collider();
        foreach (RaycastHit obj in Physics.RaycastAll(ray, distance))
        {
            if (obj.collider.gameObject == gameObject)
            {
                hit = true;
                break;
            }
        }
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            Color transparentColor = gameObject.GetComponent<SpriteRenderer>().color;
            if (hit)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(transparentColor.r, transparentColor.g, transparentColor.b, mouseHoverAlpha);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(transparentColor.r, transparentColor.g, transparentColor.b, 1f);
            }
        }
        if (gameObject.GetComponent<MeshRenderer>() != null)
        {
            Color transparentColor = gameObject.GetComponent<MeshRenderer>().material.color;  
            if (hit)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(transparentColor.r, transparentColor.g, transparentColor.b, mouseHoverAlpha);
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(transparentColor.r, transparentColor.g, transparentColor.b, 1f);
            }
        }
    }
}
