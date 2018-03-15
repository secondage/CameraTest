using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

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
    }

    static public Dictionary<string, List<AnswerStorge>> Answers = new Dictionary<string, List<AnswerStorge>>();

    static public Dictionary<string, List<Question>> QuestionMap = new Dictionary<string, List<Question>>();

    static public HospitalCellInfo selectedHospital = null; //当前医院
    static public DepartmentCellInfo selectedDepartment= null; //当前科室

    static public Dictionary<string, HospitalCellInfo> Hospitals = new Dictionary<string, HospitalCellInfo>();

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

    static public void SaveAnswer(List<Answer> answers)
    {
        try
        {
           
            AnswerStorge ans = new AnswerStorge();
            ans.hospital = selectedHospital;
            ans.department = selectedDepartment;
            ans.answers = answers;
            ans.guid = Guid.NewGuid().ToString();
            string hash = ans.hospital.name + ans.department.name + Path.GetFileNameWithoutExtension(selectedDepartment.questionPath);
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
