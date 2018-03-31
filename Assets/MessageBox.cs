using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {
    public Text titleText;
    public Text YesBtnText;
    public Text NoBtnText;
    public GameObject Panel;

    public delegate void OnReceiveMsgBoxResult(int r); //r = 1 : ok, r = 2 or other : cancel 
    public OnReceiveMsgBoxResult Callback { get; set; }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
       
    }

    public void OnAccept()
    {
        if (Panel != null)
        {
            Panel.gameObject.SetActive(false);
            if (Callback != null)
            {
                Callback(1);
            }
        }
    }

    public void OnCancel()
    {
        if (Panel != null)
        {
            Panel.gameObject.SetActive(false);
            if (Callback != null)
            {
                Callback(2);
            }
        }
    }

    public void Show(string text, string btnyestext, string btnnotext, OnReceiveMsgBoxResult callback)
    {
        if (Panel != null)
        {
            Panel.gameObject.SetActive(true);
            titleText.text = text;
            YesBtnText.text = btnyestext;
            NoBtnText.text = btnnotext;
            Callback = callback;
        }
    }


}
