using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
public class SaveLoadHandler : MonoBehaviour {
    string MainPathTo_Save_Load;
    public List<GameObject> All_Instantiated_ObjectsToSave_Load;
    public List<GameObject> Prefabs;
    ControlScript control;
    private bool Saving;
    private bool Loading;
    public string SaveName = "";
    public GameObject SaveUI;
    public GameObject LoadUI;
    public GameObject LoadSaveButton;
    void Awake()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("SaveLoadHandler"))
        {
            if(obj != gameObject)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(transform.gameObject);
        //transform.SetParent(GameObject.Find("Canvas").transform);
    }
    public void SetSaveName(InputField savename)
    {
        SaveName = savename.text;
    }
    public void SetSaveName(string savename)
    {
        SaveName = savename;
        StartLoad();
    }
    void Start()
    {

        control = GameObject.Find("Control").GetComponent<ControlScript>();
        MainPathTo_Save_Load = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            MainPathTo_Save_Load += "/../../";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            MainPathTo_Save_Load += "/../";
        }
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveUI.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadUI.SetActive(true);
                ShowSavesForLoad();
                
                //Load();
            }
        }
        if (Loading && !Application.isLoadingLevel)
        {
            Load();
        }
    }
    public void StartLoad()
    {
        Loading = true;
        Application.LoadLevel(Application.loadedLevel);
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
       string[] saves = Directory.GetFiles(MainPathTo_Save_Load + "/SaveFiles/","*.txt");
       
       float x = LoadUI.GetComponent<RectTransform>().position.x;
       float y = LoadUI.GetComponent<RectTransform>().position.y;
       foreach (string save in saves)
       {
           string s = save.Replace(MainPathTo_Save_Load + "/SaveFiles/","");
           s = s.Replace(".txt", "");
           print(save);
           GameObject temp = (GameObject)Instantiate(LoadSaveButton, new Vector3(x, y), Quaternion.identity);
           temp.transform.SetParent(LoadUI.transform);
           temp.name = "LoadSave";
           temp.GetComponentInChildren<Text>().text = s;
           addListener(temp.GetComponent<Button>(),s);
           y += LoadSaveButton.GetComponent<RectTransform>().sizeDelta.y;
       }
    }
    void addListener(Button b, string value)
    {
        b.onClick.AddListener(() => SetSaveName(value));
    }
    //TO DO
    //Save PowerLines seperate from the other objects
    //Encode/Decode the text written to the saved file
    public void Save()
    {
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



        //print(File.Exists(MainPathTo_Save_Load + "/SavedObjects.txt"));
        //if (File.Exists(MainPathTo_Save_Load + "/SavedObjects.txt"))
        //{
        //    string ReplacedPath = MainPathTo_Save_Load + "/SavedObjects.txt";
        //    string ReplacedPath_Positions = MainPathTo_Save_Load + "/Objects";

        //    File.Delete(ReplacedPath);
        //    File.Create(ReplacedPath).Dispose();

        //    if (Directory.Exists(ReplacedPath_Positions))
        //    {
        //        Directory.Delete(ReplacedPath_Positions, true);
        //        Directory.CreateDirectory(ReplacedPath_Positions);

        //        File.Create(ReplacedPath_Positions + "/Objects.txt").Dispose();


        //        Saving = true;
        //    }
        //    else
        //    {
        //        Directory.CreateDirectory(ReplacedPath_Positions);

        //        File.Create(ReplacedPath_Positions + "/Objects.txt").Dispose();

        //        Saving = true;
        //    }
        //}
        //else
        //{
        if (!Directory.Exists(MainPathTo_Save_Load + "/SaveFiles/"))
        {
            Directory.CreateDirectory(MainPathTo_Save_Load + "/SaveFiles/");
        }
            File.Create(MainPathTo_Save_Load + "/SaveFiles/" + SaveName + ".txt").Dispose();

            using (FileStream fs = new FileStream(MainPathTo_Save_Load + "/SaveFiles/" + SaveName + ".txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {

                    foreach (GameObject obj in All_Instantiated_ObjectsToSave_Load)
                    {
                        //WriteName
                        sw.WriteLine(obj.name);
                        sw.WriteLine("\t" + obj.transform.position.ToString("F4"));
                        sw.WriteLine("\t" + obj.transform.rotation.ToString("F4"));
                        List<FieldInfo> editableFields = new List<FieldInfo>();
                        List<Component> components = new List<Component>();
                        if (obj.GetComponent<PowerLineScript>() != null)
                        {
                            sw.WriteLine("\t" + obj.GetComponent<PowerLineScript>().ConnectedDots[0].transform.position.ToString());
                            sw.WriteLine("\t" + obj.GetComponent<PowerLineScript>().ConnectedDots[1].transform.position.ToString());
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
                                                sw.WriteLine("\t" + scripts[i].GetType().ToString());
                                                writeScript = false;
                                            }
                                            editableFields.Add(fieldInfo);
                                            components.Add(scripts[i]);
                                            sw.WriteLine("\t\t" + fieldInfo.Name);
                                            if (fieldInfo.FieldType == typeof(bool))
                                            {
                                                object temp = fieldInfo.GetValue(components[0]);
                                                sw.WriteLine("\t\t\t" + (bool)temp);
                                            }
                                            if (fieldInfo.FieldType == typeof(int))
                                            {

                                                object temp = fieldInfo.GetValue(components[0]);
                                                sw.WriteLine("\t\t\t" + (int)temp);
                                            }
                                            if (fieldInfo.FieldType == typeof(float))
                                            {

                                                object temp = fieldInfo.GetValue(components[0]);
                                                sw.WriteLine("\t\t\t" + (float)temp);

                                            }
                                            if (fieldInfo.FieldType == typeof(LogicGate.GateType))
                                            {
                                                object temp = fieldInfo.GetValue(components[0]);
                                                sw.WriteLine("\t\t\t" + (LogicGate.GateType)temp);
                                            }
                                        }
                                    }
                                }

                                i++;
                            }
                            foreach (FieldInfo f in editableFields)
                            {
                                EditableObject tempObject = obj.GetComponent<EditableObject>();

                            }
                        }
                    }
                }
               
          }

        //   // Saving = true;
        //}
    }
    void Load()
    {
        LoadUI.SetActive(false);
        ControlScript.CurrentMode = ControlScript.Mode.Build;
        using (FileStream fs = new FileStream(MainPathTo_Save_Load + "/SaveFiles/" + SaveName + ".txt", FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                int i = 0;
                GameObject temp = null;
                object tempComponent = null;
                FieldInfo tempFieldInfo = null;
                bool powerline = false;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
           
                    if (line[0] != '\t')
                    {
                        i = 0;
                        foreach (GameObject prefab in Prefabs)
                        {
                            if (Regex.Equals(line.Replace("(Clone)", ""), prefab.name.Replace("(Clone)", "")))
                            {
                                temp = (GameObject)Instantiate(prefab);
                                temp.name = temp.name.Replace("(Clone)", "");
                               
                                print("Prefab: " + prefab);
                                powerline = false;
                                i=1;

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
                        if (i==1)
                        {
                            line = Regex.Replace(line,@"\(","");
                            line = Regex.Replace(line, @"\)", "");
                            line = Regex.Replace(line, @"\t", "");
                            line = Regex.Replace(line, @"\s", "");
                            string[] pos = Regex.Split(line, ",");
                            print(pos[2]);
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
                                temp.GetComponent<LineRenderer>().SetPosition(i-3,v);
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
                                        print("Component: " + line);
                                        tempComponent = temp.GetComponent(line);
                                    }
                                    else
                                    {
                                        if (line[2] != '\t')
                                        {
                                            //Variable
                                            line = Regex.Replace(line, @"\t", "");
                                            print("Variable: " + line);

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
