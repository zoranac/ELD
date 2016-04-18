using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CopyButton : MonoBehaviour {
    public ObjectEditor objEditor;

	// Update is called once per frame
	void Update () {
       GetComponent<Button>().interactable = (objEditor.SelectedObject != null);
	}
}
