using UnityEngine;
using System.Collections;

public class RestartUI : MonoBehaviour {
    GameObject player;
    public GameObject restartUI;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        //restartUI = GameObject.Find("RestartUI");
	}
	
	// Update is called once per frame
	void Update () {
        if (!player.active && !restartUI.active && ControlScript.CurrentMode == ControlScript.Mode.Play)
        {
            restartUI.SetActive(true);
        }
        if (restartUI.active && ControlScript.CurrentMode != ControlScript.Mode.Play)
        {
            restartUI.SetActive(false);
        }
	}
    public void Restart()
    {
        restartUI.SetActive(false);
        player.SetActive(true);
        player.transform.position = GameObject.Find("Control").GetComponent<ControlScript>().PlayerStartPos;
    }
    public void Return()
    {
        restartUI.SetActive(false);
        GameObject.Find("Control").GetComponent<ControlScript>().SetToPlayMode();
    }
}
