using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.HellionCat.SimpleExport;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Text;
using waqashaxhmi.AndroidNativePlugin;

public class PollsConfig : MonoBehaviour
{

    [Serializable]
    public class HospitalCellInfo
    {
        public Guid projectID;        //项目id
        public string name;     //医院名称
        public DateTime createTime;          //创建时间
        public Dictionary<string, DepartmentCellInfo> departments = new Dictionary<string, DepartmentCellInfo>();
    };
    [Serializable]
    public class DepartmentCellInfo
    {
        public HospitalCellInfo hospital;
        public string name;     //科室名称
        public string questionPath; //题目文件路径
        public bool qusetionLoaded = false;
    };

    public class Question
    {
        public string id;
        public int type;
        public int limit;
        public string question;
        public string shorttext;
        public string[] answers = new string[20];
        public string[] icons = new string[12]; 
    };

    [Serializable]
    public class Answer
    {
        public DateTime startTime;
        public DateTime endTime;
        public string shorttxt;
        public string id;
        public int type;
        public int limit;
        public byte[] answers = new byte[20];
        public byte answer;
    };
    [Serializable]
    public class AnswerStorge
    {
        public string guid;
        public HospitalCellInfo hospital;
        public DepartmentCellInfo department;
        public List<Answer> answers;
        public List<byte[]> photos;
    }

    static public Dictionary<string, List<AnswerStorge>> Answers = new Dictionary<string, List<AnswerStorge>>();

    static public Dictionary<string, List<Question>> QuestionMap = new Dictionary<string, List<Question>>();

    static public HospitalCellInfo selectedHospital = null; //当前医院
    static public DepartmentCellInfo selectedDepartment= null; //当前科室

    static public Dictionary<string, HospitalCellInfo> Hospitals = new Dictionary<string, HospitalCellInfo>();
    private static readonly int MAX_ANS_COUNT = 20;

    static public string GetMD5(string msg)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    static public string Password
    {
        get
        {
            return PlayerPrefs.GetString("Password");
        }

        set
        {
            PlayerPrefs.SetString("Password", GetMD5(value));
            PlayerPrefs.Save();
        }
    }

    static public string CurrentHospital { get; set; }
    static public string CurrentDepartment { get; set; }
    static public int NumPeoples
    {
        get
        {
            return PlayerPrefs.GetInt("NumPeoples");
        }
        set
        {
            PlayerPrefs.SetInt("NumPeoples", value);
            PlayerPrefs.Save();
        }
    }

    static public HospitalCellInfo GetHospitalCellInfoByName(string name)
    {
        if (!Hospitals.ContainsKey(name))
        {
            return null; //rlready exist
        }
        return Hospitals[name];
    }

    static public int AddHospitals(string name)
    {
        if (Hospitals.ContainsKey(name))
        {
            return -1; //rlready exist
        }
        try
        {
            HospitalCellInfo hc = new HospitalCellInfo
            {
                name = name,
                projectID = Guid.NewGuid(),
                createTime = DateTime.Now
            };
            if (AddDepartment("通用科室", hc) == 0)
            {
                Hospitals.Add(name, hc);
                return 0;
            }
            else
            {
                hc = null;
                return -2;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return -2;
        }
    }

    static public void DelHospital(string name)
    {
        if (selectedHospital != null)
        {
            return;
        }
        if (Hospitals.ContainsKey(name))
        {
            Hospitals.Remove(name);
        }
        selectedHospital = null;
    }

    static public int AddDepartment(string name, HospitalCellInfo hci = null)
    {
        if (hci == null && selectedHospital == null)
        {
            return -3; //hopstial not selected
        }
        if (hci != null && hci.departments.ContainsKey(name))
        {
            return -1; //rlready exist
        }
        else if (selectedHospital != null && selectedHospital.departments.ContainsKey(name))
        {
            return -1; //rlready exist
        }
        try
        {
            DepartmentCellInfo dci = new DepartmentCellInfo
            {
                name = name,
                questionPath = "",
                hospital = hci != null ? hci : selectedHospital
            };
            if (hci != null)
            {
                hci.departments.Add(name, dci);
            }
            else
            {
                selectedHospital.departments.Add(name, dci);
            }
            return 0;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return -2;
        }
    }

    static public void DelDepartment(string name)
    {
        if (selectedHospital == null)
        {
            return;
        }
        if (selectedHospital.departments.ContainsKey(name))
        {
            selectedHospital.departments.Remove(name);
        }
        selectedDepartment = null;
    }

    static public DepartmentCellInfo GetDepartmentCellInfoByName(string name)
    {
        if (selectedHospital == null)
        {
            return null;
        }
        if (!selectedHospital.departments.ContainsKey(name))
        {
            return null; //rlready exist
        }
        return selectedHospital.departments[name];
    }

    static public void SerializeData()
    {
        FileStream fs = new FileStream(Application.persistentDataPath + "/hospital.bin", FileMode.OpenOrCreate);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, Hospitals);
        fs.Close();
        if (selectedHospital != null)
        {
            PlayerPrefs.SetString("SelectedHospital", selectedHospital.name);
            PlayerPrefs.Save();
        }
        if (selectedDepartment != null)
        {
            PlayerPrefs.SetString("SelectedDepartment", selectedDepartment.name);
            PlayerPrefs.Save();
        }
    }

