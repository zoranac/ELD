using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DotTileScript : MonoBehaviour {
	public GameObject TileAbove;
	public GameObject TileBelow;
	public GameObject TileLeft;
	public GameObject TileRight;
	public List<GameObject> Connections = new List<GameObject>();
	bool Tested = false;
	public bool Powered = false;
	public int Power = 0;
	public GameObject PowerSourceObj;
	public GameObject ObjectOnMe;
    public GameObject ObjectUnderMe;
    private SpriteRenderer sr;
    private Color TestColor;
    public bool Blocked = false;
    public GameObject ConnectionRing;
	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        TestColor = new Color32((byte)125, (byte)125, (byte)100, (byte)255);
	}
	void TestIfPowered(){

		int highestPower = 0;

		if (ObjectOnMe != null && 
		    ObjectOnMe.GetComponent<PowerOutput>() != null && 
		    ObjectOnMe.GetComponent<PowerOutput>().powerOutput > 0
		    )
		{
			highestPower = ObjectOnMe.GetComponent<PowerOutput>().powerOutput;
		}
		foreach(GameObject obj in Connections)
		{
           
            if ( obj.GetComponent<PowerLineScript>().Power > highestPower)
			{
                highestPower = obj.GetComponent<PowerLineScript>().Power;
                PowerSourceObj = obj.GetComponent<PowerLineScript>().PowerSourceObj;
			}
		}
		Power = highestPower;

		if (ObjectOnMe != null && 
		    ObjectOnMe.GetComponent<TempPowerOutput>() != null)
		{
			if (PowerSourceObj == null)
				Power = 0;
			if (ObjectOnMe.GetComponent<TempPowerOutput>().tempPowerOutput == 0)
				Power = 0;
		}


//		else if (ObjectOnMe != null && 
//		         ObjectOnMe.GetComponent<TempPowerOutput>() != null && 
//		         ObjectOnMe.GetComponent<TempPowerOutput>().tempPowerOutput > highestPower)
//		{
//			Power = ObjectOnMe.GetComponent<TempPowerOutput>().tempPowerOutput;
//		}

		if (Power > 0 && !Powered)
		{
			Powered = true;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, GetComponent<SpriteRenderer>().color.a);
		}
		else if (Power <= 0)
		{
			Powered = false;
            GetComponent<SpriteRenderer>().color = new Color(.1f, .1f, .1f, GetComponent<SpriteRenderer>().color.a);
		}
	}
	// Update is called once per frame
	void Update () {
        if (!Blocked)
        {
            if (Connections.Count > 0 || ObjectOnMe != null)
            {
                TestIfPowered();
            }
            else if (sr.color != TestColor)
            {
                sr.color = TestColor;
                Powered = false;
                Power = 0;
            }
        }
        else
        {
            Power = 0;
            Powered = false;
            PowerSourceObj = null;
        }

        if (ControlScript.CurrentMode == ControlScript.Mode.Connect && ObjectOnMe == null)
        {
            if (sr.enabled == false)
                sr.enabled = true;
        }
        if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
        {
             if (ObjectUnderMe != null && !ConnectionRing.active)
             {
                 ConnectionRing.SetActive(true);
             }
             else if (ObjectUnderMe == null && ConnectionRing.active)
             {
                 ConnectionRing.SetActive(false);
             }
        }
        else if (ConnectionRing.active)
        {
            ConnectionRing.SetActive(false);
        }
		if (!Tested)
		{
            Test();
        }
			
	}
    void Test()
    {
        foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, Vector2.up, .5f))
        {
            if (ray.collider.gameObject != this.gameObject)
            {
                if (ray.collider.tag == "DotTile")
                {
                    TileAbove = ray.collider.gameObject;
                    break;
                }
            }
        }
        foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, Vector2.down, .5f))
        {
            if (ray.collider.gameObject != this.gameObject)
            {
                if (ray.collider.tag == "DotTile")
                {
                    TileBelow = ray.collider.gameObject;
                    break;
                }
            }
        }
        foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, Vector2.left, .5f))
        {
            if (ray.collider.gameObject != this.gameObject)
            {
                if (ray.collider.tag == "DotTile")
                {
                    TileLeft = ray.collider.gameObject;
                    break;
                }
            }
        }
        foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, Vector2.right, .5f))
        {
            if (ray.collider.gameObject != this.gameObject)
            {
                if (ray.collider.tag == "DotTile")
                {
                    TileRight = ray.collider.gameObject;
                    break;
                }
            }
        }
        Tested = true;
    }
}
