using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Toast : MonoBehaviour {
    static Image Background;
    static Text Message;
    static GameObject Panel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        if (Panel == null)
        {
            Panel = this.gameObject;
            Panel.gameObject.SetActive(false);
        }
        if (Background == null)
        {
            Background = Panel.GetComponentInChildren<Button>().GetComponent<Image>();
        }
        if (Message == null)
        {
            Message = GetComponentInChildren<Text>();
        }
    }

    static void HideToast()
    {
        if (Panel != null)
        {
            if (Background != null)
            {
                Background.enabled = false;
                Background.DOKill();
               
            }
            if (Message != null)
            {
                Message.DOKill();
                Message.enabled = false;
              
            }
            Panel.gameObject.SetActive(true);
        }

    }


    static public void ShowToast(string message)
    {
        if (Panel != null)
        {
            Panel.gameObject.SetActive(true);
            if (Background != null)
            {
                Background.enabled = true;
                Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 0.9f);
                Background.DOKill();
                Background.DOFade(0.0f, 3.5f).SetEase(Ease.InQuint).OnComplete(() =>
                {
                    HideToast();
                });
            }
            if (Message != null)
            {
                //Message.c
                Message.enabled = true;
                Message.color = new Color(Message.color.r, Message.color.g, Message.color.b, 0.7f);
                Message.DOKill();
                Message.DOFade(0.0f, 3.4f).SetEase(Ease.InQuint);
                Message.text = message;
            }
               
        }
       
    }
}