    static public void UnserializeData()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/hospital.bin"))
        {
            FileStream fs = new FileStream(Application.persistentDataPath + "/hospital.bin", FileMode.Open);
            if (fs != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                Hospitals = bf.Deserialize(fs) as Dictionary<string, HospitalCellInfo>;
                fs.Close();
            }
        }
        if (PlayerPrefs.GetString("SelectedHospital") != "")
        {
            selectedHospital = GetHospitalCellInfoByName(PlayerPrefs.GetString("SelectedHospital"));
        }
        if (PlayerPrefs.GetString("SelectedDepartment") != "")
        {
            selectedDepartment = GetDepartmentCellInfoByName(PlayerPrefs.GetString("SelectedDepartment"));
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        Application.targetFrameRate = 100;
        UnserializeData();
        LoadAnswer();
    }

    static public void LoadAnswer()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/answer.bin"))
        {
            FileStream fs = new FileStream(Application.persistentDataPath + "/answer.bin", FileMode.Open);
            if (fs != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                Answers = bf.Deserialize(fs) as Dictionary<string, List<AnswerStorge>>;
                fs.Close();
            }
        }
        
    }

    public static string get_ascii(string unicodeString)
    {


        byte[] Buff = System.Text.Encoding.Unicode.GetBytes(unicodeString);
        string retStr = System.Text.Encoding.ASCII.GetString(Buff, 0, Buff.Length);
        return retStr;
   }

    static public void ExportData(string path)
    {
        try
        {
            //AnswerStorge ans = null;
            foreach (KeyValuePair<string, List<AnswerStorge>> pair in Answers)
            {
                ES2Spreadsheet sheet = new ES2Spreadsheet();
                AnswerStorge _ans = pair.Value[0];
                // Add data to cells in the spreadsheet.
                
                //List<Question> qs = QuestionMap[pair.Value[0].department.questionPath];
                for (int row = 0; row < pair.Value.Count + 1; row++)
                {
                    for (int col = 0; col < pair.Value[0].answers.Count + 5; col++)
                    {
                        if (row == 0)
                        {
                            switch (col)
                            {
                                case 0:
                                    sheet.SetCell(col, row, "病人id");
                                    break;
                                case 1:
                                    sheet.SetCell(col, row, "调查科室");
                                    break;
                                case 2:
                                    sheet.SetCell(col, row, "调查开始时间");
                                    break;
                                case 3:
                                    sheet.SetCell(col, row, "调查结束时间");
                                    break;
                                case 4:
                                    sheet.SetCell(col, row, "耗时（秒");
                                    break;
                                default:
                                    sheet.SetCell(col, row, (col - 4).ToString() + "." + pair.Value[0].answers[col - 5].shorttxt);
                                    break;
                            }
                        }
                        else
                        {
                            switch (col)
                            {
                                case 0:
                                    sheet.SetCell(col, row, pair.Value[row - 1].guid);
                                    break;
                                case 1:
                                    sheet.SetCell(col, row, pair.Value[row - 1].department.name);
                                    break;
                                case 2:
                                    sheet.SetCell(col, row, pair.Value[row - 1].answers[0].startTime.ToShortDateString().ToString() + " " +
                                       pair.Value[row - 1].answers[0].startTime.ToShortTimeString().ToString());
                                    break;
                                case 3:
                                    sheet.SetCell(col, row, pair.Value[row - 1].answers[pair.Value[row - 1].answers.Count - 1].endTime.ToShortDateString().ToString() + " " +
                                       pair.Value[row - 1].answers[pair.Value[row - 1].answers.Count - 1].endTime.ToShortTimeString().ToString());
                                    break;
                                case 4:
                                    sheet.SetCell(col, row, (pair.Value[row - 1].answers[pair.Value[row - 1].answers.Count - 1].endTime - pair.Value[row - 1].answers[0].startTime).TotalSeconds);
                                    break;
                                default:
                                    if (pair.Value[row - 1].answers[col - 5].limit == 1)
                                        sheet.SetCell(col, row, pair.Value[row - 1].answers[col - 5].answer);
                                    else
                                    {
                                        UInt32 t = 0;
                                        for (int i = 0; i < MAX_ANS_COUNT; ++i)
                                        {
                                            if (pair.Value[row - 1].answers[col - 5].answers[i] == 1)
                                            {
                                                t += (UInt32)Math.Pow(2, i);
                                            }
                                        }
                                        sheet.SetCell(col, row, t.ToString());
                                    }
                                    break;
                            }
                        }
                    }
                }
                string _folderPath = path + "/" + _ans.hospital.name;
                _folderPath += "/" + _ans.department.name;
                _folderPath += "/" + Path.GetFileNameWithoutExtension(_ans.department.questionPath);
                sheet.Save(_folderPath + "/" + "mySheet.csv");

                foreach(AnswerStorge ans in pair.Value)
                {
                    for (int i = 0; i < ans.photos.Count; ++i)
                    {
                        File.WriteAllBytes(_folderPath + "/" + ans.guid + "-" + (i + 1).ToString() + ".jpg", ans.photos[i]);
                    }
                }
            }
            //SimpleExport_ScoreCSV.ExportCSV()
        }
        catch(Exception e)
        {
#if UNITY_ANDROID
            AndroidNativePluginLibrary.Instance.ShowToast("导出数据错误，请退出后重试");
#endif
            Debug.LogError(e.Message);
        }
    }

    static public void SaveAnswer(List<Answer> answers, List<byte[]> photos, string guid)
    {
        try
        {

            AnswerStorge ans = new AnswerStorge();
            ans.hospital = selectedHospital;
            ans.department = selectedDepartment;
            ans.answers = answers;
            ans.photos = photos;
            ans.guid = guid;
            string hash = ans.hospital.name + "%" + ans.department.name + "%" + Path.GetFileNameWithoutExtension(selectedDepartment.questionPath);
            List<AnswerStorge> slist = null;
            if (Answers.ContainsKey(hash))
            {
                slist = Answers[hash];
            }
            else
            {
                slist = new List<AnswerStorge>();
                Answers[hash] = slist;
            }
            slist.Add(ans);
            string folderPath = Application.persistentDataPath + "/" + ans.hospital.name;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            folderPath += "/" + ans.department.name;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            folderPath += "/" + Path.GetFileNameWithoutExtension(selectedDepartment.questionPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            FileStream fs = new FileStream(folderPath + "/" + "answer.bin", FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, slist);
            fs.Close();
            fs = new FileStream(Application.persistentDataPath + "/" + "answer.bin", FileMode.OpenOrCreate);
            bf = new BinaryFormatter();
            bf.Serialize(fs, Answers);
            fs.Close();


        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
