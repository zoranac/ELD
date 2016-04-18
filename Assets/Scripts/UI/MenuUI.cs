using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuUI : MonoBehaviour {
    GameObject SaveLoad;
    public GameObject UI;
    public GameObject AreYouSureUI;
    public Button Ok;
    void Start()
    {
        SaveLoad = GameObject.FindGameObjectWithTag("SaveLoadHandler");
    }
    public void ActiveUI()
    {
        UI.SetActive(!UI.activeSelf);
    }
    public void Quit()
    {
        AreYouSureUI.SetActive(false);
        Application.LoadLevel(0);
    }
    public void AreYouSureQuit()
    {
        AreYouSureUI.SetActive(true);
        Ok.onClick.RemoveAllListeners();
        Ok.onClick.AddListener(() => Quit());
    }
    public void AreYouSureLoad(string save)
    {
        AreYouSureUI.SetActive(true);
        Ok.onClick.RemoveAllListeners();
        Ok.onClick.AddListener(() => SaveLoad.GetComponent<SaveLoadHandlerWebGL>().SetSaveName(save));
        Ok.onClick.AddListener(() => Cancel());
    }
    public void Cancel()
    {
        AreYouSureUI.SetActive(false);
    }
    public void Save(){
        AreYouSureUI.SetActive(false);
        SaveLoad.GetComponent<SaveLoadHandlerWebGL>().ShowSaveUI();
    }
    public void Load()
    {
        AreYouSureUI.SetActive(false);
        SaveLoad.GetComponent<SaveLoadHandlerWebGL>().ShowSavesForLoad();
    }
}
