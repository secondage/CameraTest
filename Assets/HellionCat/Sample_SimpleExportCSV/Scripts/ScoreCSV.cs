using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCSV : MonoBehaviour {

    private int m_score;
    public int score
    {
        get { return m_score; }
    }
	// Use this for initialization
	void Start () {
        this.m_score = 0;
	}
	
	// Update is called once per frame
	void Update () {
        this.m_score++;
	}
}
