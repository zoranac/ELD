using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HelpUI : MonoBehaviour {
    //public string ObjectName;
    //public string ObjectDesc;
    public Sprite[] Sprites;
    public float SpriteSwapTime = 1;
    private float spriteSetTime = 0;
    private int currentSpriteIndex = 0;
    public Text ObjectNameText;
    public Text ObjectDescText;
    public Image SpriteImage;
	// Use this for initialization
    void Update()
    {
        if (Time.time >= spriteSetTime + SpriteSwapTime)
        {
            updateSprite();
        }
    }
    void updateSprite()
    {
        if (currentSpriteIndex + 1 >= Sprites.Length)
        {
            currentSpriteIndex = 0;
        }
        else
        {
            currentSpriteIndex++;
        }
        SpriteImage.sprite = Sprites[currentSpriteIndex];
        spriteSetTime = Time.time;
    }
    public void SetUpHelpUI(string name, string desc, Sprite[] sprites)
    {
        ObjectNameText.text = name;
        //ObjectDescText.text = desc;
        ObjectDescText.text = desc.Replace("<br>", "\n");
        SpriteImage.sprite = sprites[0];
        Sprites = sprites;
        spriteSetTime = Time.time;
    }
    public void ShowHelpUI()
    {
        gameObject.SetActive(true);
    }
    public void HideHelpUI()
    {
        gameObject.SetActive(false);
    }
}
