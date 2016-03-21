//---------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------SAVING AND LOADING SYSTEM (Basic Version) BY MATT 2016 [SAVE-IT] (Matt's Creations)------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//--------------------------------------For More Informations About Matt's Creations and This Plugin Here:-------------------------------
//------------------------------------------------------http://matts-creations.webnode.sk/-----------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------THANKS FOR WATCHING AND DOWNLOAD!-------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------Visit Assets Store to buy Save and Load System PRO [SAVE-IT PRO]--------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[AddComponentMenu("Matt's Creations/Save-It_BasicFree_Plugin")]

public class Save_Load_BasicSystem : MonoBehaviour {

    //---Main Path To Save and Load Data--------
    [Header("Main Path To Save & Load")]
    public string MainPathTo_Save_Load;
    //----Like D://MySave

    //---All Objects To Load And Save Except Ignored Objects By TAG--------
    [Header("All Objects To Save & Load")]
    public List<GameObject> All_Instantiated_ObjectsToSave_Load;

    //---Ignored Objects By This TAG Variable--------
    [Header("Type Tag Of Ignored Objects")]
    public string IgnoreObjects_ByTag;
    [Header("Type Tag Of Ignored Objects")]
    public string IgnoreObjects_ByTag2;
    //---Rank/ Value/ Count or Place of loaded/ Saved Object--------
    private int Counter_ObjectRegistryNumber;



    private bool Saving;
    private bool Loading;



	void Start () 
    {

        //-----Checking Path if Exist---------------------
        //-------------ANY PATH--------
        if (!string.IsNullOrEmpty(MainPathTo_Save_Load))
        {
            if (Directory.Exists(MainPathTo_Save_Load))
            {
                Debug.Log("Our Directory To Save And Load Exist!");
            }
            else
            {
                Debug.Log("Our Directory To Save And Load Does Not Exist!");

                try
                {
                    Directory.CreateDirectory(MainPathTo_Save_Load);
                    Debug.Log("New Directory Successfully Created!");
                }
                catch (IOException e)
                {
                    Debug.LogError("Cannot Create Directory! * " + MainPathTo_Save_Load + " * Exception: " + e.Message);
                }
            }
        }
        else
        {
            Debug.LogError("Path is empty!");
        }
	}
	
	void Update () 
    {



        //---Also You Can Edit Keys--------
        if (Input.GetKeyDown(KeyCode.S))
        {
            SAVE_();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LOAD_();
        }








        //--------------------------------------------------------------------
        //---------------------------SAVING-----------------------------------
        //--------------------------------------------------------------------
        //--------------------------------------------------------------------
        //--------------------------------------------------------------------
        if (Saving)
        {
            //--------------------------------------------------------------------
            //---------------------------If Equals Lines Lenght Of SavedObjects.txt File and Lenght Of All Objects Variable-----------------------------------
            //--------------------------------------------------------------------
            if (File.ReadAllLines(MainPathTo_Save_Load + "/SavedObjects.txt").Length == All_Instantiated_ObjectsToSave_Load.ToArray().Length)
            {
                Debug.Log("Successfully Saved!");

                Saving = false;

                Counter_ObjectRegistryNumber = 0;
                All_Instantiated_ObjectsToSave_Load.Clear();
            }
            else
            {
                //--------------------------------------------------------------------
                //---------------------------Else Files Will Be Replaced-----------------------------------
                //--------------------------------------------------------------------
                if (File.ReadAllLines(MainPathTo_Save_Load + "/SavedObjects.txt").Length > 0)
                {

                        string SaveObjectIdentity_Name = File.ReadAllText(MainPathTo_Save_Load + "/SavedObjects.txt");
                        string PathToSavePositions = MainPathTo_Save_Load + "/Positions/";

                        string SaveObjectIdentity_Pos_x = File.ReadAllText(PathToSavePositions + "Pos_x.txt");
                        string SaveObjectIdentity_Pos_y = File.ReadAllText(PathToSavePositions + "Pos_y.txt");
                        string SaveObjectIdentity_Pos_z = File.ReadAllText(PathToSavePositions + "Pos_z.txt");

                        if (!string.IsNullOrEmpty(SaveObjectIdentity_Name))
                        {
                            File.WriteAllText(MainPathTo_Save_Load + "/SavedObjects.txt", SaveObjectIdentity_Name + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].name + System.Environment.NewLine);

                            if (!string.IsNullOrEmpty(SaveObjectIdentity_Pos_x))
                            {
                                File.WriteAllText(PathToSavePositions + "/Pos_x.txt", SaveObjectIdentity_Pos_x + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.x + System.Environment.NewLine);
                                File.WriteAllText(PathToSavePositions + "/Pos_y.txt", SaveObjectIdentity_Pos_y + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.y + System.Environment.NewLine);
                                File.WriteAllText(PathToSavePositions + "/Pos_z.txt", SaveObjectIdentity_Pos_z + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.z + System.Environment.NewLine);
                            }

                            Debug.Log("Still Saving... Current Saving: " + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].name);

                            AddRegistryNumber();
                        }
                        else
                        {
                            File.WriteAllText(MainPathTo_Save_Load + "/SavedObjects.txt", All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].name + System.Environment.NewLine);
                            File.WriteAllText(PathToSavePositions + "/Pos_x.txt", SaveObjectIdentity_Pos_x + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.x + System.Environment.NewLine);
                            File.WriteAllText(PathToSavePositions + "/Pos_y.txt", SaveObjectIdentity_Pos_y + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.y + System.Environment.NewLine);
                            File.WriteAllText(PathToSavePositions + "/Pos_z.txt", SaveObjectIdentity_Pos_z + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.z + System.Environment.NewLine);

                            Debug.Log("Still Saving... Current Saving: " + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].name);

                            AddRegistryNumber();
                        }


