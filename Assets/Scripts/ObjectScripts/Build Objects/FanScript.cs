using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum FacingDirection
{
    Up,
    Down,
    Left,
    Right
};
public class FanScript : ObjectRequiresConnection {
    [Editable(true)]
    public FacingDirection Direction = FacingDirection.Up;
    public GameObject particles;
    bool particlesSet = false;
	// Update is called once per frame
	void Update () {
        if (ControlScript.CurrentMode == ControlScript.Mode.Play)
        {
            PowerFan();
            ShowParticles();
        }
        else
        {
            particles.SetActive(false);
        }
	}
    void ShowParticles()
    {
        if (dotTile.Powered)
        {
            particles.SetActive(true);

            if (!particlesSet)
            {
                switch (Direction)
                {
                    case FacingDirection.Up: particles.transform.rotation = Quaternion.Euler(new Vector3(270,90,90));
                        break;
                    case FacingDirection.Down: particles.transform.rotation = Quaternion.Euler(new Vector3(90, 90, 90));
                        break;
                    case FacingDirection.Left: particles.transform.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
                        break;
                    case FacingDirection.Right: particles.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 90));
                        break;
                    default:
                        break;
                }
                particlesSet = true;
            }
        }
        else
        {
            particles.SetActive(false);
        }
    }
    void PowerFan()
    {
        if (dotTile.Powered)
        {
            Vector2 vect = Vector2.zero;
            Vector2 center = Vector2.zero;
            Vector2 shape = Vector2.zero;
            switch (Direction)
            {
                case FacingDirection.Up: vect = Vector2.up;
                    center = new Vector2(transform.position.x, transform.position.y + (float)dotTile.Power / 16);
                    shape = new Vector2(.4f, (float)dotTile.Power / 8);
                    break;
                case FacingDirection.Down: vect = Vector2.down;
                    center = new Vector2(transform.position.x, transform.position.y - (float)dotTile.Power / 16);
                    shape = new Vector2(.4f, (float)dotTile.Power / 8);
                    break;
                case FacingDirection.Left: vect = Vector2.left;
                    center = new Vector2(transform.position.x - (float)dotTile.Power / 16, transform.position.y);
                    shape = new Vector2((float)dotTile.Power / 8, .4f);
                    break;
                case FacingDirection.Right: vect = Vector2.right;
                    center = new Vector2(transform.position.x + (float)dotTile.Power / 16, transform.position.y);
                    shape = new Vector2( (float)dotTile.Power / 8,.4f);
                    break;
                default: vect = Vector2.zero;
                    break;
            }

            GameObject wall = null;
            List<GameObject> pushObjects = new List<GameObject>();

            foreach (RaycastHit2D rayhit in Physics2D.BoxCastAll(center, shape, 0, vect))
            {
               
                if (rayhit.collider.tag == "Wall")
                {
                    wall = rayhit.collider.gameObject;
                }
                if (rayhit.collider.GetComponent<PushableObject>() || rayhit.collider.tag == "Player")
                {
                    pushObjects.Add(rayhit.collider.gameObject);
                    //print((vect * 4 * dist).magnitude);
                  
                }
            }
            foreach(GameObject obj in pushObjects)
            {
                float dist = Vector3.Distance(obj.transform.position, transform.position);
                if (dist != 0)
                {
                    dist = 1 / dist;
                }

                if (wall != null)
                {
                    print(wall);
                    //up
                    if (Direction == FacingDirection.Up)
                    {
                        if (obj.transform.position.y < wall.transform.position.y && obj.transform.position.y > transform.position.y)
                        {
                            addForceToObject(obj, vect, wall, dist);
                        }
                    }
                    //down
                    else if (Direction == FacingDirection.Down)
                    {
                        if (obj.transform.position.y > wall.transform.position.y && obj.transform.position.y < transform.position.y)
                        {
                            addForceToObject(obj, vect, wall, dist);
                        }
                    }
                    //left
                    else if (Direction == FacingDirection.Left)
                    {
                        if (obj.transform.position.x > wall.transform.position.x && obj.transform.position.x < transform.position.x)
                        {
                            addForceToObject(obj, vect, wall, dist);
                        }
                    }
                    //right
                    else if (Direction == FacingDirection.Right)
                    {
                        if (obj.transform.position.x < wall.transform.position.x && obj.transform.position.x > transform.position.x)
                        {
                            addForceToObject(obj, vect, wall, dist);
                        }
                    }
                }
                else
                {
                    print("no Wall");
                    if ((vect * 4 * dist).magnitude > 2 + (obj.GetComponent<Rigidbody2D>().mass / 10))
                    {
                        print("Power");
                        obj.GetComponent<Rigidbody2D>().AddForce((vect * 4 * dist * ((float)dotTile.Power / 8)), ForceMode2D.Force);
                    }
                }
            }
        }
    }
    void addForceToObject(GameObject obj, Vector2 vect, GameObject wall, float dist)
    {
        print("Power");
        if ((vect * 4 * dist).magnitude > 2 + (obj.GetComponent<Rigidbody2D>().mass / 10))
        {
            print("Power");
            obj.GetComponent<Rigidbody2D>().AddForce((vect * 4 * dist * ((float)dotTile.Power / 8)), ForceMode2D.Force);
        }
    }
    public override void ValueChanged(object sender, object value, bool AddToUndoList)
    {
        if (sender.ToString() == "FacingDirection Direction")
        {
            int dirValue = 0;
            switch (Direction)
	        {
		    case FacingDirection.Up:dirValue = 0;
             break;
            case FacingDirection.Down:dirValue = 1;
             break;
            case FacingDirection.Left:dirValue = 2;
             break;
            case FacingDirection.Right:dirValue = 3;
             break;
            default:
             break;
	        }

            //Set value in undo handler
            if (AddToUndoList)
                UndoHandlerWebGL.instance.OnValueChanged<int>(gameObject, dirValue, int.Parse(value.ToString()), sender);

            //Set value
            switch (int.Parse(value.ToString()))
            {
                case 0: Direction = FacingDirection.Up;
                    break;
                case 1: Direction = FacingDirection.Down;
                    break;
                case 2: Direction = FacingDirection.Left;
                    break;
                case 3: Direction = FacingDirection.Right;
                    break;
		        default:
                    break;
	        }
            particlesSet = false;
        }
    }
}
