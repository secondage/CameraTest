using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionBlockCell : MonoBehaviour {
    public Image selectedImage;
    public Text hospitalNameText;
    public Text departmentNameText;
    public Text questionNameText;
    public string questionGuid;
    public DownloadPageController controller;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnToggle()
    {
        if (GetComponent<Toggle>().isOn)
        {
            controller.OnCellToggle(true, this);
            selectedImage.enabled = true;
        }
        else
        {
            //controller.OnAnswerBtnUnCheck(GetComponent<Toggle>(), index);
            controller.OnCellToggle(false, this);
            selectedImage.enabled = false;
        }
    }
}
