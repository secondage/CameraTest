using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepartmentCell : MonoBehaviour {
    public Text textName;
    public Image imgSelected;
    public DepartmentMgrPageController controller;

    public void SetToReadyStage()
    {
        Image img = GetComponent<Image>();
        img.color = Color.cyan;
        imgSelected.enabled = false;
    }

    public void SetToSelectedStage()
    {
        Image img = GetComponent<Image>();
        img.color = Color.green;
        imgSelected.enabled = true;
    }

    public void SetToNormalStage()
    {
        Image img = GetComponent<Image>();
        img.color = Color.white;
        imgSelected.enabled = false;
    }

    public void OnClick()
    {
        controller.OnClickCell(this);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
