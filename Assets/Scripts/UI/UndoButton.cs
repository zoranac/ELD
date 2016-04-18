using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UndoButton : MonoBehaviour {
    public Button undobutton;
	
	// Update is called once per frame
	void Update () {
        undobutton.interactable = UndoHandlerWebGL.instance.CanUndo();
	}
    public void Undo()
    {
        UndoHandlerWebGL.instance.Undo();
    }
}
