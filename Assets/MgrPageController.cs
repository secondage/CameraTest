using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class MgrPageController : MonoBehaviour
{
    public InputField mgrPasswordInput;
    public Button BtnExport;
    public Button BtnQuestionMgr;
    public Text TextPwd;
    public Text PwdText;
    public Text TopicText;
    public GameObject HoldInputPanel;
    public GameObject introPage;
    public GameObject hospitalPage;
    public FileChooser fileChooser;
    public GameObject answerPage;
    public GameObject holdPanel;

    public enum PWD_STAGE
    {
        NO_PWD = 0,
        VERIFY_PWD,
        VERIFY_QUESTION_PWD,
        VERIFY_EXPORT_PWD,
        NEW_PWD,
        CONFIRM_PWD,
        SAVE_PWD,
    }

    PWD_STAGE pwdStage = PWD_STAGE.NO_PWD; //0 for create ,1 for cert, 2 for change
    // Use this for initialization
    void Start()
    {
        PwdText.text = "Pwd is : " + PollsConfig.Password;
        
    }

    private void OnEnable()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
#if UNITY_ANDROID
            StartCoroutine(startCamera());
#endif
        }
        if (PollsConfig.Password == "")
        {
            TextPwd.text = "创建密码";
            BtnExport.interactable = false;
            BtnQuestionMgr.interactable = false;
            Invoke("OnSetupPwd", 0.1f);
        }
        else
        {
            TopicText.text = "";
            TextPwd.text = "管理密码";
            BtnExport.interactable = true;
            BtnQuestionMgr.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    WebCamTexture webcameratex;
    private IEnumerator startCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        Debug.Log("RequestUserAuthorization succeeded.");
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamTexture tex = new WebCamTexture(WebCamTexture.devices[1].name, 640, 480, 12);
            tex.Play();
            tex.Stop();
            tex = null;
        }
    }

    string TmpPwd;
    void SetupPwd(string pwd)
    {
        HoldInputPanel.SetActive(false);
        if (pwd == null || pwd == "")
            return;
        TopicText.text = "";
        string pattern = @"^[^ ]{6,6}$";
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(pwd))
        {
#if UNITY_ANDROID
            Toast.ShowToast("密码为6位字符，且不能存在空格");
#endif
            Debug.LogWarning("密码为6位字符，且不能存在空格");
            return;
        }
        if (pwd.Length != 6)
        {
#if UNITY_ANDROID
            Toast.ShowToast("密码长度为6位");
#endif
            return;
        }
        if (pwdStage == PWD_STAGE.NEW_PWD)
        {
            TmpPwd = PollsConfig.GetMD5(pwd);
            TopicText.text = "请再次输入管理员密码";
            Invoke("InputNewPwd", 0.5f);
            pwdStage = PWD_STAGE.CONFIRM_PWD;
        }
        else if (pwdStage == PWD_STAGE.CONFIRM_PWD) //
        {
            if (TmpPwd == PollsConfig.GetMD5(pwd))
            {
#if UNITY_ANDROID
                Toast.ShowToast("管理员密码设置成功");
#endif
                TmpPwd = "";
                PollsConfig.Password = pwd;
                PwdText.text = "Pwd is : " + PollsConfig.Password;
                TextPwd.text = "管理密码";
                BtnExport.interactable = true;
                BtnQuestionMgr.interactable = true;
                TopicText.text = "";
            }
            else
            {
#if UNITY_ANDROID
                Toast.ShowToast("密码输入不一致，请重新再试");
#endif
                TmpPwd = "";
            }
        }
        else if (pwdStage == PWD_STAGE.VERIFY_PWD)
        {
            if (PollsConfig.GetMD5(pwd) == PollsConfig.Password)
            {
                TopicText.text = "请输入新的管理员密码";
                Invoke("InputNewPwd", 0.5f);
                pwdStage = PWD_STAGE.NEW_PWD;
            }
            else
            {
#if UNITY_ANDROID
                Toast.ShowToast("请输入正确的管理员密码");
#endif
                TopicText.text = "";
            }
        }
        else if (pwdStage == PWD_STAGE.VERIFY_QUESTION_PWD)
        {
            if (PollsConfig.GetMD5(pwd) == PollsConfig.Password)
            {
                this.gameObject.SetActive(false);
                hospitalPage.gameObject.SetActive(true);
            }
            else
            {
#if UNITY_ANDROID
                Toast.ShowToast("请输入正确的管理员密码");
#endif
                TopicText.text = "";
            }
            
        }
        else if (pwdStage == PWD_STAGE.VERIFY_EXPORT_PWD)
        {
            if (PollsConfig.GetMD5(pwd) == PollsConfig.Password)
            {
                if (PollsConfig.Answers.Count == 0)
                {
#if UNITY_ANDROID
                    Toast.ShowToast("目前无可导出数据");
#endif
                }
                else
                {
                    fileChooser.setup(FileChooser.OPENSAVE.OPEN, "");
                    fileChooser.openSaveButton.GetComponentInChildren<Text>().text = "选择";
                    fileChooser.TextTopic.text = "请选择需要导出至的位置";
                    fileChooser.callbackYes = delegate (string filename, string fullname)
                    {
                    //first hide the filechooser
                    fileChooser.gameObject.SetActive(false);
                        Debug.Log("select " + fullname);
                        PollsConfig.ExportData(fullname);
                    };

                    fileChooser.callbackNo = delegate ()
                    {
                        fileChooser.gameObject.SetActive(false);
                    };
                }
            }
            else
            {
#if UNITY_ANDROID
                Toast.ShowToast("请输入正确的管理员密码");
#endif
                TopicText.text = "";
            }
            
        }
    }

    public void ResumeInput()
    {
        HoldInputPanel.SetActive(false);
    }

    public void InputNewPwd()
    {
        mgrPasswordInput.ActivateInputField();
        mgrPasswordInput.text = "";
    }

    public void OnPwdInputEnd(string pwd)
    {
        SetupPwd(pwd);
    }

    public void OnSetupPwd()
    {
        HoldInputPanel.SetActive(true);
        if (PollsConfig.Password == "")
        {
            TopicText.text = "请创建管理员密码";
            pwdStage = PWD_STAGE.NEW_PWD;
        }
        else
        {
            TopicText.text = "请输入管理员密码";
            pwdStage = PWD_STAGE.VERIFY_PWD;
        }
        mgrPasswordInput.text = "";
        mgrPasswordInput.ActivateInputField();
    }

    public void OnReturnBtnClick()
    {
        fileChooser.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        introPage.gameObject.SetActive(true);
    }

    public void OnQuestionMgrBtnClick()
    {
        /*TopicText.text = "请输入管理员密码";
        pwdStage = PWD_STAGE.VERIFY_QUESTION_PWD;
        mgrPasswordInput.text = "";
        mgrPasswordInput.ActivateInputField();*/
        this.gameObject.SetActive(false);
        hospitalPage.gameObject.SetActive(true);
    }

    public void OnExportBtnClick()
    {
        /*TopicText.text = "请输入管理员密码";
        pwdStage = PWD_STAGE.VERIFY_EXPORT_PWD;
        mgrPasswordInput.text = "";
        mgrPasswordInput.ActivateInputField();*/
        if (PollsConfig.Answers.Count == 0)
        {
#if UNITY_ANDROID
            Toast.ShowToast("目前无可导出数据");
#endif
        }
        else
        {
            fileChooser.setup(FileChooser.OPENSAVE.OPEN, "");
            fileChooser.openSaveButton.GetComponentInChildren<Text>().text = "选择";
            fileChooser.TextTopic.text = "请选择需要导出至的位置";
            fileChooser.callbackYes = delegate (string filename, string fullname)
            {
                //first hide the filechooser
                fileChooser.gameObject.SetActive(false);
                Debug.Log("select " + fullname);
                PollsConfig.ExportData(fullname);
            };

            fileChooser.callbackNo = delegate ()
            {
                fileChooser.gameObject.SetActive(false);
            };
        }
    }

}
