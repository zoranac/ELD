using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {
    public Button Load;
    public Button New;
    public GameObject SaveLoadManager;
	// Use this for initialization
	void Awake () {
        SaveLoadManager = GameObject.Find("SaveLoadHandlerWebGL");
    }
    void Start()
    {
        Load.onClick.AddListener(() => SaveLoadManager.GetComponent<SaveLoadHandlerWebGL>().ShowSavesForLoad());
        New.onClick.AddListener(() => SaveLoadManager.GetComponent<SaveLoadHandlerWebGL>().SetSaveName(""));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
