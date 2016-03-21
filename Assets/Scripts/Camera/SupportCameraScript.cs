using UnityEngine;
using System.Collections;

public class SupportCameraScript : MonoBehaviour {
    Camera mycamera;
	// Use this for initialization
	void Start () {
        mycamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Camera.main.orthographicSize != mycamera.orthographicSize)
            mycamera.orthographicSize = Camera.main.orthographicSize;
        if (Camera.main.transform.position != transform.position)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
