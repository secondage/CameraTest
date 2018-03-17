using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using waqashaxhmi.AndroidNativePlugin;
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

    public FileChooser fileChooser;

    static Dictionary<DepartmentCell, PollsConfig.DepartmentCellInfo> DepartmentCellMap = new Dictionary<DepartmentCell, PollsConfig.DepartmentCellInfo>();
    static DepartmentCell hotDepartmentCell = null;

    private float gridOriginHeight;
    // Use this for initialization
    void Start()
    {
        AndroidNativeController.OnPositiveButtonPressEvent = (message) =>
        {
            ConfirmDelDepartment();
        };
        AndroidNativeController.OnNegativeButtonPressEvent = (message) =>
        {
            // Code whatever you want on click "NO" Button.
        };

        // UpdateList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        textHospitalSelected.text = PollsConfig.selectedHospital.name;
        gridOriginHeight = grid.GetComponent<RectTransform>().sizeDelta.y;
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

    private void AddNewCell(string name, bool load = false)
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
            }
        }
        else
        {
            Debug.Log("Instantiate DepartmentCell failed.");
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("未知错误，请退出后重试");
#endif
        }
    }

    public void OnDepartmentInputNameEnd()
    {
        if (nameInput.text == "")
            return;
        string pattern = @"^[^ ]{2,16}$";
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(nameInput.text))
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("科室名称为2-16个字符，且不能存在空格");
#endif
            Debug.LogWarning("科室名称为2-16个字符，且不能存在空格");
            return;
        }
        pattern = @"^[^\/\:\*\?\""\<\>\|\,\.\。\，\？\、\；\“\”]+$";
        regex = new Regex(pattern);
        if (!regex.IsMatch(nameInput.text))
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("科室名称仅能使用汉字，英文字母，数字");
#endif
            Debug.LogWarning("科室名称仅能使用汉字，英文字母，数字");
            return;
        }
        string name = nameInput.text;
        int result = PollsConfig.AddDepartment(name);
        if (result == -1)
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("科室名已存在");
#endif
        }
        else if (result == -2)
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("创建科室失败");
#endif
        }
        else if (result == -3)
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("未选择医院");
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

    public void AddDepartment()
    {
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
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ConfirmDelDepartment();
            }
            else
            {
#if UNITY_ANDROID
                AndroidNativePluginLibrary.Instance.ShowConfirmationDialouge("删除科室", "是否删除科室：" + hotDepartmentCell.textName.text, "是", "否");
#endif
            }
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
                    hdci.questionPath = fullname;
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
}
