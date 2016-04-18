using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
public class UndoHandlerWebGL : MonoBehaviour
{

    private static UndoHandlerWebGL s_Instance = null;
    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static UndoHandlerWebGL instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(UndoHandlerWebGL)) as UndoHandlerWebGL;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof(UndoHandlerWebGL)) as UndoHandlerWebGL;
                Debug.Log("Could not locate an AManager object.  AManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }


    ControlScript control;
    GameObject objectEditor;
    int ActionIndexCount = 0;
    int MaxUndoCount = 50;
    bool mouseDown = false;
    bool rightMouseDown = false;
    int Threads = 0;
    object intialData = null;
    List<GameObject> tempList = new List<GameObject>();
    bool removeLostActions = false;
    delegate void ValueChanged<T>(GameObject g, T previousData, T newData, object variableObject, int threadValue);
    delegate void AddToList(GameObject g);
    void Start()
    {
        control = GameObject.Find("Control").GetComponent<ControlScript>();
        objectEditor = GameObject.Find("ObjectEditor");
    }
    void Update()
    {
        mouseDown = Input.GetMouseButton(0);
        rightMouseDown = Input.GetMouseButton(1);
        if (removeLostActions)
        {
            RemoveLostActions();
            removeLostActions = false;
        }
        CleanActionList();
    }
    //Starts Async Thread
    public void OnValueChanged<T>(GameObject g, T PreviousData, T NewData, object variableObject) where T : struct
    {
        Threads++;
        removeLostActions = true;
        if (intialData == null)
            intialData = PreviousData;
        StartCoroutine(AddValueChangeAction<T>(g, PreviousData,NewData, variableObject, Threads));

    }
    public void OnPlaced<T>(GameObject g) where T : struct
    {
        tempList.Add(g);
        mouseDown = Input.GetMouseButton(0);
        removeLostActions = true;
        if (Threads == 0)
        {
            StartCoroutine(AddToPlaceActionList<T>(g));
        }
        Threads++;
    }
    public void OnErased<T>(GameObject g) where T : struct
    {
        tempList.Add(g);
        removeLostActions = true;
        mouseDown = Input.GetMouseButton(0);
        rightMouseDown = Input.GetMouseButton(1);
        if (Threads == 0)
        {
            StartCoroutine(AddToEraseActionList<T>(g));
        }
        Threads++;
    }
  
    public List<Action> Actions = new List<Action>();
    public void AddMoveAction<T>(GameObject obj, Vector3 fromPos, Vector3 toPos)
    {
        Actions.Add(new Action<Vector3>(obj, Action.ActionType.Move, fromPos, toPos));
        ActionIndexCount++;
    }
    public void AddEraseAction<T>(GameObject obj, Vector3 pos)
    {
        Actions.Add(new Action<Vector3>(obj, Action.ActionType.Erase, pos));
        ActionIndexCount++;
    }
    public void AddPlaceAction<T>(GameObject obj)
    {
        Actions.Add(new Action<Vector3>(obj, Action.ActionType.Place));
        ActionIndexCount++;
    }
    public IEnumerator AddToPlaceActionList<T>(GameObject obj) where T : struct
    {

        while (mouseDown == true)
        {
            if (tempList.Count >= 50)
                break;
            yield return null;
        }
        if (ActionIndexCount == 0)
        {
            Actions.Add(new Action<T>(tempList, Action.ActionType.Place));
        }
        else
        {
            Actions.Insert(ActionIndexCount, new Action<T>(tempList, Action.ActionType.Place));
        }
        ActionIndexCount++;
        tempList.Clear();
        Threads = 0;
    }
    public IEnumerator AddToEraseActionList<T>(GameObject obj) where T : struct
    {
        while (mouseDown == true || rightMouseDown == true)
        {
            if (tempList.Count >= 50)
                break;
            yield return null;
        }
        if (ActionIndexCount == 0)
        {
            Actions.Add(new Action<T>(tempList, Action.ActionType.Erase));
        }
        else
        {
            Actions.Insert(ActionIndexCount, new Action<T>(tempList, Action.ActionType.Erase));
        }
        ActionIndexCount++;
        tempList.Clear();
        Threads = 0;
    }
    public IEnumerator AddValueChangeAction<T>(GameObject obj, T v, T v2, object variableObject, int threadValue) where T : struct
    {
        while (mouseDown == true)
        {
            //Removes thread if its not current
            if (Threads != threadValue)
            {
                yield break;
            }
            yield return null;
        }
        if (ActionIndexCount == 0)
        {
            Actions.Add(new Action<T>(obj, Action.ActionType.ValueChange, (T)intialData, v2, variableObject));
        }
        else
        {
            Actions.Insert(ActionIndexCount, new Action<T>(obj, Action.ActionType.ValueChange, (T)intialData, v2, variableObject));
        }
        ActionIndexCount++;
        intialData = null;
        Threads = 0;
    }
    //removes actions that took place after the ActionIndexCount (occurs after a player does an action after undoing)
    void RemoveLostActions()
    {
        while (Actions.Count > ActionIndexCount)
        {
            if (Actions[Actions.Count - 1].gameObject != null && !Actions[Actions.Count - 1].gameObject.activeSelf)
                Destroy(Actions[Actions.Count - 1].gameObject);
            if (Actions[Actions.Count - 1].GameObjects != null)
            {
                foreach (GameObject obj in Actions[Actions.Count - 1].GameObjects)
                {
                    if (!obj.activeSelf)
                        Destroy(obj);
                }
            }

            Actions.RemoveAt(Actions.Count - 1);
        }
    }
    public bool CanRedo()
    {
        return (Actions.Count > ActionIndexCount);
    }
    public bool CanUndo()
    {
        return (ActionIndexCount > 0);
    }
    //Clears the oldest actions if the list is too big
    void CleanActionList()
    {
        while (Actions.Count > MaxUndoCount)
        {
            if ((Actions[0].gameObject != null) && !Actions[0].gameObject.activeSelf)
                Destroy(Actions[0].gameObject);
            if (Actions[0].GameObjects != null)
            {
                foreach (GameObject obj in Actions[0].GameObjects)
                {
                    if ((obj != null) && !obj.activeSelf)
                        Destroy(obj);
                }
            }

            Actions.RemoveAt(0);
        }
        //ActionIndexCount = Actions.Count;
    }
    public void Undo()
    {
        if (ActionIndexCount > 0)
        {
            /*print(Actions[ActionIndexCount-1].TypeOfAction);
            print(Actions[ActionIndexCount-1].PreviousValue);
            print(Actions[ActionIndexCount - 1].NewValue);
            print(Actions[ActionIndexCount - 1].gameObject);*/

            //if the gameobject doesnt exist remove the action

            if (Actions[ActionIndexCount - 1].gameObject == null && Actions[ActionIndexCount - 1].GameObjects.Count <= 0)
            {
                Actions.RemoveAt(ActionIndexCount - 1);
                --ActionIndexCount;
                return;
            }
            //if action is erase
            if (Actions[ActionIndexCount - 1].TypeOfAction == Action.ActionType.Erase)
            {
                if (Actions[ActionIndexCount - 1].gameObject != null)
                {
                    SetObjectsActive(true, Actions[ActionIndexCount - 1].gameObject);   //psudo-"recreate" it
                    var testVect = Actions[ActionIndexCount - 1].PreviousValue;
                    Actions[ActionIndexCount - 1].gameObject.transform.position = (Vector3)testVect;            //reset its position
                }
                else
                {
                    foreach (GameObject obj in Actions[ActionIndexCount - 1].GameObjects)
                    {
                        SetObjectsActive(true, obj);   //psudo-"recreate" it
                    }
                }
            }
            //if action is Move
            if (Actions[ActionIndexCount - 1].TypeOfAction == Action.ActionType.Move)
            {
                var testVect = Actions[ActionIndexCount - 1].PreviousValue;
                Actions[ActionIndexCount - 1].gameObject.transform.position = (Vector3)testVect;            //reset its position
            }
            //if action is Place
            if (Actions[ActionIndexCount - 1].TypeOfAction == Action.ActionType.Place)
            {
                if (Actions[ActionIndexCount - 1].gameObject != null)
                {
                    SetObjectsActive(false, Actions[ActionIndexCount - 1].gameObject);  //psudo-"delete" it
                    if (objectEditor.GetComponent<ObjectEditor>().SelectedObject == Actions[ActionIndexCount - 1].gameObject)   //remove as selected if it is selected
                        control.RemoveSelected();
                }
                else
                {
                    foreach (GameObject obj in Actions[ActionIndexCount - 1].GameObjects)
                    {
                        SetObjectsActive(false, obj);  //psudo-"delete" it
                        if (objectEditor.GetComponent<ObjectEditor>().SelectedObject == obj)   //remove as selected if it is selected
                            control.RemoveSelected();
                    }
                }
            }
            //if action is ValueChange
            if (Actions[ActionIndexCount - 1].TypeOfAction == Action.ActionType.ValueChange)
            {
                Actions[ActionIndexCount - 1].gameObject.GetComponent<EditableObject>().ValueChanged(Actions[ActionIndexCount - 1].VariableObject, Actions[ActionIndexCount - 1].PreviousValue, false);    //return variable to old value
                print(Actions[ActionIndexCount - 1].PreviousValue);
                control.SetSelected(Actions[ActionIndexCount - 1].gameObject);                               //set as selected
            }
            --ActionIndexCount;
        }
    }
    //returns action to its current state (undos the undo)
    void SetObjectsActive(bool value, GameObject obj)
    {
        if (obj.GetComponent<PlaceableObject>() != null)
        {
            obj.GetComponent<PlaceableObject>().SetActive(value);
        }
        else if (obj.GetComponent<PowerLineScript>() != null)
        {
            obj.GetComponent<PowerLineScript>().SetActive(value);
        }
    }
    public void Redo()
    {
        if (ActionIndexCount < Actions.Count)
        {

            /*print(Actions[ActionIndexCount].TypeOfAction);
            print(Actions[ActionIndexCount].PreviousValue);
            print(Actions[ActionIndexCount].NewValue);
            print(Actions[ActionIndexCount].gameObject);*/

            if (Actions[ActionIndexCount].gameObject == null && Actions[ActionIndexCount].GameObjects.Count <= 0)
            {
                Actions.RemoveAt(ActionIndexCount);
                return;
            }

            if (Actions[ActionIndexCount].TypeOfAction == Action.ActionType.Erase)
            {
                if (Actions[ActionIndexCount].gameObject != null)
                {
                    SetObjectsActive(false, Actions[ActionIndexCount].gameObject);
                    var testVect = Actions[ActionIndexCount].PreviousValue;
                    Actions[ActionIndexCount].gameObject.transform.position = (Vector3)testVect;
                    if (objectEditor.GetComponent<ObjectEditor>().SelectedObject == Actions[ActionIndexCount].gameObject)
                        control.RemoveSelected();
                }
                else
                {
                    foreach (GameObject obj in Actions[ActionIndexCount].GameObjects)
                    {
                        SetObjectsActive(false, obj);

                        if (objectEditor.GetComponent<ObjectEditor>().SelectedObject == obj)
                            control.RemoveSelected();
                    }
                }
            }
            if (Actions[ActionIndexCount].TypeOfAction == Action.ActionType.Move)
            {
                var testVect = Actions[ActionIndexCount].NewValue;
                Actions[ActionIndexCount].gameObject.transform.position = (Vector3)testVect;
            }
            if (Actions[ActionIndexCount].TypeOfAction == Action.ActionType.Place)
            {
                if (Actions[ActionIndexCount].gameObject != null)
                {
                    SetObjectsActive(true, Actions[ActionIndexCount].gameObject);
                }
                else
                {
                    foreach (GameObject obj in Actions[ActionIndexCount].GameObjects)
                    {
                        SetObjectsActive(true, obj);
                    }
                }
            }
            if (Actions[ActionIndexCount].TypeOfAction == Action.ActionType.ValueChange)
            {
                Actions[ActionIndexCount].gameObject.GetComponent<EditableObject>().ValueChanged(Actions[ActionIndexCount].VariableObject, Actions[ActionIndexCount].NewValue, false);
                control.SetSelected(Actions[ActionIndexCount].gameObject);
            }
            ++ActionIndexCount;
        }
    }
}
