using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

public class DepartmentMgrPageController : MonoBehaviour
{
    [SerializeField]
    InputField nameInput;
    [SerializeField]
    GridLayoutGroup grid;
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    Text textHospitalSelected;
    [SerializeField]
    Text textDepartmentSelected;
    [SerializeField]
    Button btnNew;
    [SerializeField]
    Button btnDelete;
    [SerializeField]
    Button btnStart;
    [SerializeField]
    Button btnPreview;
    [SerializeField]
    Button btnLoad;
    [SerializeField]
    GameObject HospitalMgrPage;
    [SerializeField]
    GameObject IntroPage;
    public GameObject loadingPanel;
    public MessageBox messageBox;

    public GameObject HoldPanel;

    public GameObject AnswerPage;

    public FileChooser fileChooser;

    static Dictionary<DepartmentCell, PollsConfig.DepartmentCellInfo> DepartmentCellMap = new Dictionary<DepartmentCell, PollsConfig.DepartmentCellInfo>();
    static DepartmentCell hotDepartmentCell = null;

    private static float gridOriginHeight = 0;
    private int needDownload = 0;

    // Use this for initialization

    public class QuestionParser
    {
        public QuestionParser()
        {

        }
        public string id { get; set; }
        public int type { get; set; }
        public string text { get; set; }
        public string shorttxt { get; set; }
        public string index1 { get; set; }
        public string index2 { get; set; }
        public string index3 { get; set; }
        public string index4 { get; set; }
        public string index5 { get; set; }
        public int limit { get; set; }
        public string answer1 { get; set; }
        public string answer2 { get; set; }
        public string answer3 { get; set; }
        public string answer4 { get; set; }
        public string answer5 { get; set; }
        public string answer6 { get; set; }
        public string answer7 { get; set; }
        public string answer8 { get; set; }
        public string answer9 { get; set; }
        public string answer10 { get; set; }
        public string answer11 { get; set; }
        public string answer12 { get; set; }
        public string answer13 { get; set; }
        public string answer14 { get; set; }
        public string answer15 { get; set; }
        public string answer16 { get; set; }
        public string answer17 { get; set; }
        public string answer18 { get; set; }
        public string answer19 { get; set; }
        public string answer20 { get; set; }
        public string icon1 { get; set; }
        public string icon2 { get; set; }
        public string icon3 { get; set; }
        public string icon4 { get; set; }
        public string icon5 { get; set; }
        public string icon6 { get; set; }
        public string icon7 { get; set; }
        public string icon8 { get; set; }
        public string icon9 { get; set; }
        public string icon10 { get; set; }
        public string icon11 { get; set; }
        public string icon12 { get; set; }
    }

    public class DepartmentListParser
    {
        public DepartmentListParser()
        {

        }
        public int id { get; set; }
        public string name { get; set; }
        public string hospitalid { get; set; }
        public string hospitalname { get; set; }

    }

    public class DepartmentIDParser
    {
        public DepartmentIDParser()
        {

        }
        public int id { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string hospitalid { get; set; }
        public string hospitalname { get; set; }
        public string departmentid { get; set; }
        public string departmentname { get; set; }

    }

    void MessageBoxCallback(int r)
    {
        if (r == 1)
        {
           // HoldPanel.gameObject.SetActive(false);
            ConfirmDelDepartment();
        }
        else
        {
           // HoldPanel.gameObject.SetActive(false);
        }

    }
    void Start()
    {
      /*  AndroidNativeController.OnPositiveButtonPressEvent = (message) =>
        {
            HoldPanel.gameObject.SetActive(false);
            ConfirmDelDepartment();
        };
        AndroidNativeController.OnNegativeButtonPressEvent = (message) =>
        {
            HoldPanel.gameObject.SetActive(false);
            // Code whatever you want on click "NO" Button.
        };*/

        // UpdateList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        textHospitalSelected.text = PollsConfig.selectedHospital.name;
        textDepartmentSelected.text = "未选择科室";
        textDepartmentSelected.color = Color.red;
        if (gridOriginHeight == 0)
        {
            gridOriginHeight = grid.GetComponent<RectTransform>().sizeDelta.y;
        }
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
        hotDepartmentCell = null;
        UpdateList();
        btnNew.interactable = true;
        btnDelete.interactable = false;
        btnStart.interactable = false;
        btnPreview.interactable = false;
        btnLoad.interactable = false;
        PollsConfig.selectedDepartment = null;
        needDownload = 0;
        if (PollsConfig.GetDepartmentCellInfoCount(PollsConfig.selectedHospital) == 0)
        {
            loadingPanel.gameObject.SetActive(true);
            StartCoroutine(webGetDepartments(PollsConfig.selectedHospital.hospitalID));
        }
    }

