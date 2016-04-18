using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedoButton : MonoBehaviour {
    public Button redobutton;

    // Update is called once per frame
    void Update()
    {
        redobutton.interactable = UndoHandlerWebGL.instance.CanRedo();
    }
    public void Redo()
    {
        UndoHandlerWebGL.instance.Redo();
    }
}
