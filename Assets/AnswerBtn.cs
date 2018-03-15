using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerBtn : MonoBehaviour {
    public int index;
    public AnswerPageController controller;
    public Image imageNormal;
    public Image imagePress;
    public Image imageSelect;
    public Text answerText;
    public Text answerNumber;

    //public Sprite normalSprite;
    //public Sprite pressSprite;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnValueChange()
    {
        if (GetComponent<Toggle>().isOn)
        {
            controller.OnAnswerBtnCheck(GetComponent<Toggle>(), index);
        }
        else
        {
            controller.OnAnswerBtnUnCheck(GetComponent<Toggle>(), index);
        }
        // GetComponent<Button>().
        /*if (GetComponent<Image>().sprite == normalSprite)
        {
            GetComponent<Image>().sprite = pressSprite;
            controller.OnAnswerBtnCheck(GetComponent<Button>(), index);
        }
        else
        {
            GetComponent<Image>().sprite = normalSprite;
            controller.OnAnswerBtnUnCheck(GetComponent<Button>(), index);
        }*/
    }
}
