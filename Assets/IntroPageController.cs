using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroPageController : MonoBehaviour {
    [SerializeField]
    Text textSelectedHospital;
    [SerializeField]
    Text textSelecteDepartment;
    [SerializeField]
    Text textNumPeoples;
    [SerializeField]
    Button btnStart;
    [SerializeField]
    Button btnPreview;
    [SerializeField]
    Button btnMgr;
    [SerializeField]
    GameObject mgrPage;
    [SerializeField]
    GameObject answerPage;


    private void OnEnable()
    {
        if (PollsConfig.selectedHospital != null)
        {
            textSelectedHospital.text = PollsConfig.selectedHospital.name;
            textSelectedHospital.color = Color.blue;
        }
        else
        {
            textSelectedHospital.text = "未选定医院";
            textSelectedHospital.color = Color.red;
        }
        if (PollsConfig.selectedDepartment != null)
        {
            textSelecteDepartment.text = PollsConfig.selectedDepartment.name;
            textSelecteDepartment.color = Color.blue;
        }
        else
        {
            textSelecteDepartment.text = "未选定科室";
            textSelecteDepartment.color = Color.red;
        }
        if (PollsConfig.selectedHospital != null && PollsConfig.selectedDepartment != null)
        {
            btnStart.interactable = true;
            btnPreview.interactable = true;
        }
        else
        {
            btnStart.interactable = false;
            btnPreview.interactable = false;
        }
        textNumPeoples.text = "人数 " + PollsConfig.NumPeoples.ToString();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMgrBtnClick()
    {
        this.gameObject.SetActive(false);
        mgrPage.gameObject.SetActive(true);
    }

    public void OnStartBtnClick()
    {
        if (PollsConfig.selectedHospital != null && PollsConfig.selectedDepartment != null)
        {
            AnswerPageController.previewMode = false;
            this.gameObject.SetActive(false);
            answerPage.gameObject.SetActive(true);
        }
    }

    public void OnPreviewBtnClick()
    {
        if (PollsConfig.selectedHospital != null && PollsConfig.selectedDepartment != null)
        {
            AnswerPageController.previewMode = true;
            this.gameObject.SetActive(false);
            answerPage.gameObject.SetActive(true);
        }
    }
}
