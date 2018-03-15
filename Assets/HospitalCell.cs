using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HospitalCell : MonoBehaviour {
    public Text textName;
    public Text textTime;
    public Image imgSelected;
    public HospitalMgrPageController controller;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {
        controller.OnClickCell(this);
    }
}
