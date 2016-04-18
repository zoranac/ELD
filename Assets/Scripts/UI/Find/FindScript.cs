using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public class FindScript : MonoBehaviour {
    private List<GameObject> objects = new List<GameObject>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	   
	}
    public void Find(InputField inputField)
    {
        string findText = inputField.text;
        GameObject[] temp = ((GameObject[])GameObject.FindObjectsOfType(typeof(GameObject)));
        if (temp.Length != objects.Count)
        {
            objects = new List<GameObject>(temp);
        }
        foreach (GameObject obj in objects)
        {
           if (obj.layer == 8 || obj.layer == 10)
           {
               string name = obj.name.Replace("(Clone)","");
               print(name);
               if (!Regex.IsMatch(name,findText,RegexOptions.IgnoreCase))
               {
                   obj.GetComponent<SpriteRenderer>().color = new Color(.5f,.5f,.5f,.25f);
               }
               else
               {
                   obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
               }
           }
        }
    }
}
