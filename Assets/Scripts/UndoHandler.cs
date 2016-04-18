using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
public class UndoHandler : MonoBehaviour {

    private static UndoHandler s_Instance = null;
    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static UndoHandler instance {
        get {
            if (s_Instance == null) {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance =  FindObjectOfType(typeof (UndoHandler)) as UndoHandler;
            }
 
            // If it is still null, create a new instance
            if (s_Instance == null) {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof (UndoHandler)) as UndoHandler;
                Debug.Log ("Could not locate an AManager object.  AManager was Generated Automaticly.");
            }
 
            return s_Instance;
        }
    }
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
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
        ValueChanged<T> deleg = new ValueChanged<T>(AddValueChangeAction<T>);
        deleg.BeginInvoke(g, PreviousData, NewData, variableObject, Threads, new AsyncCallback(onResponse<T>), null);
      
    }
    public void OnPlaced<T>(GameObject g) where T : struct
    {
        tempList.Add(g);
        mouseDown = Input.GetMouseButton(0);
        removeLostActions = true;
        if (Threads == 0)
        {
            AddToList deleg = new AddToList(AddToPlaceActionList<T>);
            deleg.BeginInvoke(g, new AsyncCallback(onResponseAddToPlaceList<T>), null);
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
            AddToList deleg = new AddToList(AddToEraseActionList<T>);
            deleg.BeginInvoke(g, new AsyncCallback(onResponseAddToEraseList<T>), null);
        }
        Threads++;
    }
    //Closes Async Thread
    private void onResponse<T>(IAsyncResult result) where T : struct
    {
        AsyncResult res = (AsyncResult)result;
        ValueChanged<T> deleg = (ValueChanged<T>)res.AsyncDelegate;
        deleg.EndInvoke(result);
    }
    private void onResponseAddToPlaceList<T>(IAsyncResult result) where T : struct
    {
        AsyncResult res = (AsyncResult)result;
        AddToList deleg = (AddToList)res.AsyncDelegate;
        deleg.EndInvoke(result);   
    }
    private void onResponseAddToEraseList<T>(IAsyncResult result) where T : struct
    {
        AsyncResult res = (AsyncResult)result;
        AddToList deleg = (AddToList)res.AsyncDelegate;
        deleg.EndInvoke(result);
    }
    public List<Action> Actions = new List<Action>();
    public void AddMoveAction<T>(GameObject obj, Vector3 fromPos, Vector3 toPos)
    {
        Actions.Add(new Action<Vector3>(obj,Action.ActionType.Move,fromPos,toPos));
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
    public void AddToPlaceActionList<T>(GameObject obj) where T : struct
    {
        
        while (mouseDown == true)
        {
            if (tempList.Count >= 50)
                break;
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
    public void AddToEraseActionList<T>(GameObject obj) where T : struct
    {
        while (mouseDown == true || rightMouseDown == true)
        {
            if (tempList.Count >= 50)
                break;
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
    public void AddValueChangeAction<T>(GameObject obj, T v, T v2, object variableObject, int threadValue) where T : struct
    {
        while (mouseDown == true)
        {
            //Removes thread if its not current
            if (Threads != threadValue)
            {
                return;
            }
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
            if (Actions[ActionIndexCount-1].TypeOfAction == Action.ActionType.Erase)
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
                    SetObjectsActive(false, Actions[ActionIndexCount-1].gameObject);  //psudo-"delete" it
                    if (objectEditor.GetComponent<ObjectEditor>().SelectedObject == Actions[ActionIndexCount - 1].gameObject)   //remove as selected if it is selected
                        control.RemoveSelected();
                }
                else
                {
                    print(Actions[ActionIndexCount - 1].GameObjects.Count);
                    foreach(GameObject obj in Actions[ActionIndexCount - 1].GameObjects)
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
                Actions[ActionIndexCount - 1].gameObject.GetComponent<EditableObject>().ValueChanged(Actions[ActionIndexCount - 1].VariableObject, Actions[ActionIndexCount - 1].PreviousValue,false);    //return variable to old value
                control.SetSelected(Actions[ActionIndexCount - 1].gameObject);                               //set as selected
            }
            --ActionIndexCount;
        }    
    }
    //returns action to its current state (undos the undo)
    void SetObjectsActive(bool value,GameObject obj)
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
                Actions[ActionIndexCount].gameObject.GetComponent<EditableObject>().ValueChanged(Actions[ActionIndexCount].VariableObject, Actions[ActionIndexCount].NewValue,false);
                control.SetSelected(Actions[ActionIndexCount].gameObject);
            }
            ++ActionIndexCount;
        }    
    }
}


public class Action {
    public enum ActionType
    {
        Move,
        Erase,
        Place,
        ValueChange
    };
    public virtual ActionType TypeOfAction { get; set; }
    public virtual GameObject gameObject { get; set; }
    public virtual List<GameObject> GameObjects { get; set; }
    public virtual object PreviousValue { get; set; }
    public virtual object NewValue { get; set; }

    public virtual object VariableObject { get; set; }
}
public class Action<DataType> : Action where DataType : struct
{
    public override GameObject gameObject { get; set; }
    public override List<GameObject> GameObjects { get; set; }
    public override object PreviousValue { get; set; }
    public override object NewValue { get; set; }
    public override Action.ActionType TypeOfAction { get; set; }

    public virtual string VariableName { get; set; }
    public Action(GameObject g, Action.ActionType t){
        TypeOfAction = t ;
        gameObject = g;
    }
    public Action(List<GameObject> gs, Action.ActionType t)
    {
        GameObjects = new List<GameObject>();
        TypeOfAction = t;
        foreach(GameObject g in gs)
        {
            if (g != null)
                GameObjects.Add(g);
        }
    }
    public Action(GameObject g, Action.ActionType t, DataType v)
    {
        TypeOfAction = t;
        gameObject = g;
        PreviousValue = v;
    }
    public Action(GameObject g, Action.ActionType t, DataType v, DataType v2)
    {
        TypeOfAction = t;
        gameObject = g;
        PreviousValue = v;
        NewValue = v2;
    }
    public Action(GameObject g, Action.ActionType t, DataType v, DataType v2,object variableName)
    {
        TypeOfAction = t;
        gameObject = g;
        PreviousValue = v;
        NewValue = v2;
        VariableObject = variableName;
    }
}