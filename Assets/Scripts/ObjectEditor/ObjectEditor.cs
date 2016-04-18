using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.IO;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public struct FieldStruct{
	public FieldStruct(FieldInfo field,string bname,string name,GameObject obj){
		Field = field;
		BaseName = bname;
		Name = name;
        GameObjectName = obj;
	}
	public FieldInfo Field;
	public string BaseName;
	public string Name;
    public GameObject GameObjectName;
}
public class ObjectEditor : MonoBehaviour {
	public GameObject SelectedObject;
    public GameObject NameObj;
	public List<Transform> UI_transforms = new List<Transform>();
	public List<FieldStruct> editableFields = new List<FieldStruct>();
	public List<Component> components = new List<Component>();
	List<GameObject> UIObjects = new List<GameObject>();
	public GameObject slider;
	public GameObject toggle;
    public GameObject dropdown;
	// Use this for initialization
	string[] GetFieldName(string text)
	{
		char[] delimiterChars = {' '};
		string[] words = new string[10];
		words = text.Split(delimiterChars);
		
		words[0] = words[1];
		char[] newWord = new char[100];
		int w = 0;
		foreach(char character in words[0])
		{
			if (char.IsUpper(character) && w > 0)
			{
				newWord[w] = ' ';
				newWord[w+1] = character;
				w+=2;
			}
			else
			{
				if (w == 0)
					newWord[w] = char.ToUpper(character);
				else
					newWord[w] = character;
				w++;
			}
		}
		text = new string(newWord);
		string[] names = new string[2]{words[0],text};
		return names;
	}
	public void SetSelectedObject(GameObject obj){

		SelectedObject = obj;
        if (obj == null)
        {
            NameObj.GetComponent<Text>().text = "";
            foreach (GameObject uiObj in UIObjects)
            {
                Destroy(uiObj);
            }
            UIObjects.Clear();
            components.Clear();
            editableFields.Clear();
            return;
        }
        NameObj.GetComponent<Text>().text = SelectedObject.name;
		foreach(GameObject uiObj in UIObjects){
			Destroy(uiObj);
		}
		UIObjects.Clear();
		components.Clear();
		editableFields.Clear();

		Component[] scripts = SelectedObject.GetComponents(typeof(MonoBehaviour));
		const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

		int i = 0;
		while (i < scripts.Length)
		{
			FieldInfo[] fields =  scripts[i].GetType().GetFields(flags);
			foreach (FieldInfo fieldInfo in fields)
			{
				
				//Debug.Log("Obj: " + scripts[i].name + ", Field: " + fieldInfo.Name);
				foreach (System.Attribute a in fieldInfo.GetCustomAttributes(true))
				{
					if (a.ToString() == "Editable")
					{
						string text = fieldInfo.ToString();
						string[] names = GetFieldName(text);
						editableFields.Add(new FieldStruct(fieldInfo,names[0],names[1],obj));
						components.Add(scripts[i]);
                        
					}
				}
			}
     
			i++;
		}
		i = 0;
		foreach (FieldStruct f in editableFields)
		{
			FieldInfo info = f.Field;
			EditableObject tempObject = SelectedObject.GetComponent<EditableObject>();
			if (info.FieldType == typeof(bool))
			{
				GameObject Temp = (GameObject)Instantiate(toggle,UI_transforms[i].position,UI_transforms[i].rotation);
				Temp.transform.SetParent(gameObject.transform,true);
				Temp.transform.localScale = new Vector2(1f,1f);
				Temp.GetComponentInChildren<Text>().text = f.Name;
				object temp = info.GetValue(components[0]);
				Temp.GetComponent<Toggle>().isOn = (bool)temp;
                Temp.GetComponent<Toggle>().onValueChanged.AddListener(delegate { tempObject.ValueChanged(info, Temp.GetComponent<Toggle>().isOn, true); });	
				UIObjects.Add(Temp);
				i++;
				print("bool");
			}
			if (info.FieldType == typeof(int))
			{
				GameObject Temp = (GameObject)Instantiate(slider,UI_transforms[i].position,UI_transforms[i].rotation);
				Temp.transform.SetParent(gameObject.transform,true);
				Temp.transform.localScale = new Vector2(1f,1f);
				Temp.GetComponentInChildren<Text>().text = f.Name;
				Temp.GetComponent<Slider>().maxValue = PowerOutput.MaxPower;
				Temp.GetComponent<Slider>().minValue = 0;
				Temp.GetComponent<Slider>().wholeNumbers = true;
				object temp = info.GetValue(components[0]);
				Temp.GetComponent<Slider>().value = (int)temp;
				Temp.GetComponent<Slider>().onValueChanged.AddListener(delegate {tempObject.ValueChanged(info,Temp.GetComponent<Slider>().value,true);});
				UIObjects.Add(Temp);
				i++;
				print("int");
			}
			if (info.FieldType == typeof(float))
			{
                GameObject Temp = (GameObject)Instantiate(slider, UI_transforms[i].position, UI_transforms[i].rotation);
                Temp.transform.SetParent(gameObject.transform, true);
                Temp.transform.localScale = new Vector2(1f, 1f);
                Temp.GetComponentInChildren<Text>().text = f.Name;
                Temp.GetComponent<Slider>().maxValue = 10;
                Temp.GetComponent<Slider>().minValue = 0;
                Temp.GetComponent<Slider>().wholeNumbers = false;
                object temp = info.GetValue(components[0]);
                Temp.GetComponent<Slider>().value = (float)temp;
                Temp.GetComponent<Slider>().onValueChanged.AddListener(delegate { tempObject.ValueChanged(info, Temp.GetComponent<Slider>().value, true); });
                UIObjects.Add(Temp);
                i++;
				print("float");
			}
            if (info.FieldType == typeof(IOObject.Direction))
            {
                GameObject Temp = (GameObject)Instantiate(dropdown, UI_transforms[i].position, UI_transforms[i].rotation);
                Temp.transform.SetParent(gameObject.transform, true);
                Temp.transform.localScale = new Vector2(1f, 1f);
                Temp.GetComponentInChildren<Text>().text = f.Name;
                if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputUP)
                    Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Up"));
                if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputDOWN)
                    Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Down"));
                if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputLEFT)
                    Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Left"));
                if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputRIGHT)
                    Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Right"));
                Temp.GetComponent<Dropdown>().value = 0;
                Temp.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { tempObject.ValueChanged(info, Temp.GetComponent<Dropdown>().value, true); });
                UIObjects.Add(Temp);
                i++;
            }
            if (info.FieldType == typeof(FacingDirection))
            {
                GameObject Temp = (GameObject)Instantiate(dropdown, UI_transforms[i].position, UI_transforms[i].rotation);
                Temp.transform.SetParent(gameObject.transform, true);
                Temp.transform.localScale = new Vector2(1f, 1f);
                Temp.GetComponentInChildren<Text>().text = f.Name;
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Up"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Down"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Left"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Right"));
                Temp.GetComponent<Dropdown>().value = 0;
                Temp.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { tempObject.ValueChanged(info, Temp.GetComponent<Dropdown>().value, true); });
                UIObjects.Add(Temp);
                i++;
            }
            if (info.FieldType == typeof(LogicGate.GateType))
            {
                GameObject Temp = (GameObject)Instantiate(dropdown, UI_transforms[i].position, UI_transforms[i].rotation);
                Temp.transform.SetParent(gameObject.transform, true);
                Temp.transform.localScale = new Vector2(1f, 1f);
                Temp.GetComponentInChildren<Text>().text = f.Name;
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("AND"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("OR"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("XOR"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("NOT"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("NAND"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("NOR"));
                Temp.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("XNOR"));
                Temp.GetComponent<Dropdown>().value = 0;
                Temp.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { tempObject.ValueChanged(info, Temp.GetComponent<Dropdown>().value, true); });
                UIObjects.Add(Temp);
                i++;
            }
		}
	}
    void UpdateEditor()
    {
        foreach (FieldStruct f in editableFields)
        {
            FieldInfo info = f.Field;
            foreach (GameObject obj in UIObjects)
            {
                object temp = info.GetValue(components[0]);
                
                if (obj.GetComponent<Slider>() != null && (temp is int || temp is float))
                {
                    try
                    {
                        if (obj.GetComponent<Slider>().wholeNumbers && temp is int)
                            obj.GetComponent<Slider>().value = (int)temp;
                        else if (!obj.GetComponent<Slider>().wholeNumbers && temp is float)
                            obj.GetComponent<Slider>().value = (float)temp;

                       
                    }
                    catch (Exception)
                    {
                        print(f.BaseName);
                        throw;
                    }
                    break;
                  
                }
                if (obj.GetComponent<Toggle>() != null && temp is bool)
                {
                   // print(temp);
                    obj.GetComponent<Toggle>().isOn = (bool)temp;
                    break;
                }
                if (obj.GetComponent<Dropdown>() != null && temp is IOObject.Direction)
                {
                    if (info.FieldType == typeof(IOObject.Direction))
                    {
                        obj.GetComponent<Dropdown>().options.Clear();
                        int i = 0;
                        int up = -1;
                        int down = -1; 
                        int left = -1; 
                        int right = -1; 
                        if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputUP)
                        {
                            obj.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Up"));
                            up = i;
                            i++;
                        }
                        if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputDOWN)
                        {
                            obj.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Down"));
                            down = i;
                            i++;
                        }
                        if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputLEFT)
                        {
                            obj.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Left"));
                            left = i;
                            i++;

                        }
                        if (f.GameObjectName.GetComponent<Comparator>() != null && f.GameObjectName.GetComponent<Comparator>().inputRIGHT)
                        {
                            obj.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("Right"));
                            right = i;
                            i++;
                        }



                        switch ((IOObject.Direction)temp)
                        {
                            case IOObject.Direction.Up: obj.GetComponent<Dropdown>().value = up;
                                break;
                            case IOObject.Direction.Down: obj.GetComponent<Dropdown>().value = down;
                                break;
                            case IOObject.Direction.Left: obj.GetComponent<Dropdown>().value = left;
                                break;
                            case IOObject.Direction.Right: obj.GetComponent<Dropdown>().value = right;
                                break;
                            default:
                                break;
                        }
                    }

                    break;
                }
                if (obj.GetComponent<Dropdown>() != null && temp is FacingDirection)
                {
                    if (info.FieldType == typeof(FacingDirection))
                    {
                        switch ((FacingDirection)temp)
                        {
                            case FacingDirection.Up: obj.GetComponent<Dropdown>().value = 0;
                                break;
                            case FacingDirection.Down: obj.GetComponent<Dropdown>().value = 1;
                                break;
                            case FacingDirection.Left: obj.GetComponent<Dropdown>().value = 2;
                                break;
                            case FacingDirection.Right: obj.GetComponent<Dropdown>().value = 3;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (obj.GetComponent<Dropdown>() != null && temp is LogicGate.GateType)
                {
                    if (info.FieldType == typeof(LogicGate.GateType))
                    {
                        switch ((LogicGate.GateType)temp)
                        {
                            case LogicGate.GateType.AND: obj.GetComponent<Dropdown>().value = 0;
                                break;
                            case LogicGate.GateType.OR: obj.GetComponent<Dropdown>().value = 1;
                                break;
                            case LogicGate.GateType.XOR: obj.GetComponent<Dropdown>().value = 2;
                                break;
                            case LogicGate.GateType.NOT: obj.GetComponent<Dropdown>().value = 3;
                                break;
                            case LogicGate.GateType.NAND: obj.GetComponent<Dropdown>().value = 4;
                                break;
                            case LogicGate.GateType.NOR: obj.GetComponent<Dropdown>().value = 5;
                                break;
                            case LogicGate.GateType.XNOR: obj.GetComponent<Dropdown>().value = 6;
                                break;
                            default:
                                break;
                        }  
                    }

                    break;
                }
                
            }    
            
        }
    }
	void Start () {
	


	}
	// Update is called once per frame
	void Update () {
        UpdateEditor();
	}
}
