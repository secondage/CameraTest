using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DeadMosquito.AndroidGoodies;


public class IntroPageController : MonoBehaviour {
    public Text textSelectedHospital;
    public Text textSelecteDepartment;
    public Text textNumPeoples;
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

    public InputField passwordInput;
    public Text textTopic;


    public Text verText;

    private void OnEnable()
    {
#if UNITY_ANDROID
      
        var builder = new StringBuilder();
        // Device info
        builder.AppendLine("ANDROID_ID : " + AndroidDeviceInfo.GetAndroidId());
        builder.AppendLine("APP_VERSION : " + Application.version.ToString());

        builder.AppendLine("----------- Build class------------");
        builder.AppendLine("DEVICE : " + AndroidDeviceInfo.DEVICE);
        builder.AppendLine("MODEL : " + AndroidDeviceInfo.MODEL);
        builder.AppendLine("SERIAL : " + AndroidDeviceInfo.SERIAL);
        builder.AppendLine("PRODUCT : " + AndroidDeviceInfo.PRODUCT);
        builder.AppendLine("MANUFACTURER : " + AndroidDeviceInfo.MANUFACTURER);
        builder.AppendLine("RESOLUTION : " + Screen.currentResolution.ToString());

        verText.text = builder.ToString();
#endif
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
        StartCoroutine(GetWebCamAuthorization());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMgrBtnClick()
    {
        btnMgr.interactable = false;
        if (PollsConfig.Password != "")
        {
            textTopic.text = "请输入管理员密码";
            passwordInput.text = "";
            passwordInput.ActivateInputField();
        }
        else
        {
            btnMgr.interactable = true;
            this.gameObject.SetActive(false);
            mgrPage.gameObject.SetActive(true);
        }
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


    public IEnumerator GetWebCamAuthorization()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
           

        }
    }

    public void OnPwdInputEnd()
    {
        btnMgr.interactable = true;
        textTopic.text = "";
        if (passwordInput.text == "")
            return;
        if (PollsConfig.GetMD5(passwordInput.text) == PollsConfig.Password)
        {
            this.gameObject.SetActive(false);
            mgrPage.gameObject.SetActive(true);
        }
        else
        {
#if UNITY_ANDROID
            Toast.ShowToast("请输入正确的管理员密码");
#endif
        }
    }

    
}
