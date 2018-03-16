using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using waqashaxhmi.AndroidNativePlugin;

public class HospitalMgrPageController : MonoBehaviour {
    [SerializeField]
    InputField nameInput;
    [SerializeField]
    GridLayoutGroup grid;
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    Text textSelected;
    [SerializeField]
    Button btnDelete;
    [SerializeField]
    Button btnNext;
    [SerializeField]
    GameObject DepartmentMgrPage;
    [SerializeField]
    GameObject mgrPage;

    static Dictionary<HospitalCell, PollsConfig.HospitalCellInfo> HospitalCellMap = new Dictionary<HospitalCell, PollsConfig.HospitalCellInfo>();
    static HospitalCell hotHospitalCell = null;

    private float gridOriginHeight;
    // Use this for initialization
    void Start () {
        AndroidNativeController.OnPositiveButtonPressEvent = (message) => {
            ConfirmDelHospital();
        };
        AndroidNativeController.OnNegativeButtonPressEvent = (message) => {
            // Code whatever you want on click "NO" Button.
        };
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
        UpdateList();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
        if (hotHospitalCell != null)
        {
            hotHospitalCell.imgSelected.enabled = false;
        }
        PollsConfig.selectedHospital = null;
        hotHospitalCell = null;
        textSelected.text = "未选择医院";
        btnDelete.interactable = false;
        btnNext.interactable = false;
        gridOriginHeight = grid.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void UpdateList()
    {
        if (PollsConfig.Hospitals.Count <= 0)
            return;
        foreach(KeyValuePair<string, PollsConfig.HospitalCellInfo> pair in PollsConfig.Hospitals)
        {
            AddNewCell(pair.Key, true);
        }
    }

    void AddNewCell(string name, bool load = false)
    {
        GameObject newone = Instantiate(Resources.Load("ui/HospitalCell") as GameObject);
        if (newone != null)
        {
            newone.transform.SetParent(grid.transform);
            newone.transform.localScale = Vector3.one;
            newone.transform.position = Vector3.zero;
            RectTransform rti = newone.GetComponent<RectTransform>();
            rti.anchoredPosition3D = new Vector3(rti.anchoredPosition3D.x, rti.anchoredPosition3D.y, 0);

            PollsConfig.HospitalCellInfo hci = PollsConfig.GetHospitalCellInfoByName(name);
            if (hci != null)
            {
                HospitalCell hc = newone.GetComponent<HospitalCell>();
                hc.controller = this;
                hc.textName.text = hci.name;
                hc.textTime.text = "创建时间 : " + hci.createTime.ToShortDateString() + " " + hci.createTime.ToShortTimeString();
                hc.imgSelected.enabled = false;
                HospitalCellMap.Add(hc, hci);
                if (!load)
                    PollsConfig.SerializeData();
                Invoke("_refreshList", 0.1f);
            }
        }
        else
        {
            Debug.Log("Instantiate HospitalCell failed.");
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("未知错误，请退出后重试");
#endif
        }
    }

    public void OnInputNameEnd()
    {
        if (nameInput.text == "")
            return;
        string pattern = @"^[^ ]{2,16}$";
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(nameInput.text))
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("医院名称为2-16个字符，且不能存在空格");
#endif
            Debug.LogWarning("医院名称为2-16个字符，且不能存在空格");
            return;
        }
        pattern = @"^[^\/\:\*\?\""\<\>\|\,\.\。\，\？\、\；\“\”]+$";
        regex = new Regex(pattern);
        if (!regex.IsMatch(nameInput.text))
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("医院名称仅能使用汉字，英文字母，数字");
#endif
            Debug.LogWarning("医院名称仅能使用汉字，英文字母，数字");
            return;
        }
        string name = nameInput.text;
        int result = PollsConfig.AddHospitals(name);
        if (result == -1)
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("医院名已存在");
#endif
        }
        else if (result == -2)
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("创建项目失败");
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

    public void OnClickCell(HospitalCell cell)
    {
        if (HospitalCellMap.ContainsKey(cell))
        {
            PollsConfig.HospitalCellInfo hci = HospitalCellMap[cell];
            textSelected.text = "已选择 ：" + hci.name;
            if (hotHospitalCell == null)
            {
                hotHospitalCell = cell;
                cell.imgSelected.enabled = true;
            }
            else
            {
                hotHospitalCell.imgSelected.enabled = false;
                hotHospitalCell = cell;
                cell.imgSelected.enabled = true;
            }
            btnDelete.interactable = true;
            btnNext.interactable = true;
        }
    }

    public void AddHospital()
    {
        nameInput.text = "请输入医院名称";
        nameInput.ActivateInputField();
    }

    public void DelHospital()
    {
        if (hotHospitalCell == null)
            return;
        else
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ConfirmDelHospital();
            }
            else
            {
#if UNITY_ANDROID
                AndroidNativePluginLibrary.Instance.ShowConfirmationDialouge("删除医院", "是否删除医院：" + hotHospitalCell.textName.text, "是", "否");
#endif
            }
        }
    }

    public void ConfirmDelHospital()
    {
        if (HospitalCellMap.ContainsKey(hotHospitalCell))
        {
            PollsConfig.DelHospital(HospitalCellMap[hotHospitalCell].name);
            DestroyImmediate(hotHospitalCell.gameObject);
            hotHospitalCell = null;
            textSelected.text = "未选择医院";
            btnDelete.interactable = false;
            btnNext.interactable = false;
            PollsConfig.SerializeData();
            Invoke("_refreshList", 0.1f);
        }
    }

    private void _scorllToTop()
    {
        scrollRect.verticalNormalizedPosition = 1.0f;
    }

    public void OnNextClick()
    {
        if (hotHospitalCell != null)
        {
            if (HospitalCellMap.ContainsKey(hotHospitalCell))
            {
                PollsConfig.selectedHospital = HospitalCellMap[hotHospitalCell];
                DepartmentMgrPage.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
        
    }

    public void OnExitClick()
    {
        this.gameObject.SetActive(false);
        mgrPage.gameObject.SetActive(true);
    }
}
