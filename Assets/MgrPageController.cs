using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using waqashaxhmi.AndroidNativePlugin;


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

    public enum PWD_STAGE
    {
        NO_PWD = 0,
        VERIFY_PWD,
        VERIFY_QUESTION_PWD,
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
            BtnExport.interactable = false;
            BtnQuestionMgr.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    string TmpPwd;
    void SetupPwd(string pwd)
    {
        HoldInputPanel.SetActive(false);
        TopicText.text = "";
        if (pwd.Length != 6)
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("密码长度为6位");
#endif
            return;
        }
        if (pwd == "")
        {
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
                AndroidNativePluginLibrary.Instance.ShowToast("管理员密码设置成功");
#endif
                TmpPwd = "";
                PollsConfig.Password = pwd;
                PwdText.text = "Pwd is : " + PollsConfig.Password;
                TextPwd.text = "管理密码";
                BtnExport.interactable = false;
                BtnQuestionMgr.interactable = true;
                TopicText.text = "";
            }
            else
            {
#if UNITY_ANDROID
                AndroidNativePluginLibrary.Instance.ShowToast("密码输入不一致，请重新再试");
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
                AndroidNativePluginLibrary.Instance.ShowToast("请输入正确的管理员密码");
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
                AndroidNativePluginLibrary.Instance.ShowToast("请输入正确的管理员密码");
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
        this.gameObject.SetActive(false);
        introPage.gameObject.SetActive(true);
    }

    public void OnQuestionMgrBtnClick()
    {
        TopicText.text = "请输入管理员密码";
        pwdStage = PWD_STAGE.VERIFY_QUESTION_PWD;
        mgrPasswordInput.text = "";
        mgrPasswordInput.ActivateInputField();

       
    }

}
