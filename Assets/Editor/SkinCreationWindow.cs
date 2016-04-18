using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class SkinCreationWindow : EditorWindow {
    GameObject SkinObject;
    List<Sprite> sprites = new List<Sprite>();
    string SkinName = "";
    bool collapsed = false;
    //bool groupEnabled;
    int arraySize;
    Vector2 scrollPos = new Vector2();
    Object prefab;
    public string[] options;
    public int index = 0;
    List<GameObject> SkinableObjects = new List<GameObject>();
    [MenuItem("Window/Add Skin")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SkinCreationWindow window = (SkinCreationWindow)EditorWindow.GetWindow(typeof(SkinCreationWindow));
        window.Show();
    }

    void OnGUI()
    {
        SkinableObjects.Clear();
        string[] lookFor = new string[] { "Assets/Prefabs/Objects/Build" };
        List<string> s = new List<string>();
        foreach (string o in AssetDatabase.FindAssets("t:GameObject", lookFor))
        {

            Debug.Log(AssetDatabase.GUIDToAssetPath(o));

            GameObject g = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(o), typeof(GameObject));
            SkinableObjects.Add(g);
            if (g.GetComponent<SkinableObject>())
            {
                s.Add(g.name);
            }
        }
        options = s.ToArray();

        GUILayout.Label("Skinable Object", EditorStyles.boldLabel);
		index = EditorGUILayout.Popup(index, options);

       
       // SkinObject = (GameObject)EditorGUILayout.ObjectField(SkinObject, typeof(GameObject), false);
        SkinObject = SkinableObjects[index];
        if (SkinObject != null && SkinObject.GetComponent<SkinableObject>() == null)
        {
            Debug.LogError("Object is not a Skinable Object");
        }
       
        EditorGUILayout.PrefixLabel("Skin Name");
        SkinName = EditorGUILayout.TextField(SkinName);

        //collapsed = EditorGUILayout.Foldout(collapsed, "Skin Sprites");
        EditorGUILayout.LabelField("Sprites");
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Size");
            arraySize = EditorGUILayout.IntField(arraySize);
        EditorGUILayout.EndHorizontal();

        if (true)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            Rect r = EditorGUILayout.BeginVertical();
            Sprite[] temp = new Sprite[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (i == 0)
                    EditorGUILayout.PrefixLabel("Main Sprite");
                else
                    EditorGUILayout.PrefixLabel("Sprite " + (i+1));
                    if (sprites.Count-1 < i)
                        temp[i] = (Sprite)EditorGUILayout.ObjectField(temp[i], typeof(Sprite), false);
                    else
                        temp[i] = (Sprite)EditorGUILayout.ObjectField(sprites[i], typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();
            }
            sprites = new List<Sprite>(temp);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Create Skin"))
        {
            if (SkinObject == null || SkinObject.GetComponent<SkinableObject>() == null || !string.IsNullOrEmpty(SkinName) || sprites.Count > 0)
            {
                string directory = System.IO.Directory.GetCurrentDirectory() + "/Assets/Prefabs/Skins/" + SkinObject.name + " Skins/";

                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                string localPath = "Assets/Prefabs/Skins/" + SkinObject.name + " Skins/" + SkinName + ".prefab";
                if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?",
                        "A skin with that name already exists. Do you want to overwrite it?",
                        "Yes",
                        "No"))
                        CreateNew(localPath);
                }
                else
                    CreateNew(localPath);

               // Debug.Log(PrefabUtility.GetPrefabType(PrefabUtility.(prefab)));
            }
            else
            {
                Debug.LogError("ERROR CREATING SKIN PREFAB");
            }
        }

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();
    }
    void CreateNew(string localPath) {
        GameObject temp = new GameObject();
        temp.AddComponent(typeof(Skin));
        temp.GetComponent<Skin>().AllSpritesInSkin = sprites;
        temp.GetComponent<Skin>().MainSprite = sprites[0];
        prefab = PrefabUtility.CreateEmptyPrefab(localPath);
        PrefabUtility.ReplacePrefab(temp, prefab, ReplacePrefabOptions.Default);

        SkinObject.GetComponent<SkinableObject>().Skins.Add(((GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject))).GetComponent<Skin>());

        DestroyImmediate(temp);
     
	}
    void AddSkinToPrefab()
    {

    }
}
