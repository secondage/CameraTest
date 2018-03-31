using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadPageController : MonoBehaviour
{
    public GridLayoutGroup grid;
    public ScrollRect scrollRect;
    private static float gridOriginHeight = 0;
    public GameObject loadingPanel;

    public class QuestionBlockParser
    {
        public QuestionBlockParser()
        {

        }
        public string id { get; set; }
        public string hospitalName { get; set; }
        public string departmentName { get; set; }
        public string questionName { get; set; }
        public string questionGuid { get; set; }

    }

  


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (gridOriginHeight == 0)
        {
            gridOriginHeight = grid.GetComponent<RectTransform>().sizeDelta.y;
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

    IEnumerator webGetAllQuestionBlock()
    {
        WWW www = new WWW("http://47.106.71.112:8080/api/QuestionBlock/");
        yield return www;
        if (www.isDone && www.error == null)
        {
            // Debug.Log("WebData" + www.text);
            UpdateList(www.text);
            loadingPanel.SetActive(false);
            www.Dispose();
        }

    }

   /* IEnumerator webGetAllHospital()
    {
        WWW www = new WWW("http://47.106.71.112/webservice.asmx?op=GetHospitals");
        yield return www;
        if (www.isDone && www.error == null)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            List<HospitalListParser> configs = JsonConvert.DeserializeObject<List<HospitalListParser>>(www.text, settings);
           
            loadingPanel.SetActive(false);
            www.Dispose();
        }

    }*/

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

    IEnumerator webLoadQuestions(QuestionBlockCell qbc)
    {
        WWW www = new WWW("http://47.106.71.112:8080/api/Questions?questionname=" + qbc.questionGuid);
        yield return www;
        if (www.isDone && www.error == null)
        {
            downloaded++;
            if (downloaded == grid.transform.childCount)
            {
                loadingPanel.SetActive(false);
            }
            parseJson(qbc.questionGuid, www.text);
            //add hospital
            PollsConfig.AddHospitals(qbc.hospitalNameText.text, 0);
            PollsConfig.AddDepartment(qbc.departmentNameText.text, 0, PollsConfig.GetHospitalCellInfoByName(qbc.hospitalNameText.text));
            PollsConfig.DepartmentCellInfo dci = PollsConfig.GetDepartmentCellInfoByName(qbc.departmentNameText.text, PollsConfig.GetHospitalCellInfoByName(qbc.hospitalNameText.text));
            dci.questions = PollsConfig.QuestionMap[qbc.questionGuid];
            dci.qusetionLoaded = true;
            www.Dispose();
        }

    }

    void AddNewCell(string h, string d, string q, string g)
    {
        GameObject newone = Instantiate(Resources.Load("ui/QuestionBlockToggle") as GameObject);
        if (newone != null)
        {
            newone.transform.SetParent(grid.transform);
            newone.transform.localScale = Vector3.one;
            newone.transform.position = Vector3.zero;
            RectTransform rti = newone.GetComponent<RectTransform>();
            rti.anchoredPosition3D = new Vector3(rti.anchoredPosition3D.x, rti.anchoredPosition3D.y, 0);


            QuestionBlockCell qbc = newone.GetComponent<QuestionBlockCell>();
            qbc.controller = this;
            qbc.hospitalNameText.text = h;
            qbc.departmentNameText.text = d;
            qbc.questionNameText.text = q;
            qbc.questionGuid = g;


        }
        else
        {
            Debug.Log("Instantiate QuestionBlockToggle failed.");
#if UNITY_ANDROID
            Toast.ShowToast("未知错误，请退出后重试");
#endif
        }
    }



    public void UpdateList(string json)
    {
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        List<QuestionBlockParser> configs = JsonConvert.DeserializeObject<List<QuestionBlockParser>>(json, settings);
        List<PollsConfig.Question> qs = new List<PollsConfig.Question>();

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }

        if (configs.Count == 0)
        {
            Toast.ShowToast("获取问卷列表失败，请稍后重试");
            return;
        }
        if (configs.Count == 1 && configs[0].hospitalName == "")
        {
            Toast.ShowToast("获取问卷列表失败，请稍后重试");
            return;
        }
        foreach (QuestionBlockParser c in configs)
        {
            AddNewCell(c.hospitalName, c.departmentName, c.questionName, c.questionGuid);
        }
        Invoke("_refreshList", 0.1f);
    }

    public void RefleshQuestionBlocks()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(webGetAllQuestionBlock());
    }


    int downloaded = 0;
    public void OnLoadBtnClick()
    {
        loadingPanel.SetActive(true);
        downloaded = 0;
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (grid.transform.GetChild(i).gameObject.GetComponent<Toggle>().isOn)
            {
                QuestionBlockCell qbc = grid.transform.GetChild(i).gameObject.GetComponent<QuestionBlockCell>();
                if (!PollsConfig.QuestionMap.ContainsKey(qbc.questionGuid))
                {
                    StartCoroutine(webLoadQuestions(qbc));
                }

            }
        }

    }

    public void OnCellToggle(bool b, QuestionBlockCell qbc)
    {

    }

}