                        Saving = true;
                    }
                    else
                    {
                        string SaveObjectIdentity = MainPathTo_Save_Load + "/SavedObjects.txt";
                        string PathToSavePositions = MainPathTo_Save_Load + "/Positions/";


                        File.WriteAllText(SaveObjectIdentity, All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].name + System.Environment.NewLine);

                        File.WriteAllText(PathToSavePositions + "Pos_x.txt", All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.x + System.Environment.NewLine);
                        File.WriteAllText(PathToSavePositions + "Pos_y.txt", All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.y + System.Environment.NewLine);
                        File.WriteAllText(PathToSavePositions + "Pos_z.txt", All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].transform.position.z + System.Environment.NewLine);
                    
                        Debug.Log("Still Saving... Current Saving: " + All_Instantiated_ObjectsToSave_Load[Counter_ObjectRegistryNumber].name);


                        AddRegistryNumber();
                    }
                    
                }
        
        }












        //--------------------------------------------------------------------
        //---------------------------LOADING-----------------------------------
        //--------------------------------------------------------------------
        //--------------------------------------------------------------------
        //--------------------------------------------------------------------
        if (Loading)
        {
            if (All_Instantiated_ObjectsToSave_Load.ToArray().Length == File.ReadAllLines(MainPathTo_Save_Load + "/SavedObjects.txt").Length)
            {
                Debug.Log("Successfully Loaded!");

                Loading = false;

                All_Instantiated_ObjectsToSave_Load.Clear();

                Counter_ObjectRegistryNumber = 0;
            }
            else
            {
                CreateNewBlock();
            }
        }
	}











    //--------------------------------------------------------------------
    //---------------------------LOADING/ CREATING BLOCK-----------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    void CreateNewBlock()
    {

        string PathToLoad_Name = MainPathTo_Save_Load + "/SavedObjects.txt";

        string PathToLoad_Pos_x = MainPathTo_Save_Load+"/Positions" + "/Pos_x.txt";
        string PathToLoad_Pos_y = MainPathTo_Save_Load+"/Positions" + "/Pos_y.txt";
        string PathToLoad_Pos_z = MainPathTo_Save_Load+"/Positions" + "/Pos_z.txt";

        GameObject NewBlock = GameObject.CreatePrimitive(PrimitiveType.Cube)as GameObject;

        All_Instantiated_ObjectsToSave_Load.Add(NewBlock);

        NewBlock.SetActive(false);

        NewBlock.transform.name = File.ReadAllLines(PathToLoad_Name)[Counter_ObjectRegistryNumber];

        if (GameObject.Find(NewBlock.name) != null)
        {
            if (GameObject.Find(NewBlock.name).name == NewBlock.name)
            {
                Destroy(GameObject.Find(NewBlock.name));

                Debug.Log("Replaced Object");
            }
        }

        NewBlock.transform.position = new Vector3( 
            float.Parse(File.ReadAllLines(PathToLoad_Pos_x)[Counter_ObjectRegistryNumber]),
            float.Parse(File.ReadAllLines(PathToLoad_Pos_y)[Counter_ObjectRegistryNumber]),
            float.Parse( File.ReadAllLines(PathToLoad_Pos_z)[Counter_ObjectRegistryNumber]));

        NewBlock.SetActive(true);

        AddRegistryNumber();

    }














    //--------------------------------------------------------------------
    //---------------------------SAVE FUNCTION-----------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    void SAVE_()
    {
        foreach (PlaceableObject obj in FindObjectsOfType(typeof(PlaceableObject)))
        {
            GameObject OneObject = obj.gameObject;
            if (OneObject.tag != IgnoreObjects_ByTag || OneObject.tag != IgnoreObjects_ByTag2)
            {
                if (OneObject.name == GameObject.Find(OneObject.name).name && OneObject.transform.position != GameObject.Find(OneObject.name).transform.position)
                {
                    OneObject.name += "_Cloned_" + Random.Range(10, 999) + "_" + File.ReadAllLines(MainPathTo_Save_Load + "/SavedObjects.txt").Length.ToString();
                    All_Instantiated_ObjectsToSave_Load.Add(OneObject);
                }
                else
                {
                    All_Instantiated_ObjectsToSave_Load.Add(OneObject);
                }
            }
        }
            



        if (File.Exists(MainPathTo_Save_Load + "/SavedObjects.txt"))
        {
            string ReplacedPath = MainPathTo_Save_Load + "/SavedObjects.txt";
            string ReplacedPath_Positions = MainPathTo_Save_Load + "/Positions";

            File.Delete(ReplacedPath);
            File.Create(ReplacedPath).Dispose();

            if (Directory.Exists(ReplacedPath_Positions))
            {
                Directory.Delete(ReplacedPath_Positions, true);
                Directory.CreateDirectory(ReplacedPath_Positions);

                File.Create(ReplacedPath_Positions + "/Pos_x.txt").Dispose();
                File.Create(ReplacedPath_Positions + "/Pos_y.txt").Dispose();
                File.Create(ReplacedPath_Positions + "/Pos_z.txt").Dispose();

                Saving = true;
            }
            else
            {
                Directory.CreateDirectory(ReplacedPath_Positions);

                File.Create(ReplacedPath_Positions + "/Pos_x.txt").Dispose();
                File.Create(ReplacedPath_Positions + "/Pos_y.txt").Dispose();
                File.Create(ReplacedPath_Positions + "/Pos_z.txt").Dispose();

                Saving = true;
            }
        }
        else
        {
            File.Create(MainPathTo_Save_Load + "/SavedObjects.txt").Dispose();

            string ReplacedPath_Positions = MainPathTo_Save_Load + "/Positions";

            Directory.CreateDirectory(ReplacedPath_Positions);

            File.Create(ReplacedPath_Positions+"/Pos_x.txt").Dispose();
            File.Create(ReplacedPath_Positions+"/Pos_y.txt").Dispose();
            File.Create(ReplacedPath_Positions+"/Pos_z.txt").Dispose();

            Saving = true;
        }
    }








    //--------------------------------------------------------------------
    //---------------------------LOAD FUNCTION-----------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    void LOAD_()
    {
        if (File.Exists(MainPathTo_Save_Load + "/SavedObjects.txt"))
        {
            if (File.ReadAllText(MainPathTo_Save_Load + "/SavedObjects.txt") != "")
            {
                Loading = true;
            }
            else
            {
                Debug.Log("There is no saved data yet!");
            }
        }
        else
        {
            Debug.LogError("The path to load does not exist!");
        }
    }







    //--------------------------------------------------------------------
    //---------------------------COUNT ADDER-----------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    void AddRegistryNumber()
    {
        Counter_ObjectRegistryNumber++;
    }
}