    public void UpdateList()
    {
        if (PollsConfig.selectedHospital.departments.Count <= 0)
            return;
        foreach (KeyValuePair<string, PollsConfig.DepartmentCellInfo> pair in PollsConfig.selectedHospital.departments)
        {
            AddNewCell(pair.Key, true);
        }
    }

    private DepartmentCell AddNewCell(string name, bool load = false)
    {
        GameObject newone = Instantiate(Resources.Load("ui/DepartmentCell") as GameObject);
        if (newone != null)
        {
            newone.transform.SetParent(grid.transform);
            newone.transform.localScale = Vector3.one;
            newone.transform.position = Vector3.zero;
            RectTransform rti = newone.GetComponent<RectTransform>();
            rti.anchoredPosition3D = new Vector3(rti.anchoredPosition3D.x, rti.anchoredPosition3D.y, 0);

            PollsConfig.DepartmentCellInfo dci = PollsConfig.GetDepartmentCellInfoByName(name);
            if (dci != null)
            {
                DepartmentCell dc = newone.GetComponent<DepartmentCell>();
                dc.controller = this;
                dc.textName.text = dci.name;
                if (dci.qusetionLoaded)
                {
                    dc.SetToReadyStage();
                }
                else
                {
                    dc.SetToNormalStage();
                }
                DepartmentCellMap.Add(dc, dci);
                if (!load)
                {
                    PollsConfig.SerializeData();
                }
                Invoke("_refreshList", 0.1f);
                return dc;
            }
            return null;
        }
        else
        {
            Debug.Log("Instantiate DepartmentCell failed.");
#if UNITY_ANDROID
            Toast.ShowToast("未知错误，请退出后重试");
#endif
            return null;
        }
    }


    IEnumerator webGetQuestionsID(DepartmentCell dc, string departmentid)
    {
        WWW www = new WWW("http://47.106.71.112/webservice.asmx/GetQuestionnairesByDepartmentID?departmentid=" + departmentid);
        yield return www;
        if (www.isDone && www.error == null && www.text != "")
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            List<DepartmentIDParser> configs = JsonConvert.DeserializeObject<List<DepartmentIDParser>>(www.text, settings);
            www.Dispose();
            if (!PollsConfig.QuestionMap.ContainsKey(configs[0].id.ToString()))
            {
                needDownload++;
                loadingPanel.SetActive(true);
                StartCoroutine(webLoadQuestions(dc, configs[0].departmentname, configs[0].id.ToString()));
            }
            else
            {
                PollsConfig.DepartmentCellInfo dci = PollsConfig.GetDepartmentCellInfoByName(configs[0].departmentname);
                dci.questions = PollsConfig.QuestionMap[configs[0].id.ToString()];
                dci.qusetionLoaded = true;
                dci.questionID = configs[0].id.ToString();
                dc.SetToReadyStage();
                PollsConfig.SerializeData();
            }
        }
        else
        {
            Debug.Log("There no questions in department " + departmentid);
        }
    }


    IEnumerator webLoadQuestions(DepartmentCell dc, string departmentname, string id)
    {
        WWW www = new WWW("http://47.106.71.112/webservice.asmx/GetNaire?id=" + id);
        yield return www;
        if (www.isDone && www.error == null)
        {
            needDownload--;
            if (needDownload == 0)
            {
                loadingPanel.SetActive(false);
            }
            parseJson(id, www.text);
            //add hospital
            PollsConfig.DepartmentCellInfo dci = PollsConfig.GetDepartmentCellInfoByName(departmentname);
            dci.questions = PollsConfig.QuestionMap[id];
            dci.qusetionLoaded = true;
            dci.questionID = id;
            dc.SetToReadyStage();
            PollsConfig.SerializeData();
            www.Dispose();
        }

    }


