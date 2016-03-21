using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerScript : MonoBehaviour {
	GameObject control;
	enum MovingDirection{
		Left,
		Right,
		Up,
		Down,
		None
	};
	public int MoveDirection = 3;
	public bool Moving = false;
	private MovingDirection movingDirection;
	private List<MovingDirection> previousMovingDirections = new List<MovingDirection>();
	public float Speed;
	// Use this for initialization
	void Start () {
		control = GameObject.Find("Control");
	}
	void Update()
    {
        if (ControlScript.CurrentMode == ControlScript.Mode.Play)
        {
            CheckInFront();
        }
    }
	// Update is called once per frame
	void FixedUpdate () {
        if (ControlScript.CurrentMode == ControlScript.Mode.Play)
		{
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
			Move();	
			
			//print (movingDirection.ToString());
		}
	}
	void CheckInFront(){
		if (Input.GetKeyDown(KeyCode.Space))
		{
			foreach (RaycastHit2D obj in Physics2D.RaycastAll(transform.position,transform.up,.5f)){
				if (obj.collider.tag == "InteractableObject" ){
					obj.collider.GetComponent<InteractableObject>().Interact();
					print("interact");
				}
			}
		}
	}
	void Move(){
		if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))){
			foreach (MovingDirection dir in previousMovingDirections){
				if (dir == MovingDirection.Left){
					previousMovingDirections.Remove(dir);
					break;
				}
			}
			
			if (movingDirection == MovingDirection.Left)
				movingDirection = MovingDirection.None;
		}
		if ((!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))){
			foreach (MovingDirection dir in previousMovingDirections){
				if (dir == MovingDirection.Right){
					previousMovingDirections.Remove(dir);
					break;
				}
			}
			
			if (movingDirection == MovingDirection.Right)
				movingDirection = MovingDirection.None;
		}
		if ((!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))){
			foreach (MovingDirection dir in previousMovingDirections){
				if (dir == MovingDirection.Up){
					previousMovingDirections.Remove(dir);
					break;
				}
			}
			
			if (movingDirection == MovingDirection.Up)
				movingDirection = MovingDirection.None;
		}
		if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow))){
			foreach (MovingDirection dir in previousMovingDirections){
				if (dir == MovingDirection.Down){
					previousMovingDirections.Remove(dir);
					break;
				}
			}
			
			if (movingDirection == MovingDirection.Down)
				movingDirection = MovingDirection.None;
		}

		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
			if (movingDirection != MovingDirection.None)
				previousMovingDirections.Add(movingDirection);
			movingDirection = MovingDirection.Left;
		}
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
			if (movingDirection != MovingDirection.None)
				previousMovingDirections.Add(movingDirection);
			movingDirection = MovingDirection.Right;
		}
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
			if (movingDirection != MovingDirection.None)
				previousMovingDirections.Add(movingDirection);
			movingDirection = MovingDirection.Up;
		}
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
			if (movingDirection != MovingDirection.None)
				previousMovingDirections.Add(movingDirection);
			movingDirection = MovingDirection.Down;
		}
		//if a key has not just been pressed but is still being held down
		if (movingDirection == MovingDirection.None){
			//if there are previousMovingDirections
			if (previousMovingDirections.Count > 0){
				if (previousMovingDirections[previousMovingDirections.Count-1] == MovingDirection.Left){
					if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
						movingDirection = MovingDirection.Left;
						previousMovingDirections.RemoveAt(previousMovingDirections.Count-1);
					}
				}
				else if (previousMovingDirections[previousMovingDirections.Count-1] == MovingDirection.Right){
					if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
						movingDirection = MovingDirection.Right;
						previousMovingDirections.RemoveAt(previousMovingDirections.Count-1);
					}
				}
				else if (previousMovingDirections[previousMovingDirections.Count-1] == MovingDirection.Up){
					if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
						movingDirection = MovingDirection.Up;
						previousMovingDirections.RemoveAt(previousMovingDirections.Count-1);
					}
				}
				else if (previousMovingDirections[previousMovingDirections.Count-1] == MovingDirection.Down){
					if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
						movingDirection = MovingDirection.Down;
						previousMovingDirections.RemoveAt(previousMovingDirections.Count-1);
					}
				}
				//else just move if you need to
			}else{
				if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
					if (movingDirection != MovingDirection.None)
						previousMovingDirections.Add(movingDirection);
					movingDirection = MovingDirection.Left;
				}
				if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
					if (movingDirection != MovingDirection.None)
						previousMovingDirections.Add(movingDirection);
					movingDirection = MovingDirection.Right;
				}
				if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
					if (movingDirection != MovingDirection.None)
						previousMovingDirections.Add(movingDirection);
					movingDirection = MovingDirection.Up;
				}
				if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
					if (movingDirection != MovingDirection.None)
						previousMovingDirections.Add(movingDirection);
					movingDirection = MovingDirection.Down;
				}
			}
		}
		
		if (movingDirection == MovingDirection.Left){
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
				transform.rotation = Quaternion.Euler(0,0,90);
				transform.position = transform.position + (transform.up * Speed);
			}
		}
		if (movingDirection == MovingDirection.Right){
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
				transform.rotation = Quaternion.Euler(0,0,270);
				transform.position = transform.position + (transform.up * Speed);			
			}
		}
		if (movingDirection == MovingDirection.Up){
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
				transform.rotation = Quaternion.Euler(0,0,0);
				transform.position = transform.position + (transform.up * Speed);			
			}
		}
		if (movingDirection == MovingDirection.Down){
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
				transform.rotation = Quaternion.Euler(0,0,180);
				transform.position = transform.position + (transform.up * Speed);	
			}
		}

		if (movingDirection == MovingDirection.Left){
			Moving = true;
			MoveDirection = 0;
		}
		if (movingDirection == MovingDirection.Up){
			Moving = true;
			MoveDirection = 1;
		}
		if (movingDirection == MovingDirection.Right){
			Moving = true;
			MoveDirection = 2;
		}
		if (movingDirection == MovingDirection.Down){
			Moving = true;
			MoveDirection = 3;
		}
		if (movingDirection == MovingDirection.None){
			Moving = false;
		}
	}
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Trap")
        {
            DestroyPlayer();
        }
    }
    public void DestroyPlayer()
    {
        //Kill player (when player dies restart ui appears)
        gameObject.SetActive(false);
    }
}
