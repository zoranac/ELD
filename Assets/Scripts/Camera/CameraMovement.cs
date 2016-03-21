using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    //GameObject mouse;
    Camera myCamera;
    public float leftBufferPercentage;
    public float leftLimitPercentage;
    bool Focus = false;
    Vector2 lastclickPos;
	// Use this for initialization
	void Start () {
        //mouse = GameObject.Find("Mouse");
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(2))
        {
            lastclickPos = Input.mousePosition;

        }
        if (Input.GetMouseButton(2))
        {
            transform.Translate(-(((Vector2)myCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)myCamera.ScreenToWorldPoint(lastclickPos)))/(5 / myCamera.orthographicSize));
            lastclickPos = Input.mousePosition;
        }
        //if (Focus)
        //{
        //    if (Input.mousePosition.x <= (leftBufferPercentage/100) * Screen.width && Input.mousePosition.x > (leftLimitPercentage / 100) * Screen.width)
        //    {
        //        transform.Translate(Vector3.left / 20);
        //    }
        //    if (Input.mousePosition.x >= Screen.width - 10)
        //    {
        //        transform.Translate(Vector3.right / 20);
        //    }
        //    if (Input.mousePosition.y <= 10)
        //    {
        //        transform.Translate(Vector3.down / 20);
        //    }
        //    if (Input.mousePosition.y >= Screen.height - 10)
        //    {
        //        transform.Translate(Vector3.up / 20);
        //    }
        //}
    }
    void OnApplicationFocus(bool focus)
    {
        Focus = focus;
    }
}
