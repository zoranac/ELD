using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
public class SaveLoadHandlerWebGL : MonoBehaviour
{
    string MainPathTo_Save_Load;
    public List<GameObject> All_Instantiated_ObjectsToSave_Load;
    public List<GameObject> Prefabs;
    List<GameObject> LoadButtons = new List<GameObject>();
    ControlScript control;
    private bool Saving;
    private bool Loading;
    private int NumberOfSaves=0;
    public string SaveName = "";
    public GameObject SaveUI;
    public GameObject LoadUI;
    public GameObject LoadSaveButton;
    public GameObject LoadingUI;
    public MenuUI MenuUI;
    AsyncOperation async;
    int loadProgress = 0;
    void Awake()
    {
        //PlayerPrefs.DeleteAll();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SaveLoadHandler"))
        {
            if (obj != gameObject)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(transform.gameObject);
        //transform.SetParent(GameObject.Find("Canvas").transform);
        LoadUI.SetActive(false);
    }
    public void SetSaveName(InputField savename)
    {
        SaveName = savename.text;
    }
    public void SetSaveName(string savename)
    {
        print(savename);
        SaveName = savename;
        StartCoroutine(StartLoad());
    }
    void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Test"))
        {
            control = GameObject.Find("Control").GetComponent<ControlScript>();
            MenuUI = GameObject.Find("MenuUI").GetComponent <MenuUI>();
        }
        LoadUI.SetActive(false);
    }
    public void ShowSaveUI()
    {
        SaveUI.SetActive(true);
    }
    public void Update()
    {
       // print(SaveName + " " + Loading + " " + async.isDone);
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ShowSaveUI();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                ShowSavesForLoad();

                //Load();
            }
        }
        
        if (Loading && async.isDone)
        {
            Load();
        }
    }
    public IEnumerator StartLoad()
    {
        loadProgress = 0;
        Loading = true;
        async = SceneManager.LoadSceneAsync("Test");
        while (!async.isDone)
        {
            loadProgress = (int)(async.progress * 100);
            LoadUI.SetActive(false);
            LoadingUI.SetActive(true);
            yield return null;
        }
        LoadingUI.SetActive(false);
    }
    public void Cancel()
    {
        SaveUI.SetActive(false);
        LoadUI.SetActive(false);
        foreach (Button obj in LoadUI.GetComponentsInChildren<Button>())
        {
            if (obj.gameObject.name == "LoadSave")
            {
                Destroy(obj.gameObject);
            }
        }
    }
    public void ShowSavesForLoad()
    {
        LoadUI.SetActive(true);
        NumberOfSaves = PlayerPrefs.GetInt("NumberOfSaves");
        string[] saves = new string[NumberOfSaves];
        //print(NumberOfSaves);
        for (int i = 0; i < NumberOfSaves; i++)
		{
            saves[i] = PlayerPrefs.GetString("save" + i);
		}

        foreach (GameObject obj in LoadButtons)
        {
            Destroy(obj);
        }
        LoadButtons.Clear();

        float x = LoadUI.GetComponent<RectTransform>().position.x;
        float y = LoadUI.GetComponent<RectTransform>().position.y;
        foreach (string save in saves)
        {
            print(save);
            GameObject temp = (GameObject)Instantiate(LoadSaveButton, new Vector3(x, y), Quaternion.identity);
            temp.transform.SetParent(LoadUI.transform);
            temp.name = "LoadSave";
            temp.GetComponentInChildren<Text>().text = save;
         
            temp.GetComponent<RectTransform>().localScale = new Vector2(temp.GetComponent<RectTransform>().localScale.x * ((float)Screen.width / 1024),
                                                                        temp.GetComponent<RectTransform>().localScale.y * ((float)Screen.height / 768));
            addListener(temp.GetComponent<Button>(), save);
            LoadButtons.Add(temp);
            y += (LoadSaveButton.GetComponent<RectTransform>().sizeDelta.y + 15) * ((float)Screen.height / 768);
        }
    }
    void addListener(Button b, string value)
    {
        if (MenuUI != null)
            b.onClick.AddListener(() => MenuUI.AreYouSureLoad(value));
        else
            b.onClick.AddListener(() => SetSaveName(value));
    }
    //TO DO
    //Save PowerLines seperate from the other objects
    //Encode/Decode the text written to the saved file
    public void Save()
    {
        NumberOfSaves = PlayerPrefs.GetInt("NumberOfSaves");
        PlayerPrefs.SetString("save" + (NumberOfSaves), SaveName);
        NumberOfSaves += 1;
        print(SaveName);
        SaveUI.SetActive(false);
        All_Instantiated_ObjectsToSave_Load.Clear();
        foreach (PlaceableObject obj in FindObjectsOfType(typeof(PlaceableObject)))
        {
            GameObject OneObject = obj.gameObject;
            if (!All_Instantiated_ObjectsToSave_Load.Exists(x => OneObject == x))
                All_Instantiated_ObjectsToSave_Load.Add(OneObject);
            //if (OneObject.name == GameObject.Find(OneObject.name).name && OneObject.transform.position != GameObject.Find(OneObject.name).transform.position)
            //{
            //   // OneObject.name += "_Cloned_" + Random.Range(10, 999) + "_" + File.ReadAllLines(MainPathTo_Save_Load + "/SavedObjects.txt").Length.ToString();

            //}
            //else
            //{
            //    All_Instantiated_ObjectsToSave_Load.Add(OneObject);
            //}

        }
        foreach (PowerLineScript obj in FindObjectsOfType(typeof(PowerLineScript)))
        {
            GameObject OneObject = obj.gameObject;
            if (!All_Instantiated_ObjectsToSave_Load.Exists(x => OneObject == x))
                All_Instantiated_ObjectsToSave_Load.Add(OneObject);
        }
        string saveData = "";
        saveData += "Player " + GameObject.Find("Player").transform.position.ToString() + "\n";
        foreach (GameObject obj in All_Instantiated_ObjectsToSave_Load)
        {
            //WriteName
            saveData += obj.name +"\n";
            saveData+="\t" + obj.transform.position.ToString("F4")+"\n";
            saveData += "\t" + obj.transform.rotation.ToString("F4") + "\n";
            List<FieldInfo> editableFields = new List<FieldInfo>();
            List<Component> components = new List<Component>();
            if (obj.GetComponent<PowerLineScript>() != null)
            {
                saveData += "\t" + obj.GetComponent<PowerLineScript>().ConnectedDots[0].transform.position.ToString() + "\n";
                saveData += "\t" + obj.GetComponent<PowerLineScript>().ConnectedDots[1].transform.position.ToString() + "\n";
            }
            else
            {
                Component[] scripts = obj.GetComponents(typeof(MonoBehaviour));
                const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

                int i = 0;
                while (i < scripts.Length)
                {
                    FieldInfo[] fields = scripts[i].GetType().GetFields(flags);
                    bool writeScript = true;
                    foreach (FieldInfo fieldInfo in fields)
                    {

                        //Debug.Log("Obj: " + scripts[i].name + ", Field: " + fieldInfo.Name);
                        foreach (System.Attribute a in fieldInfo.GetCustomAttributes(true))
                        {
                            if (a.ToString() == "Editable")
                            {
                                if (writeScript)
                                {
                                    saveData += "\t" + scripts[i].GetType().ToString() + "\n";
                                    writeScript = false;
                                }
                                editableFields.Add(fieldInfo);
                                components.Add(scripts[i]);
                                saveData += "\t\t" + fieldInfo.Name + "\n";
                                if (fieldInfo.FieldType == typeof(bool))
                                {
                                    object temp = fieldInfo.GetValue(components[0]);
                                    saveData += "\t\t\t" + (bool)temp + "\n";
                                }
                                if (fieldInfo.FieldType == typeof(int))
                                {

                                    object temp = fieldInfo.GetValue(components[0]);
                                    saveData +="\t\t\t" + (int)temp + "\n";
                                }
                                if (fieldInfo.FieldType == typeof(float))
                                {

                                    object temp = fieldInfo.GetValue(components[0]);
                                    saveData +="\t\t\t" + (float)temp + "\n";

                                }
                                if (fieldInfo.FieldType == typeof(LogicGate.GateType))
                                {
                                    object temp = fieldInfo.GetValue(components[0]);
                                    saveData +="\t\t\t" + (LogicGate.GateType)temp + "\n";
                                }
                                if (fieldInfo.FieldType == typeof(IOObject.Direction))
                                {
                                    object temp = fieldInfo.GetValue(components[0]);
                                    saveData +="\t\t\t" + (IOObject.Direction)temp + "\n";
                                }
                                if (fieldInfo.FieldType == typeof(FacingDirection))
                                {
                                    object temp = fieldInfo.GetValue(components[0]);
                                    saveData += "\t\t\t" + (FacingDirection)temp + "\n";
                                }
                            }
                        }
                    }

                    i++;
                }

                //foreach (FieldInfo f in editableFields)
                //{
                //    EditableObject tempObject = obj.GetComponent<EditableObject>();

                //}
            }
        }
        PlayerPrefs.SetString(SaveName + "Data", saveData);
        PlayerPrefs.SetInt("NumberOfSaves", NumberOfSaves);
        //   // Saving = true;
        //}
    }
    void Load()
    {
        //print("Load: " + SaveName);
        if (string.IsNullOrEmpty(SaveName))
        {
            return;
        }

        LoadUI.SetActive(false);
        ControlScript.CurrentMode = ControlScript.Mode.Build;
        string data = PlayerPrefs.GetString(SaveName + "Data");
        using (StringReader sr = new StringReader(data))
        {
            int i = 0;
            GameObject temp = null;
            object tempComponent = null;
            FieldInfo tempFieldInfo = null;
            bool powerline = false;
            while (true)
            {
                string line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                if (line.Contains("Player "))
                {
                    line = Regex.Replace(line, "Player ", "");
                    line = Regex.Replace(line, @"\(", "");
                    line = Regex.Replace(line, @"\)", "");
                    line = Regex.Replace(line, @"\t", "");
                    line = Regex.Replace(line, @"\s", "");
                    string[] pos = Regex.Split(line, ",");
                    //print(pos[2]);
                    GameObject.Find("Player").transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                }
                if (line[0] != '\t')
                {
                    i = 0;
                    foreach (GameObject prefab in Prefabs)
                    {
                        if (Regex.Equals(line.Replace("(Clone)", ""), prefab.name.Replace("(Clone)", "")))
                        {
                            temp = (GameObject)Instantiate(prefab);
                            temp.name = temp.name.Replace("(Clone)", "");

                            //print("Prefab: " + prefab);
                            powerline = false;
                            i = 1;

                            if (temp.name == "Power Line")
                            {
                                powerline = true;
                            }
                            if (ControlScript.CurrentMode == ControlScript.Mode.Build)
                            {
                                if (temp.layer == 8)
                                {
                                    temp.GetComponent<SpriteRenderer>().enabled = true;
                                }
                                else if (temp.layer == 10)
                                {
                                    temp.GetComponent<SpriteRenderer>().enabled = false;
                                }
                                else if (temp.name == "Power Line")
                                {
                                    temp.GetComponent<LineRenderer>().enabled = false;
                                }
                            }
                            else if (ControlScript.CurrentMode == ControlScript.Mode.Connect)
                            {
                                if (temp.layer == 8)
                                {
                                    temp.GetComponent<SpriteRenderer>().enabled = false;
                                }
                                else if (temp.layer == 10)
                                {
                                    temp.GetComponent<SpriteRenderer>().enabled = true;
                                }
                                else if (temp.name == "Power Line")
                                {
                                    temp.GetComponent<LineRenderer>().enabled = true;
                                }
                            }
                            else if (ControlScript.CurrentMode == ControlScript.Mode.Play)
                            {
                                if (temp.layer == 8)
                                {
                                    temp.GetComponent<SpriteRenderer>().enabled = false;
                                }
                                else if (temp.layer == 10)
                                {
                                    temp.GetComponent<SpriteRenderer>().enabled = false;
                                }
                                else if (temp.name == "Power Line")
                                {
                                    temp.GetComponent<LineRenderer>().enabled = false;
                                }
                            }
                            break;
                        }
                    }
                }
                else
                {
                    if (i == 1)
                    {
                        line = Regex.Replace(line, @"\(", "");
                        line = Regex.Replace(line, @"\)", "");
                        line = Regex.Replace(line, @"\t", "");
                        line = Regex.Replace(line, @"\s", "");
                        string[] pos = Regex.Split(line, ",");
                        //print(pos[2]);
                        temp.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                        i++;
                    }
                    else if (i == 2)
                    {
                        line = Regex.Replace(line, @"\(", "");
                        line = Regex.Replace(line, @"\)", "");
                        line = Regex.Replace(line, @"\t", "");
                        line = Regex.Replace(line, @"\s", "");
                        string[] pos = Regex.Split(line, ",");
                        temp.transform.rotation = new Quaternion(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3]));
                        i++;
                    }
                    else if (i >= 3)
                    {
                        if (powerline)
                        {
                            line = Regex.Replace(line, @"\(", "");
                            line = Regex.Replace(line, @"\)", "");
                            line = Regex.Replace(line, @"\t", "");
                            line = Regex.Replace(line, @"\s", "");
                            string[] pos = Regex.Split(line, ",");
                            Vector3 v = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                            temp.GetComponent<LineRenderer>().SetPosition(i - 3, v);
                            temp.GetComponent<PowerLineScript>().AddConnectedDotsFromLoad(v);
                            i++;
                        }
                        else
                        {
                            if (line[0] == '\t')
                            {
                                if (line[1] != '\t')
                                {
                                    //component
                                    line = Regex.Replace(line, @"\t", "");
                                    //print("Component: " + line);
                                    tempComponent = temp.GetComponent(line);
                                }
                                else
                                {
                                    if (line[2] != '\t')
                                    {
                                        //Variable
                                        line = Regex.Replace(line, @"\t", "");
                                        //print("Variable: " + line);

                                        tempFieldInfo = tempComponent.GetType().GetField(line);
                                        //Value


                                    }
                                    else
                                    {
                                        //Value
                                        line = Regex.Replace(line, @"\t", "");
                                        if (tempFieldInfo.FieldType == typeof(bool))
                                        {
                                            //print(line);
                                            //tempComponent.GetType().GetField(tempFieldInfo.Name).SetValue(tempFieldInfo.Name, bool.Parse(line));
                                            tempFieldInfo.SetValue(tempComponent, bool.Parse(line));
                                        }
                                        if (tempFieldInfo.FieldType == typeof(int))
                                        {
                                            //tempComponent.GetType().GetField(tempFieldInfo.Name).SetValue(tempFieldInfo.Name, int.Parse(line));
                                            tempFieldInfo.SetValue(tempComponent, int.Parse(line));
                                        }
                                        if (tempFieldInfo.FieldType == typeof(float))
                                        {
                                            //tempComponent.GetType().GetField(tempFieldInfo.Name).SetValue(tempFieldInfo.Name, float.Parse(line));
                                            tempFieldInfo.SetValue(tempComponent, float.Parse(line));
                                        }
                                        if (tempFieldInfo.FieldType == typeof(LogicGate.GateType))
                                        {
                                            //tempComponent.GetType().GetField(tempFieldInfo.Name).SetValue(tempFieldInfo.Name, line);
                                            switch (line)
                                            {
                                                case "AND": tempFieldInfo.SetValue(tempComponent, (LogicGate.GateType.AND));
                                                    break;
                                                case "OR": tempFieldInfo.SetValue(tempComponent, (LogicGate.GateType.OR));
                                                    break;
                                                case "XOR": tempFieldInfo.SetValue(tempComponent, (LogicGate.GateType.XOR));
                                                    break;
                                                case "NOT": tempFieldInfo.SetValue(tempComponent, (LogicGate.GateType.NOT));
                                                    break;
                                                case "NAND": tempFieldInfo.SetValue(tempComponent, (LogicGate.GateType.NAND));
                                                    break;
                                                case "NOR": tempFieldInfo.SetValue(tempComponent, (LogicGate.GateType.NOR));
                                                    break;
                                                case "XNOR": tempFieldInfo.SetValue(tempComponent, (LogicGate.GateType.XNOR));
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        if (tempFieldInfo.FieldType == typeof(IOObject.Direction))
                                        {
                                            //tempComponent.GetType().GetField(tempFieldInfo.Name).SetValue(tempFieldInfo.Name, line);
                                            switch (line)
                                            {
                                                case "Up": tempFieldInfo.SetValue(tempComponent, IOObject.Direction.Up);
                                                    break;
                                                case "Down": tempFieldInfo.SetValue(tempComponent, IOObject.Direction.Down);
                                                    break;
                                                case "Left": tempFieldInfo.SetValue(tempComponent, IOObject.Direction.Left);
                                                    break;
                                                case "Right": tempFieldInfo.SetValue(tempComponent, IOObject.Direction.Right);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        if (tempFieldInfo.FieldType == typeof(FacingDirection))
                                        {
                                            //tempComponent.GetType().GetField(tempFieldInfo.Name).SetValue(tempFieldInfo.Name, line);
                                            switch (line)
                                            {
                                                case "Up": tempFieldInfo.SetValue(tempComponent, FacingDirection.Up);
                                                    break;
                                                case "Down": tempFieldInfo.SetValue(tempComponent, FacingDirection.Down);
                                                    break;
                                                case "Left": tempFieldInfo.SetValue(tempComponent, FacingDirection.Left);
                                                    break;
                                                case "Right": tempFieldInfo.SetValue(tempComponent, FacingDirection.Right);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        Loading = false;
    }
}