    IEnumerator webGetDepartments(int id)
    {
        WWW www = new WWW("http://47.106.71.112//webservice.asmx/GetDepartmentsByHospitalID?hospitalid=" + id.ToString());
        yield return www;
        if (www.isDone && www.error == null)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            List<DepartmentListParser> configs = JsonConvert.DeserializeObject<List<DepartmentListParser>>(www.text, settings);
            foreach (DepartmentListParser c in configs)
            {
                int result = PollsConfig.AddDepartment(c.name, c.id);
                if (result == -1)
                {
#if UNITY_ANDROID
                    Toast.ShowToast("科室名已存在");
#endif
                }
                else if (result == -2)
                {
#if UNITY_ANDROID
                    Toast.ShowToast("创建科室失败");
#endif
                }
                else if (result == -3)
                {
#if UNITY_ANDROID
                    Toast.ShowToast("未选择医院");
#endif
                }
                else
                {
                    DepartmentCell dc = AddNewCell(c.name);
                    if (dc != null)
                    {
                        StartCoroutine(webGetQuestionsID(dc, c.id.ToString()));
                    }
                }
            }
            //loadingPanel.SetActive(false);
            www.Dispose();
            
        }

    }



    public void OnDepartmentInputNameEnd()
    {
        HoldPanel.gameObject.SetActive(false);
        if (nameInput.text == "")
            return;
        string pattern = @"^[^ ]{2,16}$";
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(nameInput.text))
        {
#if UNITY_ANDROID
            Toast.ShowToast("科室名称为2-16个字符，且不能存在空格");
#endif
            Debug.LogWarning("科室名称为2-16个字符，且不能存在空格");
            return;
        }
        pattern = @"^[^\/\:\*\?\""\<\>\|\,\.\。\，\？\、\；\“\”]+$";
        regex = new Regex(pattern);
        if (!regex.IsMatch(nameInput.text))
        {
#if UNITY_ANDROID
            Toast.ShowToast("科室名称仅能使用汉字，英文字母，数字");
#endif
            Debug.LogWarning("科室名称仅能使用汉字，英文字母，数字");
            return;
        }
        string name = nameInput.text;
        int result = PollsConfig.AddDepartment(name, 0);
        if (result == -1)
        {
#if UNITY_ANDROID
            Toast.ShowToast("科室名已存在");
#endif
        }
        else if (result == -2)
        {
#if UNITY_ANDROID
            Toast.ShowToast("创建科室失败");
#endif
        }
        else if (result == -3)
        {
#if UNITY_ANDROID
            Toast.ShowToast("未选择医院");
#endif
        }
        else
        {
            AddNewCell(name);
        }
    }

    /// <summary>
    /// 刷新列表Grid控件的尺寸
    /// </summary>
    private void _refreshList()
    {
        if (grid != null)
        {
            RectTransform rt = grid.GetComponent<RectTransform>();
            GridLayoutGroup glg = grid.GetComponent<GridLayoutGroup>();

            //if ((_items.Count / glg.constraintCount) * (glg.cellSize.y + glg.spacing.y) > rt.sizeDelta.y) {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, Math.Max(gridOriginHeight, (glg.transform.childCount) * (glg.cellSize.y + glg.spacing.y) - glg.spacing.y));
            //scrollRect.enabled = (rt.sizeDelta.y > gridOriginHeight);
            scrollRect.verticalNormalizedPosition = 1.0f;
            //}
            //Invoke("_scorllToTop", 0.1f);
        }
    }


    public void RefleshDepartments()
    {
        PollsConfig.DelAllDepartment();
        PollsConfig.ClearQuestionMap();
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
        hotDepartmentCell = null;
        btnNew.interactable = true;
        btnDelete.interactable = false;
        btnStart.interactable = false;
        btnPreview.interactable = false;
        btnLoad.interactable = false;
        PollsConfig.selectedDepartment = null;
        loadingPanel.gameObject.SetActive(true);
        needDownload = 0;
        StartCoroutine(webGetDepartments(PollsConfig.selectedHospital.hospitalID));
    }

    public void AddDepartment()
    {
        HoldPanel.gameObject.SetActive(true);
        nameInput.text = "请输入科室名称";
        nameInput.ActivateInputField();
    }


    public void OnClickCell(DepartmentCell cell)
    {
        if (DepartmentCellMap.ContainsKey(cell))
        {
            PollsConfig.DepartmentCellInfo dci = DepartmentCellMap[cell];
            textDepartmentSelected.text = "已选择 ：" + dci.name;
            textDepartmentSelected.color = Color.blue;
            if (hotDepartmentCell == null)
            {
                hotDepartmentCell = cell;
                hotDepartmentCell.SetToSelectedStage();
            }
            else
            {
                PollsConfig.DepartmentCellInfo hdci = DepartmentCellMap[hotDepartmentCell];
                if (hdci != null)
                {
                    if (hdci.qusetionLoaded)
                    {
                        hotDepartmentCell.SetToReadyStage();
                    }
                    else
                    {
                        hotDepartmentCell.SetToNormalStage();
                    }
                    hotDepartmentCell = cell;
                    hotDepartmentCell.SetToSelectedStage();
                }
            }
            
            btnDelete.interactable = true;
            btnStart.interactable = dci.qusetionLoaded;
            btnPreview.interactable = dci.qusetionLoaded;
            btnLoad.interactable = !dci.qusetionLoaded;
        }
    }

    public void DelDepartment()
    {
        if (hotDepartmentCell == null)
            return;
        else
        {
            /*  if (Application.platform == RuntimePlatform.WindowsEditor)
              {
                  ConfirmDelDepartment();
              }
              else
              {
                  HoldPanel.gameObject.SetActive(true);
  #if UNITY_ANDROID
                  AndroidNativePluginLibrary.Instance.ShowConfirmationDialouge("删除科室", "是否删除科室：" + hotDepartmentCell.textName.text, "是", "否");
  #endif
              }*/
            messageBox.Show("是否删除科室：" + hotDepartmentCell.textName.text, "是", "否", MessageBoxCallback);
        }
    }

    public void ConfirmDelDepartment()
    {
        if (DepartmentCellMap.ContainsKey(hotDepartmentCell))
        {
            PollsConfig.DelDepartment(DepartmentCellMap[hotDepartmentCell].name);
            DestroyImmediate(hotDepartmentCell.gameObject);
            hotDepartmentCell = null;
            textDepartmentSelected.text = "未选择科室";
            textDepartmentSelected.color = Color.red;
            btnDelete.interactable = false;
            btnStart.interactable = false;
            btnLoad.interactable = false;
            btnPreview.interactable = false;
            PollsConfig.SerializeData();
            Invoke("_refreshList", 0.1f);
        }
    }


    public void BackToHospitalMgrPage()
    {
        fileChooser.gameObject.SetActive(false);
        hotDepartmentCell = null;
        this.gameObject.SetActive(false);
        HospitalMgrPage.gameObject.SetActive(true);

    }

    
   


    public void OnLoadQuestion()
    {
        // EditorUtility.OpenFilePanel("sss", "\\", ".json");
        fileChooser.setup(FileChooser.OPENSAVE.OPEN, "json");
        fileChooser.openSaveButton.GetComponentInChildren<Text>().text = "打开";
        fileChooser.TextTopic.text = "请选择需要打开的题目文件";
        fileChooser.callbackYes = delegate (string filename, string fullname)
        {
            //first hide the filechooser
            fileChooser.gameObject.SetActive(false);
            Debug.Log("select " + fullname);
            /*
            if (PollsConfig.QuestionMap.ContainsKey(fullname))
            {
                if (hotDepartmentCell != null)
                {
                    PollsConfig.DepartmentCellInfo hdci = DepartmentCellMap[hotDepartmentCell];
                    if (hdci != null)
                    {
                        hdci.qusetionLoaded = true;
                        hdci.questionPath = fullname;
                    }
                    PollsConfig.selectedDepartment = hdci;
                    btnPreview.interactable = true;
                    btnStart.interactable = true;
                    PollsConfig.SerializeData();
                }
            }
            else
            {
                //jsonPath = fullname;
                //jsonName = filename;
                //StartCoroutine(LoadWWW());
            }*/

            if (hotDepartmentCell != null)
            {
                PollsConfig.DepartmentCellInfo hdci = DepartmentCellMap[hotDepartmentCell];
                if (hdci != null)
                {
                    hdci.qusetionLoaded = true;
                    hdci.questionID = fullname;
                }
                PollsConfig.selectedDepartment = hdci;
                btnPreview.interactable = true;
                btnStart.interactable = true;
                btnLoad.interactable = false;
                PollsConfig.SerializeData();
            }

        };

        fileChooser.callbackNo = delegate ()
        {
            fileChooser.gameObject.SetActive(false);
        };
    }

    public void OnStartClick()
    {
        PollsConfig.DepartmentCellInfo hdci = DepartmentCellMap[hotDepartmentCell];
        if (hdci != null)
        {
            PollsConfig.selectedDepartment = hdci;
        }
        PollsConfig.SerializeData();
        this.gameObject.SetActive(false);
        IntroPage.gameObject.SetActive(true);

    }

    public void OnPreviewClick()
    {
        PollsConfig.DepartmentCellInfo hdci = DepartmentCellMap[hotDepartmentCell];
        if (hdci != null)
        {
            PollsConfig.selectedDepartment = hdci;
        }
        AnswerPageController.previewMode = true;
        //this.gameObject.SetActive(false);
        AnswerPage.gameObject.SetActive(true);
    }

    private void parseJson(string guid, string json)
    {
        try
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            List<QuestionParser> configs = JsonConvert.DeserializeObject<List<QuestionParser>>(json, settings);
            List<PollsConfig.Question> qs = new List<PollsConfig.Question>();

            foreach (QuestionParser d in configs)
            {
                PollsConfig.Question q = new PollsConfig.Question();
                q.id = d.id;
                q.type = d.type;
                q.limit = d.limit;
                q.question = d.text;
                q.shorttext = d.shorttxt;
                q.indeices[0] = d.index1;
                q.indeices[1] = d.index2;
                q.indeices[2] = d.index3;
                q.indeices[3] = d.index4;
                q.indeices[4] = d.index5;
                q.answers[0] = d.answer1;
                q.answers[1] = d.answer2;
                q.answers[2] = d.answer3;
                q.answers[3] = d.answer4;
                q.answers[4] = d.answer5;
                q.answers[5] = d.answer6;
                q.answers[6] = d.answer7;
                q.answers[7] = d.answer8;
                q.answers[8] = d.answer9;
                q.answers[9] = d.answer10;
                q.answers[10] = d.answer11;
                q.answers[11] = d.answer12;
                q.answers[12] = d.answer13;
                q.answers[13] = d.answer14;
                q.answers[14] = d.answer15;
                q.answers[15] = d.answer16;
                q.answers[16] = d.answer17;
                q.answers[17] = d.answer18;
                q.answers[18] = d.answer19;
                q.answers[19] = d.answer20;
                q.icons[0] = d.icon1;
                q.icons[1] = d.icon2;
                q.icons[2] = d.icon3;
                q.icons[3] = d.icon4;
                q.icons[4] = d.icon5;
                q.icons[5] = d.icon6;
                q.icons[6] = d.icon7;
                q.icons[7] = d.icon8;
                q.icons[8] = d.icon9;
                q.icons[9] = d.icon10;
                q.icons[10] = d.icon11;
                q.icons[11] = d.icon12;
                qs.Add(q);
            }
            PollsConfig.QuestionMap.Add(guid, qs);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
        }
    }
}
